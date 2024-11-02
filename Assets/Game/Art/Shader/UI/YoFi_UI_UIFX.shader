Shader "YoFi/UI/UIFX"
{
    Properties
    {
        [HideInInspector][PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector]_Color ("Tint", Color) = (1,1,1,1)

        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255

        [HideInInspector]_ColorMask ("Color Mask", Float) = 15

        [HideInInspector][Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        ///UI
    
        [Space(10)]
        [Header(Main Tex)]
        _MainAddST("Main Add ST",vector) = (1,1,0,0)
        _MainTexSpeedAndOffset("MainTex Speed And Offset",vector) = (0,0,0,0)
        _MainColorIntensity("MainColor Intensity", Range( 0 , 20)) = 1

        
        [Space(10)]
        [Header(Sub Tex)]
        [Toggle(_SUB_ON)] _SUB_ON ("Use Sub Tex", Float) = 0
            _SubTex("SubTex", 2D) = "white" {}
            _SubTexSpeedAndOffset("SubTex Speed And Offset",vector) = (0,0,0,0)
            [HDR]_SubTexColor("SubTex Color", Color) = (1,1,1,1)
            [Toggle]_SubTexUsePolarCoord("SubTexUsePolarCoord", float) = 0.0
            [Enum(Blend,0,Add,1,Multiply,2)] _SubTexBlendMod("SubTex Blend Mod",float) = 0
        
        [Space(10)]
        [Header(Distort Tex)]
        [Toggle(_DISTORT_ON)] _DISTORT_ON ("Use Distort Tex", Float) = 0
            _DistortTex("Distort Tex", 2D) = "white" {}
            _DistortTexSpeedAndOffset("DistortTex Speed And Offset",vector) = (0,0,0,0)
            _DistortFactorU("Distort Factor U", Float) = 0
            _DistortFactorV("Distort Factor V", Float) = 0

            [Toggle]_DistortTexUsePolarCoord("DistortTex Use PolarCoord", float) = 0.0
            [Toggle]_DistortMainTex("Distort MainTex", Float) = 0
            [Toggle]_DistortSubTex("Distort SubTex", Float) = 0
            [Toggle]_DistortDissolveTex("Distort DissolveTex", Float) = 0
            [Toggle]_DistortMaskTex("Distort MaskTex", Float) = 0


        
        [Space(10)]        
        [Header(Dissolve Tex)]
        [Toggle(_DISSOLVE_ON)] _DISSOLVE_ON ("Use Dissolve Tex", Float) = 0

            _DissolveTex("DissolveTex", 2D) = "white" {}
            _DissolveTexSpeedAndOffset("DissolveTex Speed And Offset",vector) = (0,0,0,0)
            [HDR]_DissolveColor("Dissolve Color", Color) = (1,1,1,1)
            _DissolveFactor("Dissolve Factor", Range( 0 , 1)) = 0
            _DissolveSoft("Dissolve Soft", Range( 0.01 , 1)) = 0.5
            _DissolveWide("Dissolve Wide", Range( 0 , 0.99)) = 0.5

            [Toggle] _DissolveTexUsePolarCoord("DissolveTex Use PolarCoord", float) = 0.0


        [Space(10)]
        [Header(Mask Tex)]
        [Toggle(_MASK_ON)] _MASK_ON ("Use Mask Tex", Float) = 0

            _MaskTex("MaskTex", 2D) = "white" {}
            _MaskTexSpeedAndOffset("MaskTex Speed And Offset",vector) = (0,0,0,0)

            _MaskValueOffset("MaskValue Offset", Range(-1,1)) = 0
            _MaskValuePower("MaskValue Power", Range(1,20)) = 1
            [Toggle] _MaskTexUsePolarCoord("MaskTex Use PolarCoord", float) = 0.0



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

        Cull[_Cull]
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]
        


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #pragma shader_feature_local _SUB_ON
            #pragma shader_feature_local _DISSOLVE_ON
            #pragma shader_feature_local _DISTORT_ON
            #pragma shader_feature_local _MASK_ON

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
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                //float4 sceneUV : TEXCOORD3;
                
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            //UI

            float4 _MainAddST;
            float4 _MainTexSpeedAndOffset;
            float _MainColorIntensity;

            //
            sampler2D _SubTex;
            float4 _SubTex_ST;
            float4 _SubTexColor;
            float4 _SubTexSpeedAndOffset;
            float _SubTexUsePolarCoord;
            float _SubTexBlendMod;
            //
            sampler2D _DistortTex;
            float4 _DistortTex_ST;
            float _DistortFactorU;
            float _DistortFactorV;
            float4 _DistortTexSpeedAndOffset;
            float _DistortMainTex;
            float _DistortSubTex;
            float _DistortDissolveTex;
            float _DistortMaskTex;
            float _DistortTexUsePolarCoord;

            //
            sampler2D _DissolveTex;
            float4 _DissolveTex_ST;
            float4 _DissolveColor;
            float4 _DissolveTexSpeedAndOffset;
            float _DissolveFactor;
            float _DissolveSoft;
            float _DissolveWide;
            float _DissolveTexUsePolarCoord;

            //
            sampler2D _MaskTex;
            float4 _MaskTex_ST;
            float4 _MaskTexSpeedAndOffset;
            float _MaskValueOffset;
            float _MaskValuePower;
            float _MaskTexUsePolarCoord;
            

            


            half4 ColorBlend(half4 base, half4 blend , float mod)
            {
                half alpha = base.a;
                if(mod == 1)
                {
                    half3 rgb = lerp(base.rgb,base.rgb + blend.rgb,blend.a);
                    return half4(rgb,alpha);
                }
                if(mod == 2)
                {
                    half3 rgb = lerp(base.rgb,base.rgb * blend.rgb,blend.a);
                    return half4(rgb,alpha);
                }
                //0
                half3 rgb = lerp(base,blend,blend.a);
                return half4(rgb,alpha);

            }


            void PolarCoord(inout float2 UV ,float2 Center)
            {
                float2 delta = UV - Center;
                float radius = length(delta) * 2;
                float angle = atan2(delta.x, delta.y) * 1.0/6.28;
                UV = float2(radius, angle);
            }
            void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
            {
                float2 delta = UV - Center;
                float radius = length(delta) * 2 * RadialScale;
                float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
                Out = float2(radius, angle);
            }
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
			{
			    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
			}


            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                
                OUT.texcoord = v.texcoord.xy;
                
                //OUT.sceneUV = ComputeScreenPos (OUT.vertex);

                
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float4 finalResoult = float4(0,0,0,0);
                float2 uv = IN.texcoord.xy;
                float2 main_uv =  IN.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
                float time = _Time.y;

                //扭曲数值计算
                #ifdef _DISTORT_ON
                float2 uv_DistortTex_speed = _DistortTexSpeedAndOffset.xy;
                float2 uv_DistortTex = uv * _DistortTex_ST.xy + _DistortTex_ST.zw;
                float2 uv_DistortTex_Center =  float2(0.5,0.5) * _DistortTex_ST.xy;
                //使用极坐标
                UNITY_BRANCH
                if(_DistortTexUsePolarCoord){PolarCoord(uv_DistortTex,uv_DistortTex_Center);}
                    uv_DistortTex = frac( time * uv_DistortTex_speed) + uv_DistortTex + _DistortTexSpeedAndOffset.zw;
                half4 distortTex = 	tex2D(_DistortTex,uv_DistortTex);
                
                half2 distortValue = distortTex.r * half2( _DistortFactorU,_DistortFactorV);
                #endif
               


                
                float2 uv_MainTex_speed = _MainTexSpeedAndOffset.xy;
                float2 uv_MainTex =  main_uv * _MainAddST.xy + _MainAddST.zw;
                    //应用主纹理扭曲
                    #ifdef _DISTORT_ON
                     uv_MainTex += distortValue * _DistortMainTex;
                    #endif
                float2 uv_MainTex_offset = uv_MainTex + frac(time * uv_MainTex_speed) + _MainTexSpeedAndOffset.zw;
                ///UIMainTex
                half4 color = IN.color * (tex2D(_MainTex, uv_MainTex_offset) + _TextureSampleAdd);
                color.rgb *=  _MainColorIntensity;
                finalResoult = color;

                
            #ifdef _SUB_ON
                float2 uv_SubTex_speed = _SubTexSpeedAndOffset.xy;
                float2 uv_SubTex =  uv * _SubTex_ST.xy + _SubTex_ST.zw;
                float2 uv_SubTex_Center =  float2(0.5,0.5) * _SubTex_ST.xy;
                //使用极坐标
                UNITY_BRANCH
                if(_SubTexUsePolarCoord){PolarCoord(uv_SubTex,uv_SubTex_Center);}
                    //应用副纹理扭曲
                    #ifdef _DISTORT_ON
                     uv_SubTex += distortValue * _DistortSubTex;
                    #endif
                float2 uv_SubTex_offset = uv_SubTex + frac(time * uv_SubTex_speed) + _SubTexSpeedAndOffset.zw;
                half4 subColor = tex2D(_SubTex, uv_SubTex_offset) * _SubTexColor;
                //应用
                finalResoult = ColorBlend(finalResoult,subColor,_SubTexBlendMod);
            #endif




                //溶解相关
            #ifdef _DISSOLVE_ON
                float2 uv_DissolveTex_speed = _DissolveTexSpeedAndOffset.xy;
                float2 uv_DissolveTex = uv * _DissolveTex_ST.xy + _DissolveTex_ST.zw;
                float2 uv_DissolveTex_Center =  float2(0.5,0.5) * _DissolveTex_ST.xy;
                //使用极坐标
                UNITY_BRANCH
                if(_DissolveTexUsePolarCoord){PolarCoord(uv_DissolveTex,uv_DissolveTex_Center);}
                //应用扭曲纹理扭曲
                #ifdef _DISTORT_ON
                    uv_DissolveTex += distortValue * _DistortDissolveTex;
                #endif
                uv_DissolveTex = frac( time * uv_DissolveTex_speed) + uv_DissolveTex + _DissolveTexSpeedAndOffset.zw;
                
                
                half4 dissolveTex = tex2D(_DissolveTex,uv_DissolveTex);
                half dissolveTex_value = dissolveTex.r;
                half dissolveFactor = _DissolveFactor;

                //溶解算法
                float rampValue = 1;
                Unity_Remap_float(dissolveTex_value,float2(0,1 ),float2(1,2), rampValue);
				float value =  rampValue - dissolveFactor*2;
				float softvalue = smoothstep(_DissolveSoft*0.5,0.5,value);
				float edge = smoothstep(_DissolveWide-_DissolveSoft*0.5,_DissolveWide,softvalue);

                finalResoult.rgb = lerp( _DissolveColor.rgb,finalResoult.rgb,edge);
                finalResoult.a *= softvalue;
            #endif
                

                // 遮罩相关
                half mask_final = 1.0;
            #ifdef _MASK_ON

                float2 uv_MaskTex_speed = _MaskTexSpeedAndOffset.xy;
                float2 uv_MaskTex = uv * _MaskTex_ST.xy + _MaskTex_ST.zw;
                float2 uv_MaskTex_Center =  float2(0.5,0.5) * _MaskTex_ST.xy;
                //使用极坐标
                UNITY_BRANCH
                if(_MaskTexUsePolarCoord){PolarCoord(uv_MaskTex,uv_MaskTex_Center);}
                //应用遮罩纹理扭曲
                #ifdef _DISTORT_ON
                    uv_MaskTex += distortValue * _DistortMaskTex;
                #endif
                half2 uv_MaskTex_Offset = uv_MaskTex + frac(time * uv_MaskTex_speed) + _MaskTexSpeedAndOffset.zw;
                 
                half4 maskTex = tex2D(_MaskTex,uv_MaskTex_Offset);
                mask_final = maskTex.r;
                mask_final = saturate(pow( clamp(mask_final + _MaskValueOffset,0,1), _MaskValuePower));

            #endif
                finalResoult.a *= mask_final;

                
                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
                ///


                finalResoult = saturate(finalResoult);
                
                return finalResoult;
            }
            ENDCG
            
        }
    }

//	CustomEditor "Render.Editor.ShaderGUI.CustomGUI"

}
