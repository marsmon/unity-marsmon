Shader "Builtin-YoFi/VFX/Model"
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
        _MainAlpha("MainAlpha", Range( 0 , 10)) = 1
        [Toggle(_FFNL_ON)] _FFnl("FFnl", Float) = 0
        [HDR]_FnlColor("FnlColor", Color) = (1,1,1,1)
        _FnlScale("FnlScale", Range( 0 , 2)) = 0
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


        [Toggle]_UseStencil("Use Stencil", Float) = 0
        _StencilValue ("Stencil Value", Range(60,150)) = 60
        [Enum(UnityEngine.Rendering.CompareFunction)]_CompOp ("Comp Op", Range(1,8)) = 8
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

            float4 _FnlColor;
            float _ReFnl;
            float _FnlScale;
            float _FnlPower;

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
            sampler2D _ColorTex;
            sampler2D _DistortTex;
            sampler2D _DissolveTex;
            sampler2D _MaskTex;
            


        ENDHLSL
        Pass
        {
            Name "WriteDepthOnly"
            Tags 
            { 
                "RenderType"="Transparent" 
                "LightMode" = "ForwardBase"
            }
            ZWrite On
            ColorMask 0
            Stencil {
                Ref [_StencilValue]
                Comp [_CompOp]
            }

        }
        UsePass "Builtin-YoFi/VFX/Universal/VFXForward"
        
    }	
    CustomEditor "VFX_Universal_GUI"
    Fallback "Hidden/InternalErrorShader"
}
