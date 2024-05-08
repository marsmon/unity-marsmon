
Shader "YoFi/GGS_Char_Simple"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white"{}
        _DarkTex("Dark Tex",2D) = "white"{}
        [Header (Bloom)]
        [Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 1
        _CBLocalTex("CB Tex",2D) = "black"{}  
        [Header (Outline)]
        _Outline ("Outline width",float) = 1
        // _Outline2 ("Outline width2",float) = 1

        _FillColor("Fill color(fx)",Color) = (1,1,1,0.5)
        [Enum(Default,0,Add,1,Multiplica,2,Blend,3,Alpha,4)] _Fillblend("Blend Value(Fill,fx)",float) = 0

        [Enum(OFF,0,R,1,G,1,B,2,A,3)] _v("Blend Value(Fill,fx)",float) = 0
    }
    SubShader
    {
        Tags{"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100
        
        CGINCLUDE
        #include "../Common/Included/YF_ZMXS_3DmodelIncluded.cginc"
        ENDCG
        

        Pass
        { 
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vertKWL
            #pragma fragment fragKWL 

            #include "UnityCG.cginc" 
            
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
                
                half sssFactor = saturate((NdotL + DT_CHAR_OUTNL(i)) * 25.0);  
                fixed4 finalColor = 1;
                finalColor.rgb = saturate(lerp(sssColor.rgb,baseColor.rgb,sssFactor));
                finalColor = ZMXSBloomOutfrag(finalColor,cbcolor);
                finalColor = blendmodel(finalColor);
                return finalColor;
            }
            ENDCG
        }

        Pass
        {
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            
            // 第一个Pass使用轮廓线颜色渲染整个背面的面片
            NAME "Outline" 
            offset 1,10 
            Cull Front 
            CGPROGRAM
            #pragma vertex vertOuline 
            #pragma fragment fragOuline 
            ENDCG
        } 

   }
    CustomEditor "YF.Art.YF_Char_BaseGUI"
}
