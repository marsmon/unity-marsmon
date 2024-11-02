// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Apotheosis/UI/Mask"
{
    Properties
    {
        [PerRendererData] [MainTexture] _MainTex("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MaskTex("Mask Texture", 2D) = "white" {}
        [PerRendererData][Toggle] _InverseMask("Inverse Mask", float) = 0
        _Color("Tint", Color) = (1,1,1,1)
        _Rotate("_Rotate",Range(-360,360)) = 0.0

        [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp("BlendOp", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("Src Blend Mode", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendDst("Dst Blend Mode", Float) = 10

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Stencil
            {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }

            Cull Off
            Lighting Off
            ZWrite Off
            BlendOp[_BlendOp]
            Blend[_BlendSrc][_BlendDst]
            ZTest[unity_GUIZTestMode]
            ColorMask[_ColorMask]

            Pass
            {
                Name "Default"
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
                #pragma multi_compile_local _ TRANSFORM_ROTATE_ON

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float4 texcoord  : TEXCOORD0;
                    half4  mask : TEXCOORD2;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                sampler2D _MaskTex;
                fixed4 _Color;
                fixed4 _TextureSampleAdd;
                float4 _ClipRect;
                float4 _MainTex_ST;
                float4 _MaskTex_ST;
                float4 _MainAtlasMapping;
                float4 _MaskAtlasMapping;
                float _UIMaskSoftnessX;
                float _UIMaskSoftnessY;
                float _Rotate;
                float _Radius;
                float _InverseMask;

                float4 _MaskPosition;
                float4 _MaskX;
                float4 _MaskY;
                #define XAXIS _MaskX.xyz
                #define YAXIS _MaskY.xyz
                #define XLEN _MaskX.w
                #define YLEN _MaskY.w

                float2 RotateInDegrees(float2 dir, float degrees)
                {
                    float alpha = degrees * UNITY_PI / 180.0;
                    float sina, cosa;
                    sincos(alpha, sina, cosa);
                    float2x2 mat = float2x2(cosa, -sina, sina, cosa);
                    return float2(mul(mat, dir));
                }

                v2f vert(appdata_t v)
                {
                    v2f OUT;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                    float4 vPosition = UnityObjectToClipPos(v.vertex);
                    OUT.vertex = vPosition;

                    float2 pixelSize = vPosition.w;
                    pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                    OUT.texcoord.xy = v.texcoord.xy;
                    #if TRANSFORM_ROTATE_ON
                        float3 posw = mul(UNITY_MATRIX_M, float4(v.vertex.xyz, 1.0)).xyz;
                        float3 delta = posw - _MaskPosition.xyz;
                        OUT.texcoord.z = dot(delta, XAXIS) / XLEN;
                        OUT.texcoord.w = dot(delta, YAXIS) / YLEN;
                    #else
                        OUT.texcoord.zw = (v.texcoord.xy - _MainAtlasMapping.xy) / _MainAtlasMapping.zw;
                    #endif

                    float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                    OUT.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                    OUT.color = v.color * _Color;
                    return OUT;
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    half4 color = IN.color * (tex2D(_MainTex, TRANSFORM_TEX(IN.texcoord.xy, _MainTex)) + _TextureSampleAdd);

                    float2 uv_mask = IN.texcoord.zw;

                    #if !TRANSFORM_ROTATE_ON
                        float2 center = float2(0.5,0.5);
                        uv_mask = RotateInDegrees(uv_mask - center, _Rotate) + center;
                        uv_mask = uv_mask * _MaskTex_ST.xy + _MaskTex_ST.zw;
                    #endif

                    uv_mask = saturate(uv_mask);
                    uv_mask = uv_mask * _MaskAtlasMapping.zw + _MaskAtlasMapping.xy;//uv映射到各自图集

                    half alpha = tex2D(_MaskTex, uv_mask).a;

                    alpha = lerp(alpha, 1.0 - alpha, _InverseMask);
                    // return step(IN.texcoord3.x, _Radius);
                    alpha *= step(distance(IN.texcoord.zw, float2(0.5,0.5)), _Radius);
                    color.a *= alpha;
                    //color.a = min(color.a, alpha);

                    #ifdef UNITY_UI_CLIP_RECT
                        half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                        color.a *= m.x * m.y;
                    #endif

                    #ifdef UNITY_UI_ALPHACLIP
                        clip(color.a - 0.001);
                    #endif

                    color.rgb *= color.a;

                    return color;
                }
                ENDCG
            }
        }
}
