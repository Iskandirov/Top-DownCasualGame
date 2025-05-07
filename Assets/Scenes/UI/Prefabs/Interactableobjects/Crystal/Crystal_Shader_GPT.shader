Shader "Custom/FresnelCrystal"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _GlowColor ("Glow Color", Color) = (0.5, 0.8, 1, 1)
        _FresnelPower ("Fresnel Power", Float) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            fixed4 _BaseColor;
            fixed4 _GlowColor;
            float _FresnelPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(UnityWorldSpaceViewDir(v.vertex.xyz));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float fresnel = pow(1.0 - saturate(dot(i.worldNormal, i.viewDir)), _FresnelPower);
                fixed4 color = _BaseColor + fresnel * _GlowColor;
                return color;
            }
            ENDHLSL
        }
    }
}