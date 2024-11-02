// This is a premultiply-alpha adaptation of the built-in Unity shader "UI/Default" in Unity 5.6.2 to allow Unity UI stencil masking.

Shader "YoFi/Spine/SkeletonGraphic"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[Toggle(_CANVAS_GROUP_COMPATIBLE)] _CanvasGroupCompatible("CanvasGroup Compatible", Int) = 1
		_Color ("Tint", Color) = (1,1,1,1)
		_ChangeColorRC("Change color(fx R Channel)",Color) = (1,1,1,1)
		[Toggle]_isChangeCol("Is Change Color",float) = 0

		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector] _Stencil ("Stencil ID", Float) = 0
		[HideInInspector][Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255

		[HideInInspector] _ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

		// Outline properties are drawn via custom editor.
		[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
		[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
		[HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
		[HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
		[HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
		[HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
		[HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0

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
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Normal"

			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma shader_feature _ _CANVAS_GROUP_COMPATIBLE
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "..\Common\Included\YFEffectCGForYFPost.cginc"

			float4 _ChangeColorRC, _ChangeColorGC, _ChangeColorBC, _FillColor;
			float _Fillblend,_isFront,_isChangeCol;
            
			
			#pragma multi_compile __ UNITY_UI_ALPHACLIP


			inline half4 PMAGammaToTargetSpace(half4 gammaPMAColor) {
				#if UNITY_COLORSPACE_GAMMA
					return gammaPMAColor;
				#else
					return gammaPMAColor.a == 0 ?
					half4(GammaToLinearSpace(gammaPMAColor.rgb), gammaPMAColor.a) :
					half4(GammaToLinearSpace(gammaPMAColor.rgb / gammaPMAColor.a) * gammaPMAColor.a, gammaPMAColor.a);
				#endif
			};
			inline half3 TargetToGammaSpace(half3 targetColor) {
				#if UNITY_COLORSPACE_GAMMA
					return targetColor;
				#else
					return LinearToGammaSpace(targetColor);
				#endif
			};

			struct VertexInput {
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput {
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			VertexOutput vert (VertexInput IN) {
				VertexOutput OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
				OUT.texcoord = IN.texcoord;

				#ifdef UNITY_HALF_TEXEL_OFFSET
					OUT.vertex.xy += (_ScreenParams.zw-1.0) * float2(-1,1);
				#endif

				#ifdef _CANVAS_GROUP_COMPATIBLE				
					half4 vertexColor = IN.color;
					// CanvasGroup alpha sets vertex color alpha, but does not premultiply it to rgb components.
					vertexColor.rgb *= vertexColor.a;
					// Unfortunately we cannot perform the TargetToGamma and PMAGammaToTarget transformations,
					// as these would be wrong with modified alpha.
				#else
					// Note: CanvasRenderer performs a GammaToTargetSpace conversion on vertex color already,
					// however incorrectly assuming straight alpha color.
					float4 vertexColor = PMAGammaToTargetSpace(half4(TargetToGammaSpace(IN.color.rgb), IN.color.a));
				#endif
				OUT.color = vertexColor * float4(_Color.rgb * _Color.a, _Color.a); // Combine a PMA version of _Color with vertexColor.
				return OUT;
			}

			sampler2D _MainTex;


			inline float4 blendmodel(in float4 finalcol) {
				float4 fxcol = 0;
				#if defined(_STRAIGHT_ALPHA_INPUT)
					fxcol = finalcol.a * _FillColor;
				#else
					fxcol = _FillColor;
				#endif
				
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

			float4 frag(VertexOutput IN) : SV_Target{
				float4 finalcol = 0;
				float4 texColor = tex2D(_MainTex, IN.texcoord);
				_ChangeColorRC.rgb *= _ChangeColorRC.a * 2;
				_ChangeColorGC.rgb *= _ChangeColorGC.a * 2;
				_ChangeColorBC.rgb *= _ChangeColorBC.a * 2;
				
				texColor.rgb = lerp(texColor.rgb, saturate(texColor.rgb * _ChangeColorRC.rgb), lerp(0, IN.color.r,_isChangeCol));
				fixed a = lerp(1.0, IN.color.g,_isFront) * texColor.a;
				texColor.a = IN.color.a * a ;
				finalcol = texColor;
				// 获取特效使用的全覆盖颜色
				finalcol = blendmodel(finalcol);
				#if defined(_STRAIGHT_ALPHA_INPUT)
					finalcol.rgb *= finalcol.a;
				#endif
				return BloomOutfrag(saturate(finalcol));
			}

			ENDCG
		}
	}
	CustomEditor "SpineShaderWithOutlineGUI"
}
