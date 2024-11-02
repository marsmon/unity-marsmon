Shader "YoFi/UI/Silhouette"
{
    Properties
    {
        _MainTex("Main Tex",2D) = "white"{}
        _Tint("Color",Color) = (1,1,1,1)
    }
        SubShader
    {
        Tags{ "RenderType" = "Transparent" "Queue"="Transparent"}
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Tint;
            
            struct a2v
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };
            struct v2f
            {
                float2 uv :TEXCOORD0;
                float4 pos : SV_POSITION;
            };
            v2f vert(a2v a)
            {
                v2f f;
                f.uv = TRANSFORM_TEX(a.texcoord, _MainTex);
                f.pos = UnityObjectToClipPos(a.vertex);
                return f;
            }
            fixed4 frag (v2f i) :SV_Target
            {
                const fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 color = fixed4(_Tint.rgb, texColor.a);
                return color;
            }
            ENDCG
        }
    }
}