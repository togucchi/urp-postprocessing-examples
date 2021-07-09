Shader "Hidden/Toguchi/PostProcessing/Flare"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

    TEXTURE2D_X(_MainTex);

    float4 _FlareVector;
    float4 _FlareColor;
    float4 _ParaVector;
    float4 _ParaColor;

    half3 ApplyFlare(half3 color, float2 screenPos)
    {
        float2 flarePos = _FlareVector.xy;
        float2 paraPos = _ParaVector.xy;

        float flare = 1.0 - clamp(length(flarePos - screenPos) * _FlareVector.z, 0, 1);
        float para = 1.0 - clamp(length(paraPos - screenPos) * _ParaVector.z, 0, 1);

        color = color * lerp(float3(1, 1, 1), _ParaColor, para) + lerp(float3(0, 0, 0), _FlareColor, flare);
        return color;
    }

    half4 Frag(Varyings input) : SV_Target
    {
        half4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp , input.uv);
        color.rgb = ApplyFlare(color.rgb, (input.uv - 0.5) * 2.0);

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
            Name "Flare"

            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag
            ENDHLSL
        }
    }
}