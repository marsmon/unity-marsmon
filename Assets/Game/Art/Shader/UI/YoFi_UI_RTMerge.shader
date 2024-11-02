// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "YoFi/UI/RTMerge"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Texture2 ("Texture2", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

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
            sampler2D _Texture2;

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                fixed4 col1 = tex2D(_MainTex, i.uv);
                fixed4 col2 = tex2D(_Texture2, i.uv);
                fixed4 col;
                col = lerp(col1, col2, col2.a);
                return col;
            }
        ENDCG
        }
    }
}
