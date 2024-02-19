Shader "Dite/Util/YFJsonAnim"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // _FrameRect("Frame Rect",vector) = (1,1,0,0)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            // "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 Color : Color;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 Color : Color;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID // use this to access instanced properties in the fragment shader.
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;


            #ifdef UNITY_INSTANCING_ENABLED

                UNITY_INSTANCING_BUFFER_START(PerDrawYFAnim) 
                    UNITY_DEFINE_INSTANCED_PROP(float4x4, DT_FrameRect)                     
                    // UNITY_DEFINE_INSTANCED_PROP(fixed4, DT_FrameOffset) 
                UNITY_INSTANCING_BUFFER_END(PerDrawYFAnim)

                #define _FrameRect      UNITY_ACCESS_INSTANCED_PROP(PerDrawYFAnim, DT_FrameRect)
                // #define _FrameOffset      UNITY_ACCESS_INSTANCED_PROP(PerDrawYFAnim, DT_FrameOffset)
            #else
                float4x4 DT_FrameRect;
                // fixed4 DT_FrameOffset;

                #define _FrameRect   DT_FrameRect
                // #define _FrameOffset  DT_FrameOffset
            #endif // instancing
 

            // inline fixed IsInRect(float2 uv,float4 rect)
			// {
			// 	float2 pPos = step(_FrameRect.xy, uv) * step(uv, _FrameRect.zw );
			// 	return pPos.x * pPos.y;
			// }

            // void getdata(float4 v0,float2 v1,inout float4 rect,inout float4 offset){
            //     // rect.x = v0.x * _MainTex_TexelSize.x;
            //     // rect.y = v0.y * _MainTex_TexelSize.y;
            // }
            // this.FrameRect.x = x / texsize.x;
            // this.FrameRect.y = (texsize.y - h - y) / texsize.y; 
            // this.FrameRect.z = w / texsize.x ;
            // this.FrameRect.w = h / texsize.y ;
            float4x4 aaaa;

            v2f vert (appdata v)
            {
                v2f o;                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
 
                float3x3 af = float3x3(_FrameRect[0].xyz,_FrameRect[1].xyz,0,0,1);
                float3x3 af2 = float3x3(_FrameRect[2].xyz,_FrameRect[3].xyz,0,0,1);
                 

                float4 pos = v.vertex;
                pos.xy = pos.xy * v.uv * 2.0;// * _FrameOffset.zw + _FrameOffset.xy;
                pos.xy = mul( af2, float3(pos.xy,1)); 
                o.vertex = UnityObjectToClipPos(pos); 
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) ;// * _FrameRect.zw + _FrameRect.xy;
                o.uv = mul( af, float3(o.uv,1));
                // o.uv +=  _FrameRect.xy;
                o.Color = v.Color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i); 
                float2 xxx =  0.5 / (_MainTex_TexelSize.zw * 0.01);
                float2 uv = i.uv ;

                
                fixed4 col = tex2D(_MainTex, uv ) * i.Color ; 
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
