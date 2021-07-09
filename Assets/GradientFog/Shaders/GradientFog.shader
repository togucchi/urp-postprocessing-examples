Shader "Hidden/Toguchi/PostProcessing/GradientFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

    TEXTURE2D_X(_MainTex);
    TEXTURE2D_X(_CameraDepthTexture);

    TEXTURE2D_X(_RampTex);
    float _Intensity;

    half4 Frag(Varyings input) : SV_Target
    {
        half4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp , input.uv);
        float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_LinearClamp, input.uv).r;
        depth = Linear01Depth(depth, _ZBufferParams);

        half4 ramp = SAMPLE_TEXTURE2D_X(_RampTex, sampler_LinearClamp, float2(depth, 0));
        color.rgb = lerp(color.rgb, ramp.rgb, _Intensity * ramp.a);
        
        return color;
    }
ENDHLSL
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZTest Always ZWrite Off Cull Off

        Pass
        {
            Name "GradientFog"

            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
