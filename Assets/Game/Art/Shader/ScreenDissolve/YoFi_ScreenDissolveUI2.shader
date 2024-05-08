
Shader "YoFi/UI/ScreenDissolveUI2"
{
    Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		_DissTex("Dissolve Texture", 2D) = "white"{}

		[Vector2(1)] _DissolveCenterUV("Dissolve Center UV", Vector) = (0,1,0)
		_WorldSpaceScale("World Space Dissolve Factor", float) = 0.1


		_EdgeWidth("Edge Wdith", float) = 0
		[HDR]_DlvEdgeColor("Dissolve Edge Color", Color) = (0.0, 0.0, 0.0, 0)
		[HDR]GreyColor("Grey Color", Color) = (0.22, 0.707, 0.071, 1)
		_Smoothness("Smoothness", Range(0.001, 1)) = 0.2

		[ScaleOffset] _DissTex_Scroll("Scroll", Vector) = (0, 0, 0, 0)

		_Clip("Clip", float) = 2
	}

    SubShader
    {
		Tags { "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature ORIGIN_TO_GRAY

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;

				float4 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
			sampler2D _DissTex;
            float4 _MainTex_ST;
			float4 _DissTex_ST;
			float2 _DissolveCenterUV;
			half _Clip;
			half _WorldSpaceScale;
			half _Smoothness;
			float4 _DlvEdgeColor;
            float4 GreyColor;
			float _EdgeWidth;
			float _OpenScale;
			float _Scale;
			float _OpenRotate;
			float _Rotate;
			float _OpenOffset;
			float4 _Offset;

			float2 _DissTex_Scroll;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _DissTex) + frac(_DissTex_Scroll.xy * _Time.x);

                return o;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = tex2D(_MainTex, IN.uv.xy);
                fixed dissove = tex2D(_DissTex, IN.uv.zw).r;

				float2 newUV = IN.uv.xy;
                newUV -= float2(0.5,0.5);
                newUV /= _Clip;
                newUV += float2(0.5,0.5);
				fixed4 dissoveCol = tex2D(_DissTex, newUV);

                #ifdef ORIGIN_TO_GRAY
                    color.rgb = color.rgb * (dissoveCol.a) + dot(color.rgb * (1-dissoveCol.a), GreyColor.rgb);
                #else
                    color.rgb = color.rgb * (1-dissoveCol.a) + dot(color.rgb * (dissoveCol.a), GreyColor.rgb);
                #endif
                
                return color;
            }
            ENDCG
        }
    }
}