// Shader Graph code representation for 2D top-down water
// (HLSL-like pseudocode for Shader Graph nodes)

Shader "Custom/TopDownWater2D"
{
    Properties
    {
        _WaterColor("Water Color", Color) = (0.2, 0.5, 0.8, 1)
        _FlowSpeed("Flow Speed", Float) = 0.1
        _WaveStrength("Wave Strength", Float) = 0.05
        _WaveIntensity("Wave Intensity", Float) = 0.2
        _FlowDirection("Flow Direction", Vector) = (0, -1, 0, 0)
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _ShowWaves("Show Waves", Float) = 1
        _EdgeLightening("Edge Lightening", Float) = 0.3
        _EdgeFalloff("Edge Falloff", Float) = 3.0
        _DistortionSpeed("Distortion Speed", Float) = 2.0
        _DistortionStrength("Distortion Strength", Float) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _NoiseTex;
            float4 _WaterColor;
            float _FlowSpeed;
            float _WaveStrength;
            float _WaveIntensity;
            float4 _FlowDirection;
            float _ShowWaves;
            float _EdgeLightening;
            float _EdgeFalloff;
            float _DistortionSpeed;
            float _DistortionStrength;
            float4 _NoiseTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

           fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
            
                // Apply Flow
                uv += normalize(_FlowDirection.xy) * _Time.y * _FlowSpeed;
            
                // Sinusoidal UV Distortion
                uv.x += sin((uv.y + _Time.y * _DistortionSpeed) * 6.283) * _DistortionStrength;
                uv.y += cos((uv.x + _Time.y * _DistortionSpeed) * 6.283) * _DistortionStrength;
            
                // --- Apply Tiling and Offset ---
                float2 noiseUV = uv * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
                //---------------------------------
            
                // Wave Noise
                float2 noise = tex2D(_NoiseTex, noiseUV).rg - 0.5;
                uv += noise * _WaveStrength * _ShowWaves;
            
                float noiseSample = tex2D(_NoiseTex, uv * _NoiseTex_ST.xy + _NoiseTex_ST.zw).r;
            
                fixed4 finalColor = _WaterColor;
                finalColor.rgb += noiseSample * _WaveIntensity * _ShowWaves;
            
                // Edge Lightening
                float edgeFactor = saturate(abs(i.uv.y - 0.5) * _EdgeFalloff);
                float lightening = (1.0 - edgeFactor) * _EdgeLightening;
                finalColor.rgb += lightening;
            
                return finalColor;
            }
            ENDCG
        }
    }
}
