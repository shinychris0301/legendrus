Shader "LegendsOfRus/UI/HomePlayGold"
{
    Properties
    {
        [PerRendererData] _MainTex ("Font atlas", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Highlight ("Bevel highlight", Color) = (1,0.94,0.68,1)
        _Recess ("Bevel recess", Color) = (0.30,0.13,0.025,1)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float4 uv : TEXCOORD0;
                float4 bounds : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 uv : TEXCOORD0;
                float4 mask : TEXCOORD1;
                float4 bounds : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            sampler2D _MainTex;
            fixed4 _Color, _TextureSampleAdd, _Highlight, _Recess;
            float4 _ClipRect;
            float _UIMaskSoftnessX, _UIMaskSoftnessY;
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
                o.uv = v.uv;
                o.bounds = v.bounds;
                float2 pixelSize = o.vertex.w / abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
                float4 rect = clamp(_ClipRect, -2e10, 2e10);
                o.mask = float4(v.vertex.xy * 2 - rect.xy - rect.zw,
                    0.25 / (0.25 * float2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));
                return o;
            }
            half GlyphAlpha(float2 uv, float4 bounds, float2 pixel)
            {
                // A continuous half-pixel border avoids a hard comparison at
                // packed atlas boundaries. Derive spacing from this glyph's UV
                // step, rather than the material's shared atlas texture size.
                float2 inside = saturate(min((uv - bounds.xy) / pixel, (bounds.zw - uv) / pixel) + .5);
                uv = clamp(uv, bounds.xy + pixel * .5, bounds.zw - pixel * .5);
                return (tex2D(_MainTex, uv) + _TextureSampleAdd).a * inside.x * inside.y;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 atlas = tex2D(_MainTex, i.uv.xy) + _TextureSampleAdd;
                fixed4 color = atlas * i.color;
                // Light is evaluated inside the existing glyph alpha: no stroke
                // expansion, raster word image, or change to its silhouette.
                float2 pixel = max(abs(i.uv.zw) / 3.5, .00000001);
                half upper = GlyphAlpha(i.uv.xy + i.uv.zw, i.bounds, pixel);
                half lower = GlyphAlpha(i.uv.xy - i.uv.zw, i.bounds, pixel);
                half bevel = saturate(dot(abs(i.uv.zw), float2(1e7, 1e7)));
                color.rgb = lerp(color.rgb, _Highlight.rgb, saturate(atlas.a - upper) * .95 * bevel);
                color.rgb = lerp(color.rgb, _Recess.rgb, saturate(atlas.a - lower) * .85 * bevel);
                // Shadow vertices have a zero sample step and keep their color.
                #ifdef UNITY_UI_CLIP_RECT
                half2 mask = saturate((_ClipRect.zw - _ClipRect.xy - abs(i.mask.xy)) * i.mask.zw);
                color.a *= mask.x * mask.y;
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - .001);
                #endif
                return color;
            }
            ENDCG
        }
    }
}
