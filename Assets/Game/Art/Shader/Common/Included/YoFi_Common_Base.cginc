//***********************************************************************************
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
// 一些初始的设置
// #include "YFShaderCommonBase.cginc"
//***********************************************************************************#ifndef YF_DATA_UTIL_BASE_UNITY_CG_INCLUDED
#ifndef YF_COMMON_BASE_UNITY_CG_INCLUDED
#define YF_COMMON_BASE_UNITY_CG_INCLUDED
#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc"
#include "YFProjectTeamSettings.cginc"
 
//r:pow(1~32)

uniform sampler2D PowDataDite;  //数据贴图


#ifndef YF_SHADER_WORKTIME
    #ifdef YF_USECUSTOMTIME
        //Shader.SetGlobalDTFloat("_yfTime",yofitime % 100);
        uniform DTFloat _yfTime;
        #define YF_SHADER_WORKTIME _yfTime
    #else
        #define YF_SHADER_WORKTIME _Time.y
    #endif
#endif

//最终输出时候使用这个宏，方便后期新增最后的内容
#define DT_OutFragColor(col) col


#define FXRenderStudio 1


#ifndef UNITY_SPECCUBE_LOD_STEPS
#define UNITY_SPECCUBE_LOD_STEPS (6)
#endif

//伪造灯光
// #define XH_HBWORK_DT
// ------------------------------------------------------------------
//灯光相关Shader parameters
// DTFloat4 _LightColor0;
uniform DTFloat4 _YFGLightDir;
uniform DTFloat4 _YFGLightCol;
uniform DTFloat4 _YFGShadowcol;
uniform DTFloat4 _YFGShadowPlane;

// Computes world space light direction, from world space position
inline DTFloat3 dt_UnityWorldSpaceLightDir( in DTFloat3 worldPos,in DTFloat4 lightdir )
{
    lightdir.y = lightdir.w > 0 ?max(1,lightdir.y):lightdir.y;
    return lightdir.xyz - max( worldPos * lightdir.w ,0.0001f);
}

inline void dt_init_Pseudo_Light(in DTFloat4 WP,inout DTFloat4 _YFGLightDir,inout DTFloat4 _YFGLightCol){    
    #ifdef _PSEUDOLIGHT_ON
        DTFloat c = _YFGLightDir.x + _YFGLightDir.y + _YFGLightDir.z + _YFGLightDir.w;
        if (c == 0 )
        {
            _YFGLightDir.xyz = DTFloat3(0.5,-1,0.0);
        }
        else{
            _YFGLightDir.xyz = normalize(dt_UnityWorldSpaceLightDir(WP.xyz,_YFGLightDir));        
        }
        _YFGLightCol.rgb = _YFGLightCol.rgb;        
    #else
        _YFGLightDir.xyz = normalize(UnityWorldSpaceLightDir(WP.xyz));//_WorldSpaceLightPos0.xyz - o.WP * _WorldSpaceLightPos0.w);
        _YFGLightCol.rgb = _LightColor0.rgb;
    #endif
}



// DTFloat4 _GLightCol;
// fixed _GLightType;
// DTFloat4 _GLightDir;
// DTFloat4 _GShadowcol;

// inline void LightInIt(inout DTFloat4 _GLightCol,inout DTFloat4 _GLightDir){ 
//     //桌面  SHADER_API_DESKTOP ，移动平台 SHADER_API_MOBILE
// #ifdef SHADER_API_DESKTOP
//     _GLightCol = lerp(DTFloat4(1,1,1,1),_GLightCol,step(-0.00001,_GLightCol.r+_GLightCol.g+_GLightCol.b - 0.0001));
//     _GLightDir.xyz = lerp(DTFloat3(0,1,-1),_GLightDir.xyz,step(-0.00001,abs(_GLightDir.x)+abs(_GLightDir.y)+abs(_GLightDir.z) - 0.0001));
//     _GLightDir.w = lerp(1.0,_GLightDir.w,step(-0.00001,_GLightDir.w - 0.0001));
// #else
//     _GLightCol = _GLightCol;
//     _GLightDir = _GLightDir;
// #endif
// }



#endif // YF_COMMON_BASE_UNITY_CG_INCLUDED

