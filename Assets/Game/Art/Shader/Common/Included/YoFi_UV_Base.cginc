//***********************************************************************************
// 特效中各种矩阵相关的方法
// #include "YFShaderUVUtilBase.cginc"

//== 扰动 扭曲 UV
//== 极坐标
//***********************************************************************************

#ifndef YF_UV_UTIL_BASE_UNITY_CG_INCLUDED
#define YF_UV_UTIL_BASE_UNITY_CG_INCLUDED

#include "UnityCG.cginc"
#include "YoFi_Common_Base.cginc"
#include "YoFi_Matrix_Base.cginc"

// ===============================================UV 移动==========================================================
inline DTFloat2 dt_moveUV(DTFloat2 _uv,DTFloat2 _speed){ 
    return _uv + _speed;
}
// ===============================================UV 旋转==========================================================
// // 非矩阵算法
// inline DTFloat2 dt_rotateUV(DTFloat2 _uv,DTFloat4 _v)
// {
//     DTFloat2 uv = _uv * 2.0 + -1.0;
//     DTFloat v = radians(_v.x * 360.0);
//     DTFloat sa = DT_Sin(v);
//     DTFloat ca = DT_Cos(v);
//     DTFloat2x2 rotv = DTFloat2x2(ca,-sa,sa,ca);
//     return mul(uv,rotv) * 0.5 + 0.5;
// }
// 矩阵算法
inline DTFloat2 dt_rotateUV(DTFloat2 _uv,DTFloat _v)
{
    #if DT_EDITORMODE
    DTFloat v = radians(_v * 360.0); //这里只有非优化模式才会使用
    DT_MAT_UV_CR(v);
    #else
    DT_MAT_UV_CR(_v);
    #endif
    return DT_UV_mul(mat,_uv);
}
// ===============================================UV 缩放==========================================================
inline DTFloat2 dt_scaleUV(DTFloat2 _uv,DTFloat2 value){ 
    DTFloat2 _value = 1.0 - value;
    DT_MAT_UV_CS(_value.x,_value.y); // out :mat 
    return  DT_UV_mul(mat,_uv);
}


// ===============================================扰动 扭曲 UV==========================================================
//扰动 扭曲 UV
inline DTFloat dt_DistortUV(DTFloat _uv, DTFloat2 _v, DTFloat2 _pow){
    DTFloat uv = _uv * -1 * _v ;
    return _uv + uv * _pow;
}

inline DTFloat2 dt_DistortUV(DTFloat2 _uv, DTFloat2 _v, DTFloat2 _pow){
    DTFloat2 uv = _uv * -1 * _v ;
    return _uv + uv * _pow;
}

// 算法有问题。未改
// inline DTFloat2 dt_DistortUVcentre(DTFloat2 _uv, DTFloat _v, DTFloat _pow){
//     // DTFloat2 uv = DTFloat2(_uv.x,abs(1.0 -_uv.y));
//     DTFloat2 uv = (_uv * -2 + 1) * _v;
//     return _uv + uv * _pow;
// }

// ===============================================极坐标==========================================================
//== 极坐标
inline DTFloat2 dt_Polar(DTFloat2 _uv) // 默认算法
{
    #if DT_EDITORMODE
        //0~1的1象限转-0.5~0.5的四象限
        DTFloat2 uv = _uv - 0.5;
        //d为各个象限坐标到0点距离,数值为0~0.5
        DTFloat distance = length(_uv * 2 - 1);
        //0~0.5放大到0~1
        // distance *= 2;
        //4象限坐标求弧度范围是 [-pi,+pi]
        DTFloat angle0= atan2(uv.x,uv.y);
        //把 [-pi,+pi]转换为0~1
        DTFloat angle= angle0 / UNITY_PI * 0.5 + 0.5;
        //输出角度与距离
        return DTFloat2(angle,distance);
    #else
        //直接读取数据贴图
        return tex2D( PowDataDite, _uv ,ddx(_uv)*0.1,ddy(_uv)*0.1).zw * DTFloat2(1.0,2.0);
    #endif
}                



#endif // YF_UV_UTIL_BASE_UNITY_CG_INCLUDED

