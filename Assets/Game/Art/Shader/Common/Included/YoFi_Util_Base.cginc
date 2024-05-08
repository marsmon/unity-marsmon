//***********************************************************************************
// 特效中各种矩阵相关的方法
// #include "YFShaderUtilBase.cginc"

//== 获取摄像机方向
//== 贴图通道获取
//== POW
//== Ramp
//== 切线空间法线算法
//== DTFloatDir算法
//***********************************************************************************

#ifndef YF_DATA_UTIL_BASE_UNITY_CG_INCLUDED
#define YF_DATA_UTIL_BASE_UNITY_CG_INCLUDED

#include "UnityCG.cginc"
#include "YoFi_Common_Base.cginc"


// ===============================================获取摄像机方向========================================================================
DTFloat3 dt_WorldViewDir(DTFloat4 vertex)
{
    return normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, vertex).xyz));
}

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
 
#endif // YF_DATA_UTIL_BASE_UNITY_CG_INCLUDED

