// Shader "Unlit/YoFi_Effect_Base"
Shader "YoFi/Effect/FX_Base"
{
    Properties
    { 
        //=======================Key Properties======================= 
        [Toggle] _TIMEDT("__TIMEDT", Float)=0
        [Toggle] _SPROUT("__SPROUT", Float)=0
        [Toggle] _FRESNEL("__FRESNEL", Float)=0
        [Toggle] _DTDISTOR("__DTDISTOR", Float)=0
        [Toggle] _DTCOLORMASK("__DTCOLORMASK", Float)=0
        [Toggle] _DTDISSOLUTION("__DTDISSOLUTION", Float)=0
        [Toggle] _ALPHATEST("__ALPHATEST", Float)=0
        //=======================BASE Properties=======================
        _mode("__Mode",int)= 516
        _Shadermode("__ShaderMode",int)= 5
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("__cull", Float) = 0
        [Enum(Off,0.0,On,1.0)]_ZWrite("__zw", Float) = 0.0
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("__src", Float) = 5.0
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("__dst", Float) = 10.0
        _ColorMask("__colormask", Float) = 14 
        _ClipValue("Cull off",float) = 0.5        
        //=======================Bloom Properties=======================
        _CBLocalThreshhold("CB Threshhold",range(0.0,1.0)) = 1
        _CBLocalColor("CB Color",Color) = (1,1,1,1)
        _CBLocalLight("CB 亮度",float) = 1
        [Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 0
        //=======================Main Properties========================
        [Header(Main)]
        [MainTexture]_MainTex("Main Tex(RGBA)", 2D) = "white"{}
        [MainColor][Gamma]_TintColor("Tint Color", Color) = (1.0,1.0,1.0,1.0)
        _Brightness("Brightness",float) = 1
        _MainValue1("Value 1",vector) = (0,0,0,0) 
        _MainbitSw("Main Mode",int)= 2432
        //=======================Dist Properties=======================
        [Header(Dist)]
        _DistTex("Dist Tex(RGBA)", 2D) = "black"{}
        _DistValue1("Dist Value 1",vector) = (0,0,0,0)        
        _DistValue2("Dist Value 2",vector) = (0,0,1,0) 
        _DistbitSw("Dist Mode",int)= 2448
        //=======================Color Mask Properties=======================
        [Header(Color Mask)]
        _ColorMaskTex("ColorMask Tex(RGBA)", 2D) = "black"{}
        _ColorMaskValue0("ColorMask  Value 0",vector) = (1,1,1,1) 
        _ColorMaskValue1("ColorMask  Value 1",vector) = (0,0,0,0)
        _ColorMaskValue2("ColorMask  Value 2",vector) = (0,0,1,0) 
        _ColorMaskbitSw("ColorMask Mode",int)= 2448
        //=======================FNL Properties=======================
        [Header(FNL)] 
        _FNLValue0("FNL Value 0",vector) = (1,1,1,1)
        _FNLValue1("FNL Value 1",vector) = (0,0,0,0)        
        _FNLValue2("FNL Value 2",vector) = (0.5,0,1,0)  
        _FNLbitSw("FNL Mode",int)= 2448
        //=======================Dissolution Properties=======================
        [Header(Dissolution)]
        _DissTex("Diss Tex(RGBA)", 2D) = "white"{}
        _DissValue0("Diss  Value 0",vector) = (1,1,1,1)
        _DissValue1("Diss  Value 1",vector) = (0,0,0,0)        
        _DissValue2("Diss  Value 2",vector) = (1,0,1,1) 
        _DissbitSw("Diss Mode",int)= 2448 
        _DissbitSw2("Diss Mode2",int)= 9
        //=======================other Properties=======================
        [Header(Other)] 
        _SproutValue("Sprout Value",vector) = (1,0,0,0) 
        _SproutbitSw("Sprout Mode",int)= 2432

        [Toggle]_ClipGroundbitSw("是否有地面[Y = 0]",float) = 0
        _ClipGroundValue("地面接缝过度",float) = 0.5

        // _StencilComp ("Stencil Comparison", Float) = 8
        // _Stencil ("Stencil ID", Float) = 0
        // _StencilOp ("Stencil Operation", Float) = 0
        // _StencilWriteMask ("Stencil Write Mask", Float) = 255
        // _StencilReadMask ("Stencil Read Mask", Float) = 255
        
    }
    SubShader
    {
        // Stencil
        // {
            //     Ref [_Stencil]
            //     Comp [_StencilComp]
            //     Pass [_StencilOp]
            //     ReadMask [_StencilReadMask]
            //     WriteMask [_StencilWriteMask]
        // }

        // Tags
        // {
            //     "Queue"="Transparent"
            //     "IgnoreProjector"="True"
            //     "RenderType"="Transparent"
            //     "PreviewType"="Plane"
            //     "CanUseSpriteAtlas"="True"
        // }
        // Cull Off
        // Lighting Off
        // ZWrite Off
        // ZTest [unity_GUIZTestMode]
        // Blend SrcAlpha OneMinusSrcAlpha
        // ColorMask [_ColorMask]

        Tags { "RenderType"="Opaque" } 
        Lighting Off
        BlendOp Add //[_BlendOp]
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        Cull [_Cull]
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma shader_feature __ _TIMEDT_ON
            #pragma shader_feature __ _ALPHATEST_ON
            
            #pragma shader_feature __ _SPROUT_ON
            #pragma shader_feature __ _FRESNEL_ON
            #pragma shader_feature __ _DISTOR_ON
            #pragma shader_feature __ _COLORMASK_ON
            #pragma shader_feature __ _DISSOLUTION_ON
            
            #pragma multi_compile_instancing
            
            #pragma vertex vert
            #pragma fragment frag
            #include "../Common/Included/YFEffectCGFunVF.cginc"

            sampler2D _MainTex;float4 _MainTex_ST;
            sampler2D _DistTex;float4 _DistTex_ST;
            sampler2D _ColorMaskTex;float4 _ColorMaskTex_ST;
            sampler2D _DissTex;float4 _DissTex_ST;
            float4 _TintColor;
            float _Brightness;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 texcoord2 : TEXCOORD2;
            };

            struct v2f
            {
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4x4 texcoord :TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            }; 

            v2f vert (appdata v)
            {
                v2f o = (v2f)0;                
                Set_ParticleCustom(float4(v.texcoord0.zw,v.texcoord1.xy),float4(v.texcoord1.zw,v.texcoord2.xy),v.color.a);
                o.uv = v.texcoord0.xy;
                o.color = v.color;  
                float nv = abs(dot(UnityObjectToWorldNormal(v.normal), dt_WorldViewDir(v.vertex)));                               
                float sprotDiss;
                float4 pos = v.vertex;
                sproutVert(v.texcoord0.xy,v.normal,pos,sprotDiss);
                o.vertex = UnityObjectToClipPos(pos);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.texcoord[0] = mul(unity_ObjectToWorld, v.vertex); 
                o.texcoord[1] = float4(nv,sprotDiss,0,0);
                o.texcoord[2] = float4(v.texcoord0.zw,v.texcoord1.xy);
                o.texcoord[3] = float4(v.texcoord1.zw,v.texcoord2.xy);
                return o;
            } 
            fixed4 frag (v2f i) : SV_Target
            {
                Set_ParticleCustom(i.texcoord[2],i.texcoord[3],i.color.a);                
                float2 distUV = DistFrag(_DistTex,TRANSFORM_TEX(i.uv.xy,_DistTex));
                fixed4 col = 0;
                MainTexData(_MainTex,TRANSFORM_TEX(i.uv.xy,_MainTex),_TintColor,distUV,col);                
                col *= i.color;
                ColorMaskFrag(_ColorMaskTex,col,TRANSFORM_TEX(i.uv.xy,_ColorMaskTex),distUV);
                FNLFrag(col,i.texcoord[1].x,distUV);
                SproutFrag(col,i.texcoord[1].y);
                DissolutionFrag(_DissTex,col,TRANSFORM_TEX(i.uv.xy,_DissTex),distUV);
                ClipGroundVert(col,i.texcoord[0]);
                col.rgb *= _Brightness;
                col = LastExportColor(col);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    } 
    CustomEditor "YF.Art.YFShaderEditor.YoFi_FXShader_GUIBase"
}
