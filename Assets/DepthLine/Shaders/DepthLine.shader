Shader "Hidden/Toguchi/PostProcessing/DepthLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            sampler2D _CameraDepthTexture;

            float _DepthLineBias;
            float _DepthLineMaxWeight;
            float _DepthLineSamplingRate;
            float _DepthLineWidth;

            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float depth = tex2D(_CameraDepthTexture, i.uv).r;

                float width = _DepthLineWidth;
                float maxWeight = _DepthLineMaxWeight;
                float texelWidth = width *  _MainTex_TexelSize.xy;

                float2 rightSample = float2(1.0, 0) * texelWidth + i.uv;
                float rightDepth = tex2D(_CameraDepthTexture, rightSample).r;

                float2 leftSample = float2(-1.0, 0) * texelWidth + i.uv;
                float leftDepth = tex2D(_CameraDepthTexture, leftSample).r;

                float2 topSample = float2(0, -1.0) * texelWidth + i.uv;
                float topDepth = tex2D(_CameraDepthTexture, topSample).r;

                float2 bottomSample = float2(0, 1.0) * texelWidth + i.uv;
                float bottomDepth = tex2D(_CameraDepthTexture, bottomSample).r;

                float final = clamp(depth - rightDepth, -maxWeight, maxWeight)
                + clamp(depth - leftDepth, -maxWeight, maxWeight)
                + clamp(depth - topDepth, -maxWeight, maxWeight)
                + clamp(depth - bottomDepth, -maxWeight, maxWeight);
                final = pow(final, _DepthLineBias) * _DepthLineSamplingRate;

                col = lerp(col, float4(0, 0, 0, 1), saturate(final));

                return col;
            }
            ENDCG
        }
    }
}
