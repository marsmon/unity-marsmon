Shader "Hidden/DT/YFCharBaseCombineTexData"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ILMTex ("_ILMTex", 2D) = "white" {}
        _DetailTex ("_DetailTex", 2D) = "white" {}
        _EmissionTex ("_EmissionTex", 2D) = "black" {} 
        // _pow("bloom pow",float) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _ILMTex,_DetailTex,_EmissionTex;

            fixed4 frag (v2f i) : SV_Target
            { 
                fixed4 baseColor = tex2D(_MainTex, i.uv);
                fixed4 ilm = tex2D(_ILMTex,i.uv);
                fixed4 emission = tex2D(_EmissionTex,i.uv);
                fixed3 detail = tex2D(_DetailTex,i.uv);
                
                fixed4 finalColor = baseColor;
                // finalColor.a = baseColor.a;
                finalColor.rgb = baseColor.rgb * detail.rgb * ilm.a;
                finalColor.rgb = saturate(finalColor.rgb + emission.rgb); 
                return finalColor; 
            }
            ENDCG
        } 
    }
}
