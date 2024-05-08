Shader "Hidden/YoFi/ShineWarsShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            sampler2D _MaskTex;
            float4 _SunColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 LastCol = 0;
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 Maskcol = saturate(tex2D(_MaskTex, i.uv));
                fixed tmpValue = max(Maskcol.r,max(Maskcol.g,Maskcol.b));
                
                fixed3 lightCol = saturate( col.rgb * 0.4 + clamp(0.0,0.5, col.rgb * Maskcol.rgb * Maskcol.a));
                LastCol.rgb = lerp( _SunColor.rgb * col.rgb, lightCol , tmpValue);
                LastCol.a = col.a;
                return LastCol;
            }
            ENDCG
        }
    }
}
