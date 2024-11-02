Shader "Builtin-YoFi/VFX/DoubleLayer"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)]_Scr("Scr", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 10
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
        
        [Space(6)][Header(Main Map)]
        _MainTex("MainTex", 2D) = "white" {}
        [HDR]_MainColor("MainColor", Color) = (1,1,1,1)
        _ScaleMainTex("_ScaleMainTex", Float) = 1
        _RotateMainTex("_RotateMainTex", Range(0,1)) = 0
        _MainTexRotateSpeed("_MainTexRotateSpeed", Float) = 0
        [Toggle]_MainTexAR("MainTexAR", Float) = 0
        _MainTexUSpeed("MainTexUSpeed", Float) = 0
        _MainTexVSpeed("MainTexVSpeed", Float) = 0
        [Toggle]_CustomDataMainOn("_CustomDataMainOn", Float) = 0
        [Toggle]_MainTexClamp("MainTexClamp", Float) = 0

        [Space(6)][Header(Second Map)]
        _SecondTex("SecondTex", 2D) = "white" {}
        _SecondTexUSpeed("SecondTexUSpeed", Float) = 0
        _SecondTexVSpeed("SecondTexVSpeed", Float) = 0
        [Toggle]_SecondTexAR("_SecondTexAR", Float) = 0
        [HDR]_SecondColor("_SecondColor", Color) = (1,1,1,1)
        _ScaleSecondTex("_ScaleSecondTex", Float) = 1
        _RotateSecondTex("_RotateSecondTex", Range(0,1)) = 0
        _SecondTexRotateSpeed("_SecondTexRotateSpeed", Float) = 0
        [Toggle]_SecondTexClamp("_SecondTexClamp", Float) = 0
        
        [Enum(Alpha,0,Add,1)]_SecondBlendMode("_SecondBlendMode", Float) = 0
        
        
        _ColorTex("ColorTex", 2D) = "white" {}
        [HDR]_ColorTexTint("_ColorTexTint", Color) = (1,1,1,1)
        _ColorTexUSpeed("ColorTexUSpeed", Float) = 0
        _ColorTexVSpeed("ColorTexVSpeed", Float) = 0
        _ColorTexIntensity("ColorTexIntensity", Range(0,1)) = 0
        [Toggle]_ColorApplyMainTex("_ColorApplyMainTex", Float) = 0
        [Toggle]_ColorApplySecondTex("_ColorApplySecondTex", Float) = 0
        


        [Space(6)][Header(Mask)]
        [Toggle(_FMASKTEX_ON)] _FMaskTex("FMaskTex", Float) = 0
        _MaskTex("MaskTex", 2D) = "white" {}
        _MaskValueOffset("_MaskValueOffset", Range(-1,1)) = 0
        _MaskValuePower("_MaskValuePower", Range(1,20)) = 1
        [Toggle]_MaskTexAR("MaskTexAR", Float) = 1
        [Toggle]_MaskTexClamp("MaskTexClamp", Float) = 0
        [Toggle]_CustomMaskTex("CustomMaskTex", Float) = 0
        _MaskTexUSpeed("MaskTexUSpeed", Float) = 0
        _MaskTexVSpeed("MaskTexVSpeed", Float) = 0
        [Toggle] _PolarCoordMode("Polar", float) = 0.0
        _PolarCoordPower ("Polar Power", Range(0.1,1)) = 0.2
        [Toggle]_MaskApplyMainTex("_MaskApplyMainTex", Float) = 0
        [Toggle]_MaskApplySecondTex("_MaskApplySecondTex", Float) = 0

        [Space(6)][Header(Distort)]
        [Toggle(_FDISTORTTEX_ON)] _FDistortTex("FDistortTex", Float) = 0
        _DistortTex("DistortTex", 2D) = "white" {}
        [Toggle]_DistortTexAR("DistortTexAR", Float) = 1
        _DistortFactorU("DistortFactorU", Range( 0 , 1)) = 0
        _DistortFactorV("DistortFactorV", Range( 0 , 1)) = 0
        _DistortTexUSpeed("DistortTexUSpeed", Float) = 0
        _DistortTexVSpeed("DistortTexVSpeed", Float) = 0
        [Toggle]_DistortMainTex("DistortMainTex", Float) = 0
        [Toggle]_DistortSecondTex("DistortSecondTex", Float) = 0
        [Toggle]_DistortMaskTex("DistortMaskTex", Float) = 0
        [Toggle]_DistortDissolveTex("DistortDissolveTex", Float) = 0
        
        [Space(6)][Header(Dissolve)]
        [Toggle(_FDISSOLVETEX_ON)] _FDissolveTex("FDissolveTex", Float) = 0
        _DissolveTex("DissolveTex", 2D) = "white" {}
        [Toggle]_DissolveTexAR("DissolveTexAR", Float) = 1
        [HDR]_DissolveColor("DissolveColor", Color) = (1,1,1,1)
        [Toggle]_CustomDissolve("CustomDissolve", Float) = 0
        _DissolveFactor("DissolveFactor", Range( 0 , 1)) = 0
        _DissolveSoft("DissolveSoft", Range( 0.01 , 1)) = 0.5
        _DissolveWide("DissolveWide", Range( 0 , 0.99)) = 0.5
        _DissolveTexUSpeed("DissolveTexUSpeed", Float) = 0
        _DissolveTexVSpeed("DissolveTexVSpeed", Float) = 0
        [Toggle]_DissolveTexClamp("DissolveTexClamp", Float) = 0
        [Toggle]_DissolveApplyMainTex("_DissolveApplyMainTex", Float) = 0
        [Toggle]_DissolveApplySecondTex("_DissolveApplySecondTex", Float) = 0

        [Space(6)][Header(Other)]
        _MainAlpha("MainAlpha", Range( 0 , 10)) = 1


        [Enum(Alpha,0,Add,1)]_BlendMode("BlendMode", Float) = 0

        [Enum(Normal,4,Always Top,8)] _ZTest ("ZTest Mode", float) = 4.0



        
        [Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 0
        _CBLocalThreshold("CB Threshhold",range(0.0,1.0)) = 1
        _CBLocalColor("CB Color",Color) = (1,1,1,1)
        _CBLocalLight("CB 亮度",float) = 1
        
        [Toggle]_IsClipGround("是否有地面[8h6H,Y = 0]",float) = 0
        _IsClipGroundBlur("地面接缝过度[8h6h]",float) = 0.5

        
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
        Cull [_CullMode]
        AlphaToMask Off

        HLSLINCLUDE
             
            
            #include "UnityShaderVariables.cginc"
		    #include "UnityCG.cginc"
            #include "HLSLSupport.cginc"

            


            
        CBUFFER_START(UnityPerMaterial)
            float _CullMode;
            float _Scr;
            float _BlendMode;
            float _Dst;
            float _MainAlpha;
            //float _DepthFade;

            

            float _ScaleSecondTex;
            float _RotateSecondTex;
            float4 _SecondTex_ST;
            float _SecondTexUSpeed;
            float _SecondTexVSpeed;
            float _SecondTexRotateSpeed;

            float4 _SecondColor;
            float _SecondTexClamp;
            float _SecondTexAR;
            float _SecondBlendMode;
            
            float4 _ColorTex_ST;
            float4 _ColorTexTint;
            float _ColorTexIntensity;
            float _ColorTexUSpeed;
            float _ColorTexVSpeed;
            float _ColorApplyMainTex;
            float _ColorApplySecondTex;

            float _ScaleMainTex;
            float _RotateMainTex;
            float _MainTexRotateSpeed;

            float4 _MainTex_ST;
            float4 _MainColor;
            float _MainTexAR;
            float _CustomDataMainOn;
            float _MainTexVSpeed;
            float _MainTexUSpeed;
            float _MainTexClamp;
            

            float4 _DissolveTex_ST;
            float4 _DissolveColor;
            float _DissolveTexAR;
            float _CustomDissolve;
            float _DissolveTexUSpeed;
            float _DissolveTexVSpeed;
            float _DissolveFactor;
            float _DissolveSoft;
            float _DissolveWide;
            float _DissolveTexClamp;
            float _DissolveApplyMainTex;
            float _DissolveApplySecondTex;

            float4 _DistortTex_ST;
            float _DistortMainTex;
            float _DistortSecondTex;
            float _DistortDissolveTex;
            float _DistortMaskTex;
            float _DistortFactorU;
            float _DistortFactorV;
            float _DistortTexVSpeed;
            float _DistortTexUSpeed;
            float _DistortTexAR;

            float4 _MaskTex_ST;
            float _MaskTexAR;
            float _MaskTexUSpeed;
            float _MaskTexVSpeed;
            float _CustomMaskTex;
            float _MaskTexClamp;
            float _PolarCoordMode;
            float _PolarCoordPower;
            float _MaskValueOffset;
            float _MaskValuePower;
            float _MaskApplyMainTex;
            float _MaskApplySecondTex;
            
            
            float _StencilValue;
            float _CompOp;

            float _IsClipGround, _IsClipGroundBlur;

            float _CBLocalThreshold;
            float4 _CBLocalColor;
            float _CBLocalLight;
            half _CBGlobalBool;
            half _CBLocalBanBool;

        CBUFFER_END
            
            sampler2D _MainTex;
            sampler2D _SecondTex;
            sampler2D _DistortTex;
            sampler2D _DissolveTex;
            sampler2D _MaskTex;
            sampler2D _ColorTex;


        ENDHLSL

        Pass
        {
            Name "VFXForward"
            Tags 
            { 
                "RenderType"="Transparent" 
                "LightMode" = "ForwardBase"
            }
            Blend [_Scr] [_Dst], One OneMinusSrcAlpha
            ZWrite Off
            ZTest  [_ZTest]
            Offset 0 , 0
            ColorMask RGBA
            
            
            
            HLSLPROGRAM
            
            //#include "Assets/AssetsPackage/Shaders/Common/Included/YFEffectBase.cginc"
            //#include "Assets/AssetsPackage/Shaders/Common/Included/YoFi_FXShow_Base.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _FDISSOLVETEX_ON
            #pragma shader_feature_local _FDISTORTTEX_ON
            #pragma shader_feature_local _FMASKTEX_ON

            


           
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
			{
			    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
			}
            
            inline float4 BloomOutfrag(float4 col){
                if(_CBLocalBanBool == 1){
                    return col;
                }
                if(_CBGlobalBool == 1){
                    float cc = dot(col.rgb, float3(0.2126729f,  0.7151522f, 0.0721750f)) ;
                    half bcol = smoothstep(1.0 - max(0,_CBLocalThreshold), 1.0 , cc);
                    half3 nc =  bcol;
                    half3 value = nc * _CBLocalLight * _CBLocalColor.rgb;
                    value *= col.rgb * col.a;
                    col.rgb += value; 
                    return col;
                }
                else{
                    return col;
                }
            }
            inline float dt_ClipInWorldY(float worldposY)
            {
	            if(_IsClipGround > 0){		
    	            return smoothstep(0, _IsClipGroundBlur,worldposY);
	            }else{
		            return 1.0;
	            }
                
            }
            inline float4 LastExportColor(float4 fragcolor, float worldPos_y)
            {
	            fragcolor = saturate(fragcolor);
	            fragcolor.a *= dt_ClipInWorldY(worldPos_y);
	            return BloomOutfrag(fragcolor);
            } 

            inline float UnityGet2DClipping(in float2 position, in float4 clipRect)
            {
                float2 inside = step(clipRect.xy, position.xy) * step(position.xy, clipRect.zw);
                return inside.x * inside.y;
            }

            struct VertexInput
            {
                float4 vertex    : POSITION;

                float4 color      : COLOR;
                float4 texcoord  : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            
#if defined(_FMASKTEX_ON) || defined(_FDISSOLVETEX_ON) 
                float4 texcoord2 : TEXCOORD2;
#endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 positionHCS : SV_POSITION;
                float4 color       : COLOR;
                float4 uv          : TEXCOORD0;
                float4 customData1 : TEXCOORD1;
#if defined(_FMASKTEX_ON) || defined(_FDISSOLVETEX_ON) 
                float4 customData2 : TEXCOORD2;
#endif
                float3 positionWS  : TEXCOORD3;

            // #ifdef _FDEPTH_ON
            //     float4 positionSS  : TEXCOORD6;
            // #endif
                float3 positionOS  : TEXCOORD7;
                float4 positionSS : TEXCOORD8;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };


            VertexOutput vert(VertexInput v)
            {
                VertexOutput OUT = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.color = v.color;
                OUT.uv.xy = v.texcoord.xy;
                OUT.uv.zw = 0;
                OUT.customData1 = v.texcoord1;
#if defined(_FMASKTEX_ON) || defined(_FDISSOLVETEX_ON) 
                OUT.customData2 = v.texcoord2;
#endif
                
                //float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
                float3 positionWS = mul(UNITY_MATRIX_M,float4(v.vertex.xyz, 1.0));
                float4 positionHCS = mul(UNITY_MATRIX_VP,float4(positionWS, 1.0));
                OUT.positionSS = ComputeScreenPos(positionHCS);
                
            // #ifdef _FDEPTH_ON
            //     OUT.positionSS = ComputeScreenPos(positionHCS);
            // #endif
                OUT.positionWS = positionWS;
                OUT.positionHCS = positionHCS;
                OUT.positionOS = v.vertex.xyz;
                return OUT;
            }
            
            half4 frag(VertexOutput IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

                float2 uv = IN.uv.xy;
                float4 customData1 = IN.customData1;
#if defined(_FMASKTEX_ON) || defined(_FDISSOLVETEX_ON) 
                float4 customData2 = IN.customData2;
#endif
                //自定义数据1的xy是主纹理的缩放和透明度，zw是副贴图的缩放和透明度
                //数据2的xy是遮罩的偏移，z是溶解程度
                float4 color_vertex = IN.color;
                // half3 ndc = IN.positionSS.xyz / (IN.positionSS.w  + 0.00000000001);
                // half2 screenUV = ndc.xy;
                float time = _Time.y;
                
                float2 uv_MainTex_speed = (float2(_MainTexUSpeed , _MainTexVSpeed));
                float2 uv_MainTex = uv * _MainTex_ST.xy + _MainTex_ST.zw;

				uv_MainTex -= float2(0.5,0.5);
				uv_MainTex /= _CustomDataMainOn ? customData1.x:_ScaleMainTex ;
				uv_MainTex += float2(0.5,0.5);
            
				float angle = frac(_RotateMainTex  + frac(time * _MainTexRotateSpeed) )* 6.28318530718;
				uv_MainTex -= float2(0.5,0.5);
				uv_MainTex = float2(uv_MainTex.x*cos(angle)-uv_MainTex.y*sin(angle),uv_MainTex.y*cos(angle) + uv_MainTex.x*sin(angle));
				uv_MainTex += float2(0.5,0.5);
				
                float2 uv_MainTex_offset = uv_MainTex + frac(time * uv_MainTex_speed);
                
                
                float2 uv_secondTex_speed = (float2(_SecondTexUSpeed , _SecondTexVSpeed));
                float2 uv_secondTex =  uv * _SecondTex_ST.xy + _SecondTex_ST.zw;
                uv_secondTex -= float2(0.5,0.5);
				uv_secondTex /= _CustomDataMainOn ? customData1.z : _ScaleSecondTex;
				uv_secondTex += float2(0.5,0.5);
            
				float angle_secondTex = frac(_RotateSecondTex + customData1.w + frac(time * _SecondTexRotateSpeed) )* 6.28318530718;
				uv_secondTex -= float2(0.5,0.5);
				uv_secondTex = float2(uv_secondTex.x*cos(angle_secondTex)-uv_secondTex.y*sin(angle_secondTex),uv_secondTex.y*cos(angle_secondTex) + uv_secondTex.x*sin(angle_secondTex));
				uv_secondTex += float2(0.5,0.5);
                
                float2 uv_secondTex_offset = uv_secondTex + frac(time * uv_secondTex_speed);

                float2 uv_colorTex_speed = (float2(_ColorTexUSpeed , _ColorTexVSpeed));
                float2 uv_colorTex = uv * _ColorTex_ST.xy + _ColorTex_ST.zw;
                float2 uv_colorTex_offset = uv_colorTex + frac(time * uv_colorTex_speed);

                //扭曲相关
            #ifdef _FDISTORTTEX_ON
                float2 uv_DistortTex_speed = (float2(_DistortTexUSpeed , _DistortTexVSpeed));
                float2 uv_DistortTex = uv * _DistortTex_ST.xy + _DistortTex_ST.zw;
                    uv_DistortTex = frac( time * uv_DistortTex_speed) + uv_DistortTex;
                half4 distortTex = 	tex2D(_DistortTex,uv_DistortTex);


                half2 distortValue = lerp(distortTex.a,distortTex.r,_DistortTexAR) * half2( _DistortFactorU,_DistortFactorV);
                uv_MainTex += distortValue * _DistortMainTex;	
                uv_MainTex_offset = uv_MainTex + frac(time * uv_MainTex_speed) ;
                uv_secondTex += distortValue * _DistortSecondTex;
                uv_secondTex_offset = uv_secondTex + frac(time * uv_secondTex_speed);
            #endif

                uv_MainTex_offset =  lerp(uv_MainTex_offset,clamp(uv_MainTex_offset,0,1.0),_MainTexClamp);
                half4 mainTex = tex2D(_MainTex,uv_MainTex_offset);
                mainTex.a = lerp(mainTex.a , mainTex.r,_MainTexAR) * (_CustomDataMainOn ? customData1.y:1);
                half4 mainColor = ( _MainColor * mainTex );


                uv_secondTex_offset =  lerp(uv_secondTex_offset,clamp(uv_secondTex_offset,0,1.0),_SecondTexClamp);
                half4 secondTex = tex2D(_SecondTex,uv_secondTex_offset);
                secondTex.a = lerp(secondTex.a , secondTex.r,_SecondTexAR) * (_CustomDataMainOn ? customData1.w : 1);
                half4 secondColor = ( _SecondColor * secondTex );


                half4 colorTex = tex2D(_ColorTex,uv_colorTex_offset);
                colorTex.rgb = lerp(1, colorTex.rgb * _ColorTexTint.rgb,_ColorTexIntensity);
                
                mainColor.rgb *= lerp(1, colorTex.rgb,_ColorApplyMainTex);
                secondColor.rgb *= lerp(1,colorTex.rgb,_ColorApplySecondTex);

                
                half4 mainColor_final_useDissolve = mainColor;
                half4 secondColor_final_useDissolve = secondColor;

                //溶解相关
            #ifdef _FDISSOLVETEX_ON
                half2 uv_DissolveTex_speed = (half2(_DissolveTexUSpeed , _DissolveTexVSpeed));
                half2 uv_DissolveTex = uv * _DissolveTex_ST.xy + _DissolveTex_ST.zw;

                #ifdef _FDISTORTTEX_ON
                    uv_DissolveTex += distortValue * _DistortDissolveTex;
                #endif
                uv_DissolveTex = frac( time * uv_DissolveTex_speed) + uv_DissolveTex;
                
                uv_DissolveTex =  lerp(uv_DissolveTex,clamp(uv_DissolveTex,0,1.0),_DissolveTexClamp);
                
                half4 dissolveTex = tex2D(_DissolveTex,uv_DissolveTex);
                half dissolveTex_value = lerp(dissolveTex.a,dissolveTex.r,_DissolveTexAR);
                half dissolveFactor = lerp(_DissolveFactor,customData2.z,_CustomDissolve);

                //溶解算法
                float rampValue = 1;
                Unity_Remap_float(dissolveTex_value,float2(0,1 ),float2(1,2), rampValue);
				float value =  rampValue - dissolveFactor*2;
				float softvalue = smoothstep(_DissolveSoft*0.5,0.5,value);
				float edge = smoothstep(_DissolveWide-_DissolveSoft*0.5,_DissolveWide,softvalue);
				
                
                if(_DissolveApplyMainTex)
                {
                mainColor_final_useDissolve.rgb = lerp(_DissolveColor.rgb,mainColor_final_useDissolve.rgb,edge);
                mainColor_final_useDissolve.a *= softvalue;
                }
                if (_DissolveApplySecondTex)
                {
                secondColor_final_useDissolve.rgb = lerp(_DissolveColor.rgb,secondColor_final_useDissolve.rgb,edge);
                secondColor_final_useDissolve.a *= softvalue;
                }
            #endif

                

                float4 mainColor_final = ( color_vertex *  mainColor_final_useDissolve );
                float4 secondColor_final = ( color_vertex *  secondColor_final_useDissolve );

                // 遮罩相关
                half mask_final = 1.0;
            #ifdef _FMASKTEX_ON

                half2 uv_MaskTex_speed = (half2(_MaskTexUSpeed , _MaskTexVSpeed));
                half2 uv_MaskTex = uv * _MaskTex_ST.xy + _MaskTex_ST.zw;
                half2 uv_MaskTex_Distort = uv_MaskTex;

                // 极坐标 Polar Coord
                half2 uv_PolarMask = uv_MaskTex_Distort - _MaskTex_ST.xy * 0.5;
                uv_PolarMask = half2(pow(length(uv_PolarMask), _PolarCoordPower), ((atan2(uv_PolarMask.y, uv_PolarMask.x) / (2.0 * UNITY_PI)) + 0.5));
                uv_MaskTex_Distort = lerp(uv_MaskTex_Distort, uv_PolarMask, _PolarCoordMode);

                #ifdef _FDISTORTTEX_ON
                    uv_MaskTex_Distort += distortValue * _DistortMaskTex;
                #endif
                half2 uv_MaskTex_Temp = uv_MaskTex_Distort + frac(time * uv_MaskTex_speed) + _CustomMaskTex *customData2.xy ;
                uv_MaskTex_Temp = lerp(uv_MaskTex_Temp,clamp(uv_MaskTex_Temp,0,1.0),_MaskTexClamp);
                 
                half4 maskTex = tex2D(_MaskTex,uv_MaskTex_Temp);
                mask_final = lerp(maskTex.a, maskTex.r, _MaskTexAR);
                mask_final = saturate(pow( clamp(mask_final + _MaskValueOffset,0,1), _MaskValuePower));

                mainColor_final.a *= lerp(1, mask_final,_MaskApplyMainTex) ;
                secondColor_final.a *= lerp(1,mask_final,_MaskApplySecondTex);

            #endif



                half4 finalColor = mainColor_final;
                if(_SecondBlendMode)
                {
                    //混合
                    finalColor.rgb =  secondColor_final.a * secondColor_final.rgb + (1 - secondColor_final.a) * mainColor_final.rgb * mainColor_final.a;
                    finalColor.a = saturate(secondColor_final.a + mainColor_final.a*(1 - secondColor_final.a));
                }
                else
                {
                    //相加
                    finalColor.rgb =  secondColor_final.rgb * secondColor_final.a + mainColor_final.rgb * mainColor_final.a ;
                    finalColor.a = saturate(secondColor_final.a + mainColor_final.a*(1 - secondColor_final.a));
                }
                
                finalColor.a = saturate( finalColor.a * _MainAlpha);


                //half3 Color = lerp(color_final,color_final * alpha_final,_BlendMode).rgb;
                //half Alpha = alpha_final;
                half3 Color = mainColor_final.rgb;
                half Alpha = mainColor_final.a;


                // finalColor.rgb = mainColor_final_useDissolve.rgb;
                // finalColor.a =mainColor_final_useDissolve.a;
                finalColor = saturate(finalColor);
	            finalColor *= dt_ClipInWorldY(IN.positionWS.y);
	            return BloomOutfrag(finalColor);

                return finalColor;
            }
            ENDHLSL
        }
    }	
    //CustomEditor "VFX_DoubleLayer_GUI"
    Fallback "Hidden/InternalErrorShader"
}
