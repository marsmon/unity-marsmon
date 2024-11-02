Shader "YoFi/UI/ScreenMask_RoundRect"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    
        [Space(15)]
        _MainAddST("Main Add ST",vector) = (1,1,0,0)
        [Space(15)]
        [Header(Rect Mask)]
        _Rect("Rect",vector) = (50,50,100,100)
        _Value("Value",range(0,1)) = 1
        _Radius("Radius",range(0,1)) = 0.1
        _Fade("Fade Pixel",range(0,200)) = 50



    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull[_Cull]
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]
        


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

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
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                float4 sceneUV : TEXCOORD3;
                // float4 myUV : TEXCOORD4;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            float4 _MainAddST;
            float _UseSceneUV;
            half _Value;
            half _Radius;
            half _Fade;
            float4 _Rect;




            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));



                // _Rect = lerp(float4(0,0,_ScreenParams.x,_ScreenParams.y),_Rect,_Value);
                // float4 myclampedRect = clamp(_Rect, -2e10, 2e10);
                
                
                OUT.sceneUV = ComputeScreenPos (OUT.vertex);
                // float2 scenePixel = OUT.sceneUV * _ScreenParams.xy;
                // OUT.myUV = float4(scenePixel * 2 - myclampedRect.xy - myclampedRect.zw , 0,0);

                
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 main_uv =  IN.texcoord * _MainAddST.xy + _MainAddST.zw;
                
                half4 color = IN.color * (tex2D(_MainTex, main_uv) + _TextureSampleAdd);

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif


                _Rect = clamp(_Rect, -2e10, 2e10);
                float targetWidth = _Rect.z - _Rect.x;
                float targetHeight = _Rect.w - _Rect.y;
                float2 offsetStart = saturate(_Radius) * float2(targetWidth,targetHeight) + _Fade;

                float2 scenePixel = IN.sceneUV * _ScreenParams.xy;
                float2 myUV = scenePixel*2 -  _Rect.xy - _Rect.zw;

        
                //这里要和顶点部分算法一致
                _Rect = lerp(float4(0-offsetStart.x,0-offsetStart.y,_ScreenParams.x+offsetStart.x,_ScreenParams.y+offsetStart.y),_Rect,_Value);


                
                float width = _Rect.z - _Rect.x;
                float height = _Rect.w - _Rect.y;
                float max_Radius = min(width,height);

                //从0到1映射到半径比例
                float radius =  saturate(_Radius) * max_Radius ;//圆角半径
                float fade = clamp(_Fade,0.01,max_Radius * 0.5);//羽化像素
                float2 stepMin = float2(fade.xx);
                
                half2 rectMask = smoothstep(float2( width , height ) - stepMin,float2( width , height ), abs(myUV.xy));
                float rectvalue = max( rectMask.x , rectMask.y);
                
                

                // 右上角圆心坐标
                float2 TRC = myUV.xy - float2(width,height) + float2(radius,radius);
                //右下角圆心坐标
                float2 BRC = myUV.xy - float2(width ,-height) + float2(radius,-radius);
                //左下角圆心坐标
                float2 BLC = myUV.xy + float2(width,height)  + float2(-radius,-radius);
                //左上角圆心坐标
                float2 TLC = myUV.xy + float2(width,-height) + float2(-radius,radius);
                
                
                
                float dist;
                float sphere_TRC = 0;
                // 判定当前UV是否在圆外侧
                UNITY_FLATTEN
                if (TRC.x > IN.sceneUV.x && TRC.y > IN.sceneUV.y)
                {
                    // 计算当前UV点与圆心距离
                    dist = distance(TRC,IN.sceneUV);
                    sphere_TRC = smoothstep(radius- stepMin,radius,dist);
                }
                
                float sphere_BRC = 0;
                UNITY_FLATTEN
                if (BRC.x > IN.sceneUV.x && BRC.y < IN.sceneUV.y)
                {
                    dist = distance(BRC,IN.sceneUV);
                    sphere_BRC = smoothstep(radius- stepMin,radius,dist);
                }
                
                float sphere_BLC = 0;
                UNITY_FLATTEN
                if (BLC.x < IN.sceneUV.x && BLC.y < IN.sceneUV.y)
                {
                    dist = distance(BLC,IN.sceneUV);
                    sphere_BLC = smoothstep(radius- stepMin,radius,dist);
                }
                float sphere_TLC = 0;
                UNITY_FLATTEN
                if (TLC.x < IN.sceneUV.x && TLC.y > IN.sceneUV.y)
                {
                    dist = distance(TLC,IN.sceneUV);
                    sphere_TLC = smoothstep(radius- stepMin,radius,dist);
                }
                

                float allSphere = max(max(max(sphere_TRC , sphere_BRC),sphere_BLC),sphere_TLC);

                float finalMask = max( rectvalue , allSphere);
                

                color.a *= finalMask ;
                

                color.rgb *= color.a;
                
                return color;
            }
            ENDCG
            
        }
    }
}
