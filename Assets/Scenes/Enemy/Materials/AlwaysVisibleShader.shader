Shader "Custom/AlwaysVisibleStable"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Name "AlwaysVisiblePass"
            Tags { "LightMode"="Always" } // <- Ключова зміна!

            ZWrite On
            ZTest Always
            Cull Off
            Lighting Off
            ColorMask RGBA
            Blend One Zero

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
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(1, 1, 1, 1); // білий піксель
            }
            ENDCG
        }
    }
}