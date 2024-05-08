Shader "YoFi/Effect/Dissolution_WithParticle"
{
    Properties
    {
        // []
        [HideInInspector]_mode("__Mode",float)= 0 
        [HideInInspector][Enum(UnityEngine.Rendering.CullMode)]_Cull("__cull", Float) = 0 //[Enum(UnityEngine.Rendering.CullMode)]
        [HideInInspector][Enum(Off,0.0,On,1.0)]_ZWrite("__zw", Float) = 0.0//[Enum(Off,0.0,On,1.0)]
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("__src", Float) = 5.0 //[Enum(UnityEngine.Rendering.BlendMode)]
        [HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("__dst", Float) = 10.0//[Enum(UnityEngine.Rendering.BlendMode)]
        [HideInInspector]_ColorMask("__colormask", Float) = 14
        
      
        [HideInInspector][Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 0
        [HideInInspector]_CBLocalThreshhold("CB Threshhold",range(0.0,1.0)) = 1
        [HideInInspector]_CBLocalColor("CB Color",Color) = (1,1,1,1)
        [HideInInspector]_CBLocalLight("CB 亮度",float) = 1
        
        [Header(Main Texture)]
        [MainColor][HDR]_MainColor("Tint Color", Color) = (0.75,0.75,0.75,1.0)
        _Brightness("Main Tex 亮度",float) = 1
        [MainTexture]_MainTex("Main Tex(RGBA)", 2D) = "white"{}
        
        [Header(Other Texture)]
        [Toggle]_isParticleMode("是否是粒子模式",float) = 0
        _Effect0Tex("溶解 Tex(R)", 2D) = "white"{}
        _Effect1Tex("Mask Tex(RGB/A)", 2D) = "white"{}

        [Header(Mask)]
        [Toggle]_IsMFlip("Is Flip",float) = 0
        [Enum(Alpha,0.0,Gray,1.0)]_IsGray("Is Gray",float) = 0
        
        [Header(Dissolution)]
        [Toggle]_IsDClip("Is Clip",float) = 0
        [Toggle]_IsDFlip("Is Flip",float) = 0
        [Enum(Value,0,Particle.a,1,Particle.custom2.z,2)] _valueMode("属性模式:",float) = 0
        _DissValue("Value",Range(0.0,1.0)) = 1
        [HDR]_LineColor("描边颜色",Color) = (0.0,0.0,0.0,1.0)
        _LineWidth("边宽度",Range(0.0,1.0)) = 0

        [Header(Other)]
        [Toggle]_IsClipGround("是否有地面[Y = 0]",float) = 0
        _IsClipGroundBlur("地面接缝过度",float) = 0.5 
        

    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType" = "Transparent"  } //"IgnoreProjector" = "True" "PerformanceChecks"="False"
        Lighting Off
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        Cull [_Cull]
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM                
            #pragma shader_feature __ _ALPHATEST_ON
            // #pragma multi_compile_fog
            // 可以关闭 FOG 不使用时可以注销掉，减少变体 FOG_LINEAR FOG_EXP FOG_EXP2
            #pragma multi_compile_instancing
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
             
            #include "../Common/Included/YFEffectCGFun.cginc"
            
            sampler2D _MainTex,_Effect0Tex,_Effect1Tex;
            DTFloat4 _MainColor,_MainTex_ST,_Effect0Tex_ST,_Effect1Tex_ST,_LineColor;
            DTFloat _Brightness,_IsMFlip,_IsGray,_IsDClip,_IsDFlip,_DissValue,_LineWidth,_valueMode,_isParticleMode;
            

            struct appdata_t {

                DTFloat4 vertex : POSITION;
                DTFloat4 color : COLOR;
                DTFloat4 texcoord : TEXCOORD0;
                DTFloat4 custom1 : TEXCOORD1;
                DTFloat4 custom2 : TEXCOORD2;
                DTFloat3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                DTFloat4 vertex : SV_POSITION;
                DTFloat4 color : COLOR;
                DTFloat4 uv : TEXCOORD0;                
                DTFloat4 uv1texcoord : TEXCOORD1;
                DTFloat4 worldPos : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);                
                o.color = v.color * _MainColor;

                o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex) + v.texcoord.zw * _isParticleMode;
                o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_Effect0Tex) + v.custom1.xy * _isParticleMode;
                o.uv1texcoord.xy = TRANSFORM_TEX(v.texcoord.xy,_Effect1Tex) + v.custom1.zw * _isParticleMode;
                o.uv1texcoord.zw = v.custom2.xy;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            // dt_Polar

            fixed4 frag (v2f i) : SV_Target
            { 
                float v = 1;
                
                fixed4 col0 = tex2D(_MainTex, i.uv.xy);
                fixed dissc = tex2D(_Effect0Tex, i.uv.zw).r;
                fixed4 maskc = tex2D(_Effect1Tex, i.uv1texcoord.xy);

                float mask = lerp(maskc.a, dt_getGray(maskc), _IsGray);                
                dissc = dt_Flip(dissc,_IsDFlip);
                mask = dt_Flip(mask,_IsMFlip);

                DTFloat dissv = _DissValue;
                if(_valueMode == 2){
                    dissv = i.uv1texcoord.z;
                }
                else if(_valueMode == 1){
                    dissv = i.color.a;
                }
                else{
                    dissv = _DissValue;
                }
                
                fixed4 col = 2.0f * i.color * col0 * _Brightness;
                float2 Dissv = dt_DissolveBase(dissc,dissv,_LineWidth,_IsDClip) * mask;

                col.rgb = lerp(col.rgb,_LineColor.rgb  , Dissv.x - Dissv.y );
                col.a = lerp(_LineColor.a * col0.a,col.a,Dissv.y ) * Dissv.x * dt_Bool0(v);
                col = saturate(col);

                col *= dt_ClipInWorldY(i.worldPos.y);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return BloomOutfrag(col);
            }
            ENDCG            
        }
    }
    CustomEditor "DiteScripts.YF.EditoShaderGUI.Div.ShaderGUIBase_MaskDiss"
    FallBack off   
}