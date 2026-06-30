Shader "BulletHeaven/BackgroundGrid"
{
    Properties
    {
        _BgColor   ("Background", Color)         = (0.07, 0.08, 0.11, 1)
        _GridColor ("Grid Line",  Color)         = (0.17, 0.19, 0.25, 1)
        _GridSize  ("Cell Size (world units)", Float)   = 2.0
        _LineWidth ("Line Width (px)", Range(0.5, 6)) = 1.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Background" }
        ZWrite Off
        ZTest Always

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 posOS : POSITION; };
            struct Varyings   { float4 posCS : SV_POSITION; float2 worldXY : TEXCOORD0; };

            CBUFFER_START(UnityPerMaterial)
                half4  _BgColor;
                half4  _GridColor;
                float  _GridSize;
                float  _LineWidth;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs p = GetVertexPositionInputs(IN.posOS.xyz);
                OUT.posCS   = p.positionCS;
                OUT.worldXY = p.positionWS.xy;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv    = IN.worldXY / _GridSize;
                float2 fw    = fwidth(uv);
                float2 dist  = min(frac(uv), 1.0 - frac(uv));
                float2 lines = dist / fw;
                float  t     = 1.0 - saturate(min(lines.x, lines.y) / _LineWidth);
                return lerp(_BgColor, _GridColor, t);
            }
            ENDHLSL
        }
    }
}