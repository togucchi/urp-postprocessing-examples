Shader "Toguchi/PostProcessing/StarGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        CGINCLUDE

        #include "UnityCG.cginc"

        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;
        float4 _Parameter;

        #define BRIGHTNESS_THRESHOLD _Parameter.x
        #define INTENSITY            _Parameter.y
        #define ATTENUATION          _Parameter.z

        ENDCG

        Pass
        {
            Name "Brightness"

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return max(col - BRIGHTNESS_THRESHOLD, 0) * INTENSITY;
                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "Blur"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            int    _Iteration;
            float2 _Offset;

            struct v2f
            {
                float4 pos    : SV_POSITION;
                half2  uv     : TEXCOORD0;
                half   power  : TEXCOORD1;
                half2  offset : TEXCOORD2;
            };

            v2f vert(appdata_img v)
            {
                v2f o;

                o.pos    = UnityObjectToClipPos (v.vertex);
                o.uv     = v.texcoord;

                o.power  = pow(4, _Iteration - 1);
                o.offset = _MainTex_TexelSize.xy * _Offset * o.power;

                return o;
            }

            float4 frag(v2f input) : SV_Target
            {
                half4 color = half4(0, 0, 0, 0);
                half2 uv    = input.uv;


                for (int j = 0; j < 4; j++)
                {
                    color += saturate(tex2D(_MainTex, uv) * pow(ATTENUATION, input.power * j));
                    uv += input.offset;
                }

                return color;
            }

            ENDCG
        }

        Pass
        {
            Name "BlurComposite"

            Blend OneMinusDstColor One

            CGPROGRAM

            #pragma  vertex vert_img
            #pragma  fragment frag

            fixed4 frag(v2f_img input) : SV_Target
            {
                return tex2D(_MainTex, input.uv);
            }

            ENDCG
        }

        Pass
        {
            Name "Composite"

            CGPROGRAM

            #pragma  vertex vert_img
            #pragma  fragment frag

            sampler2D _CompositeTex;

            fixed4 frag(v2f_img input) : SV_Target
            {
                float4 mainColor = tex2D(_MainTex, input.uv);
                float4 compositeColor = tex2D(_CompositeTex, input.uv);
                return (mainColor + compositeColor);
            }

            ENDCG
        }
    }
}
