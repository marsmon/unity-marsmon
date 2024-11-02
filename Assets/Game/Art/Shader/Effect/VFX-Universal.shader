Shader "Builtin-YoFi/VFX/Universal"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)]_Scr("Scr", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 10
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
        _MainTex("MainTex", 2D) = "white" {}
        _ColorTex("ColorTex", 2D) = "white" {}
        _ColorTexUSpeed("ColorTexUSpeed", Float) = 0
        _ColorTexVSpeed("ColorTexVSpeed", Float) = 0
        _ColorTexIntensity("ColorTexIntensity", Range(0,1)) = 1
        [Toggle]_MainTexScreenUV("_MainTexScreenUV", Float) = 0
        [Toggle]_ColorTexScreenUV("_ColorTexScreenUV", Float) = 0

        [Toggle]_ColorTexAddMode("_ColorTexAddMode", Float) = 0
        [HDR]_ColorTexTint("_ColorTexTint", Color) = (1,1,1,1)
        _ColorTexUseMainColorFactor("_ColorTexUseMainColorFactor", Range(0,1)) = 0
        
        [Toggle]_MainTexAR("MainTexAR", Float) = 0
        [HDR]_MainColor("MainColor", Color) = (1,1,1,1)
        _MainTexUSpeed("MainTexUSpeed", Float) = 0
        _MainTexVSpeed("MainTexVSpeed", Float) = 0
        [Toggle]_CustomMainTex("CustomMainTex", Float) = 0
        [Toggle(_FMASKTEX_ON)] _FMaskTex("FMaskTex", Float) = 0
        [Toggle] _PolarCoordMode("Polar", float) = 0.0
        _PolarCoordPower ("Polar Power", Range(0.1,1)) = 0.2
        _MaskTex("MaskTex", 2D) = "white" {}
        [Toggle]_MaskTexAR("MaskTexAR", Float) = 1
        _MaskTexUSpeed("MaskTexUSpeed", Float) = 0
        _MaskTexVSpeed("MaskTexVSpeed", Float) = 0
        _MaskValueOffset("_MaskValueOffset", Range(-1,1)) = 0
        _MaskValuePower("_MaskValuePower", Range(1,20)) = 1
        
        [Toggle(_FDISTORTTEX_ON)] _FDistortTex("FDistortTex", Float) = 0
        _DistortTex("DistortTex", 2D) = "white" {}
        [Toggle]_DistortTexAR("DistortTexAR", Float) = 1
        _DistortFactorU("DistortFactorU", Float) = 0
        _DistortFactorV("DistortFactorV", Float) = 0
        _DistortTexUSpeed("DistortTexUSpeed", Float) = 0
        _DistortTexVSpeed("DistortTexVSpeed", Float) = 0
        [Toggle]_DistortMainTex("DistortMainTex", Float) = 0
        [Toggle]_DistortMaskTex("DistortMaskTex", Float) = 0
        [Toggle]_DistortDissolveTex("DistortDissolveTex", Float) = 0
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
        [Toggle(_FFNL_ON)] _FFnl("FFnl", Float) = 0
        [HDR]_FnlColor("FnlColor", Color) = (1,1,1,1)
        _FnlScale("FnlScale", Range( 0 , 2)) = 0
        _FnlMainColorFactor("_FnlMainColorFactor", Range( 0 , 1)) = 0
        _FnlPower("FnlPower", Range( 1 , 10)) = 1
        [Toggle]_ReFnl("ReFnl", Float) = 0
        [Enum(Alpha,0,Add,1)]_BlendMode("BlendMode", Float) = 0
        [Enum(Normal,4,Always Top,8)] _ZTest ("ZTest Mode", float) = 4.0
        [Toggle]_MainTexClamp("MainTexClamp", Float) = 0
        [Toggle]_MaskTexClamp("MaskTexClamp", Float) = 0
        [Toggle]_DissolveTexClamp("DissolveTexClamp", Float) = 0
        [Toggle]_CustomMaskTex("CustomMaskTex", Float) = 0

        
        [Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 0
        _CBLocalThreshold("CB Threshhold",range(0.0,1.0)) = 1
        _CBLocalColor("CB Color",Color) = (1,1,1,1)
        _CBLocalLight("CB 亮度",float) = 1
        
        [Toggle]_IsClipGround("是否有地面[8h6H,Y = 0]",float) = 0
        _IsClipGroundBlur("地面接缝过度[8h6h]",float) = 0.5



        [hideininspector]_StencilComp ("Stencil Comparison", Float) = 8
        [hideininspector]_Stencil ("Stencil ID", Float) = 0
        [hideininspector]_StencilOp ("Stencil Operation", Float) = 0
        [hideininspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [hideininspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
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
        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
            
        HLSLINCLUDE
             
            
            #include "UnityShaderVariables.cginc"
		    #include "UnityCG.cginc"
            #include "HLSLSupport.cginc"

            


            
        CBUFFER_START(UnityPerMaterial)
            float _CullMode;
            float _Scr;
            float _BlendMode;
            float _Dst;

            float4 _ColorTex_ST;
            float _ColorTexIntensity;
            float _ColorTexUSpeed;
            float _ColorTexVSpeed;
            
            float _MainTexScreenUV;
            float _ColorTexScreenUV;
            
            float4 _MainTex_ST;
            float4 _MainColor;
            float _MainTexAR;
            float _CustomMainTex;
            float _MainTexVSpeed;
            float _MainTexUSpeed;
            float _MainTexClamp;
            float4 _ColorTexTint;
            float _ColorTexAddMode;
            float _ColorTexUseMainColorFactor;

            float4 _FnlColor;
            float _ReFnl;
            float _FnlScale;
            float _FnlPower;
            float _FnlMainColorFactor;

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

            float4 _DistortTex_ST;
            float _DistortMainTex;
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
            

            float _IsClipGround, _IsClipGroundBlur;

            float _CBLocalThreshold;
            float4 _CBLocalColor;
            float _CBLocalLight;
            half _CBGlobalBool;
            half _CBLocalBanBool;

        CBUFFER_END
            
            sampler2D _MainTex;
            sampler2D _ColorTex;
            sampler2D _DistortTex;
            sampler2D _DissolveTex;
            sampler2D _MaskTex;
            


        ENDHLSL

        Pass
        {
            Name "VFXForward"
            Tags 
            { 
                "RenderType"="Transparent" 
                //"LightMode" = "ForwardBase"
            }
            Blend [_Scr] [_Dst], One OneMinusSrcAlpha
            ZWrite Off
            ZTest  [_ZTest]
            Offset 0 , 0

            

            
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _FDISSOLVETEX_ON
            #pragma shader_feature_local _FDISTORTTEX_ON
            #pragma shader_feature_local _FFNL_ON
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
            #ifdef _FFNL_ON
                float3 normal    : NORMAL;
            #endif
                float4 color      : COLOR;
                float4 texcoord  : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {
                float4 positionHCS : SV_POSITION;
                float4 color       : COLOR;
                float4 uv          : TEXCOORD0;
                float4 customData1 : TEXCOORD1;
                float4 customData2 : TEXCOORD2;
                float3 positionWS  : TEXCOORD3;
            #ifdef _FFNL_ON
                float4 normalWS    : TEXCOORD4;
            #endif
                float3 positionOS  : TEXCOORD5;
                float4 positionSS : TEXCOORD6;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };


            VertexOutput vert(VertexInput v)
            {
                VertexOutput OUT = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
            #ifdef _FFNL_ON
                OUT.normalWS.xyz = normalize(mul(float4(v.normal,0.0),unity_WorldToObject).xyz);
                OUT.normalWS.w = 0;			
            #endif
                OUT.color = v.color;
                OUT.uv.xy = v.texcoord.xy;
                OUT.uv.zw = 0;
                OUT.customData1 = v.texcoord1;

                OUT.customData2 = v.texcoord2;

                float3 positionWS = mul(UNITY_MATRIX_M,float4(v.vertex.xyz, 1.0));
                float4 positionHCS = mul(UNITY_MATRIX_VP,float4(positionWS, 1.0));
                OUT.positionSS = ComputeScreenPos(positionHCS);
                
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
                float4 customData2 = IN.customData2;


                float4 color_vertex = IN.color;
                half3 ndc = IN.positionSS.xyz / (IN.positionSS.w  + 0.00000000001);
                half2 screenUV = ndc.xy;
                float time = _Time.y;
                
                float2 uv_MainTex_speed = (float2(_MainTexUSpeed , _MainTexVSpeed));
                float2 uv_MainTex = (_MainTexScreenUV ? screenUV : uv) * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 uv_MainTex_offset = uv_MainTex + frac(time * uv_MainTex_speed) + _CustomMainTex *customData1.xy;
                float2 uv_colorTex_speed = (float2(_ColorTexUSpeed , _ColorTexVSpeed));
                float2 uv_colorTex = (_ColorTexScreenUV ? screenUV : uv) * _ColorTex_ST.xy + _ColorTex_ST.zw;
                float2 uv_colorTex_offset = uv_colorTex + frac(time * uv_colorTex_speed);

                //扭曲相关
            #ifdef _FDISTORTTEX_ON
                float2 uv_DistortTex_speed = (float2(_DistortTexUSpeed , _DistortTexVSpeed));
                float2 uv_DistortTex = uv * _DistortTex_ST.xy + _DistortTex_ST.zw;
                    uv_DistortTex = frac( time * uv_DistortTex_speed) + uv_DistortTex;
                half4 distortTex = 	tex2D(_DistortTex,uv_DistortTex);


                half2 distortValue = lerp(distortTex.a,distortTex.r,_DistortTexAR) * half2( _DistortFactorU,_DistortFactorV);
                uv_MainTex += distortValue * _DistortMainTex;	
                uv_MainTex_offset = uv_MainTex + frac(time * uv_MainTex_speed) + _CustomMainTex *customData1.xy ;
            #endif

                uv_MainTex_offset =  lerp(uv_MainTex_offset,clamp(uv_MainTex_offset,0,1.0),_MainTexClamp);
                half4 mainTex = tex2D(_MainTex,uv_MainTex_offset);
                half4 colorTex = tex2D(_ColorTex,uv_colorTex_offset);
                colorTex *= _ColorTexTint;
                colorTex.rgb *= lerp(1, mainTex.rgb,_ColorTexUseMainColorFactor);

                if(_ColorTexAddMode)
                {
                    mainTex.rgb += colorTex.rgb * mainTex.rgb * colorTex.a * _ColorTexIntensity;
                }
                else
                {
                    mainTex.rgb = lerp(mainTex.rgb, mainTex.rgb * colorTex.rgb,_ColorTexIntensity);
                }

                
                mainTex.a = lerp(mainTex.a , mainTex.r,_MainTexAR);
                half4 mainColor =  _MainColor;
                half4 mainColor_final = ( mainColor * mainTex  * color_vertex);


                //half dissolveAlpha_final = 1.0;
                half4 mainColor_final_useDissolve = mainColor_final;

                //溶解相关
            #ifdef _FDISSOLVETEX_ON
                half2 uv_DissolveTex_speed = (half2(_DissolveTexUSpeed , _DissolveTexVSpeed));
                half2 uv_DissolveTex = uv * _DissolveTex_ST.xy + _DissolveTex_ST.zw;

                #ifdef _FDISTORTTEX_ON
                    uv_DissolveTex += distortValue * _DistortDissolveTex;
                #endif
                uv_DissolveTex = frac( time * uv_DissolveTex_speed) + uv_DissolveTex +  _CustomDissolve * customData2.zw ;
                
                uv_DissolveTex =  lerp(uv_DissolveTex,clamp(uv_DissolveTex,0,1.0),_DissolveTexClamp);
                
                half4 dissolveTex = tex2D(_DissolveTex,uv_DissolveTex);
                half dissolveTex_value = lerp(dissolveTex.a,dissolveTex.r,_DissolveTexAR);
                //half dissolveFactor = lerp(_DissolveFactor,customData1.z, _CustomDissolve);
                half dissolveFactor = _CustomDissolve ? customData1.z : _DissolveFactor;

                //溶解算法
                float rampValue = 1;
                Unity_Remap_float(dissolveTex_value,float2(0,1 ),float2(1,2), rampValue);
				float value =  rampValue - dissolveFactor*2;
				float softvalue = smoothstep(_DissolveSoft*0.5,0.5,value);
				float edge = smoothstep(_DissolveWide-_DissolveSoft*0.5,_DissolveWide,softvalue);

                mainColor_final_useDissolve.rgb = lerp( _DissolveColor.rgb,mainColor_final_useDissolve.rgb,edge);
                mainColor_final_useDissolve.a *= softvalue;
                
            #endif

                float4 color_final =  mainColor_final_useDissolve;

                half3 fresnel_color = half3(0,0,0);
                half fresnel_Alpha = 1.0;
            #ifdef _FFNL_ON
                float3 worldNormal = IN.normalWS.xyz;
                float3 WorldPosition = IN.positionWS;
                half fresnelNdotV279 = 0;
                if(unity_OrthoParams.w>0)//当是正交相机时
                {
                    float3 normalView = mul((float3x3)UNITY_MATRIX_V, worldNormal).xyz;
                    fresnelNdotV279 = normalView.z;
                }
                else
                {
                    float3 worldViewDir = ( _WorldSpaceCameraPos.xyz - WorldPosition );
                    worldViewDir = normalize(worldViewDir);
                    fresnelNdotV279 = dot( worldNormal, worldViewDir );
                }

                
                half refnl = _ReFnl;
                half fresnelNode279 = ( 0.0 + _FnlScale * pow(abs(1.0 - fresnelNdotV279), _FnlPower));
                half temp_output_283_0 = saturate( fresnelNode279 );
                half4 FnlMainColor286 = ( _FnlColor * temp_output_283_0 * _FnlColor.a );
                half ReFnlAlpha318 = ( 1.0 - temp_output_283_0 );
                
                FnlMainColor286 = lerp(FnlMainColor286,FnlMainColor286*color_final,_FnlMainColorFactor);
                
                fresnel_color = FnlMainColor286 * (1-refnl);
                fresnel_Alpha = lerp(1.0,ReFnlAlpha318,refnl);

            #endif

                color_final.rgb  += fresnel_color.rgb;
                color_final.a *= fresnel_Alpha;

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


            #endif




                float alpha_final = color_final.a * mask_final;
                


                half3 Color = lerp(color_final,color_final * alpha_final,_BlendMode).rgb;
                half Alpha = alpha_final;
                
                half4 finalColor = half4(Color, Alpha);
                finalColor = saturate(finalColor);
	            finalColor *= dt_ClipInWorldY(IN.positionWS.y);
	            return BloomOutfrag(finalColor);

                return finalColor;
            }
            ENDHLSL
        }
    }	
    CustomEditor "VFX_Universal_GUI"
    Fallback "Hidden/InternalErrorShader"
}
