
// YoFiUnityCG  ..\Common\Included\YFEffectCGFunVF.cginc
#ifndef YF_EFFECT_CGFUNVF_DITE_INCLUDED
#define YF_EFFECT_CGFUNVF_DITE_INCLUDED 
    #include "UnityCG.cginc"
	#include "YFProjectTeamSettings.cginc"
	// YoFiUnityCG
	//***********************************************************************************
	// 特效中各种矩阵相关的方法
	// == common YF_EFFECT_CGCOMMON_DITE_INCLUDED
	// == util YF_UTIL_DITE_INCLUDED
	// == UV Dist YOFI_UVMatrixFUN_DITE_INCLUDED
	// == MainTex YOFI_MAINFUN_DITE_INCLUDED
	// == Fnl YF_FNLFUN_DITE_INCLUDED
	// == ColorMask YF_COLORMASKFUN_DITE_INCLUDED
	// == Diss YF_DISSFUN_DITE_INCLUDED
	// == YFPOST_Bloom YF_YFPOST_DITE_INCLUDED
	// == other YF_OTHER_DITE_INCLUDED
	//***********************************************************************************
 	/* 
	//=======================BASE Properties=======================
	_mode("__Mode",int)= 516
	_Shadermode("__ShaderMode",int)= 5
	[Enum(UnityEngine.Rendering.CullMode)]_Cull("__cull", Float) = 0
	[Enum(Off,0.0,On,1.0)]_ZWrite("__zw", Float) = 0.0
	[Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("__src", Float) = 5.0
	[Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("__dst", Float) = 10.0
	_ColorMask("__colormask", Float) = 14 
	_ClipValue("Cull off",float) = 0.5        
	//=======================Bloom Properties=======================
	_CBLocalThreshhold("CB Threshhold",range(0.0,1.0)) = 1
	_CBLocalColor("CB Color",Color) = (1,1,1,1)
	_CBLocalLight("CB 亮度",float) = 1
	[Toggle]_CBLocalBanBool("单独关闭Bloom",float) = 0
	//=======================Main Properties========================
	[Header(Main)]
	[MainTexture]_MainTex("Main Tex(RGBA)", 2D) = "white"{}
	[MainColor][Gamma]_TintColor("Tint Color", Color) = (1.0,1.0,1.0,1.0)
	_Brightness("Brightness",float) = 1
	_MainValue1("Value 1",vector) = (0,0,0,0) 
	_MainbitSw("Main Mode",int)= 2448
	//=======================Dist Properties=======================
	[Header(Dist)]
	_DistTex("Dist Tex(RGBA)", 2D) = "black"{}
	_DistValue1("Dist Value 1",vector) = (0,0,0,0)        
	_DistValue2("Dist Value 2",vector) = (0,0,1,0) 
	_DistbitSw("Dist Mode",int)= 2448        
	//=======================Color Mask Properties=======================
	[Header(Color Mask)]
	_ColorMaskTex("ColorMask Tex(RGBA)", 2D) = "black"{}
	_ColorMaskValue0("ColorMask  Value 0",vector) = (1,1,1,1) 
	_ColorMaskValue1("ColorMask  Value 1",vector) = (0,0,0,0)
	_ColorMaskValue2("ColorMask  Value 2",vector) = (0,0,1,0) 
	_ColorMaskbitSw("ColorMask Mode",int)= 2448
	//=======================FNL Properties=======================
	[Header(FNL)] 
	_FNLValue0("FNL Value 0",vector) = (1,1,1,1)
	_FNLValue1("FNL Value 1",vector) = (0,0,0,0)        
	_FNLValue2("FNL Value 2",vector) = (0.5,0,1,0)  
	_FNLbitSw("FNL Mode",int)= 2448
	//=======================Dissolution Properties=======================
	[Header(Dissolution)]
	_DissTex("Diss Tex(RGBA)", 2D) = "white"{}
	_DissValue0("Diss  Value 0",vector) = (1,1,1,1)
	_DissValue1("Diss  Value 1",vector) = (0,0,0,0)        
	_DissValue2("Diss  Value 2",vector) = (1,0,1,1) 
	_DissbitSw("Diss Mode",int)= 2448 
	_DissbitSw2("Diss Mode2",int)= 9
	//=======================other Properties=======================
	[Header(Other)] 
	_SproutValue("Sprout Value",vector) = (1,0,0,0) 
	_SproutbitSw("Sprout Mode",int)= 2448

	[Toggle]_ClipGroundbitSw("是否有地面[Y = 0]",float) = 0
	_ClipGroundValue("地面接缝过度",float) = 0.5

	// _StencilComp ("Stencil Comparison", Float) = 8
	// _Stencil ("Stencil ID", Float) = 0
	// _StencilOp ("Stencil Operation", Float) = 0
	// _StencilWriteMask ("Stencil Write Mask", Float) = 255
	// _StencilReadMask ("Stencil Read Mask", Float) = 255 
	*/

	// #ifdef ShaderDebugMode
	// 	#define DISTORT_DEBUG
	// 	#define MAIN_DEBUG
	// 	#define DISSOLUTION_DEBUG
	// 	#define COLORMASK_DEBUG
	// 	#define FNL_DEBUG
	// 	#define SPROUT_DEBUG
	// #endif

	// Using standalone files
	// #define USEING_STANDALONE_FILES 
	// #ifdef USEING_STANDALONE_FILES
	// 	#include "./Function/YoFiUtilFun.cginc"
	// 	#include "./Function/YoFiUVMatrixFun.cginc"
	// 	#include "./Function/YoFiBaseTexFun.cginc"
	// 	#include "./Function/YoFiFNLFun.cginc"
	// 	#include "./Function/YoFiDissolutionFun.cginc"
	// 	#include "./Function/YoFiColorMaskFun.cginc"
	// #endif 

	#ifndef USEING_STANDALONE_FILES
	//Util
		#ifndef YF_UTIL_DITE_INCLUDED
		#define YF_UTIL_DITE_INCLUDED
			//***********************************************************************************
			// 特效中各种矩阵相关的方法
			// #include "YFShaderUtilBase.cginc"
			//== 自定义 Float 精度
			//== 优化过的数学算法
			//== 获取摄像机方向
			//== 贴图通道获取
			//== POW
			//== Ramp
			//== 切线空间法线算法
			//== DTFloatDir算法
			//== _ALPHATEST_ON
			//== YF_USECUSTOMTIME
			//== BlendValue
			//***********************************************************************************
			//#include "UnityCG.cginc"


			

			// ===============================================必须出现的面板属性  Properties=========================
			#ifndef ONLYWORK
				#ifdef _ALPHATEST_ON
					uniform DTfloat _ClipValue;
					DTfloat2 ClipData;
					#define YFCustomClip clip(ClipData.x - ClipData.y);
				#else	
					#define YFCustomClip
				#endif



				//扰动属性
				uniform DTfloat4 _DistValue1;
				uniform DTfloat4 _DistValue2;
				uniform uint _DistbitSw;
				//主贴图属性  
				uniform DTfloat4 _MainValue1;
				uniform int _MainbitSw;
				//菲尼尔属性 
				uniform DTfloat4 _FNLValue0;
				uniform DTfloat4 _FNLValue1;
				uniform DTfloat4 _FNLValue2;
				uniform uint _FNLbitSw;
				//Color Mask 属性
				uniform DTfloat4  _ColorMaskValue0;
				uniform DTfloat4  _ColorMaskValue1;
				uniform DTfloat4  _ColorMaskValue2;
				uniform uint _ColorMaskbitSw;
				//Dissolution 属性
				uniform DTfloat4 _DissValue0;
				uniform DTfloat4 _DissValue1;
				uniform DTfloat4 _DissValue2;
				uniform uint _DissbitSw;				
				uniform uint _DissbitSw2;
				//生长 属性
				uniform DTfloat4 _SproutValue;
				uniform uint _SproutbitSw;
				//地面裁切
				uniform DTfloat _ClipGroundValue;
				uniform uint _ClipGroundbitSw;

				#ifdef YF_POST_BLOOM 
					DTfloat _CBLocalThreshhold;
					DTfloat4 _CBLocalColor;
					DTfloat _CBLocalLight;
					fixed _CBGlobalBool;
					fixed _CBLocalBanBool;
				#endif

			#endif	
			// ===============================================Properties========================================================================
			#ifndef ONLYWORK
				#ifdef YF_USECUSTOMTIME
					//Shader.SetGlobalDTFloat("_yfTime",yofitime % 10);
					uniform DTFloat _yfTime; 
					#define YF_SHADER_WORKTIME _yfTime
					#define DTTimeAccuracy 10
					#define DTTimeWork(s) (floor(s * DTTimeAccuracy) / DTTimeAccuracy)
				#else
					#define YF_SHADER_WORKTIME _Time.y
					#define DTTimeWork(s) s
				#endif 

				// 设置时间 可改其他变量来控制时间，关闭是1执行：YF_SHADER_WORKTIME * speed
				#ifdef _TIMEDT_ON 
					#define DT_AUTOTIME(speed) YF_SHADER_WORKTIME * DTTimeWork(speed)
				#else
					#define DT_AUTOTIME(speed) speed
				#endif

				struct YFFXInputData{
					float4x4 value;
					uint mask;
				}; 
				inline uint mergebitswitch(int animbsw,int srcbsw )
				{
					uint bsw = animbsw << 0;
					bsw += srcbsw << 13;
					return bsw;
				} 
				inline void splitbitswitch(int bsw,out int animbsw,out int srcbsw)
				{
					animbsw = bsw >> 0 & 0x1FFF;
					srcbsw = bsw >> 13;
				}
			#endif

			 
			// ===============================================自定义 tex2D========================================================================
			inline fixed4 DTtex2d(sampler2D _Tex,float2 _srcuv)
			{
				fixed4 color = tex2D(_Tex, _srcuv);
				color.rbg *= color.a;
				return saturate(color);
			}
			inline fixed4 DTtex2d(sampler2D _Tex,float2 _srcuv,float b)
			{
				fixed4 color = DTtex2d(_Tex, _srcuv) * b;
				return saturate(color);
			}
			inline fixed4 DTtex2d(sampler2D _Tex,float2 _srcuv,fixed4 _col)
			{
				fixed4 color = DTtex2d(_Tex, _srcuv) * _col;
				return saturate(color);
			}
			inline fixed4 DTtex2d(sampler2D _Tex,float2 _srcuv,float b,fixed4 _col)
			{
				fixed4 color = DTtex2d(_Tex, _srcuv) * _col * b;
				return saturate(color);
			}


			// ===============================================优化过的数学算法========================================================================

			#if DT_EDITORMODE
				#define DT_Sin(v) sin(v)
				#define DT_Cos(v) cos(v)
				#define DT_SinV(v) sin(v)
				#define DT_CosV(v) cos(v)
			#else
				#define DT_Sin(v) (tex2D(PowDataDite,DTFloat2(v, 0.25)).g * 2.0 + -1.0)
				#define DT_Cos(v) (tex2D(PowDataDite,DTFloat2(v, 0.75)).g * 2.0 + -1.0)

				#define DT_SinV(v) (tex2Dlod(PowDataDite,DTFloat4(v, 0.25, 0.0, 0.0)).g * 2.0 + -1.0)
				#define DT_CosV(v) (tex2Dlod(PowDataDite,DTFloat4(v, 0.75, 0.0, 0.0)).g * 2.0 + -1.0)
			#endif
		
			// ===============================================获取摄像机方向========================================================================
			// DTFloat3 dt_WorldViewDir(DTFloat4 vertex)
			// {
			//     return normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, vertex).xyz));
			// }
			#ifdef YOFI_CAMERA_ORTHOGRAPHIC
				uniform float4 _CustomOrthographicDir;
				DTFloat3 dt_WorldViewDir(DTFloat4 vertex)
				{
					return normalize(_CustomOrthographicDir.xyz);
				}
			#else
				DTFloat3 dt_WorldViewDir(DTFloat4 vertex)
				{
					return normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, vertex).xyz));
				}
			#endif

			//=============================================================贴图通道获取=============================================================
			//置灰 ###被使用了###
			inline DTFloat dt_getGray(DTFloat4 col){
				return dot(col.rgb, DTFloat3(0.2126729f,  0.7151522f, 0.0721750f)) * col.a;
			}

			//贴图去黑底方法 UnityDiteDelBlackAdd
			DTFloat4 dt_DelBlackAdd(DTFloat4 color)
			{
				DTFloat _maxcolor =  max(color.x,max(color.y,color.z));
				// color.a = min(color.a,_maxcolor);
				DTFloat ss = lerp(1.0,_maxcolor,step(1.0/256.0,_maxcolor));
				return DTFloat4(color.rgb / ss * color.a,_maxcolor);
			}

			// ===============================================POW==================================================================================
			//POW 优化
			inline DTFloat dt_Pow2(DTFloat x)
			{
				return x*x;
			}

			inline DTFloat dt_Pow3(DTFloat x)
			{
				return x*x * x;
			}

			inline DTFloat dt_Pow4(DTFloat x)
			{
				return x*x * x*x;
			}

			inline DTFloat dt_Pow5(DTFloat x)
			{
				return x*x * x*x * x;
			}

			inline DTFloat dt_Pow(DTFloat x, DTFloat n)
			{ 
				#if DT_EDITORMODE
					return pow(x, n);
				#else
					DTFloat v1 = 0.03125 * floor(n);// 1.0 / 32.0
					DTFloat s1 = tex2D( /*固定数据贴图 1-32阶*/PowDataDite, DTFloat2(x,v1 + 0.015625 )).r;
					return s1;
				#endif
			}
			inline DTFloat4 dt_Pow(DTFloat4 v, DTFloat n)
			{
				return DTFloat4(dt_Pow(v.x, n),dt_Pow(v.y, n),dt_Pow(v.z, n),dt_Pow(v.w, n)); 
			}

			inline DTFloat dt_PowV(DTFloat x, DTFloat n)
			{ 
				#if DT_EDITORMODE
					return pow(x, n);
				#else
					DTFloat v1 = 0.03125 * floor(n);// 1.0 / 32.0
					DTFloat s1 = tex2Dlod( /*固定数据贴图 1-32阶*/PowDataDite, DTFloat4(x,v1 + 0.015625, 0.0, 0.0 )).r;
					return s1;
				#endif
			}
			inline DTFloat4 dt_PowV(DTFloat4 v, DTFloat n)
			{
				return DTFloat4(dt_PowV(v.x, n),dt_PowV(v.y, n),dt_PowV(v.z, n),dt_PowV(v.w, n)); 
			}


			// =============================================== Ramp  ============================================================================
			//从x->y转成a->b
			inline DTFloat dt_Ramp(DTFloat _v,DTFloat _x ,DTFloat _y ,DTFloat _a,DTFloat _b)
			{
				return _a + (_v - _x) * (_b - _a) / (_y - _x);
			}
			//输入XY，输出0-1
			inline DTFloat dt_RampXY01(DTFloat _v,DTFloat _x ,DTFloat _y)
			{
				return (_v - _x) / (_y - _x);
			}
			//输入0-1 输出a-b
			inline DTFloat dt_RampIn01(DTFloat _v,DTFloat _a ,DTFloat _b)
			{
				return _a + _v  * (_b - _a);
			}

			//输入0-1 输出 -1 - 1
			inline DTFloat dt_RampZF1(DTFloat _v)
			{
				return _v * 2.0 + -1.0;
			}

			//从x->y转成 -1 - 1
			inline DTFloat dt_RampZF1(DTFloat _v,DTFloat _x ,DTFloat _y)
			{
				return (_v - _x) * 2.0 / ( _y - _x ) + -1.0;
			}


			// ===============================================切线空间法线算法========================================================================
			
			// TEXCOORD(i,i+1,i+2) [3x3:tangentToWorld | 1x3: worldPos]
			#define DT_TBNW(i)  DTFloat4 tbnw[3]: TEXCOORD##i

			//n可以传入法线贴图
			inline DTFloat3 dt_LoadWorldNomal(DTFloat4 tbn[3],DTFloat3 n)
			{
				DTFloat3 worldN = 0;
				worldN.x = dot(tbn[0].xyz, n);
				worldN.y = dot(tbn[1].xyz, n);
				worldN.z = dot(tbn[2].xyz, n);
				return normalize(worldN);//tangent space normal, if written
			}
			//直接获取世界法线
			inline DTFloat3 dt_LoadWorldNomal(DTFloat4 tbn[3])
			{
				DTFloat3 worldN = 0;
				worldN.x = dot(tbn[0].xyz, DTFloat3(0,0,1));
				worldN.y = dot(tbn[1].xyz, DTFloat3(0,0,1));
				worldN.z = dot(tbn[2].xyz, DTFloat3(0,0,1));
				return normalize(worldN);//tangent space normal, if written
			}


			#define DT_TBNW_VERT(v,n,t)  \
				DTFloat4 posWorld = mul(unity_ObjectToWorld, v);\
				DTFloat3 normalWorld = UnityObjectToWorldNormal(n);\
				fixed3 worldTangent = UnityObjectToWorldDir(t.xyz);\
				fixed tangentSign = t.w * unity_WorldTransformParams.w;\
				fixed3 worldBinormal = cross(normalWorld, worldTangent) * tangentSign;\
				o.tbnw[0] = DTFloat4(worldTangent.x, worldBinormal.x, normalWorld.x, posWorld.x);\
				o.tbnw[1] = DTFloat4(worldTangent.y, worldBinormal.y, normalWorld.y, posWorld.y);\
				o.tbnw[2] = DTFloat4(worldTangent.z, worldBinormal.z, normalWorld.z, posWorld.z)



			// ===============================================DTFloatDir算法========================================================================
			inline DTFloat3 dt_Unity_SafeNormalize(DTFloat3 inVec)
			{
				DTFloat dp3 = max(0.001f, dot(inVec, inVec));
				return inVec * rsqrt(dp3);
			}
			
			// // ===============================================YF_USECUSTOMTIME========================================================================
			// #ifdef ONLYWORK
			// 	#ifdef YF_USECUSTOMTIME
			// 		//Shader.SetGlobalDTFloat("_yfTime",yofitime % 10);
			// 		uniform DTFloat _yfTime; 
			// 		#define YF_SHADER_WORKTIME _yfTime
			// 		#define DTTimeAccuracy 10
			// 		#define DTTimeWork(s) (floor(s * DTTimeAccuracy) / DTTimeAccuracy)
			// 	#else
			// 		#define YF_SHADER_WORKTIME _Time.y
			// 		#define DTTimeWork(s) s
			// 	#endif 

			// 	// 设置时间 可改其他变量来控制时间，关闭是1执行：YF_SHADER_WORKTIME * speed
			// 	#ifdef _TIMEDT_ON 
			// 		#define DT_AUTOTIME(speed) YF_SHADER_WORKTIME * DTTimeWork(speed)
			// 	#else
			// 		#define DT_AUTOTIME(speed) speed
			// 	#endif
			// #endif

			// ===============================================BlendValue========================================================================
			// 0:Add 1:BLEND 2:MULTIPLY
			inline fixed3 dt_GEtBlendValue(fixed3 c0,fixed3 c1,DTfloat v,uint blendmode)
			{ 
				if (blendmode == 1)
				{ //BLEND 
					return lerp(c0.rgb, c1.rgb, v);
				}
				else if (blendmode == 2)
				{ //MULTIPLY
					return c0.rgb * c1.rgb;
				}
				else
				{ //Add
					return c0.rgb + c1.rgb;
				}
			}
		
		#endif // YF_UTIL_DITE_INCLUDED
	//UV Dist
		#ifndef YOFI_UVMatrixFUN_DITE_INCLUDED
		#define YOFI_UVMatrixFUN_DITE_INCLUDED 
			float _DT_ParticleCustom[9] ; 

			inline void Set_ParticleCustom(float4 a,float4 b,float cola){
				_DT_ParticleCustom[0] = a.x;
				_DT_ParticleCustom[1] = a.y;
				_DT_ParticleCustom[2] = a.z;
				_DT_ParticleCustom[3] = a.w;
				_DT_ParticleCustom[4] = b.x;
				_DT_ParticleCustom[5] = b.y;
				_DT_ParticleCustom[6] = b.z;
				_DT_ParticleCustom[7] = b.w;			
				_DT_ParticleCustom[8] = cola;
			}
			inline DTfloat dt_ParticleCustomWork(float v, uint mode){
				if(mode < 9){
					return _DT_ParticleCustom[mode];
				}else{
					return v;
				}
			}

			inline DTfloat dt_ParticleCustomWork(float v, uint mode,uint isauto){
				if(mode < 9){
					return _DT_ParticleCustom[mode];
				}else{
					if(isauto > 0){					
						return DT_AUTOTIME(v);
					}else{					
						return v;
					}
				}
			}

			#if DT_EDITORMODE
				#define DT_Sin(v) sin(v)
				#define DT_Cos(v) cos(v)
				#define DT_SinV(v) sin(v)
				#define DT_CosV(v) cos(v)
			#else
				#define DT_Sin(v) (tex2D(PowDataDite,DTFloat2(v, 0.25)).g * 2.0 + -1.0)
				#define DT_Cos(v) (tex2D(PowDataDite,DTFloat2(v, 0.75)).g * 2.0 + -1.0)

				#define DT_SinV(v) (tex2Dlod(PowDataDite,DTFloat4(v, 0.25, 0.0, 0.0)).g * 2.0 + -1.0)
				#define DT_CosV(v) (tex2Dlod(PowDataDite,DTFloat4(v, 0.75, 0.0, 0.0)).g * 2.0 + -1.0)
			#endif
			//==============================================================
			// UV 默认矩阵
			// |1.0,0.0,0.0|
			// |0.0,1.0,0.0|
			// |0.0,0.0,1.0|
			#define DT_MAT_UV_DEF DTFloat3x3(1.0,  0.0,   0.0, 0.0,  1.0,   0.0, 0.0,  0.0,   1.0)
			//==============================================================
			// UV 移动矩阵
			// |1.0,0.0, x |
			// |0.0,1.0, y |
			// |0.0,0.0,1.0|
			#define DT_MAT_UV_MOVE(v) DTFloat3x3(1.0,  0.0,   v.x, 0.0,  1.0,   v.y, 0.0,  0.0,   1.0)
			//==============================================================
			// UV 围绕中心 旋转变化矩阵 
			// |0.5,0.0,0.5| |cos,-sin,0.0| |2.0,0.0,-1.0|
			// |0.0,0.5,0.5|*|sin, cos,0.0|*|0.0,2.0,-1.0|
			// |0.0,0.0,1.0| |0.0, 0.0,1.0| |0.0,0.0, 1.0|
			#define DT_MAT_UV_ROT(v) DTFloat3x3(v.y, -v.x, (-v.y + v.x + 1.0) * 0.5, v.x, v.y, (-v.x - v.y + 1.0) * 0.5, 0.0, 0.0, 1.0)

			//==============================================================
			// UV 围绕中心 缩放变化矩阵
			// |0.5,0.0,0.5| | x ,0.0,0.0| |2.0,0.0,-1.0|
			// |0.0,0.5,0.5|*|0.0, y ,0.0|*|0.0,2.0,-1.0|
			// |0.0,0.0,1.0| |0.0,0.0,1.0| |0.0,0.0, 1.0|
			#define DT_MAT_UV_SCALE(v)  DTFloat3x3(v.x,  0.0, (1.0 - v.x) * 0.5, 0.0, v.y, (1.0 - v.y) * 0.5, 0.0, 0.0, 1.0)


			// ===============================================极坐标==========================================================
			//== 极坐标
			// inline DTFloat2 dt_Polar(DTFloat2 _uv) // 默认算法
			// {
			// 	#if DT_EDITORMODE
			// 		//0~1的1象限转-0.5~0.5的四象限
			// 		DTFloat2 uv = _uv - 0.5;
			// 		//d为各个象限坐标到0点距离,数值为0~0.5
			// 		DTFloat distance = length(_uv * 2 - 1);
			// 		//0~0.5放大到0~1
			// 		// distance *= 2;
			// 		//4象限坐标求弧度范围是 [-pi,+pi]
			// 		DTFloat angle0= atan2(uv.x,uv.y);
			// 		//把 [-pi,+pi]转换为0~1
			// 		DTFloat angle= angle0 / UNITY_PI * 0.5 + 0.5;
			// 		//输出角度与距离
			// 		return DTFloat2(angle,distance);
			// }
			inline float2 PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale)
			{
				#if DT_EDITORMODE
					//官方算法
					float2 delta = UV - Center;
					float radius = length(delta) * 2 * RadialScale;
					float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
					return float2(radius, angle);
				#else
					//直接读取数据贴图
					return tex2D( PowDataDite, _uv ,ddx(_uv)*0.1,ddy(_uv)*0.1).zw * DTFloat2(1.0,2.0);
				#endif
			}

			inline void PolarCoordinates_float(inout float2 srcuv){		
				srcuv = PolarCoordinates_float(srcuv ,float2(0.5,0.5),1,1); 
			}
			
			inline void UpdateMatrix(DTfloat2 v,uint mode,inout DTFloat3x3 d){
				if(mode == 1)
				{// move
					d = DT_MAT_UV_MOVE(v);	
				}
				else if(mode == 2)
				{// ROT
					DTFloat r = v.x;
					#if DT_EDITORMODE
					r = radians(v.x * 360.0); //这里只有非优化模式才会使用
					#endif
					DTFloat2 csv = DTFloat2(DT_Sin(r),DT_Cos(r)); 
					d = DT_MAT_UV_ROT(csv);		 
				}
				else if( mode == 3){
					d = DT_MAT_UV_SCALE(v);
				}
				else{
					d = DT_MAT_UV_DEF;
				}			
			}		
			inline DTfloat2 GetAnimUV(in float3x3 mat,float2 uv)
			{
				return mul(mat,float3(uv,1));
			}
			//=======================Properties=======================
			// [Toggle] _isPolar("--is Polar ",float) = 0 
			// [Enum(NONE,0,MOVE,1,ROT,2)] _UVAnimMode("--UV Anim mode",float )= 0 
			// [Toggle] _isUVAuto("--is UVAuto",float) = 0
			// _AnimUValue("--Anim U Value",float )= 0 
			// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_AnimUmode("--Anim U mode",float )= 9
			// _AnimVValue("--Anim V Value",float )= 0 
			// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_AnimVmode("--Anim V mode",float )= 9
			//=======================Properties=======================
			
			//=======================DistortFun======================= 
			
			//=======================DIST Properties=======================
			// [Header(Dist [DST])]
			// _DistTex(" Dist Tex", 2D) = "white"{}  
			// _DST_Brightness("Dist Pow ",float) = 1 
			// [Toggle] _DSTisFlip("isFlip ",float) = 0
			// [Toggle] _DSTisGray("isGray ",float) = 0
			
			// [Toggle] _DST_isPolar("--is Polar ",float) = 0 
			// [Enum(NONE,0,MOVE,1,ROT,2)] _DST_UVAnimMode("--UV Anim mode",float )= 0 
			// [Toggle] _DST_isUVAuto("--is UVAuto",float) = 0
			// _DST_AnimUValue("--Anim U Value",float )= 0 
			// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_DST_AnimUmode("--Anim U mode",float )= 9
			// _DST_AnimVValue("--Anim V Value",float )= 0 
			// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_DST_AnimVmode("--Anim V mode",float )= 9	
			//=======================DIST Properties=======================
			 
			//=======================DistortFun Properties=======================
			inline DTFloat2 dt_DistortUV(DTFloat2 _uv, DTFloat2 _v, DTFloat2 _pow){
				DTFloat2 uv = _uv *  _v * -1 ;
				return _uv + uv * _pow; 
			}

			#define AnimDistProp(name) float name##AnimUValue,name##AnimVValue,name##DistPowX,name##DistPowY;\
					uint name##isDiss,name##isPolar,name##UVAnimMode,name##isUVAuto,name##AnimUmode,name##AnimVmode
			#define AnimDistFun(name,diss) GetAnimUVWork(name##AnimUValue,name##AnimVValue,name##DistPowX,name##DistPowY,\
					name##isDiss,name##isPolar,name##UVAnimMode,name##isUVAuto,name##AnimUmode,name##AnimVmode,diss)
			struct DT_UV_Dist_ANIM_Data{
				float2 distvalue;		
				uint IsOn;
				
				float2 value; 
				uint isAutoTime;
				uint UVAnimMode;//[Enum(NONE,0,MOVE,1,ROT,2,SCA,3)]
				uint IsPolar; //[Toggle]
				uint U_Mode;//[c1.x,c1.y,c1.z,c1.w,c2.x,c2.y,c2.z,c2.w]
				uint V_Mode;//[c1.x,c1.y,c1.z,c1.w,c2.x,c2.y,c2.z,c2.w]
			};
			
			inline float4 GetAnimUVWork(
				float v0,float v1,float DistvalueX,float DistvalueY,
				uint _IsDistOn,uint _isPolar,uint _UVAnimMode,uint _isAuto,
				uint _AnimUmode,uint _AnimVmode,out uint animid)
			{
				animid = 0;
				animid += (uint)_isPolar << 0;
				animid += (uint)_isAuto << 1;
				animid += (uint)_UVAnimMode << 2;
				animid += (uint)_AnimUmode << 4;
				animid += (uint)_AnimVmode << 8;
				animid += (uint)_IsDistOn << 12; 
				return float4(v0,v1,DistvalueX,DistvalueY);
			}
			inline DT_UV_Dist_ANIM_Data SetAnimUVWork(float4 vv,uint dv)
			{ 
				DT_UV_Dist_ANIM_Data d = (DT_UV_Dist_ANIM_Data)0;
				d.distvalue = vv.zw;
				d.value = vv.xy;
				d.IsPolar = dv >> 0 & 0x1;
				d.isAutoTime = dv >> 1 & 0x1;
				d.UVAnimMode = dv >> 2 & 0x3;
				d.U_Mode = dv >> 4 & 0xF;
				d.V_Mode = dv >> 8 & 0xF;
				d.IsOn = dv >> 12 & 0x1;
				return d;
			}
			inline float2 AnimUVWork(float2 uv,float2 distuvCol,DT_UV_Dist_ANIM_Data d)
			{		 
				#ifdef _DTDISTOR_ON 
					if(d.IsOn > 0){
						uv = dt_DistortUV(uv, distuvCol, d.distvalue);
					}
				#endif
				float3x3 _matrix = DT_MAT_UV_DEF;
				float2 uvValue = 0;
				uvValue.x = dt_ParticleCustomWork(d.value.x,d.U_Mode,d.isAutoTime);
				uvValue.y = dt_ParticleCustomWork(d.value.y,d.V_Mode,d.isAutoTime);
				
				UpdateMatrix(uvValue,d.UVAnimMode,_matrix);
				if(d.IsPolar > 0)
				{
					PolarCoordinates_float(uv);
				}
				return GetAnimUV(_matrix,uv);
			}
			
			#ifdef  DISTORT_DEBUG
				float _DST_Brightness;
				uint _DSTisFlip,_DSTisGray,_DSTIsTowChannel;
				// AnimPropBase(_DST_);
				AnimDistProp(_DST_);
				inline YFFXInputData DS_DebugValue(){ 
					YFFXInputData k = (YFFXInputData)0;
					uint _dissData = 0;
					float4 AnimData = AnimDistFun(_DST_,_dissData);
					// float3 AnimData = AnimFunBase(_DST_); 
					k.value[1] = AnimData;
					k.value[2] = float4(0,0,_DST_Brightness,0);
					int srcsw = (uint)_DSTisFlip << 0; 
					k.mask = mergebitswitch(_dissData,srcsw); 
					return k;
				}
			#else
				inline YFFXInputData DS_DebugValue(){
					YFFXInputData k = (YFFXInputData)0;
					k.mask = _DistbitSw;
					k.value[1] = _DistValue1;
					k.value[2] = _DistValue2;
					return k;
				}
			#endif
			inline float2 DistFrag(sampler2D _DistTex,float2 _srcuv)
			{			
				#ifndef _DTDISTOR_ON
					return _srcuv;
				#endif 
				YFFXInputData k = DS_DebugValue();
				uint srcsw,animsw;
				splitbitswitch(k.mask,animsw,srcsw);

				uint IsFlip = srcsw >> 5 & 0x1;
				uint IsGray = srcsw >> 6 & 0x1;

				DT_UV_Dist_ANIM_Data uvanim = SetAnimUVWork(k.value[1],animsw);
				float2 uv = AnimUVWork(_srcuv,float2(0,0),uvanim); 
				fixed4 dc = tex2D(_DistTex, uv);
				if(IsGray > 0){ 
					dc = dt_getGray(dc);
				}
				float2 distuvCol = dc.rg;


				if( IsFlip > 0){
					distuvCol  = 1.0 - saturate(distuvCol);
				}
				distuvCol *= k.value[2].z;
				return saturate(distuvCol);
			}
			//独立使用时，比如FNL
			inline DTFloat2 DistortUVWork(DTFloat2 uv,float2 distuvCol,float2 distvalue,float ison){
				if(ison > 0){
					return dt_DistortUV(uv, distuvCol, distvalue);
				}else{
					return uv;
				}
			}
		#endif // YOFI_UVMatrixFUN_DITE_INCLUDED
	//MainTex
		#ifndef YOFI_MAINFUN_DITE_INCLUDED
		#define YOFI_MAINFUN_DITE_INCLUDED 
			// ************************************************************************************************************************************
			// 	属性的声明
			// ************************************************************************************************************************************

			//=======================BASE TEX Properties=======================
			// [Header(Base Tex)]
			// _MainTex("--Main Tex", 2D) = "white"{}
			// _TintColor("--Tint Color", Color) = (0.75,0.75,0.75,1.0)
			// _Brightness("--Brightness",float)= 1

			// [Toggle] _MainisPolar("--is Polar ",float) = 0 
			// [Toggle] _MainisUVAuto("--is UVAuto",float) = 0
			// [Enum(NONE,0,MOVE,1,ROT,2)] _MainUVAnimMode("--UV Anim mode",float )= 0 	
			// _MainAnimUValue("--Anim U Value",float )= 0 
			// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_MainAnimUmode("--Anim U mode",float )= 9
			// _MainAnimVValue("--Anim V Value",float )= 0 
			// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_MainAnimVmode("--Anim V mode",float )= 9
			// [Toggle] _MainisDiss("--Is Diss",float) = 0
			// _MainDistPowX("--Dist Pow X",float) = 0        
			// _MainDistPowY("--Dist Pow Y",float) = 0
			//=======================BASE TEX Properties=======================
			  
			
			#ifdef MAIN_DEBUG
				AnimDistProp(_Main);
				inline YFFXInputData Main_DebugValue(){  
					YFFXInputData k =  (YFFXInputData)0;
					uint _dissData = 0;
					float4 AnimData = AnimDistFun(_Main,_dissData);
					k.value[1] = AnimData;
					k.mask = _dissData;
					return k;
				}
			#else
				inline YFFXInputData Main_DebugValue(){
					YFFXInputData k =  (YFFXInputData)0;
					k.value[1] = _MainValue1;
					k.mask = _MainbitSw;
					return k;
				}
			#endif

			inline void MainTexData(sampler2D _Tex,float2 _srcuv,float2 distuvCol,out DTfloat4 srcCol)
			{ 
				YFFXInputData k = Main_DebugValue();
				uint animbsw,srcbsw;
				splitbitswitch(k.mask,animbsw,srcbsw);
				DT_UV_Dist_ANIM_Data uvanim = SetAnimUVWork(k.value[1],animbsw);
				float2 uv = AnimUVWork(_srcuv,distuvCol,uvanim);			
				srcCol = DTtex2d(_Tex, uv);
				srcCol = saturate(srcCol);
				#ifdef _ALPHATEST_ON
					ClipData.x = srcCol.a;
					ClipData.y = _ClipValue;
				#endif	 
			} 

			inline void MainTexData(sampler2D _Tex,float2 _srcuv,float4 _color,float2 distuvCol,out DTfloat4 srcCol)
			{ 
				YFFXInputData k = Main_DebugValue();
				DT_UV_Dist_ANIM_Data uvanim = SetAnimUVWork(k.value[1],k.mask);
				float2 uv = AnimUVWork(_srcuv,distuvCol,uvanim);			
				srcCol = DTtex2d(_Tex, uv, _color );
				srcCol = saturate(srcCol);			
				#ifdef _ALPHATEST_ON
					ClipData.x = srcCol.a;
					ClipData.y = _ClipValue;
				#endif
			} 
		#endif // YOFI_MAINFUN_DITE_INCLUDED
	//Fnl
		#ifndef YF_FNLFUN_DITE_INCLUDED
		#define YF_FNLFUN_DITE_INCLUDED
			//#include "UnityCG.cginc" 
			//=============================================================菲尼尔算法=============================================================
			// FNLv x:法线与视线的DOT,Y:过度的范围,Z:偏移量
			inline DTFloat dt_FNR_Work(DTFloat nv,DTFloat exp0,DTFloat exp1){
				return smoothstep(exp0,exp1,nv);
			}

			inline DTFloat3 dt_GetFNLBlendValue(fixed3 c0,DTfloat v,float4 col,uint blend)
			{  
				if (blend == 1)
				{ 	//BLEND 
					return lerp(c0, lerp(c0,col.rgb,col.a), v);
				}
				else if (blend == 2)
				{ 	//MULTIPLY
					return c0.rgb * lerp(1.0,col.rgb,v); 
				}
				else
				{ //Add
					return c0.rgb + col.rgb * v; 
				}
			}
			//=======================FNL Properties=======================
			// [Header(FNL)]
			// _FNLcol("--FNL color",color) = (1,0,0,1)
			// _FNLv0("--V0",range(-1,1)) = 0
			// _FNLv1("--V1",range(-1,1)) = 0
			// _FNLBrightness("--FNL",float) = 1
			// [Enum(Add,0,BLEND,1,MULTIPLY,2)] _FNLColBlend("Col Blend",float )= 0
			// [Enum(viewAll,0,Src.a,1,fnl.a,2,Only Fnl,3)] _FNLalphaMode("Apl Blend",float )= 0
			// [Toggle] _FNLisDiss("_CMisDiss",float) = 0	
			// _FNLdistX("_FNLdistX",float) = 0			
			//=======================Properties=======================
			
			#ifdef FNL_DEBUG		
				float4 _FNLcol;
				float _FNLv0,_FNLv1,_FNLdistX,_FNLBrightness; //_FNLdistY,
				uint _FNLColBlend,_FNLalphaMode,_FNLisDiss;
				inline YFFXInputData FNL_DebugValue(){
					YFFXInputData k =  (YFFXInputData)0;
					k.value[0] = _FNLcol;
					k.value[1] = float4(0, 0, _FNLdistX, 0);
					k.value[2] = float4(_FNLv0,_FNLv1,_FNLBrightness,0);
					uint srcsw = 0;
					srcsw += _FNLColBlend << 0;
					srcsw += _FNLalphaMode << 2;
					// srcsw += _FNLisDiss << 4;
					k.mask = mergebitswitch(0,srcsw);
					return k;
				}
			#else
				inline YFFXInputData FNL_DebugValue()
				{
					YFFXInputData k =  (YFFXInputData)0;
					k.value[0] = _FNLValue0;
					k.value[1] = _FNLValue1;
					k.value[2] = _FNLValue2;
					k.mask = _FNLbitSw;
					return k;
				}
			#endif
			//   mergebitswitch  splitbitswitch 继续改其他的读取
			inline void FNLFrag(inout DTFloat4 srcCol,float nv,float2 dist)
			{				
				#ifdef _FRESNEL_ON  
					YFFXInputData k = FNL_DebugValue(); 
					uint animbsw,srcbsw;
					splitbitswitch(k.mask,animbsw,srcbsw);
					uint isDiss = animbsw >> 12 & 0x1;
					uint BrightnesslendMode  = srcbsw >> 0 & 0x3;
					uint alphaMode = srcbsw >> 2 & 0x3;

					DTFloat fnlv = dt_FNR_Work(nv, k.value[2].x * 2, k.value[2].y * 2);
					#ifndef UNITY_COLORSPACE_GAMMA
						fnlv = GammaToLinearSpaceExact (fnlv);
					#endif
					#ifdef _DTDISTOR_ON
						fnlv = DistortUVWork(fnlv,dist,k.value[1].zz, isDiss);
					#endif

					if(alphaMode == 3){
						srcCol.a *= fnlv;
						#ifdef _ALPHATEST_ON
							ClipData.x = srcCol.a;
							ClipData.y = _ClipValue;
						#endif
					}
					else{
						DTfloat4 col = srcCol.a;
						fixed4 cc = 0;
						cc.xyz =  saturate( k.value[0].xyz * k.value[2].z);
						col.rgb = dt_GetFNLBlendValue(srcCol.rgb, fnlv, cc, BrightnesslendMode);
						
						if (alphaMode == 0)
						{
							col.a = saturate(srcCol.a + fnlv);
						}
						else if (alphaMode == 2) 
						{
							col.a = fnlv;
						}
						#ifdef _ALPHATEST_ON
							ClipData.x = col.a;
							ClipData.y = _ClipValue;
						#endif
						srcCol = saturate(col); 
					} 
				#endif
			}

			
		#endif // YF_FNLFUN_DITE_INCLUDED
	//ColorMask
		#ifndef YF_COLORMASKFUN_DITE_INCLUDED
		#define YF_COLORMASKFUN_DITE_INCLUDED
			//#include "UnityCG.cginc"
		
		
			//=======================COLORMASK Properties=======================
			// [Header(ColorMask)]
			// _ColorMaskTex("--ColorMask Tex", 2D) = "white"{}
			// _CMcol("--ColorMask Color", Color) = (0.75,0.75,0.75,1.0)
			// _CMBrightness("--pow",float)= 1
			// [Toggle] _CMisGray("--is Gray ",float) = 0 	
			// [Enum(Add,0,BLEND,1,MULTIPLY,2)] _CMblendMode("  blendMode",float )= 0
			// _CMColBlend("  ColBlend",float)= 0
			// [Enum(Add,0,BLEND,1,MULTIPLY,2)] _CMalphaMode("  alphaMode",float )= 0
			// _CMAplBlend("  AplBlend",float)= 0

			// [Toggle] _CMisPolar("--is Polar ",float) = 0 
			// [Enum(NONE,0,MOVE,1,ROT,2)] _CMUVAnimMode("--UV Anim mode",float )= 0 
			// [Toggle] _CMisUVAuto("--is UVAuto",float) = 0
			// _CMAnimUValue("--Anim U Value",float )= 0 
			// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_CMAnimUmode("--Anim U mode",float )= 9
			// _CMAnimVValue("--Anim V Value",float )= 0 
			// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_CMAnimVmode("--Anim V mode",float )= 9
			// [Toggle] _CMisDiss("--Is Diss",float) = 0
			// _CMDistPowX("--ColorMask Dist Pow X",float) = 0        
			// _CMDistPowY("--ColorMask Dist Pow Y",float) = 0
			//=======================COLORMASK Properties=======================
			// #ifdef ONLYWORK  
			// 	//ColorMask 需要的属性
			// 	// sampler2D _ColorMaskTex;float4 _ColorMaskTex_ST;
			// 	float4 _ColorMaskValue0;
			// 	float4 _ColorMaskValue1;
			// 	float4 _ColorMaskValue2;
			// 	uint _ColorMaskbitSw;  
			// #endif
				


			#ifdef COLORMASK_DEBUG	
				AnimDistProp(_CM);
				float4 _CMcol;
				float _CMColBlend,_CMAplBlend,_CMBrightness; 
				uint _CMblendMode,_CMalphaMode,_CMisclip,_CMisFlip,_CMisGray;
				
				inline YFFXInputData CM_DebugValue(){
					YFFXInputData k =  (YFFXInputData)0;
					uint _dissData;
					float4 AnimData = AnimDistFun(_CM,_dissData);
					
					k.value[0] = _CMcol;
					k.value[1] = float4(AnimData);
					k.value[2] = float4(_CMColBlend,_CMAplBlend,_CMBrightness,0); 
					uint did = 0;
					did += (uint)_CMblendMode << 0; // 4种模式
					did += (uint)_CMalphaMode << 2; // 4种模式
					did += (uint)_CMisclip << 4;
					did += (uint)_CMisFlip << 5;
					did += (uint)_CMisGray << 6;
 
					// k.value[0] = _ColorMaskValue0;
					// k.value[1] = _ColorMaskValue1;
					// k.value[2] = _ColorMaskValue2;					
					k.mask = mergebitswitch(_dissData,did); 
					return k;					
				} 
			#else
				inline YFFXInputData CM_DebugValue()
				{
					YFFXInputData k =  (YFFXInputData)0;
					k.value[0] = _ColorMaskValue0;
					k.value[1] = _ColorMaskValue1;
					k.value[2] = _ColorMaskValue2;
					k.mask = _ColorMaskbitSw;
					return k;
				}
			#endif

			
			struct DT_ColorMaskData{		
				DTfloat AplBlend;
				DTfloat ColBlend;

				DT_UV_Dist_ANIM_Data uvanim;
				
				uint IsGray; //[Toggle]
				uint IsOld; //[Toggle]
				uint IsClip; //[Toggle] 

				// uint blendMode; //[Enum(Add,0,BLEND,1,MULTIPLY,2)]
				uint alphaMode; //[Enum(Add,0,BLEND,1,MULTIPLY,2)]
				
			};	

			inline DT_ColorMaskData Get_ColorMaskData(YFFXInputData k)
			{
				DT_ColorMaskData d = (DT_ColorMaskData)0;
				uint animbsw,srcbsw;
				splitbitswitch(k.mask,animbsw,srcbsw);

				// d.blendMode = srcbsw >> 0 & 0x3; // 4种模式
				d.alphaMode = srcbsw >> 2 & 0x3; // 4种模式
				d.IsClip = srcbsw >> 4 & 0x1;
				d.IsOld =  srcbsw >> 5 & 0x1;
				d.IsGray =  srcbsw >> 6 & 0x1; 
				d.uvanim = SetAnimUVWork(k.value[1],animbsw);
				// d.ColBlend = k.value[2].x;
				d.AplBlend = k.value[2].y;
				return d;
			}
			fixed _IsNewColorMask;
			inline void ColorMaskFrag(sampler2D _Tex,inout DTFloat4 srcCol,float2 _srcuv,float2 distuvCol)
			{		
				#ifdef _DTCOLORMASK_ON 
					YFFXInputData k = CM_DebugValue(); 
					DT_ColorMaskData d = Get_ColorMaskData(k);	
					
					float4 customCol = k.value[0]/*颜色*/ * k.value[2].z /*亮度*/;
					
					float2 uv = AnimUVWork(_srcuv,distuvCol,d.uvanim);
					fixed4 maskTexColor = DTtex2d(_Tex, uv, 1.0);
					maskTexColor = saturate(maskTexColor * customCol);
					#ifdef ZMXS
						//全是为过去莫名其妙的买单
						if(d.IsOld > 0){
							maskTexColor *= 0.5;
						}
						//全是为过去莫名其妙的买单
					#endif
					if(d.IsGray > 0){
						maskTexColor = dt_getGray(maskTexColor);
					}
					fixed4 col = 0;
					if (d.alphaMode == 1)
					{
						col = lerp(srcCol, maskTexColor, d.AplBlend);
					}
					else if (d.alphaMode == 2)
					{
						col =  srcCol * maskTexColor;
					}
					else if (d.alphaMode == 3)
					{
						DTFloat4 c = srcCol;
						c.a *= maskTexColor.a;
						col =  c;
					}
					else
					{
						col =  saturate(srcCol + maskTexColor);
					}
					col.a = lerp(col.a,maskTexColor.a,d.IsClip);
 
					srcCol = saturate( col);
					#ifdef _ALPHATEST_ON
						ClipData.x = srcCol.a;
						ClipData.y = _ClipValue;
					#endif			 
				#endif
			} 
			
		#endif // YF_COLORMASKFUN_DITE_INCLUDED
	//Diss
		#ifndef YF_DISSFUN_DITE_INCLUDED
		#define YF_DISSFUN_DITE_INCLUDED
			//#include "UnityCG.cginc" 
			//=======================Dissolution Properties=======================
			// [Header(Dissolution)]
			// _DissTex("--Dissolution Tex", 2D) = "white"{}		
			// _DISSBrightness("--Brightness",float) = 1
			// _DISSValue("--Value",Range(0.0,1.0)) = 0

			// _DISSLinewidth("--Line Width",float) = 0
			// _DISSlineBrightness("--Line Brightness",float) = 0
			// _DISSLinecol("--Line Color", Color) = (0.75,0.75,0.75,1.0)
			// [Enum(Add,0,BLEND,1,MULTIPLY,2)] _DISSLineColBlend("--LineColBlend",float )= 0
			// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_DISSvaluemode("--value mode",float )= 9


			// [Toggle] _DISSisclip("--is Clip ",float) = 0 
			// [Toggle] _DISSisFlip("--is Flip ",float) = 0 
			// [Toggle] _DISSisGray("--is Gray ",float) = 0 
			// [Toggle] _DISSIsSrcApl("--is Src Apl ",float) = 0 


			// [Toggle] _DISSisPolar("--is Polar ",float) = 0 
			// [Enum(NONE,0,MOVE,1,ROT,2)] _DISSUVAnimMode("--UV Anim mode",float )= 0 
			// [Toggle] _DISSisUVAuto("--is UVAuto",float) = 0
			// _DISSAnimUValue("--Anim U Value",float )= 0 
			// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_DISSAnimUmode("--Anim U mode",float )= 9
			// _DISSAnimVValue("--Anim V Value",float )= 0 
			// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_DISSAnimVmode("--Anim V mode",float )= 9
			// [Toggle] _DISSisDiss("--Is Diss",float) = 0
			// _DISSDistPowX("--ColorMask Dist Pow X",float) = 0        
			// _DISSDistPowY("--ColorMask Dist Pow Y",float) = 0
			//=======================Dissolution Properties=======================
			
			// ************************************************************************************************************************************
			// 	溶解裁切,可过渡可裁切 基础方法
			// ************************************************************************************************************************************
			//基础溶解值:_v0 = [0-1]
			//溢出溶解值:_v1 = [0-1]
			//输出:X是溢出的描边,Y是基础的颜色
			inline DTFloat2 dt_GetDissValue(DTFloat _mask,DTFloat _v0,DTFloat _v1)
			{
				DTFloat v = _v0 * (1.0 + _v1 * 2.0);
				DTFloat2 ov =  DTFloat2(dt_RampZF1(v, _v1, 1.0 + _v1), dt_RampZF1(saturate(v))) ;//dt_RampXYZF1(_v0,_v1);   
				return saturate(_mask + ov);
			}
			// #ifdef ONLYWORK  
			// 	//Dissolution 需要的属性
			// 	// sampler2D _DissTex;float4 _DissTex_ST; 
			// 	float4  _DissValue0;
			// 	float4  _DissValue1;
			// 	float4  _DissValue2;
			// 	uint _DissbitSw; 
			// #endif
			

			#ifdef DISSOLUTION_DEBUG
				AnimDistProp(_DISS); 
				float4 _DISSLinecol;
				float _DISSValue,_DISSLinewidth,_DISSlineBrightness,_DISSBrightness;
				uint _DISSLineColBlend,_DISSisclip,_DISSisFlip,_DISSisGray,_DISSIsSrcApl,_DISSvaluemode;

				inline YFFXInputData DISS_DebugValue(){		
					YFFXInputData k = (YFFXInputData)0; 
					uint _dissData;
					float4 AnimData = AnimDistFun(_DISS,_dissData);
				
					k.value[0] = _DISSLinecol;
					k.value[1] = AnimData;
					k.value[2] = float4(_DISSValue,_DISSLinewidth,_DISSBrightness,_DISSlineBrightness);
					uint did = 0;
					// did = 0;
					did += (uint)_DISSLineColBlend << 0; // 4种模式
					did += (uint)_DISSisclip << 4;
					did += (uint)_DISSisFlip << 5;
					did += (uint)_DISSisGray << 6;			
					did += (uint)_DISSIsSrcApl << 7; // 4种模式
					did += (uint)_DISSvaluemode << 8; // 4种模式 

					k.mask = mergebitswitch(_dissData,did);
					// did += (uint)_dissData << 12;// 0x1FFF
					// k.mask = did;
					return k;
				} 
			#else
				inline YFFXInputData DISS_DebugValue(){
					YFFXInputData k = (YFFXInputData)0; 
					k.value[0] = _DissValue0;
					k.value[1] = _DissValue1;
					k.value[2] = _DissValue2; 
					k.mask = _DissbitSw;

					return k;
				}
				
			#endif

			struct DTDissData{
				
				DTFloat value; //多状态属性
				uint valuemode;
				DTFloat Brightness;
				DTFloat lineWidth;
				DTFloat lineBrightness;
				DTFloat4 lineColor;

				DT_UV_Dist_ANIM_Data uvanim;

				uint lineblendMode; //Add Mule Blnend
				uint IsClip; //bool
				uint IsFlip; //bool 
				uint IsGray; //bool
				uint IsSrcApl; //bool
		
			}; 
			
			inline DTDissData GetDissData(YFFXInputData k)
			{ 
				// uint animbsw,srcbsw;
				// splitbitswitch(k.mask,animbsw,srcbsw);
				uint animbsw,srcbsw;
				splitbitswitch(k.mask,animbsw,srcbsw);
				// uint bitSw = k.mask;

				DTDissData d = (DTDissData)0;
				d.lineblendMode = srcbsw >> 0 & 0xF; // 4种模式 2
				#ifdef _ALPHATEST_ON
					d.IsClip = 1;
				#else
					d.IsClip = srcbsw >> 4 & 0x1;
				#endif
				d.IsFlip = srcbsw >> 5 & 0x1;
				d.IsGray = srcbsw >> 6 & 0x1;
 
				d.IsSrcApl = _DissbitSw2 >> 4 & 0x1;
				d.valuemode = _DissbitSw2 >> 0 & 0xF;
				d.uvanim = SetAnimUVWork(k.value[1],animbsw); 

				d.value = k.value[2].x; 
				d.lineWidth = k.value[2].y; 
				d.lineBrightness = k.value[2].w;			
				d.Brightness = k.value[2].z;
				d.lineColor = k.value[0];
				return d;
			}

			inline void DissolutionFrag(sampler2D _Tex,inout DTFloat4 srcCol,DTfloat2 _srcuv,float2 distuvCol)
			{
				#ifdef _DTDISSOLUTION_ON 
					YFFXInputData k = DISS_DebugValue(); 
					DTDissData d = GetDissData(k);
					float2 uv = AnimUVWork(_srcuv,distuvCol,d.uvanim);
					fixed4 dissCol = DTtex2d(_Tex, uv, d.Brightness);
					dissCol = saturate(dissCol);
					float dissValue = dissCol.r;
					if(d.IsGray > 0){
						dissValue = dt_getGray(dissCol);
					}
					if(d.IsFlip > 0){
						dissValue = 1.0 - dissValue;
					}

					float dissv = dt_ParticleCustomWork(d.value,d.valuemode);
					float2 ExpotValue = dt_GetDissValue(dissValue, dissv, d.lineWidth);
					if(d.IsClip > 0){
						ExpotValue = step(0.5,ExpotValue);
					}
					if(d.IsSrcApl > 0){
						ExpotValue *= srcCol.a;
					}

					DTFloat3 c;
					DTFloat a = d.lineColor.a * step(0.0000001, d.lineWidth);
					if (d.lineblendMode == 1)
					{ //BLEND 
						c = lerp(srcCol.rgb, d.lineColor.rgb , a);
					}
					else if (d.lineblendMode == 2)
					{ //MULTIPLY
						c = srcCol.rgb * (d.lineColor.rgb * a + abs(1.0 - a));
					}
					else
					{ //Add
						c = srcCol.rgb + d.lineColor.rgb * a;
					}
					DTFloat4 e;
					srcCol.rgb = lerp(saturate(c) * d.lineBrightness, srcCol.rgb, ExpotValue.x) * ExpotValue.y;
					srcCol.a = ExpotValue.y * srcCol.a;
					
					#ifdef _ALPHATEST_ON
						ClipData.x = saturate(srcCol.a);
						ClipData.y = (1.0 - ExpotValue.y) * 1.000001;//溶解在最后一次输出，所以这里直接等于溶解值
					#else
						srcCol.a *= ExpotValue.y;
					#endif 
					srcCol = saturate(srcCol);
				#endif 
			} 
		#endif // YF_DISSFUN_DITE_INCLUDED
	//sprout 生长
		#ifndef YF_SPROUTFUN_DITE_INCLUDED
		#define YF_SPROUTFUN_DITE_INCLUDED
			// #ifdef ONLYWORK  
			// 	float2 _SproutValue;
			// 	uint _SproutbitSw;
			// #endif
			
			#ifdef SPROUT_DEBUG
				//=======================Sprout Properties=======================
				// [Header(Sprout)]
				// [Toggle]_SPROUT("--生长 开关",float) = 0
				// [Enum(U,0,V,1,FU,2,FV,3)]_SproutDir("--生长方向",float) = 0
				// _SproutValue1("--生长",range(0.0,1.0)) = 1
				// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_SproutValueMode("--Sprout Value mode",float )= 9
				// _SproutNicety("--生长细节",float) = 0
				// [Enum(None,9,c1.x,0,c1.y,1,c1.z,2,c1.w,3)]_SproutNicetyMode("--Sprout Nicety mode",float )= 9
				//=======================Sprout Properties=======================
				uint _SproutDir,_SproutValueMode,_SproutNicetyMode;
				float _SproutValue1,_SproutNicety;
				inline YFFXInputData Setsprout()
				{
					YFFXInputData k = (YFFXInputData)0;
					k.value[2].x = _SproutValue1;
					k.value[2].y = _SproutNicety;
					uint dd = _SproutDir << 0;
					dd += _SproutValueMode << 4;
					dd += _SproutNicetyMode << 8; 
					k.mask = mergebitswitch(0,dd);				
					return k;
				}
			#else
				inline YFFXInputData Setsprout(){
					YFFXInputData k = (YFFXInputData)0;
					k.value[2] = _SproutValue;//DTfloat4(_SproutValue1.xy,0,0); 
					k.mask = _SproutbitSw;
					return k;
				}
			#endif
			inline void sproutVert(float2 srcuv,float3 normal,inout float4 pos,inout float sprotdir){
				sprotdir = 1;
				#ifdef _SPROUT_ON
					YFFXInputData k = Setsprout(); 

					float _sprotdir = srcuv.x;
					uint SproutDir = k.mask >> 0 & 0xF;
					if(SproutDir % 2 > 0)
					{   
						_sprotdir = srcuv.y;
					}
					if(SproutDir > 1){
						sprotdir = 1.0 - _sprotdir;
					}else{
						sprotdir = _sprotdir;
					}
					uint Nicetymode = k.mask >> 8 & 0xF;
					float v1 = dt_ParticleCustomWork(k.value[2].y,Nicetymode);
					pos.xyz += normal * v1 * _sprotdir;
				#endif
			}
			inline void SproutFrag(inout float4 srcCol, float sprotdir)
			{ 
				
				#ifdef _SPROUT_ON
					YFFXInputData k = Setsprout();
					// uint animbsw,bitSw;
					// splitbitswitch(k.mask,animbsw,bitSw);
					uint bitSw = k.mask;

					uint valuemode = bitSw >> 4 & 0xF;
					float v =  dt_ParticleCustomWork(k.value[2].x,valuemode);
					srcCol.a *= step(1.0 - v,sprotdir);
					#ifdef _ALPHATEST_ON
						ClipData.x = saturate(srcCol.a);
					#endif
				#endif
			}
		#endif //YF_SPROUTFUN_DITE_INCLUDED
	//YFPOST Bloom
		#ifndef YF_YFPOST_DITE_INCLUDED
		#define YF_YFPOST_DITE_INCLUDED  
			// [Header(Custom Bloom)] // CB=>Custom Bloom
			// [HideInInspector]_CBLocalThreshhold("CB Threshhold",range(0.0,1.0)) = 1
			// [HideInInspector]_CBLocalColor("CB Color",Color) = (1,1,1,1)
			// [HideInInspector]_CBLocalLight("CB 亮度",float) = 1
			// [HideInInspector][Toggle]_CBLocalBanBool("Ban开关",float) = 0
			
			
			inline DTfloat4 BloomOutfrag(DTfloat4 col){
				#ifndef YF_POST_BLOOM
					return col;
				#else
					if(_CBLocalBanBool == 1){
						return col;
					}
					if(_CBGlobalBool == 1){
						DTfloat cc = dot(col.rgb, float3(0.2126729f,  0.7151522f, 0.0721750f)) ;
						fixed bcol = smoothstep(1.0 - max(0,_CBLocalThreshhold), 1.0 , cc ) * col.a ;
						fixed3 nc = col.rgb * bcol;
						col.rgb += nc * _CBLocalLight * _CBLocalColor.rgb;
						// col.rgb *= col.a;
						return col;
					}
					else{
						return col;
					}
				#endif
			}
		#endif // YF_YFPOST_DITE_INCLUDED

	//other
		#ifndef YF_OTHER_DITE_INCLUDED		
		#define YF_OTHER_DITE_INCLUDED
			// ************************************************************************************************************************************
			// 	=区域外不显示
			// ************************************************************************************************************************************
			
			//=======================ClipGround Properties======================= 
			// [Toggle]_ClipGroundbitSw("是否有地面[Y = 0]",float) = 0
			// _ClipGroundValue("地面接缝过度",float) = 0.5
			//=======================ClipGround Properties======================= 
			// #ifdef ONLYWORK  
			// 	DTfloat _ClipGroundbitSw, _ClipGroundValue;
			// #endif
			inline DTFloat dt_ClipInWorldY(DTFloat worldposY)
			{
				if(_ClipGroundbitSw > 0){
					#ifdef _ALPHATEST_ON
						float g = step(0,worldposY);
						ClipData.x *= g; 
						return g;
					#else
						return smoothstep(0.0, _ClipGroundValue, worldposY);
					#endif
				}else{
					return 1.0;
				}
			}
			inline void ClipGroundVert(inout fixed4 srccol,DTfloat4 worldPos)
			{
				srccol *= dt_ClipInWorldY(worldPos.y);
			}
			//=============================================================UI 特效裁切==================================================================
			//ui上遇到scrollview 时，特效在UI上裁切不显示的方法
			//黑帮目前使用的是平面坐标 既从顶点函数 UnityObjectToClipPos(v.vertex);出来的顶点
			// 方式1
			//     通过获取RECT的4个顶点的极限坐标来裁切
			//     RectTransform rectTransform = go.GetComponent<RectTransform>();
			//     Vector3[] corners = new Vector3[4];
			//     rectTransform.GetWorldCorners(corners);
			//     DTFloat minX = corners[0].x;
			//     DTFloat minY = corners[0].y;
			//     DTFloat maxX = corners[2].x;
			//     DTFloat maxY = corners[2].y;
			//     return new Vector4(minX, minY, maxX, maxY);
			//  方式2
			// //通过获取RECT的中心点以及向[X,Y]边衍生的距离来确定范围。计算比较繁琐，效率有点低
			// //这里的变量可以是常量。因为都是直接从引擎读取并且游戏立项后都不会再改动了的
			// // ===========================================================================
			// // 计算过程
			// // vector2 pos0 = rectTransform.anchoredPosition;
			// // vector2 piont = rectTransform.pivot;
			// // vector2 size = rectTransform.rect;
			// DTFloat osize = Camera.main.orthographicSize;//正交相机的size 默认是5，最好是改成比例是1:1
			// DTFloat viewsetHight = 1080f; //Canvas里设置的相机画布的高度
			// DTFloat PerPixels = 100; //Canvas里设置的每像素比例。

			// DTFloat bfenbiPerPixels = 1.0f / PerPixels; 
			// 每像素与单位M之间的比例关系 0.925925925f<=1920*1080 
			// DTFloat bili = osize / (viewsetHight * bfenbiPerPixels * 0.5f);
			// // ===========================================================================
			// Vector2 p = rectTransform.pivot * 2.0f - Vector2.one ;
			// Vector4 pos = Vector3.one;
			// pos.z = rectTransform.rect.width * 0.5f;
			// pos.w = rectTransform.rect.height * 0.5f;
			// pos.x = rectTransform.anchoredPosition.x - p.x * pos.z;
			// pos.y = rectTransform.anchoredPosition.y - p.y * pos.w;               
			// return  pos * bfenbiPerPixels * bili;
			// DTFloat b = 1 - ceil(saturate(_v.z * _v.w));

			DTFloat4 _ScrollViewRect; //UI裁切用的的属性。输入世界坐标。必须在正交摄像机下
			inline DTFloat DT_UIscrollviewClip( DTFloat4 pos, DTFloat4 _v){
				// _v = DTFloat4(0,-10,10,-1); //测试代码
				DTFloat result = sign(_v.z - _v.x) * sign( _v.w - _v.y); //* sign(v.x + v.y + v.z + v.w); 
				if (result == 0){
					return 1;
				}
				#if 1
					result = step(_v.x, pos.x) * step(pos.x, _v.z) * step(_v.y, pos.y) * step(pos.y, _v.w);
				#else				
					DTFloat2 r = ceil(saturate(_v.zw - abs(_v.xy - pos.xy)));
					result = r.x * r.y;
				#endif
				return result; 
			}
			inline DTFloat DT_UIscrollviewClip( DTFloat4 pos){
				fixed c = DT_UIscrollviewClip(pos,_ScrollViewRect);
				#ifdef _ALPHATEST_ON
					ClipData.x *= c;
				#endif
				return c;
			}		
			//=============================================================Get BlendforRender ==================================================================
			#ifndef FXRenderStudio
			#define FXRenderStudio 0
			#endif
			DTFloat _ImageExporterRenderShaderADDMASK;
			DTFloat _ImageExporterRenderShaderBlendMASK;
			uint _DstBlend;

			inline void ForYOFIU3dtoImage(inout DTFloat4 fragcolor)
			{
			#if FXRenderStudio
				if (_DstBlend > 1)
				{
					fragcolor *= (1 - _ImageExporterRenderShaderBlendMASK);
				}
				else
				{
					fragcolor *= (1 - _ImageExporterRenderShaderADDMASK);
				}
			#else
				fragcolor = fragcolor;
			#endif
			}

			//=============================================================LastExportColor ==================================================================
			inline DTFloat4 LastExportColor(DTFloat4 fragcolor)
			{ 
				// fragcolor *= DT_UIscrollviewClip(worldPos);
				ForYOFIU3dtoImage(fragcolor);
				fragcolor = saturate(fragcolor);
				YFCustomClip
				fragcolor = BloomOutfrag(fragcolor);
				return fragcolor;
			} 
		#endif // YF_YFPOST_DITE_INCLUDED
	#endif
	


	
#endif // YF_EFFECT_CGFUNVF_DITE_INCLUDED