
Shader "YoFi/GGS_Char_Simple_Graphic"   
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white"{}
        _DarkTex("Dark Tex",2D) = "white"{}
        [Header (Bloom)]
        [Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 0 
        _CBLocalTex("CB Tex",2D) = "black"{}  
        [Header (Outline)]
        _Outline ("Outline width",float) = 1

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"} 
        // LOD 100 
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        CGINCLUDE
        #include "../Common/Included/YF_ZMXS_3DmodelIncluded.cginc"
        #include "UnityUI.cginc"
        ENDCG
        
        pass{
            ZWrite On
            colorMask 0
            CGPROGRAM
            #pragma vertex vertKWL
            #pragma fragment frag
            fixed4 frag (v2f i) : SV_Target{
                return 0;
            }
            ENDCG
        }

        Pass
        {
            // 第一个Pass使用轮廓线颜色渲染整个背面的面片
            NAME "Outline" 
            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            
            offset 1,10 
            Cull Front
            CGPROGRAM
            #pragma vertex vertOuline 
            #pragma fragment fragOuline 
            ENDCG
        }  
        Pass
        { 
            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            CGPROGRAM
            #pragma vertex vertKWL
            #pragma fragment fragKWL 
            

            fixed4 fragKWL (v2f i) : SV_Target
            {
                //CB的颜色贴图，不能动但能缩小
                fixed4 cbcolor = tex2D(_CBLocalTex,i.uv.xy);  
                //基础固有色，RGBA都不可以动
                fixed4 baseColor = tex2D(_MainTex, i.uv.xy);
                fixed4 sssColor = tex2D(_DarkTex,i.uv.xy);

                half3 worldNormal = normalize(i.worldNormal);
                half3 worldLightDir = normalize(-_CustomLightDir.xyz); 
                half NdotL = dot(worldLightDir,worldNormal); 
                // half sssFactor = saturate(NdotL * 25.0); 
                half sssFactor = saturate((NdotL + DT_CHAR_OUTNL(i)) * 25.0);  
                fixed4 finalColor = 1;
                finalColor.rgb = saturate(lerp(sssColor.rgb,baseColor.rgb,sssFactor));
                clip(finalColor.a - 0.001);
                return ZMXSBloomOutfrag(finalColor,cbcolor);
            }
            ENDCG
        }
    }
    CustomEditor "YF.Art.YF_Char_BaseGUI"
}
