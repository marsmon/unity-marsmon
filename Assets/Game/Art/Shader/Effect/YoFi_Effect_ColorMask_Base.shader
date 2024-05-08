
//guid: b31bcfefb30f2684d82ced76b7ae0bf2
Shader "YoFi/Effect/ColorMask_Base" {
    Properties {
        // [HideInInspector]
        [HideInInspector]_mode("__Mode",float)= 2049 
        [HideInInspector]_Cull("__cull", Float) = 0 //[Enum(UnityEngine.Rendering.CullMode)]
        [HideInInspector]_ZWrite("__zw", Float) = 0.0//[Enum(Off,0.0,On,1.0)]
        [HideInInspector]_SrcBlend("__src", Float) = 5.0 //[Enum(UnityEngine.Rendering.BlendMode)]
        [HideInInspector]_DstBlend("__dst", Float) = 10.0//[Enum(UnityEngine.Rendering.BlendMode)]
        [HideInInspector]_ColorMask("__colormask", Float) = 14 //[Enum(Default,14.0,On Alpha,15.0)]

 
        [Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 0
        _CBLocalThreshhold("CB Threshhold",range(0.0,1.0)) = 1
        _CBLocalColor("CB Color",Color) = (1,1,1,1)
        _CBLocalLight("CB 亮度",float) = 1
        
        [Header(Texture)]
        [MainColor]_MainColor("Tint Color", Color) = (0.75,0.75,0.75,1.0)
        [MainTexture]_MainTex("Main Tex(RGBA)", 2D) = "white"{}
        _MainAnimData("ma",vector) = (0,0,0,0)
        _MainValue("mv",vector) = (1,0,0,0)
        _MainBase("mb",vector) = (1,0,0,0)

        

        _Effect0Color("Tint Color", Color) = (0.75,0.75,0.75,1.0)
        _Effect0Tex("Main Tex(RGBA)", 2D) = "white"{}
        _Effect0AnimData("ma",vector) = (0,0,0,0)
        _Effect0Value("mv",vector) = (1,0,0,0)
        _Effect0Base("mb",vector) = (1,0,0,2)


        [Header(Fresnel)]
        [HDR]_FresnelColor ("Fresnel Color", Color) = (0.5,0.5,0.5,1.0)
        _FresnelValue("fnl_v",vector) = (0,0.5,0,1)
        _FresnelBase("fnl_b",vector) = (0,0,0,0)

        _BillboradSize("BBs",vector) = (1,1,1,0)
        [PerRendererData]_ScrollViewRect("svr",vector) = (0,0,0,0)

        [Toggle]_IsClipGround("是否有地面[8h6H,Y = 0]",float) = 0
        _IsClipGroundBlur("地面接缝过度[8h6h]",float) = 0.5
        [Toggle]_IsNewColorMask("下山新亮度开关",float) = 0
 
        
    }

    Category
    {
        Tags { "Queue"="Transparent" "RenderType" = "Transparent"  } //"IgnoreProjector" = "True" "PerformanceChecks"="False"
        Lighting Off
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        Cull [_Cull]
        ColorMask [_ColorMask]

        SubShader {
            Pass {
                CGPROGRAM
                #pragma shader_feature __ _TIMEDT_ON
                #pragma shader_feature __ _FRESNEL_ON
                #pragma shader_feature __ _ALPHATEST_ON
                // #pragma shader_feature __ _POLAR_ON
                // _DTBILLBORAD_FX  弹道面片  _DTBILLBORAD_UI 对应scrollview裁切  _DTBILLBORAD_VC view Camer永远对着摄像机
                #pragma shader_feature __ _DTBILLBORAD_FX _DTBILLBORAD_VC
                
                // #pragma multi_compile_fog
                // 可以关闭 FOG 不使用时可以注销掉，减少变体 FOG_LINEAR FOG_EXP FOG_EXP2
                #pragma shader_feature FOG_ON
                #pragma multi_compile_instancing
                
                #pragma vertex vertBase
                #pragma fragment fragColMask
                #pragma target 2.0  

                
                #include "../Common/Included/YFEffectBase.cginc"
                fixed4 _cccc;
                
                ENDCG
            }
        }        
    }
    CustomEditor "DiteScripts.YF.EditoShaderGUI.YFEffectBaseGUI"
    FallBack off    
}
