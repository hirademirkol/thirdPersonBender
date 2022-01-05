Shader "Custom/StandardTriplanar"
{
    Properties
    {
        _MainTex ("Diffuse", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _ST ("Mapping", Vector) = (.1,.1,0,0)
        _BumpScale ("Normal Scale", Float) = 1
        _Mask ("Mask Map", 2D) = "green" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _Mask;
        float4 _ST;
        float _BumpScale;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // clamp (saturate) and increase(pow) the worldnormal value to use as a blend between the projected textures
            float3 blendNormal = saturate(pow(WorldNormalVector (IN, o.Normal) * 1.4,4));

            // normal triplanar for x, y, z sides
            half4 xn = tex2D(_BumpMap, IN.worldPos.zy * _ST.xy + _ST.zw);
            half4 yn = tex2D(_BumpMap, IN.worldPos.zx * _ST.xy + _ST.zw);
            half4 zn = tex2D(_BumpMap, IN.worldPos.xy * _ST.xy + _ST.zw);

            half4 normal = zn;
            normal = lerp(normal, xn, blendNormal.x);
            normal = lerp(normal, yn, blendNormal.y);

            // diffuse triplanar for x, y, z sides
            float3 xc = tex2D(_MainTex, IN.worldPos.zy * _ST.xy + _ST.zw);
            float3 yc = tex2D(_MainTex, IN.worldPos.zx * _ST.xy + _ST.zw);
            float3 zc = tex2D(_MainTex, IN.worldPos.xy * _ST.xy + _ST.zw);

            float3 color = zc;
            color = lerp(color, xc, blendNormal.x);
            color = lerp(color, yc, blendNormal.y);

            // mask triplanar for x, y, z sides
            float4 xm = tex2D(_Mask, IN.worldPos.zy * _ST.xy + _ST.zw);
            float4 ym = tex2D(_Mask, IN.worldPos.zx * _ST.xy + _ST.zw);
            float4 zm = tex2D(_Mask, IN.worldPos.xy * _ST.xy + _ST.zw);

            float4 mask = zm;
            mask = lerp(mask, xm, blendNormal.x);
            mask = lerp(mask, ym, blendNormal.y);

            o.Albedo = color;
            o.Normal = UnpackScaleNormal(normal, _BumpScale);
            o.Metallic = mask.r;
            o.Smoothness = mask.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
