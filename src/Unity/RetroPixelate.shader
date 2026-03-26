Shader "Hidden/RetroPixelate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Float) = 4
        _ColorBits ("Color Bits Per Channel", Float) = 5
        _ScanlineOpacity ("Scanline Opacity", Float) = 0.3
        _UseDithering ("Use Dithering", Integer) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelSize;
            float _ColorBits;
            float _ScanlineOpacity;
            int _UseDithering;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3 ReduceColor(float3 color, float bits)
            {
                float levels = pow(2.0, bits);
                return floor(color * levels) / (levels - 1.0);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Pixelate
                float2 pixelCoord = floor(i.uv * _ScreenParams.xy / _PixelSize) * _PixelSize;
                float2 pixelUV = pixelCoord / _ScreenParams.xy;
                
                fixed4 col = tex2D(_MainTex, pixelUV);

                // Reduce color palette
                col.rgb = ReduceColor(col.rgb, _ColorBits);

                // Add scanlines
                float scanline = mod(pixelCoord.y, 2.0) < 1.0 ? 1.0 : (1.0 - _ScanlineOpacity);
                col.rgb *= scanline;

                return col;
            }
            ENDCG
        }
    }
}
