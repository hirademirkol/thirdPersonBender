Shader "Unlit/SonarTerrain"
{
    Properties
    {
        [MainColor] _MainColor ("Main Color", Color) = (1,1,1,1)
        _Threshold ("Threshold", Float) = 0.5
        _Power ("Power", Int) = 1
        _Phase ("Phase", Float) = 20
    }
    SubShader
    {
        // _NormalMap pass will get ignored by terrain basemap generation code. Put here so that the VTC can use it to generate cache for normal maps.
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
            "Name" = "_NormalMap"
            "Format" = "A2R10G10B10"
            "Size" = "1"
        }

        LOD 100

        ZTest Always Cull Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        //Blend One [_DstBlend]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _MainColor;

            //Variables for color calculation
            float _Threshold;
            int _Power;

            //Variables for wave calculation
            float4 _Origin;
            float _Distance;
            float _Speed;
            float _Phase;

            //Variables for terrain rendering with splat map
            float4 _Splat0_ST;
            float4 _Splat1_ST;
            float4 _Splat2_ST;
            float4 _Splat3_ST;
            
            sampler2D _Normal0, _Normal1, _Normal2, _Normal3;
            float _NormalScale0, _NormalScale1, _NormalScale2, _NormalScale3;

            sampler2D _Control;
            float4 _Control_ST;
            float4 _Control_TexelSize;


            float2 ComputeControlUV(float2 uv)
            {
                // adjust splatUVs so the edges of the terrain tile lie on pixel centers
                return (uv * (_Control_TexelSize.zw - 1.0f) + 0.5f) * _Control_TexelSize.xy;
            }


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;

                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z
                float3 worldPos : TEXCOORD4;
                float3 cameraDir : FLOAT4;
                float3 normal : NORMAL;
                float3 distanceVector : FLOAT3;

                float2 texcoord1 : TEXCOORD5;
                float2 texcoord2 : TEXCOORD6;
                float2 texcoord3 : TEXCOORD7;
                float2 texcoord4 : TEXCOORD8;
            };


            v2f vert (appdata v)
            {
                v2f o;

                //Coordinate mappings
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 uv = TRANSFORM_TEX(v.uv, _Control);
                o.uv = ComputeControlUV(uv);
                o.texcoord1 = TRANSFORM_TEX(uv, _Splat0);
                o.texcoord2 = TRANSFORM_TEX(uv, _Splat1);
                o.texcoord3 = TRANSFORM_TEX(uv, _Splat2);
                o.texcoord4 = TRANSFORM_TEX(uv, _Splat3);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                //Normalized direction to camera
                o.cameraDir = normalize(WorldSpaceViewDir(v.vertex));

                //World Normal, Tangent and Bitangent Calculation
                half3 wNormal = UnityObjectToWorldNormal(v.normal);
                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;

                //Tangent space matrix and normal output
                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
                o.normal = wNormal;

                //Vector from the wave origin
                o.distanceVector = mul(unity_ObjectToWorld, v.vertex) - _Origin;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = 1;
                //Initialize texture tangent and world normals
                //half3 tNormal = UnpackNormal(tex2D(_BumpMap, i.uv));
                float4 alpha = tex2D(_Control, i.uv);
                float3 tNormal;
                tNormal = UnpackNormalWithScale(tex2D(_Normal0, i.texcoord1), _NormalScale0) * alpha.x;
                tNormal += UnpackNormalWithScale(tex2D(_Normal1, i.texcoord2), _NormalScale1) * alpha.y;
                tNormal += UnpackNormalWithScale(tex2D(_Normal2, i.texcoord3), _NormalScale2) * alpha.z;
                tNormal += UnpackNormalWithScale(tex2D(_Normal3, i.texcoord4), _NormalScale3) * alpha.w;
                                
                half3 worldNormal;

                //Calculate world normals from texture tangent normals
                worldNormal.x = dot(i.tspace0, tNormal);
                worldNormal.y = dot(i.tspace1, tNormal);                
                worldNormal.z = dot(i.tspace2, tNormal);

                // Color calculated as multiplication of normal and camera direction
                color.rgb = pow(dot(worldNormal, i.cameraDir), _Power) > _Threshold;

                //Distance calculated as horizontal+vertical distance
                float dist = length(i.distanceVector.xz) + abs(i.distanceVector.y);
                
                //Mask for waves only on sonarfront
                float mask = abs(dist-_Distance)<0.5;
                float wave = (sin(-_Time.y * _Speed + dist*_Phase)+1)*0.5; //Mapped to [0,1]
                
                //Wave added to color
                color.rgb += wave * mask;
                
                //Alpha is a mask of circle with radius _Distance with 1 falloff at egde
                color.a = 0.5-0.5*clamp(dist-_Distance+0.5,0,1);

                return color * _MainColor;
            }
            ENDCG
        }
    }
}