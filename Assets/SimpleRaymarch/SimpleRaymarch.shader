Shader "Hidden/Toguchi/PostProcessing/SimpleRaymarch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

    TEXTURE2D_X(_MainTex);
    TEXTURE2D_X_FLOAT(_CameraDepthTexture);
    SAMPLER(sampler_CameraDepthTexture);

    half4 _MainTex_ST;
    float4 _MainTex_TexelSize;

    float4 _CamWorldSpace;
    float4x4 _CamFrustum, _CamToWorld;
    int _MaxIterations;
    float _MaxDistance;
    float _MinDistance;

    float4 _SpherePos;
    float _SphereRadius;

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

    float SdSphere(float3 position, float3 origin, float radius)
    {
        return distance(position, origin) - radius;
    }

    float DistanceField(float3 p)
    {
        return SdSphere(p, _SpherePos.xyz, _SphereRadius);
    }

    half4 SimpleRaymarching(float3 rayOrigin, float3 rayDirection, float depth)
    {
        half4 result = float4(1, 1, 1, 1);
        float t = 0.01;

        for(int i = 0; i < _MaxIterations; i++)
        {
            if(t > _MaxDistance || t >= depth)
            {
                result = float4(rayDirection, 0);
                break;
            }

            float3 p = rayOrigin + rayDirection * t;
            float d = DistanceField(p);

            if(d <= _MinDistance)
            {
                result = float4(1, 1, 1, 1);
                break;
            }

            t += d;
        }

        return result;
    }

    half4 Frag(RayVaryings input) : SV_Target
    {
        half4 color = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp , input.uv);

        float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_LinearClamp, input.uv).r;
        depth = Linear01Depth(depth, _ZBufferParams);
        depth *= length(input.ray);

        float3 rayOrigin = _CamWorldSpace;
        float3 rayDir = normalize(input.ray);
        float4 result = SimpleRaymarching(rayOrigin, rayDir, depth);
        
        color = half4(color * (1.0 - result.w) + result.xyz * result.w, 1.0);

       //  color.rgb = rayDir;
        
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
    }
}
