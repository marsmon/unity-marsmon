Shader "Builtin-YoFi/VFX/DistortRGB"
{
    Properties
	{
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
        [Enum(Normal,4,Always Top,8)] _ZTest ("ZTest Mode", float) = 4.0
		
		_MaskTex("MaskTex", 2D) = "white" {}
		_NoiseTex("NoiseTex", 2D) = "black" {}

		[Toggle] _ValueToColor("Value To Color", float) = 0.0
		[Toggle] _PolarCoordMode("Polar Coord Mode", float) = 0.0

		[Toggle] _CustomData("Custom Data", float) = 0.0
		_NoiseTexUSpeed("NoiseTex U Speed", Range( -10 , 10)) = 0
		_NoiseTexVSpeed("NoiseTex V Speed", Range( -10 , 10)) = 0
		_MaskTexUSpeed("MaskTex U Speed", Range( -10 , 10)) = 0
		_MaskTexVSpeed("MaskTex V Speed", Range( -10 , 10)) = 0
		_DistortIntensityU("Distort Intensity U", Range( -10 , 10)) = 1
		_DistortIntensityV("Distort Intensity V", Range( -10 , 10)) = 1
		
		_ChannelOffsetR("Channel Offset R", Range( -10 , 10)) = 0
		_ChannelOffsetG("Channel Offset G", Range( -10 , 10)) = 0
		_ChannelOffsetB("Channel Offset B", Range( -10 , 10)) = 0

	}
    
    SubShader
    {
        // Draw after all opaque geometry
        Tags { "Queue" = "Transparent" }

        // Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }

        // Render the object with the texture generated above, and invert the colors
        Pass
        {
            Name "Distort"
			
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest  [_ZTest]
			Offset 0 , 0
			ColorMask RGBA
			
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            CBUFFER_START(UnityPerMaterial)
			half4 _MaskTex_ST;
			half4 _NoiseTex_ST;


			half _NoiseTexUSpeed;
			half _NoiseTexVSpeed;
			half _MaskTexUSpeed;
			half _MaskTexVSpeed;

			half _ValueToColor;
            half _PolarCoordMode;
			half _CustomData;
			half _DistortIntensityU;
			half _DistortIntensityV;
            float _ChannelOffsetR;
            float _ChannelOffsetG;
            float _ChannelOffsetB;
			CBUFFER_END

			sampler2D _MaskTex;
			sampler2D _NoiseTex;
            sampler2D _BackgroundTexture;

            //直角坐标系 转 极坐标
			float2 Polar(float2 uv_in)
			{
			    //0~1的1象限转-0.5~0.5的四象限
				float2 uv = uv_in-0.5;
				//d为各个象限坐标到0点距离,数值为0~0.5
				float distance=length(uv);
				//0~0.5放大到0~1
				distance *=2;
				//4象限坐标求弧度范围是 [-pi,+pi]
				float angle=atan2(uv.x,uv.y);
				//把 [-pi,+pi]转换为0~1
				float angle01=angle/3.14159/2+0.5;
				//输出角度与距离
				return float2(angle01,distance);
			}

            struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				half4 color : COLOR;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float4 color : COLOR;
				float4 uv : TEXCOORD3;
				float4 customData1 : TEXCOORD4;
				float4 normalWS : TEXCOORD5;
				float4 grabPos : TEXCOORD6;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			

            VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.normalWS.xyz = normalize(mul(float4(v.normal,0.0),unity_WorldToObject).xyz);
				o.normalWS.w = 0;						
				o.color = v.color;
				o.uv.xy = v.texcoord.xy;
				o.uv.zw = 0;
				o.customData1 = v.texcoord1;
				float3 positionWS = mul(UNITY_MATRIX_M,float4(v.vertex.xyz, 1.0));
				float4 positionCS = mul(UNITY_MATRIX_VP,float4(positionWS, 1.0));
				o.grabPos = ComputeGrabScreenPos(positionCS);
				o.worldPos = positionWS;
				o.clipPos = positionCS;

				return o;
			}

			
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}


			half4 frag ( VertexOutput IN ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float3 WorldPosition = IN.worldPos;
				float2 uv = IN.uv.xy;
				half3 normalWS = IN.normalWS.xyz;
				half4 customData1 = IN.customData1;
				float4 color_vertex = IN.color;

				
				half2 uv_NoiseTex = lerp(uv, Polar(uv), _PolarCoordMode) * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
				half2 uv_NoiseTex_Speed = half2(_NoiseTexUSpeed,_NoiseTexVSpeed);
				half2 uv_NoiseTex_offset = uv_NoiseTex  - frac( uv_NoiseTex_Speed * _Time.y * 0.25);
				
				half4 noiseTex = tex2D(_NoiseTex, uv_NoiseTex_offset);
				half2 uv_MaskTex = uv * _MaskTex_ST.xy + _MaskTex_ST.zw;
				uv_MaskTex = _CustomData ? uv_MaskTex + customData1.xy : uv_MaskTex;
				half2 uv_MaskTex_Speed = half2(_MaskTexUSpeed,_MaskTexVSpeed);
				half2 uv_MaskTex_offset = uv_MaskTex  - frac(uv_MaskTex_Speed * _Time.y *0.25);
				half4 maskTex = tex2D(_MaskTex,uv_MaskTex_offset);


				


				half finalAlpha = maskTex.r * noiseTex.r;
				half2 distortIntensity = half2(_DistortIntensityU,_DistortIntensityV);
				half2 distortValue = _CustomData ? distortIntensity * customData1.z : distortIntensity;
				distortValue = distortValue * 0.02 *finalAlpha;
				float4 uv_BackgroundTexture = float4( IN.grabPos.x + distortValue.x,  IN.grabPos.y + distortValue.y, IN.grabPos.z,IN.grabPos.w);
                half scenceColorR = tex2Dproj(_BackgroundTexture, uv_BackgroundTexture + _ChannelOffsetR * 0.01).r;
                half scenceColorG = tex2Dproj(_BackgroundTexture, uv_BackgroundTexture + _ChannelOffsetG * 0.01).g;
                half scenceColorB = tex2Dproj(_BackgroundTexture, uv_BackgroundTexture + _ChannelOffsetB * 0.01).b;
				half3 scenceColor = half3(scenceColorR,scenceColorG,scenceColorB);

				half3 finalColor = _ValueToColor ? finalAlpha : scenceColor;

				return half4( finalColor, finalAlpha * color_vertex.a);
			}

            
            ENDCG
        }

    }
}