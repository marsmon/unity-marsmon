
Shader "YoFi/GGS_Char_Shadow"
{
    Properties
    { 
        
        _MainTex ("Main Tex", 2D) = "white"{}
        _jiaodu("jiaodu",float) = 0

        _FillColor("Fill color(fx)",Color) = (1,1,1,0.5)
        [Enum(Default,0,Add,1,Multiplica,2,Blend,3,Alpha,4)] _Fillblend("Blend Value(Fill,fx)",float) = 0
    }
    SubShader
    { 
        Tags {"Queue"="Transparent" "IgnoreProjector"="True"  "RenderType"="Transparent"} 
        LOD 100   
        Lighting Off  
        
        CGINCLUDE
        #include "../Common/Included/YF_ZMXS_3DmodelIncluded.cginc"
        

        #ifdef IN_EDITOR_MODE 
            #define _ShadowViewAngle _jiaodu 
        #endif  
        inline float4 Lastout(in float4 finalcol) { 
            fixed4 f = blendmodel(finalcol);
            f.a = saturate(f.a * finalcol.a); 
            return f;
        }

        
        float _ShadowViewAngle;

        v2f vertKWLShadow(appdata v)
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
            //基础固有色，RGBA都不可以动
            fixed4 finalColor = tex2D(_MainTex, i.uv.xy); 
            
            return Lastout(finalColor);
        } 

        ENDCG 
        //后
        Pass
        { 
            Cull off
            ZWrite off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vertKWLShadow
            #pragma fragment fragKWL
            #include "UnityCG.cginc" 
            ENDCG
        }
        
    }
    // CustomEditor "YF.Art.YF_Char_BaseGUI"
}
