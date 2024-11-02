
// YoFiUnityCG
#ifndef YF_EFFECT_CGFUN_DITE_INCLUDED
#define YF_EFFECT_CGFUN_DITE_INCLUDED
#include "UnityCG.cginc"
#include "YFBaseData.cginc"


// ************************************************************************************************************************************
// 	ClipGrpound 地板裁切
// ************************************************************************************************************************************
// DTfloat _IsClipGround, _IsClipGroundBlur;

// inline DTFloat dt_ClipInWorldY(DTFloat worldposY)
// {
// 	if(_IsClipGround > 0){		
//     	return smoothstep(0, _IsClipGroundBlur,worldposY);
// 	}else{
// 		return 1.0;
// 	}
    
// }
// ************************************************************************************************************************************
// 	溶解裁切,可过渡可裁切 基础方法
// ************************************************************************************************************************************
// DTFloat _IsClip;
inline DTFloat dt_DissolveBase(DTFloat maskValue,DTFloat value,DTfloat _IsClip)
{
	if(_IsClip == 1){                    
		return step(1.0 - maskValue,value);
	}
	else{
		return saturate(maskValue + (value * 2.0 - 1.0));
	}
}

// ==================================================================================================================
// 	带描边的溶解方法 
// ==================================================================================================================
inline DTFloat2 dt_DissolveBase(DTFloat maskValue, DTFloat value,DTfloat lineWidth,DTfloat _IsClip)
{	 
	DTFloat lw = value - lineWidth * smoothstep(1.0, 1.0 - lineWidth, value);
	DTFloat2 Dv = fixed2(0,0);
	Dv.x = dt_DissolveBase(maskValue.x,value,_IsClip);
	Dv.y= dt_DissolveBase(maskValue.x,lw,_IsClip);
	return Dv;
}
// ************************************************************************************************************************************
// 	溶解裁切,可过渡可裁切
// ************************************************************************************************************************************
// DTFloat _IsFlip;
inline DTFloat dt_Flip (DTFloat value,DTFloat _IsFlip){
	return dt_Flip(DTFloat4(value,0,0,0),_IsFlip).x;
}
inline DTFloat2 dt_Flip (DTFloat2 value,DTFloat _IsFlip){
	return dt_Flip(DTFloat4(value.x,value.y,0,0),_IsFlip).xy;
}
inline DTFloat3 dt_Flip (DTFloat3 value,DTFloat _IsFlip){
	return dt_Flip(DTFloat4(value.x,value.y,value.z,0),_IsFlip).xyz;
}
// ************************************************************************************************************************************
// 	Color Mask 
// ************************************************************************************************************************************
inline DTFloat4 Dt_ColorMaskWork(DTFloat4 _srcCol, DTFloat4 _mask, DTFloat vx, int _isclip, int _isblend)
{
	DTFloat4 mask = _mask;
	mask.a = lerp(_mask.a, dt_getGray(_mask), _isclip);

	if (_isblend == 1)
	{
		return lerp(_srcCol, mask, vx);
	}
	else if (_isblend == 2)
	{
		return _srcCol * mask;
	}
	else
	{
		return saturate(_srcCol + mask);
	}
}


// ************************************************************************************************************************************
// 	简单判断的返回 是不是 0 或 1
// ************************************************************************************************************************************
// 0或大于0 返回1
inline DTfloat dt_Bool0(DTfloat v){
	return step(1e-5,v);
}
// 1或大于1 返回1
inline DTfloat dt_Bool1(DTfloat v){
	return step(1.0,v + 1e-5);
}

// ************************************************************************************************************************************
// 	极坐标 FUN
// ************************************************************************************************************************************
inline DTfloat2 dt_PolarUV(float2 uv,fixed isPolar){
	if(isPolar > 0){
		return dt_Polar(uv) ;
	}else{
		return uv; 
	}
}
inline DTfloat2 dt_PolarUV(float2 uv,fixed isPolar,float2 offset){
	return dt_PolarUV(uv,isPolar) + offset;
}
#endif // YF_EFFECT_CGFUN_DITE_INCLUDED

