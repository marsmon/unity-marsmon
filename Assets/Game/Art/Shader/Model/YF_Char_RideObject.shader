
Shader "YoFi/GGS_Char_RideObject"
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
        _jiaodu("jiaodu",float) = 0
        // _Outline2 ("Outline width2",float) = 1

        _FillColor("Fill color(fx)",Color) = (1,1,1,0.5)
        [Enum(Default,0,Add,1,Multiplica,2,Blend,3,Alpha,4)] _Fillblend("Blend Value(Fill,fx)",float) = 0

        _v("Aaa",float) = 1
    }
    SubShader
    {
        Tags{"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100
        
        CGINCLUDE
        #include "../Common/Included/YF_ZMXS_3DmodelIncluded.cginc"

        #ifdef IN_EDITOR_MODE 
            #define _ShadowViewAngle _jiaodu 
        #endif
        
        float _ShadowViewAngle;

        ENDCG
        

        Pass
        { 
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vertKWLRideObject
            #pragma fragment fragKWL 

            #include "UnityCG.cginc" 
                        
            v2f vertKWLRideObject(appdata v)
            {
                v2f o = (v2f)0; 
                float _viewAngle = _ShadowViewAngle;
                float4 worldPos = GetNewWorldPos(v.vertex, _viewAngle);
                o.vertex = UnityWorldToClipPos(worldPos);
                o.worldPos = worldPos.xyz;

                // 重新计算深度值
                float4 verticalClipPos = UnityObjectToClipPos(v.vertex);
                o.vertex.z = verticalClipPos.z / verticalClipPos.w * o.vertex.w; 
                o.uv.xy = TRANSFORM_TEX(v.uv0, _MainTex); 
                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                o.color = v.color;
                return o;
            }
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
            #pragma vertex vertOulineRideObject 
            #pragma fragment fragOuline 

            v2f vertOulineRideObject(appdata v)
            {
                v2f o = (v2f)0;

                float _viewAngle = _ShadowViewAngle;
                float4 worldPos = GetNewWorldPos(v.vertex, _viewAngle);
                // //顶点和法线转到观察空间
                o.vertex = mul(UNITY_MATRIX_V, worldPos);
                float3 normal = normalize(mul((float3x3)UNITY_MATRIX_MV, v.normal));
                // 对z分量进行处理，防止内凹的模型扩张顶点后出现背面面片遮挡正面面片的情况
                normal.z = -0.5;
                // 在观察空间下把模型顶点沿着法线方向向外扩张一段距离
                o.vertex = o.vertex + float4(normal, 0) * _Outline * 0.1 * v.color.a;
                // 转换到裁剪空间
                o.vertex = mul(UNITY_MATRIX_P, o.vertex);
                // 重新计算深度值
                float4 verticalClipPos = UnityObjectToClipPos(v.vertex);
                o.vertex.z = verticalClipPos.z / verticalClipPos.w * o.vertex.w;
                o.color = v.color;
                return o;
            }
            ENDCG
        } 

   }
    CustomEditor "YF.Art.YF_Char_BaseGUI"
}
