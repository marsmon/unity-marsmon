//***********************************************************************************
// 特效中各种公告板的方法
// #include "YFShaderBillborad.cginc"
//***********************************************************************************
#ifndef YF_BILLBORAD_UNITY_CG_INCLUDED
#define YF_BILLBORAD_UNITY_CG_INCLUDED
#include "UnityCG.cginc"
#include "YoFi_Common_Base.cginc"


//=============================================================公告板=矩阵=================================================================
// UnityObjectToClipPos //unity_ObjectToWorld    //unity_WorldToObject
//使用对象与世界坐标系对齐 世界坐标是xyz,y是面片的法线方向
//     ＿＿＿z＿＿＿＿  默认面片的方向和结构 ＿＿＿y＿＿＿ 
//    ╱     ↑       ╱                       |           |
//   ╱      ● -→ x ╱                        |           x
//  ╱    ↙       ╱                         |           |
// ╱  y          ╱                          |           |
// ￣￣￣￣￣￣￣￣                         ￣￣￣￣￣￣￣

inline DTFloat4x4 dtWorldScaleMat(){
    DTFloat4x4 mat = unity_ObjectToWorld;

    DTFloat mx = length(DTFloat3(mat[0].x,mat[1].x,mat[2].x));
    DTFloat my = length(DTFloat3(mat[0].y,mat[1].y,mat[2].y));
    DTFloat mz = length(DTFloat3(mat[0].z,mat[1].z,mat[2].z));

    mat[0].xyz = DTFloat3(mx,0,0);
    mat[1].xyz = DTFloat3(0,my,0);
    mat[2].xyz = DTFloat3(0,0,mz);
    return mat;
}

//公告版 
inline DTFloat4 DTCambillborad(DTFloat4 _vertex)
{
    DTFloat4 _center = DTFloat4(0, 0, 0, 1);
    //转换矩阵，只保留缩放和位移
    DTFloat4x4 mat = dtWorldScaleMat();
    //==========================================================
    //垂直地面并朝向摄像机的方向
    #if defined(_DTBILLBORAD_MAP0) 
        DTFloat3x3 upmat;
        // DTFloat3 c = UnityObjectToViewPos(_center);
        DTFloat3 v = DTFloat3(0, 0,-1);
        DTFloat3 nor = mul(UNITY_MATRIX_V, DTFloat3(0,1,0)) ;

        upmat[0] = normalize(cross(nor,v)); //x
        upmat[1] = nor; //y
        upmat[2] = v; //z

        DTFloat3 pos = mul(upmat, _vertex.xyz );
        
        pos = c + mul(pos,(DTFloat3x3)mat);
        return mul(UNITY_MATRIX_P, DTFloat4(pos,_vertex.w));
    //==========================================================
    //垂直地面并永远朝向摄像机， 面片要求 X:侧面 Y:向上 Z:向正面(法线)
    #elif defined(_DTBILLBORAD_MAP1)
        DTFloat3x3 upmat;
        DTFloat3 worldpos = DTFloat3(unity_ObjectToWorld[0].w,unity_ObjectToWorld[1].w,unity_ObjectToWorld[2].w);
        DTFloat3 v =  normalize(_WorldSpaceCameraPos -  worldpos);
        DTFloat3 nor = DTFloat3(0,1,0);
        upmat[0] = normalize(cross(nor,v)); //x
        upmat[1] = normalize(nor); //y
        upmat[2] = normalize(cross(nor,upmat[0])); //z
        DTFloat3 pos = mul(upmat, _vertex.xyz );
        mat[2].z = mat[0].x;
        pos = worldpos + mul(pos,(DTFloat3x3)mat); 
        return mul(UNITY_MATRIX_VP, DTFloat4(pos,_vertex.w));    
    //==========================================================
    //正对摄像机 DTCamNoRotBillborad
    #elif defined(_DTBILLBORAD_VC)
        DTFloat3 c = UnityObjectToViewPos(_center);
        DTFloat3 MVpos = c - mul((DTFloat3x3)mat,_vertex.xzy);
        return mul(UNITY_MATRIX_P, DTFloat4(MVpos,_vertex.w));

    //==========================================================
    //特效弹道用的公告板 _DTBILLBORAD_FX
    #elif defined(_DTBILLBORAD_FX)  
        DTFloat3 viewer = mul(unity_WorldToObject, DTFloat4(_WorldSpaceCameraPos, 1));
        DTFloat3 lpos = mul(_vertex.xyz,(DTFloat3x3)unity_ObjectToWorld);
        DTFloat3 v = normalize( viewer.xyz - _center );
        v.z = 0;
        DTFloat3x3 upmat;
        upmat[0] = normalize(cross(DTFloat3(0,0,1),v)); //x
        upmat[1] =  v; //y
        upmat[2] = normalize(cross(upmat[0],v)); //z
        DTFloat3 pos = mul(upmat, _vertex );
        mat[1].y = mat[0].x;
        return UnityObjectToClipPos(DTFloat4(mul(pos,(DTFloat3x3)unity_ObjectToWorld),_vertex.w));

    //==========================================================

    #else
        return UnityObjectToClipPos(_vertex);
    #endif    
}

#endif // YF_BILLBORAD_UNITY_CG_INCLUDED

