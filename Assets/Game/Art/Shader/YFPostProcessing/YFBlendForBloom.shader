// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/YFBlendForBloom" {
	Properties {
		_MainTex ("Screen Blended", 2D) = "" {}
		_ColorBuffer ("Color", 2D) = "" {} 
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv[2] : TEXCOORD0;
	};
	struct v2f_mt {
		float4 pos : SV_POSITION;
		float2 uv[5] : TEXCOORD0;
	};

	struct v2f_blur {
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half4 uv01 : TEXCOORD1;
		half4 uv23 : TEXCOORD2;
		half4 uv45 : TEXCOORD3;
		half4 uv67 : TEXCOORD4;
	};


	
	sampler2D _ColorBuffer;
	sampler2D _MainTex;
	
	half4 _ColorBuffer_TexelSize;
	half4 _MainTex_TexelSize;

	sampler2D _maskTex;
	half4 _CBThreshholdCol;
	half4 _CBData;// xy = offset , z = _Intensity, w = _Threshhold
	 
 
	v2f_blur vertWithMultiCoordsVertical (appdata_img v) {
		v2f_blur o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;
		half4 d = half4(0,1, 0,-1);
		o.uv01 =  v.texcoord.xyxy + _CBData.xyxy * d;
		o.uv23 =  v.texcoord.xyxy + _CBData.xyxy * d * 2.0;
		o.uv45 =  v.texcoord.xyxy + _CBData.xyxy * d * 3.0;
		o.uv67 =  v.texcoord.xyxy + _CBData.xyxy * d * 4.0;
		o.uv67 =  v.texcoord.xyxy + _CBData.xyxy * d * 5.0;
		return o;  
	}
	v2f_blur vertWithMultiCoordsHorizontal (appdata_img v) {
		v2f_blur o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;
		half4 d = half4(1,0, -1,0);
		o.uv01 =  v.texcoord.xyxy + _CBData.xyxy * d;
		o.uv23 =  v.texcoord.xyxy + _CBData.xyxy * d * 2.0;
		o.uv45 =  v.texcoord.xyxy + _CBData.xyxy * d * 3.0;
		o.uv67 =  v.texcoord.xyxy + _CBData.xyxy * d * 4.0;
		o.uv67 =  v.texcoord.xyxy + _CBData.xyxy * d * 5.0;
		return o;  
	}

	//13
	half4 fragGaussBlur (v2f_blur i) : SV_Target {
		half4 color = half4 (0,0,0,0);
		color += 0.225 * tex2D (_MainTex, i.uv);
		color += 0.150 * tex2D (_MainTex, i.uv01.xy);
		color += 0.150 * tex2D (_MainTex, i.uv01.zw);
		color += 0.110 * tex2D (_MainTex, i.uv23.xy);
		color += 0.110 * tex2D (_MainTex, i.uv23.zw);
		color += 0.075 * tex2D (_MainTex, i.uv45.xy);
		color += 0.075 * tex2D (_MainTex, i.uv45.zw);	
		color += 0.0525 * tex2D (_MainTex, i.uv67.xy);
		color += 0.0525 * tex2D (_MainTex, i.uv67.zw);
		return color;
	} 
	
	v2f_mt vertMultiTap( appdata_img v ) {
		v2f_mt o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv[4] = v.texcoord.xy;
		o.uv[0] = v.texcoord.xy + _MainTex_TexelSize.xy * 0.5;
		o.uv[1] = v.texcoord.xy - _MainTex_TexelSize.xy * 0.5;	
		o.uv[2] = v.texcoord.xy - _MainTex_TexelSize.xy * half2(1,-1) * 0.5;	
		o.uv[3] = v.texcoord.xy + _MainTex_TexelSize.xy * half2(1,-1) * 0.5;	
		return o;
	}

	half4 fragMultiTapMax (v2f_mt i) : SV_Target {
		half4 outColor = tex2D(_MainTex, i.uv[4].xy);
		outColor = max(outColor, tex2D(_MainTex, i.uv[0].xy));
		outColor = max(outColor, tex2D(_MainTex, i.uv[1].xy));
		outColor = max(outColor, tex2D(_MainTex, i.uv[2].xy));
		outColor = max(outColor, tex2D(_MainTex, i.uv[3].xy));
		return outColor;
	}

	v2f vert( appdata_img v ) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv[0] =  v.texcoord.xy;
		o.uv[1] =  v.texcoord.xy;
		
		#if UNITY_UV_STARTS_AT_TOP
			if (_ColorBuffer_TexelSize.y < 0) 
			o.uv[1].y = 1-o.uv[1].y;
		#endif
		return o;
	}
	
	half4 fragScreen (v2f i) : SV_Target {
		half4 addedbloom = tex2D(_MainTex, i.uv[0].xy) * _CBData.z * _CBThreshholdCol;//_Intensity;
		half4 screencolor = tex2D(_maskTex, i.uv[1]);// tex2D(_ColorBuffer, i.uv[1]);
		return 1-(1-addedbloom)*(1-screencolor);
	}

	half4 fragScreenCheap(v2f i) : SV_Target {
		half4 addedbloom = tex2D(_MainTex, i.uv[0].xy) * _CBData.z * _CBThreshholdCol;//* _Intensity;
		half4 screencolor = tex2D(_ColorBuffer, i.uv[1]);
		return 1-(1-addedbloom)*(1-screencolor);
	}

	half4 fragAdd (v2f i) : SV_Target {
		half4 addedbloom = tex2D(_MainTex, i.uv[0].xy) ;		
		half4 screencolor = tex2D(_ColorBuffer, i.uv[1]);
		return  _CBData.z * addedbloom * _CBThreshholdCol  + screencolor;
		// return  _Intensity * addedbloom  + screencolor   ;
	}
	//1
	half4 fragAddCheap (v2f i) : SV_Target {
		half4 addedbloom = tex2D(_MainTex, i.uv[0].xy);
		half4 screencolor = tex2D(_ColorBuffer, i.uv[1]);
		return  _CBData.z * addedbloom * _CBThreshholdCol   + screencolor;
		// return _Intensity * addedbloom + screencolor;
	}

	half4 fragVignetteMul (v2f i) : SV_Target {
		return tex2D(_MainTex, i.uv[0].xy) * tex2D(_ColorBuffer, i.uv[0]);
	}

	half4 fragVignetteBlend (v2f i) : SV_Target {
		return half4(1,1,1, tex2D(_ColorBuffer, i.uv[0]).r);
	}

	half4 fragClear (v2f i) : SV_Target {
		return 0;
	}

	half4 fragAddOneOne (v2f i) : SV_Target {
		half4 addedColors = tex2D(_MainTex, i.uv[0].xy);
		return addedColors * _CBData.z;//_Intensity;
	}

	half4 frag1Tap (v2f i) : SV_Target {
		return tex2D(_MainTex, i.uv[0].xy);
	}
	
	

	half4 fragMultiTapBlur (v2f_mt i) : SV_Target {
		half4 outColor = 0;
		outColor += tex2D(_MainTex, i.uv[0].xy);
		outColor += tex2D(_MainTex, i.uv[1].xy);
		outColor += tex2D(_MainTex, i.uv[2].xy);
		outColor += tex2D(_MainTex, i.uv[3].xy);
		return outColor/4;
	}
	fixed4 fragGetApl (v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv[1]) * tex2D(_maskTex, i.uv[1]).r;
		// just invert the colors
		// col.rgb = 1 - col.rgb;
		return col;
	}

	//12
	half4 fragColorThresh(v2f i) : SV_Target 
	{
		half4 color = tex2D(_MainTex, i.uv[0]);
		// color.rgb = max(half3(0,0,0), color.rgb -_CBThreshholdCol.rgb);		
		color.rgb = max(half3(0,0,0), color.rgb - _CBData.www); //
		return color;
	}
	

	ENDCG 
	
	Subshader {
		ZTest Always Cull Off ZWrite Off

		// 0: nicer & softer "screen" blend mode	  		  	
		Pass {    

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragScreen
			ENDCG
		}

		// 1: "add" blend mode 
		Pass {    

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragAdd
			ENDCG
		}
		// 2: several taps, maxxed
		Pass {    

			CGPROGRAM
			#pragma vertex vertMultiTap
			#pragma fragment fragMultiTapMax
			ENDCG
		} 
		// 3: vignette blending
		Pass {    

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragVignetteMul
			ENDCG
		} 
		// 4: nicer & softer "screen" blend mode(cheapest)
		Pass {    

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragScreenCheap
			ENDCG
		}  
		// 5: "add" blend mode (cheapest)
		Pass {    

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragAddCheap
			ENDCG
		}     
		// 6: used for "stable" downsampling (blur)
		Pass {    

			CGPROGRAM
			#pragma vertex vertMultiTap
			#pragma fragment fragMultiTapBlur
			ENDCG
		}   
		// 7: vignette blending (blend to dest)
		Pass {    
			
			Blend Zero SrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragVignetteBlend
			ENDCG
		}
		// 8: clear
		Pass {    
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragClear
			ENDCG
		}   
		// 9: fragAddOneOne
		Pass {    

			Blend One One
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragAddOneOne
			ENDCG
		}  
		// 10: max blend
		Pass {    

			BlendOp Max
			Blend One One
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag1Tap
			ENDCG
		}
		// 11: Get Mask blend
		Pass {
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragGetApl
			ENDCG
		}
		
		// 12: "Hidden/BrightPassFilter2" 1
		Pass 
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment fragColorThresh

			ENDCG
		}
		//13 WGaussBlur
		Pass {     

			CGPROGRAM
			
			#pragma vertex vertWithMultiCoordsVertical
			#pragma fragment fragGaussBlur
			
			ENDCG
		} 

		//14 HGaussBlur
		Pass {     

			CGPROGRAM
			
			#pragma vertex vertWithMultiCoordsHorizontal
			#pragma fragment fragGaussBlur
			
			ENDCG
		} 

		
	}

	Fallback off
	
} // shader
