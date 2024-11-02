//***********************************************************************************
// 特效中特殊表现的方法
// #include "YFDTEffectShow.cginc"

//== 颜色Flip 反相
//== Clip
//== 菲尼尔算法
//== 溶解
//***********************************************************************************
#ifndef YF_EFFECTSHOW_UNITY_CG_INCLUDED
#define YF_EFFECTSHOW_UNITY_CG_INCLUDED
#include "UnityCG.cginc"
//初始的一些设置
#include "YoFi_Common_Base.cginc"



// ===============================================颜色Flip 反相===============================================================
inline DTFloat4 dt_Flip(DTFloat4 col,DTFloat IsFlip){
    return abs(IsFlip - col);
}
//============================================================Clip==================================================================
inline DTFloat dt_ClipMode(DTFloat v, DTFloat _clip)
{
    if(_clip > 0){
        return step(0.5,v);
    }
    else{
        return v;
    }
}
inline DTFloat2 dt_ClipMode(DTFloat2 v, DTFloat _clip)
{
    if(_clip > 0){
        return step(0.5,v);
    }
    else{
        return v;
    }
}
//=============================================================菲尼尔算法=============================================================
// FNLv x:法线与视线的DOT,Y:过度的范围,Z:偏移量
inline DTFloat dt_FNRwork(DTFloat nv,DTFloat exp0,DTFloat exp1){
    return smoothstep(exp0,exp1,nv);  
    // return pow(saturate(FNLv.x + FNLv.z),max(min(32,FNLv.y),0));
}
// =============================================================溶解======================================================
//基础溶解值 _v0 = [0-1] 
//溢出溶解值_v1 = [0-1] 
//输出 X是溢出的描边， Y是基础的颜色
inline DTFloat2 dt_DissNormal(DTFloat _mask,DTFloat _v0,DTFloat _v1)
{
    DTFloat v = _v0 * (1.0 + _v1 * 2.0);
    DTFloat2 ov =  DTFloat2(dt_RampZF1(v, _v1, 1.0 + _v1), dt_RampZF1(saturate(v))) ;//dt_RampXYZF1(_v0,_v1);   
    return saturate(_mask + ov);
}


// ************************************************************************************************************************************
// 	=区域外不显示
// ************************************************************************************************************************************

DTfloat _IsClipGround, _IsClipGroundBlur;

inline DTFloat dt_ClipInWorldY(DTFloat worldposY,DTFloat ShowValue,DTFloat blurValue)
{
    if(_IsClipGround > 0){	
        return  smoothstep(-ShowValue, ShowValue + blurValue,worldposY);
    }else{
        return 1.0;
    }    
}
// ============================================================ClipGrpound 地板裁切======================================================
inline DTFloat dt_ClipInWorldY(DTFloat worldposY)
{
	if(_IsClipGround > 0){		
    	return smoothstep(0, _IsClipGroundBlur,worldposY);
	}else{
		return 1.0;
	}
    
}

#endif // YF_EFFECTSHOW_UNITY_CG_INCLUDED

