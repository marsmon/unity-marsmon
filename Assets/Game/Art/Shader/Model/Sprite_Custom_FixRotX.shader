// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// GUID: d5672eed292954c4cb2ce932856324eb
// fixed CAMERA_ROATION_X;
// IN.vertex.y *= 1.0 / cos(3.141596535898 / 180.0 * CAMERA_ROATION_X);
// 2022 08 30 11 57
// Add shader_feature _RENDMODE_ORTHOGRAPHIC _RENDMODE_FOOT
// _RENDMODE_ORTHOGRAPHIC 原始的内容，对应正交相机做Y轴缩放
// _RENDMODE_FOOT 脚印使用的 ,脚印的图片 wrap Mode:repeat
// _speed 脚印消失的速度
// _custom_time 从c#传入的当前时间 必须使用 Time.timeSinceLevelLoad,
//        这个熟悉与_Time.y在运行时记录的是一个数值。以这个数值来让shader的时间线是从0开始的。

Shader "YoFi/Sprite/Custom/FixRotX"
{
    Properties
    {
        [HideInInspector][PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector][PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [HideInInspector][PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        // [KeywordEnum(ORTHOGRAPHIC, FOOT)] _RENDMODE ("mode",float) = 0
        _speed("消失的速度",float) = 0
        _custom_time("当前_Time 时间(需传入)",float) = 0

        [Header(Custom Bloom)]        
        [Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 1
        _CBLocalThreshhold("CB Threshhold",range(0.0,1.0)) = 1
        _CBLocalColor("CB Color",Color) = (1,1,1,1)
        _CBLocalLight("CB 亮度",float) = 1

        
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment Customfrag//SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            // #pragma shader_feature _RENDMODE_ORTHOGRAPHIC _RENDMODE_FOOT
            #define _RENDMODE_FOOT
            // #include "UnitySprites.cginc"
            #include "UnityCG.cginc"
            #include "..\Common\Included\YFEffectCGForYFPost.cginc"
            
            #ifdef UNITY_INSTANCING_ENABLED

                UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                // SpriteRenderer.Color while Non-Batched/Instanced.
                UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                // this could be smaller but that's how bit each entry is regardless of type
                UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
                UNITY_INSTANCING_BUFFER_END(PerDrawSprite)

                #define _RendererColor  UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
                #define _Flip           UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)

            #endif

            CBUFFER_START(UnityPerDrawSprite)
                #ifndef UNITY_INSTANCING_ENABLED
                    fixed4 _RendererColor;
                    fixed2 _Flip;
                #endif
                float _EnableExternalAlpha;
            CBUFFER_END


            #ifdef _RENDMODE_FOOT
                float _custom_time;
                float _speed;
            #else 
                float CAMERA_ROATION_X;
            #endif
            // Material Color.
            fixed4 _Color;
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float3 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }
            

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                #ifndef _RENDMODE_FOOT
                    IN.vertex.y *= CAMERA_ROATION_X; // 35度  1.22077458876146;
                #endif
                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                
                OUT.texcoord.xy = IN.texcoord;
                #ifdef _RENDMODE_FOOT
                    if(_custom_time > 0){
                        OUT.texcoord.z =  (max(0,_Time.y - _custom_time) * -_speed );
                        }else{
                        OUT.texcoord.z = 0;
                    }
                #else
                    OUT.texcoord.z = 0;
                #endif
                OUT.color = IN.color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA
                    fixed4 alpha = tex2D (_AlphaTex, uv);
                    color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
                #endif
                return color;
            }

            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord.xy) * IN.color;
                #ifdef _RENDMODE_FOOT
                    c.a *= saturate(floor(IN.texcoord.y) + IN.texcoord.z);
                #endif
                c.rgb *= c.a;
                // c.rgb = saturate(floor(IN.texcoord.y) + IN.texcoord.z);
                // c.a = 1;
                return c;
            }
            fixed4 Customfrag (v2f i) : SV_Target
            {
                return BloomOutfrag(SpriteFrag(i));
            }
            ENDCG
        }
    }
}
