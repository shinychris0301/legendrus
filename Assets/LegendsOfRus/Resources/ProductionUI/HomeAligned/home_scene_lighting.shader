Shader "LegendsOfRus/UI/HomeSceneLighting"
{
    Properties
    {
        [PerRendererData] _MainTex ("Original whole sprite", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Profile ("Hero 0 / foliage 1", Float) = 0
        _ShadowStrength ("Selected surface shadow", Range(0,0.4)) = 0.16
        _RimStrength ("Directional rim", Range(0,1)) = 0.45
        _GlowStrength ("Exterior light", Range(0,2)) = 0.9
        _NearRadius ("Near radius in source pixels", Float) = 6
        _FarRadius ("Far radius in source pixels", Float) = 16
        _BottomFade ("Existing lower fade fraction", Float) = 0.08
        _LightColor ("Warm reflected light", Color) = (1,0.56,0.16,1)
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
        // These two imported resources are whole, unpacked sprites. UV masks refer to that stable artwork.
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="False" }
        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        // Premultiplied RGB permits additive exterior light with zero alpha.
        // Match the current UGUI premultiplied surface blend, including destination alpha.
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 lighting : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 mask : TEXCOORD1;
                float2 lighting : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            sampler2D _MainTex;
            float4 _MainTex_TexelSize, _ClipRect;
            fixed4 _Color, _TextureSampleAdd, _LightColor;
            float _Profile, _ShadowStrength, _RimStrength, _GlowStrength, _NearRadius, _FarRadius, _BottomFade;
            float _UIMaskSoftnessX, _UIMaskSoftnessY;
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color; o.uv = v.uv;
                o.lighting = v.lighting.xz;
                float2 pixelSize = o.vertex.w / abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
                float4 rect = clamp(_ClipRect, -2e10, 2e10);
                o.mask = float4(v.vertex.xy * 2 - rect.xy - rect.zw,
                    0.25 / (0.25 * float2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));
                return o;
            }
            half SpriteAlpha(float2 uv)
            {
                float2 inside = step(0, uv) * step(uv, 1);
                return (tex2D(_MainTex, saturate(uv)) + _TextureSampleAdd).a * inside.x * inside.y;
            }
            half RingAlpha(float2 uv, float radius)
            {
                float2 d = _MainTex_TexelSize.xy * radius;
                return (SpriteAlpha(uv + float2(d.x, 0)) + SpriteAlpha(uv - float2(d.x, 0)) +
                    SpriteAlpha(uv + float2(0, d.y)) + SpriteAlpha(uv - float2(0, d.y)) +
                    SpriteAlpha(uv + d * .7071) + SpriteAlpha(uv - d * .7071) +
                    SpriteAlpha(uv + float2(d.x, -d.y) * .7071) +
                    SpriteAlpha(uv + float2(-d.x, d.y) * .7071)) * .125;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                half strength = saturate(i.lighting.x);
                half foliage = step(.5, _Profile);
                float y = 1 - uv.y;
                // The diagonal split follows the accepted perched bird's neck, wing and tail.
                half bird = smoothstep(.57 + .20 * smoothstep(.15, .55, y) - .015,
                    .57 + .20 * smoothstep(.15, .55, y) + .035, uv.x);
                half clothing = smoothstep(.24, .36, y) * (1 - bird);
                half heroRim = (1 - bird) * (1 - smoothstep(.38, .72, y));
                half4 color;
                half coverage;
                if (i.lighting.y > .5)
                {
                    half alpha = SpriteAlpha(uv);
                    half spread = RingAlpha(uv, _NearRadius) * .65 + RingAlpha(uv, _FarRadius) * .35;
                    half selection = lerp(heroRim * .32 + bird, .16, foliage);
                    half lowerFade = smoothstep(0, max(_BottomFade, .0001), uv.y);
                    coverage = saturate(spread - alpha) * (1 - alpha) * selection * _GlowStrength * strength * i.color.a * lowerFade;
                    color = half4(_LightColor.rgb * i.color.rgb * coverage, 0);
                }
                else
                {
                    fixed4 source = tex2D(_MainTex, uv) + _TextureSampleAdd;
                    color = source * i.color;
                    half lightness = max(source.r, max(source.g, source.b));
                    half recess = 1 - smoothstep(.40, .92, lightness);
                    color.rgb *= 1 - lerp(clothing, 1, foliage) * recess * _ShadowStrength * strength;
                    half edge = saturate(source.a - SpriteAlpha(uv + _MainTex_TexelSize.xy * float2(-3, 3)));
                    half rimSelection = lerp(heroRim + bird * .22, .7, foliage);
                    color.rgb = saturate(color.rgb + _LightColor.rgb * edge * rimSelection *
                        _RimStrength * strength * (1 - lightness * .65));
                    // The original surface alpha, vertex RGB tint and lower-fade mesh stay intact.
                    coverage = color.a;
                    color.rgb *= color.a;
                }
                #ifdef UNITY_UI_CLIP_RECT
                half2 mask = saturate((_ClipRect.zw - _ClipRect.xy - abs(i.mask.xy)) * i.mask.zw);
                color *= mask.x * mask.y;
                coverage *= mask.x * mask.y;
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                clip(coverage - .001);
                #endif
                return color;
            }
            ENDCG
        }
    }
}
