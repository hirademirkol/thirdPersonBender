Shader "Unlit/Sonar"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Origin ("Origin", Vector) = (0,0,0,0)
        _Speed ("Speed", Range(0.0, 10.0)) = 5.0
        _Phase ("Phase", Float) = 0.0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 dist : FLOAT4;
            };

            float4 _Color;
            float4 _Origin;
            float _Distance;
            float _Speed;
            float _Phase;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.dist = mul(unity_ObjectToWorld, v.vertex) - _Origin;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {   
                float4 color;
                float distance = length(i.dist);
                float col = sin(-_Time.y * _Speed + distance * _Phase);
                color = pow(col,3); 
                return color;
            }
            ENDCG
        }
    }
}