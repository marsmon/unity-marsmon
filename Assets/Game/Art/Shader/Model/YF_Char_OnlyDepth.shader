Shader "YoFi/GGS_Char_OnlyDepth"
{
    Properties
    {
//        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags{"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100

        Pass
        {
            ZWrite On
            ZTest On
            ColorMask 0
            
            CGPROGRAM
            #pragma vertex vertKWL
            #pragma fragment frag


            #include "UnityCG.cginc"
            #include "../Common/Included/YF_ZMXS_3DmodelIncluded.cginc"

            

            fixed4 frag (v2f i) : SV_Target
            {

                half4 col = half4(0,0,0,0);
                return col;
            }
            ENDCG
        }
    }
}
