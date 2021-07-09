Shader "Hidden/Toguchi/PostProcessing/LightShaft"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

    TEXTURE2D_X(_MainTex);
    TEXTURE2D_X_FLOAT(_CameraDepthTexture);
    SAMPLER(sampler_CameraDepthTexture);

    TEXTURE2D_X(_LightShaftTempTex);

    half4 _MainTex_ST;
    float4 _MainTex_TexelSize;

    float4 _CamWorldSpace;
    float4x4 _CamFrustum, _CamToWorld;
    int _MaxIterations;
    float _MaxDistance;
    float _MinDistance;

    float _Intensity;

    struct RayVaryings
    {
        float4 positionCS    : SV_POSITION;
        float2 uv            : TEXCOORD0;
        float4 ray           : TEXCOORD1;
    };

    RayVaryings Vert_Ray(Attributes input)
    {
        RayVaryings output;
        output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
        output.uv = input.uv;

        int index = output.uv.x + 2 * output.uv.y;
        output.ray = _CamFrustum[index];
        
        return output;
    }

    float GetRandomNumber(float2 texCoord, int Seed)
    {
        return frac(sin(dot(texCoord.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
    }

    half4 SimpleRaymarching(float3 rayOrigin, float3 rayDirection, float depth)
    {
        half4 result = float4(_MainLightColor.xyz, 1) * _Intensity;
        
        float step = _MaxDistance / _MaxIterations;
        float t = _MinDistance + step * GetRandomNumber(rayDirection, _Time.y * 100);
        // float t = _MinDistance;
        float alpha = 0;

        for(int i = 0; i < _MaxIterations; i++)
        {
            if(t > _MaxDistance || t >= depth)
            {
                break;
            }
            
            float3 p = rayOrigin + rayDirection * t;
            // float d = DistanceField(p);

            float4 shadowCoord = TransformWorldToShadowCoord(p);
            float shadow = SAMPLE_TEXTURE2D_SHADOW(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, shadowCoord);
            if(shadow >= 1)
            {
                alpha += step * 0.2;
            }

            t += step;
        }

        result.a *= saturate(alpha);

        return result;
    }

    half4 Frag(RayVaryings input) : SV_Target
    {
        float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_LinearClamp, input.uv).r;
        depth = Linear01Depth(depth, _ZBufferParams);
        depth *= length(input.ray);

        float3 rayOrigin = _CamWorldSpace;
        float3 rayDir = normalize(input.ray);
        float4 result = SimpleRaymarching(rayOrigin, rayDir, depth);
        
        return result;
    }

    half4 Frag_Combine(Varyings input) : SV_Target
    {
        half4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp , input.uv);
        half4 shaft = SAMPLE_TEXTURE2D_X(_LightShaftTempTex, sampler_LinearClamp, input.uv);

        color.rgb = color.rgb * (1 - shaft.a) + shaft.rgb * shaft.a;
        
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
                #pragma vertex Vert_Ray
                #pragma fragment Frag
            ENDHLSL
        }
        
        Pass
        {
            Name "Combine"
            
            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag_Combine
            ENDHLSL
        }
    }
}
