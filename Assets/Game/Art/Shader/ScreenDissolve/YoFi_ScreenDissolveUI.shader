
Shader "YoFi/UI/ScreenDissolveUI"
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
		[Toggle]GrayToOrigin("Gray To Origin", int) = 1
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
            half GrayToOrigin;

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

                float dist = distance(_DissolveCenterUV, IN.uv.xy);

                dissove = dissove + dist * _WorldSpaceScale;

                float edge_area = saturate(1 - saturate((dissove - _Clip - _EdgeWidth) / _Smoothness));
                edge_area *= _DlvEdgeColor.a;
                if (GrayToOrigin == 1) {
                    color.rgb = dot(color.rgb * (edge_area), GreyColor.rgb) + color.rgb * (1-edge_area);
                    float grey = dot(color.rgb, GreyColor.rgb);
                } else {
                    color.rgb = color.rgb * (edge_area) + dot(color.rgb * (1-edge_area), GreyColor.rgb);
                    float grey = dot(color.rgb, GreyColor.rgb);
                }

                return color;
            }
            ENDCG
        }
    }
}