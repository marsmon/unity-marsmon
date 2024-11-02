Shader "YoFi/Spine/Skeleton" {
	Properties{
		[Header(Custom)]
		_ChangeColorRC("Change color(fx R Channel)",Color) = (1,1,1,1)
		// _ChangeColorGC("Change color(fx G Channel)",Color) = (1,1,1,1)
		
		_ChangeColorBC("Change color(fx B Channel)",Color) = (1,1,1,1)
		_FillColor("Fill color(fx)",Color) = (1,1,1,0.5)
		[Enum(Default,0,Add,1,Multiplica,2,Blend,3,Alpha,4)] _Fillblend("Blend Value(Fill,fx)",float) = 0

		// [HideInInspector] _custom_time("当前_Time 时间(需传入)",float) = 0
		[Space(5)][Header(Source)]
		_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex("Main Texture", 2D) = "black" {}
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[HideInInspector] _StencilRef("Stencil Reference", float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", float) = 8 // Set to Always as default
		[Toggle]_isFront("Is Front",float) = 0
		[Toggle]_isChangeCol("Is Change Color",float) = 0

		// Outline properties are drawn via custom editor.
		/*
		[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
		[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
		[HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
		[HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
		[HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
		[HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", float) = 1
		[HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0
		*/ 

		[Header(Custom Bloom)]        
        [Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 1
        _CBLocalThreshhold("CB Threshhold",range(0.0,1.0)) = 1
        _CBLocalColor("CB Color",Color) = (1,1,1,1)
        _CBLocalLight("CB 亮度",float) = 1
		
		[Header(BottomFade)]
		[Toggle] _BottomFade_ON("BottomFade ON", Float) = 0
		_BottomFadeHeight("Bottom Fade Height", Float) = 0
		_BottomFade("Bottom Fade", range(0.01,1.0)) = 0.2
		
		
		[Header(Dissolve)]
		[Toggle] _Dissolve_ON("Dissolve ON", Float) = 0
		_DissolveFactor("Dissolve Factor", Range( 0 , 1)) = 0
		_SpineHeight("Spine Height", Float) = 1
		_SpineBottom("Spine Bottom", Float) = 0
		_RangFade("Rang Fade", Range( 0, 1)) = 0.3
		_DissolveTex("Dissolve Tex", 2D) = "white" {}
		_DissolveMaskTex("Dissolve Mask Tex", 2D) = "black" {}
		[HDR]_DissolveColor("Dissolve Color", Color) = (1,1,1,1)
        _DissolveSoft("Dissolve Soft", Range( 0 , 0.5)) = 0.1
        _DissolveWide("Dissolve Wide", Range( 0 , 1)) = 0.05
        _DissolveUSpeed("Dissolve U Speed", Float) = 0.0
        _DissolveVSpeed("Dissolve V Speed", Float) = 0.5
		
	}

	SubShader{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "YFCB" = "Spine_Skeleton"}

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		ZTest Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Stencil {
			Ref[_StencilRef]
			Comp[_StencilComp]
			Pass Keep
		}

		Pass {
			Name "Normal"

			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"			
			#include "..\Common\Included\YFEffectCGForYFPost.cginc"

			sampler2D _MainTex;
			//Dite change
			float4 _ChangeColorRC, _ChangeColorGC, _ChangeColorBC, _FillColor;
			float _Fillblend,_isFront,_isChangeCol;

			float _SpineHeight;
			float _SpineBottom;
			float _Dissolve_ON;
			float _DissolveFactor;
			float _DissolveSoft;
			float _DissolveWide;
			float _RangFade;
			float _DissolveUSpeed;
			float _DissolveVSpeed;
			float4 _DissolveColor;
			float4 _DissolveTex_ST;
			sampler2D _DissolveTex;
			sampler2D _DissolveMaskTex;

			float _BottomFade_ON;
			float _BottomFadeHeight;
			float _BottomFade;

			void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
			{
			    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
			}

			// /*if(Delytime >= 0){
				// 	this.m_Material.SetFloat("_custom_time", Time.timeSinceLevelLoad + Delytime);
				// }else{
				// 	this.m_Material.SetFloat("_custom_time", -1);
			// }*/
			// float _custom_time; //(max(0,_Time.y - _custom_time) * -_speed );

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
			inline float isChannel(float3 c) {
				return step(1.0, c.r) * step(c.g, 0.0) * step(c.b, 0.0);
			}

			inline float4 PMAGammaToTargetSpace(float4 gammaPMAColor) {
				#if UNITY_COLORSPACE_GAMMA
					return gammaPMAColor;
				#else
					return gammaPMAColor.a == 0 ?
					float4(GammaToLinearSpace(gammaPMAColor.rgb), gammaPMAColor.a) :
					float4(GammaToLinearSpace(gammaPMAColor.rgb / gammaPMAColor.a) * gammaPMAColor.a, gammaPMAColor.a);
				#endif
			}

			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

			struct VertexOutput {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				//float3 clannel : TEXCOORD1;
				float4 vertexColor : COLOR;
				float3 positionOS  : TEXCOORD1;
			};

			VertexOutput vert(VertexInput v) {
				VertexOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				float4 c = PMAGammaToTargetSpace(v.vertexColor);
				o.vertexColor.r = c.r ;///isChannel(c.rgb);
				o.vertexColor.g = c.g ;//isChannel(c.grb);
				o.vertexColor.b = c.b ;// isChannel(c.brg);
				o.vertexColor.a = c.a;
				//o.vertexColor.rgb = lerp(1.0, o.vertexColor.rgb, o.clannel);
				o.positionOS = v.vertex.xyz;
				return o;
			}

			float4 frag(VertexOutput i) : SV_Target{
				float4 finalcol = 0;
				float4 texColor = tex2D(_MainTex, i.uv.xy);
				_ChangeColorRC.rgb *= _ChangeColorRC.a * 2;
				_ChangeColorGC.rgb *= _ChangeColorGC.a * 2;
				_ChangeColorBC.rgb *= _ChangeColorBC.a * 2;
				// #if defined(_STRAIGHT_ALPHA_INPUT)
				// 	texColor.rgb *= texColor.a;					
				// #endif

				texColor.rgb = lerp(texColor.rgb, saturate(texColor.rgb * _ChangeColorRC.rgb), lerp(0, i.vertexColor.r,_isChangeCol));
				// texColor.rgb = lerp(texColor.rgb, saturate(texColor.rgb * _ChangeColorGC.rgb), i.vertexColor.g);
				// texColor.rgb = lerp(texColor.rgb, saturate(texColor.rgb * _ChangeColorBC.rgb), i.vertexColor.b);
				fixed a = lerp(1.0, i.vertexColor.g,_isFront) * texColor.a;
				texColor.a = i.vertexColor.a * a ;
				finalcol = texColor;
				// 获取特效使用的全覆盖颜色

				//地面淡出
				UNITY_BRANCH
				if (_BottomFade_ON)
				{
					float wave = sin( i.positionOS.x * 3 + _Time.y *3 ) * 0.08;
					float fadeValue = smoothstep(_BottomFadeHeight - _BottomFade+ wave,_BottomFadeHeight + wave,i.positionOS.y);
					fadeValue = saturate(fadeValue);
					finalcol.a *= fadeValue;
				}
				 
	


				half dissolveAlpha_final = 1.0;
                half3 mainColor_final_useDissolve = finalcol.rgb;
				UNITY_BRANCH
				if (_Dissolve_ON)
				{
					half2 uv_DissolveTex = i.positionOS.xy * _DissolveTex_ST.xy + _DissolveTex_ST.zw;
					half2 uv_Dissolve_speed = (half2(_DissolveUSpeed , _DissolveVSpeed));
					uv_DissolveTex -= frac( _Time.y * uv_Dissolve_speed);
					half dissolveTex = tex2D(_DissolveTex,uv_DissolveTex).r;
					half dissolveMaskTex = tex2D(_DissolveMaskTex,i.uv.xy).r;
					

					
					float rampValue = 1;
					Unity_Remap_float(i.positionOS.y,float2(_SpineBottom,_SpineHeight),float2(0,1), rampValue);
					rampValue = 1 -rampValue;
					float fade  = 0;
					Unity_Remap_float(_DissolveFactor,float2(0,1),float2(-_RangFade,1), fade);
					float geadient = (rampValue - fade)/ _RangFade * 2;

					float value = geadient - dissolveTex;
					float softvalue = smoothstep(_DissolveSoft,0.5,value);
					float edge = distance(value,_DissolveSoft);
					edge = saturate(1 - edge/_DissolveWide);


					half3 colorValue = finalcol.rgb * _DissolveColor.rgb;
					colorValue = lerp(finalcol.rgb,colorValue,edge);
					half Alphavalue = softvalue;
					
					
					
					// float mask_y = clamp( ((1 - _DissolveFactor) - i.positionOS.y) / _RangFade,0,1);
	    //             half dissolveTex_value = lerp(dissolveTex * mask_y,max(dissolveTex,mask_y),mask_y);
	                // half dissolveWide = _DissolveFactor * (1.0+_DissolveWide) -_DissolveWide;
	                // half dissolveSoft = ( _DissolveSoft + 0.0001 );
	                // half dissolveValue = ( dissolveWide + _DissolveWide ) * (1.0 + dissolveSoft) - dissolveSoft;
	                // half smoothstepResult264 = smoothstep( dissolveValue , ( dissolveValue + dissolveSoft ) , dissolveTex_value);
	                // half4 lerpResultColor = lerp( finalcol , _DissolveColor ,  _DissolveColor.a * ( 1.0 - smoothstepResult264 ));
	                // half dissolve_temp = dissolveWide * (1.0 + dissolveSoft) - dissolveSoft;
	                // half DissolveAlpha =  smoothstep( dissolve_temp , ( dissolve_temp + dissolveSoft ) , dissolveTex_value);
	                // mainColor_final_useDissolve = lerp(lerpResultColor,texColor,dissolveMaskTex);
	                // dissolveAlpha_final = max(DissolveAlpha ,dissolveMaskTex);

					mainColor_final_useDissolve = lerp(colorValue,finalcol.rgb,dissolveMaskTex);
					dissolveAlpha_final = max(Alphavalue ,dissolveMaskTex);
				}
				finalcol.rgb = mainColor_final_useDissolve;
				finalcol.a *= dissolveAlpha_final;
				
				finalcol = blendmodel(finalcol);
				#if defined(_STRAIGHT_ALPHA_INPUT)
					finalcol.rgb *= finalcol.a;
				#endif
				return BloomOutfrag(saturate(finalcol));
			}
			ENDCG
		}
	}
	//CustomEditor "SpineShaderWithOutlineGUI"
}
