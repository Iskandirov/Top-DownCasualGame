Shader "Custom/DepthMask" 
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Lighting Off
        ZWrite On
        ColorMask 0
        Pass {}
    }
}
