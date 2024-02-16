//2021.07.10 简单实现. 马卡龙
Shader "YoFi/GGS_Char_SimpleBase"
{
    Properties
    {
        // _v("a",vector) = (0,0,0,0)

        _MainTex ("BaseTex", 2D) = "white" {}
        _ILMTex("ILMTex",2D) = "white"{}
        _SssTex("SssTex",2D) = "white"{}
        _DetailTex("DetailTex",2D) = "white"{}
 _aaaColor ("aaa color",Color) = (1,1,1,1)  
        _EmissionTex ("EmissionTex",2D) = "black"{}

        [Header (Camera offset)]
        _viewAngle("Cam Angle",float) = 0
        [Header (Outline)]
        _Outline ("Outline width",float) = 1
        _OutlineColor ("Outline color",Color) = (0,0,0,1)  
        [Header (Bloom)]
        [Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 0
        [Toggle]_CBIsSrcTex("是否使用源色",float) = 1
        _CBLocalTex("CB Tex",2D) = "black"{}
        _CBLocalThreshhold("CB Threshhold",range(0.0,1.0)) = 1
        _CBLocalColor("CB Color",Color) = (1,1,1,1)
        _CBLocalLight("CB 亮度",float) = 1   

        
        [Enum(Off,0,ADD,1.0,BLEND,2,OnlyFNL,3)]_CBFNLblend("CB 菲尼尔 混合模式",float) = 0
        _CBFNLColor("CB 菲尼尔颜色",Color) = (1,1,1,1)
        _CBFNLvalueI("CB 菲尼尔阈值 I",Range(-1.0,1.0)) = 1.0
        _CBFNLvalueII("CB 菲尼尔阈值 II",Range(-1.0,1.0)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGINCLUDE
            #define YF_POST_BLOOM
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            { 
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                half3 normal : NORMAL;
                half4 color : COLOR;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0; 
                half4 vertexColor : TEXCOORD1;
                half3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;

                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex,_ILMTex,_SssTex,_DetailTex,_EmissionTex;
            float4 _MainTex_ST,_OutlineColor; 
            float _viewAngle,_Outline;
float4 _aaaColor ;


            sampler2D _CBLocalTex;// rgb colMask,a fnlmask
            float _CBLocalThreshhold;
            float4 _CBLocalColor;
            float _CBLocalLight;
            fixed _CBGlobalBool;
            fixed _CBLocalBanBool;

            fixed _CBFNLblend,_CBFNLvalueI,_CBFNLvalueII,_CBIsSrcTex;
            fixed4 _CBFNLColor;


            float4x4 getviewAngle4x4(float a){
                float r = -radians(a % 360.0);
                float cv = cos(r);
                float sv = sin(r);
                //矩阵其实可以先算好，除非摄像机会随时变化高度
                float4x4 mat = float4x4(1.0, 0.0, 0.0, 0.0,
                    0.0,  cv,  sv, 0.0,
                    0.0, -sv,  cv, 0.0,
                    0.0, 0.0, 0.0, 1.0
                );
                return mat;
            }

            float4 ShadowProjectPos(float4 vertPos,float4 _Dir,float4 localsrcv)
            { 
                // _Dir.xyz 是方向, _Dir.w = 阈值
                // float4 shadowPos = ;

                //得到顶点的世界空间坐标
                float4 worldPos = mul(unity_ObjectToWorld , vertPos);
                // float4 shadowPos = worldPos;
                //灯光方向
                float3 Dir = normalize(_Dir.xyz);

                //阴影的世界空间坐标（低于地面的部分不做改变）
                worldPos.z = unity_ObjectToWorld[2].w + localsrcv.z  ;//min(worldPos.z , unity_ObjectToWorld[2].w);
                worldPos.xy = worldPos.xy - Dir.xy * max(0 , worldPos.z - _Dir.w) / Dir.z; 

                // //阴影的世界空间坐标（低于地面的部分不做改变）
                // shadowPos.y = min(worldPos .y , _Dir.w);
                // shadowPos.xz = worldPos .xz - Dir.xz * max(0 , worldPos .y - _Dir.w) / Dir.y; 

                return worldPos;
            } 


            inline float dt_FNRwork(float nv,float exp0,float exp1){ 
                return smoothstep(exp0,exp1,nv);   
            }

            float4 BloomOutfrag(float4 _col,fixed4 mask,float nv){
                fixed4 col = saturate(_col);
               
                #ifndef YF_POST_BLOOM
                    return col;
                #else 
                    if(_CBLocalBanBool == 1){
                        return col;
                    }
                    if(_CBGlobalBool == 1){ 
                        fixed4 c = 0;
                        c.a = col.a;
                        float cc = dot(col.rgb, float3(0.2126729f,  0.7151522f, 0.0721750f)) ;
                        fixed bcol = smoothstep(1.0 - max(0,_CBLocalThreshhold), 1.0 , cc ) * col.a ;
 
                        fixed3 nc = lerp(0.0,col.rgb * bcol,_CBIsSrcTex) * _CBLocalColor.rgb;
                        c.rgb += nc;
 
                        c.rgb += mask.rgb;

                        if(_CBFNLblend > 0)
                        {
                            float ma = mask.a == 0 ? 1 : mask.a;
                            float fnlv = dt_FNRwork(nv,_CBFNLvalueI,_CBFNLvalueII) * ma;
                            if(_CBFNLblend == 1){//add
                                c.rgb += fnlv * _CBFNLColor.rgb;
                            }
                            else if(_CBFNLblend == 2){ //blend
                                c.rgb = lerp(col.rgb , _CBFNLColor.rgb, fnlv);
                            }
                            else if(_CBFNLblend == 3){ //Only FNL
                            // col.rgb = fnlv;
                                return  float4(fnlv * _CBFNLColor.rgb,col.a);
                            }
                        }
                        
                        return col + c * _CBLocalLight;
                    }
                    else{
                        return col;
                    }
                #endif
            }

            float4 GetNewWorldPos(float4 v,float a){
                // //模型顶点坐标转世界空间坐标
				// float4 worldPos = mul(unity_ObjectToWorld, i.pos);
				// //世界空间顶点坐标转观察空间坐标
				// float4 viewPos = mul(UNITY_MATRIX_V, worldPos);
				// //观察空间坐标转裁剪空间坐标
				// float4 clipPos = mul(UNITY_MATRIX_P, viewPos);
                if(_viewAngle == 0){
                    return mul(unity_ObjectToWorld,v);
                }
                else{ 
                    float4x4 mat = getviewAngle4x4(a);
                    float4 pos = mul(mat, mul(unity_ObjectToWorld,float4(0,0,0,1)));
                    float4x4 mat2 = float4x4(1.0, 0.0, 0.0, 0.0,
                        0.0,  1.0,  0.0,   -pos.y + unity_ObjectToWorld[1][3],//pos.y < 0 ? -pos.y : 0,
                        0.0, 0.0,  1.0, -pos.z + unity_ObjectToWorld[2][3],
                        0.0, 0.0, 0.0, 1.0
                    );
                    float4 pos0 = mul(mat, mul(unity_ObjectToWorld,v));
                    pos0 = mul(mat2,pos0);
                    // half3 Dir = normalize(UnityWorldSpaceLightDir(pos0));
                    // // float3 Dir = normalize(worldLightDir);

                    // pos0.z = unity_ObjectToWorld[2].w + localsrcv.z  ;//min(worldPos.z , unity_ObjectToWorld[2].w);
                    // pos0.xy = pos0.xy - Dir.xy * max(0 , pos0.z - _Dir.w) / Dir.z; 

                    return pos0;
                }
            }
 
            v2f vertKWL (appdata v)
            { 
                v2f o; 
                float4 worldPos = mul(unity_ObjectToWorld , v.vertex);
                worldPos = GetNewWorldPos(v.vertex,_viewAngle);
                // if(_v.w > 0){
                //     half3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos)); 
                //     worldPos = ShadowProjectPos(mul(unity_WorldToObject,worldPos) ,float4(worldLightDir,0),v.vertex);
                // }
                o.vertex = UnityWorldToClipPos(worldPos);
                
                o.worldPos = worldPos.xyz; 
                // o.vertex = mul(UNITY_MATRIX_VP, worldPos);

                //重新计算深度值 
                float4 verticalClipPos = UnityObjectToClipPos(v.vertex);
                o.vertex.z = verticalClipPos.z / verticalClipPos.w * o.vertex.w ;
                 
                o.uv.xy = TRANSFORM_TEX(v.uv0, _MainTex);
                o.uv.zw = v.uv1;
                o.vertexColor = v.color;                
                o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
                return o;
            }

            fixed4 fragKWL (v2f i) : SV_Target
            {
                fixed4 ilm = tex2D(_ILMTex,i.uv.xy);
                fixed4 cbcolor = tex2D(_CBLocalTex,i.uv.xy);
                fixed4 emission = tex2D(_EmissionTex,i.uv.xy);
                fixed4 baseColor = tex2D(_MainTex, i.uv.xy);
                fixed3 sssColor = tex2D(_SssTex,i.uv.xy).rgb;
                // fixed3 detail = tex2D(_DetailTex,i.uv.zw);
                fixed3 detail = tex2D(_DetailTex,i.uv.xy);
detail = lerp(_aaaColor * detail ,detail,detail); 

                half ao = saturate((i.vertexColor.r - 0.7) * 50);
                half3 worldNormal = normalize(i.worldNormal);
                half3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                half3 halfDir = normalize(worldLightDir + worldViewDir);
                half NdotH = saturate(dot(halfDir,worldNormal));
                half NdotL = dot(worldLightDir,worldNormal);
                half sssFactor = saturate((NdotL * 0.5 + 0.5 - i.vertexColor.b * 0.5) * 50) * ao;
                fixed4 finalColor = 1;
                finalColor.rgb = lerp(sssColor,baseColor,sssFactor) * detail * ilm.a;
                finalColor.rgb = saturate(finalColor.rgb + emission.rgb);


                return BloomOutfrag(finalColor,cbcolor,dot(worldViewDir,worldNormal));
            }
            v2f vertOuline (appdata v)
            {
                v2f o;
                float4 worldPos = GetNewWorldPos(v.vertex,_viewAngle);
                // //顶点和法线转到观察空间
                o.vertex = mul(UNITY_MATRIX_V,worldPos);
                
	            // float3 normal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, v.normal));
                float3 normal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                //对z分量进行处理，防止内凹的模型扩张顶点后出现背面面片遮挡正面面片的情况
                normal.z = -0.8;
                //在观察空间下把模型顶点沿着法线方向向外扩张一段距离
                o.vertex = o.vertex + float4(normal, 0) * _Outline * 0.1;  
                //转换到裁剪空间
                o.vertex = mul(UNITY_MATRIX_P, o.vertex);
                //重新计算深度值 
                float4 verticalClipPos = UnityObjectToClipPos(v.vertex);
                o.vertex.z = verticalClipPos.z / verticalClipPos.w * o.vertex.w ;

                
                return o;
            }

            fixed4 fragOuline (v2f i) : SV_Target
            {
                //背面都使用轮廓线的颜色渲染
                return float4(_OutlineColor.rgb, 1);
            }
        ENDCG

        // 第一个Pass使用轮廓线颜色渲染整个背面的面片
        Pass
        {
            //定义pass名称后，可以在其他shader里引用
            NAME "Outline"
            offset 1,10
            //剔除正面，只渲染背面
            Cull Front 
            CGPROGRAM
            #pragma vertex vertOuline
            //vertPlaneShadow
            //vertOuline
            #pragma fragment fragOuline 
            ENDCG
        } 

        Pass
        {
            Tags{"LightMode"="ForwardBase"}
            // Cull off 
            CGPROGRAM
            #pragma vertex vertKWL
            #pragma fragment fragKWL
            #pragma multi_compile_fwdbase 
            ENDCG
        }
    }
}
