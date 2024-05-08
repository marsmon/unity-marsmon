//***********************************************************************************
// 项目组独立使用，按需要开启或关闭
// #include "YFProjectTeamSettings.cginc"
//***********************************************************************************
#ifndef YF_PROJEC_TTEAM_SETTINGS_CG_INCLUDED
#define YF_PROJEC_TTEAM_SETTINGS_CG_INCLUDED


//***********************************************************************************
// 对应shader的Time变量在游戏运行时间过长后，DTFloat精度错误的宏，开启后，将自定义一个数值由程序传入。

//***********************************使用说明************************************************
//程序从游戏开始就要往这个变量里传递一个0~1的循环值 ,作用是带动所有该shader的动画运行。
//***********************************************************************************
// #define YF_USECUSTOMTIME

// 非编辑模式下，需要关闭这个宏
#define IN_EDITOR_MODE
// 弥补下山的失误
#define ZMXS
// 是否开启自定义的bloom
#define YF_POST_BLOOM
//开启编辑模式
#define DT_EDITORMODE 1

// #define YOFI_CAMERA_ORTHOGRAPHIC
#define FOG_LINEAR

   
// ===============================================自定义 Float 精度========================================================================
//定义使用精度
#define DTFloat float

#define DTFloatN(x,b) x##b
#define DTFloat2 DTFloatN(DTFloat,2)
#define DTFloat3 DTFloatN(DTFloat,3)
#define DTFloat4 DTFloatN(DTFloat,4)

#define DTFloat2x2 DTFloatN(DTFloat,2x2)
#define DTFloat3x3 DTFloatN(DTFloat,3x3)
#define DTFloat3x4 DTFloatN(DTFloat,3x4)
#define DTFloat4x4 DTFloatN(DTFloat,4x4)

#define DTfloat DTFloat
#define DTfloat2 DTFloat2
#define DTfloat3 DTFloat3
#define DTfloat4 DTFloat4

#define DTfloat2x2 DTFloat2x2
#define DTfloat3x3 DTFloat3x3
#define DTfloat3x4 DTFloat3x4
#define DTfloat4x4 DTFloat4x4

#endif // YF_PROJEC_TTEAM_SETTINGS_CG_INCLUDED

