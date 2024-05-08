//***********************************************************************************
// 直接给与特效shader使用的
// #include "YFShaderBaseData.cginc"
//***********************************************************************************

#ifndef YF_DATA_BASE_UNITY_CG_INCLUDED
#define YF_DATA_BASE_UNITY_CG_INCLUDED
#include "UnityCG.cginc"



//初始的一些设置
#include "YoFi_Common_Base.cginc"
//杂项 包含一些常用的算法和常用的优化
#include "YoFi_Util_Base.cginc"
//各种矩阵
#include "YoFi_Matrix_Base.cginc"
//UV 相关
#include "YoFi_UV_Base.cginc"

#include "YFEffectCGForYFPost.cginc"


//公告板 相关
#include "YoFi_Billborad_Base.cginc"
//UI 相关
#include "YoFi_UI_Base.cginc"
//特效表现 相关
#include "YoFi_FXShow_Base.cginc"




// =============================================================溶解======================================================
// inline DTFloat2 DTDissWork(DTFloat _mask,DTFloat4 _maskvalue)
// {
//     return dt_DissNormal(_mask,_maskvalue.x,_maskvalue.y);
// }


#endif // YF_DATA_BASE_UNITY_CG_INCLUDED
