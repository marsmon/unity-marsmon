//2021.07.10 简单实现. 马卡龙Tex
Shader "YoFi/Editor/GGS_Char_Simple_Edit"
{
    Properties
    { 
        [Toggle]_IsCustomLight("是否使用实时光调试",float) = 0 
        [HideInInspector]_MainTex ("Main Tex", 2D) = "white" {}
        [HideInInspector]_DarkTex("Dark Tex",2D) = "white"{}
        
        _BaseTex ("Base Tex", 2D) = "white" {}
        _ILMTex("ILMTex",2D) = "white"{}
        _SssTex("SSS Tex",2D) = "white"{}
        _DetailTex("DetailTex",2D) = "white"{}  
        _EmissionTex ("EmissionTex",2D) = "black"{} 
        [Header (Bloom)]
        [Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 0 
        _CBLocalTex("CB Tex",2D) = "black"{}  
        [Header (Outline)]
        _Outline ("Outline width",float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        // Tags { "Quene"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100         
        
        CGINCLUDE 
        #include "../Common/Included/YF_ZMXS_3DmodelIncluded.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc" 
        

        sampler2D _BaseTex,_ILMTex,_SssTex,_DetailTex,_EmissionTex; 
        // float4 _OutlineColor;  
        float _IsCustomLight;

        
        fixed4 fragKWLEdit (v2f i) : SV_Target
        {
            
            fixed4 cbcolor = tex2D(_CBLocalTex,i.uv.xy);
            fixed4 baseColor = tex2D(_BaseTex, i.uv.xy);
            fixed3 sssColor = tex2D(_SssTex,i.uv.xy).rgb;


            fixed4 ilm = tex2D(_ILMTex,i.uv.xy);
            fixed4 emission = tex2D(_EmissionTex,i.uv.xy);
            fixed3 detail = tex2D(_DetailTex,i.uv.xy);
            // detail = lerp(_aaaColor * detail ,detail,detail);

            half ao = saturate((1.0 - 0.7) * 50);

            half3 worldNormal = normalize(i.worldNormal);
            // return float4(worldNormal,1);
            half3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
            if(_IsCustomLight < 1){
                worldLightDir = normalize(-_CustomLightDir.xyz); 
            } 
            // half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
            half NdotL = dot(worldLightDir,worldNormal);
            half sssFactor = saturate((NdotL * 0.5 + 0.5 - 1.0 * 0.5  + DT_CHAR_OUTNL(i)) * 50) ;//* ao;
            // return sssFactor;
            fixed4 finalColor = 1;
            finalColor.rgb = lerp(sssColor, baseColor, sssFactor) * detail * ilm.a;// 和细节贴图做了正片叠底* ilm.a; * detail * ilm.a 
            finalColor.rgb = saturate(finalColor.rgb + emission.rgb);
            // finalColor.rgb =  ilm.a;
            return ZMXSBloomOutfrag(finalColor,cbcolor); 
        }

        
        ENDCG 
        // Pass
        // {
            //     ZWrite On
            //     colorMask 0
            
            //     CGPROGRAM
            //     #pragma vertex vertKWL
            //     #pragma fragment fragOuline
            //     ENDCG
        // }
        
        // 第一个Pass使用轮廓线颜色渲染整个背面的面片
        Pass
        { 
            // ZWrite off
            NAME "Outline" 
            Cull Front 
            CGPROGRAM
            #pragma vertex vertOuline 
            #pragma fragment fragOuline 
            ENDCG
        }
        Pass
        {
            // ZWrite off
            Tags{"LightMode"="ForwardBase"} 
            CGPROGRAM 
            
            #pragma vertex vertKWL
            #pragma fragment fragKWLEdit
            ENDCG
        }
        
    }
    CustomEditor "YF.Art.YF_Char_BaseGUI"
}
