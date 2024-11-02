
Shader "YoFi/GGS_Char_MeshShadow"
{
    Properties
    {
        
        [Header (Shadow)]
        _ShadowAngle ("Shadow Angle",Range(0,1)) = 0.38
        _ShadowLength("Shadow Length",Range(0,2)) = 1
        _ShadowColor("Shadow Color",Color) = (0,0,0,0.5)
        //_ShadowFade("Shadow Fade",Range(1,20)) = 10
        _FadeOffset("Fade Offset",Range(0,5)) = 0.6
    }
    SubShader
    {
        Tags{"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        //专门写一个pass，把模型的顶点拍扁
        Pass
        {
			Name "MeshShadow"
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Stencil
            {
                Ref 1
                Comp NotEqual
                Pass Replace
            }
            CGPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow

            #include "UnityCG.cginc"
            float _ShadowAngle;
            float _ShadowLength;
			float4 _ShadowColor;
            //float _ShadowFade;
            float _FadeOffset;
            
            struct appdataShadow
            {
                float4 vertex : POSITION;
            };

            struct v2fShadow
            {
                float4 positionCHS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
                float3 originWS : TEXCOORD1;
            };


            v2fShadow vertShadow (appdataShadow v)
            {
                v2fShadow o;
                float4 worldPos = mul(unity_ObjectToWorld,v.vertex);
                float4 originWS = mul(unity_ObjectToWorld,float4(0,0,0,1));
                float angle =  _ShadowAngle * UNITY_TWO_PI;
                float x1 = cos(angle);
                float y1 = sin(angle);
                float offsetValue = worldPos.y * _ShadowLength;
                worldPos.xz += float2(x1,y1) * offsetValue;
                worldPos.y = 0.001;
                o.originWS = originWS;
                o.positionWS = worldPos;
                o.positionCHS = UnityWorldToClipPos(worldPos);
                return o;
            }

            half4 fragShadow (v2fShadow i) : SV_Target
            {
            	
				half4 col = _ShadowColor;
                float length = distance(i.positionWS,i.originWS);

                float fade = length / _ShadowLength - _FadeOffset;
                fade = 1 - saturate(fade);
				col.a *= fade;
                //col = float4(fade,fade,fade,1);
                return col;
            }
            ENDCG
        }
   }
}
