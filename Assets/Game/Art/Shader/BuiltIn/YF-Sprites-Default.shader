// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "YoFi/Sprites/Default"
{
    Properties
    {
        [Header (Fx)]
        _FillColor("Fill color(fx)",Color) = (1,1,1,0.5)
		[PerRendererData][Enum(Default,0,Add,1,Multiplica,2,Blend,3,Alpha,4)] _Fillblend("Blend Value(Fill,fx)",float) = 0

        [Header (Base)]
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

        [Header(Custom Bloom)]        
        [Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 1
        _CBLocalThreshhold("CB Threshhold",range(0.0,1.0)) = 1
        _CBLocalColor("CB Color",Color) = (1,1,1,1)
        _CBLocalLight("CB 亮度",float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment Customfrag// SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
            #include "..\Common\Included\YFEffectCGForYFPost.cginc"

            fixed4 _FillColor;
            fixed _Fillblend;

            inline float4 blendmodel(in float4 finalcol) {
                float4 fxcol = finalcol.a * _FillColor;
				// float4 fxcol = 0;
				// #if defined(_STRAIGHT_ALPHA_INPUT)
				// 	fxcol = finalcol.a * _FillColor;
				// #else
				// 	fxcol = _FillColor;
				// #endif
				
				if (_Fillblend == 1) {
					return float4(fxcol.rgb + finalcol.rgb, finalcol.a);
				}
				else if (_Fillblend == 2)
				{
					return float4(fxcol.rgb * finalcol.rgb, finalcol.a);
				}
				else if (_Fillblend == 3) {
					return  float4(lerp(finalcol.rgb, fxcol.rgb, fxcol.a), finalcol.a);
				}
				else if (_Fillblend == 4) {
					return  float4(finalcol.rgb, fxcol.a * finalcol.a);
				}
				else if (_Fillblend == 5) {
					fixed3 v = dot(finalcol.rgb, unity_ColorSpaceGrey);
					return  float4(v, finalcol.a);
				}
				else {
					return finalcol;
				}
			}

            fixed4 Customfrag (v2f i) : SV_Target
            {
                fixed4 finalcol = SpriteFrag(i);//获取源shader的输出颜色
                finalcol = blendmodel(finalcol);//加特效
                finalcol = BloomOutfrag(saturate(finalcol));//加bloom
                return finalcol;
            }
        ENDCG
        }
    }
}
