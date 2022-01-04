Shader "Unlit/SonarTriplanar"
{
    Properties
    {
        [MainColor] _MainColor ("Main Color", Color) = (1,1,1,1)
        [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
        _Threshold ("Threshold", Float) = 0.5
        _Power ("Power", Int) = 1
        _Phase ("Phase", Float) = 20
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
                float3 worldNormal : NORMAL;
                float3 distanceVector : FLOAT3;
            };

            float4 _MainColor;

            sampler2D _BumpMap;
            float4 _BumpMap_ST;

            //Variables for color calculation
            float _Threshold;
            int _Power;

            //Variables for wave calculation
            float4 _Origin;
            float _Distance;
            float _Speed;
            float _Phase;

            v2f vert (appdata v)
            {
                v2f o;

                //Coordinate mappings
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BumpMap);
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
                o.worldNormal = wNormal;

                //Vector from the wave origin
                o.distanceVector = mul(unity_ObjectToWorld, v.vertex) - _Origin;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = 1;
                //Normal triplanar for x, y, z sides
                float3 blendNormal = saturate(pow(i.worldNormal * 1.4,4));
                
                half4 xn = tex2D(_BumpMap, i.worldPos.zy * i.uv);
                half4 yn = tex2D(_BumpMap, i.worldPos.zx * i.uv);
                half4 zn = tex2D(_BumpMap, i.worldPos.xy * i.uv);
                
                half4 normal = zn;
                normal = lerp(normal, xn, blendNormal.x);
                normal = lerp(normal, yn, blendNormal.y);

                //Initialize texture tangent and world normals
                half3 tNormal = UnpackNormal(normal);
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