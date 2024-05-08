//***********************************************************************************
// 直接给与特效shader使用的
// #include "YF_ZMXS_3DmodelIncluded.cginc" Assets\shader\YF\YF_ZMXS_3DmodelIncluded.cginc
//***********************************************************************************

#ifndef YF_ZMXS_3D_MODEL_UNITY_CG_INCLUDED
#define YF_ZMXS_3D_MODEL_UNITY_CG_INCLUDED



#include "YFProjectTeamSettings.cginc"
// #define YF_POST_BLOOM
#include "UnityCG.cginc"

#define  DT_CHAR_OUTLine(i) (i.color.r)

#define DT_CHAR_OUTNL(i) (1 - i.color.g)

struct appdata
{
    float4 vertex : POSITION;
    float2 uv0 : TEXCOORD0; 
    // float2 uv1 : TEXCOORD1;
    half3 normal : NORMAL;
    float4 color : COLOR;
};

struct v2f
{
    float4 uv : TEXCOORD0;
    float4 color : COLOR;
    // half4 vertexColor : TEXCOORD1;
    half3 worldNormal : TEXCOORD2;
    float3 worldPos : TEXCOORD3;

    float4 vertex : SV_POSITION;
};

sampler2D _MainTex, _DarkTex;
float4 _MainTex_ST; //,_OutlineColor;
float _Outline;     //_viewAngle,
float4 _CustomLightDir;

fixed _CBGlobalBool;
fixed _CBLocalBanBool;
sampler2D _CBLocalTex;

float4 _FillColor;
float _Fillblend;

inline float4 blendmodel(in float4 finalcol) {
    float4 fxcol = _FillColor;
    
    if (_Fillblend == 1) {
        return float4(fxcol.rgb + finalcol.rgb, finalcol.a);
    }
    else if (_Fillblend == 2)
    {
        return float4(fxcol.rgb * finalcol.rgb, finalcol.a);
    }
    else if (_Fillblend == 3) {
        return  float4(lerp(finalcol.rgb, fxcol.rgb, fxcol.a), finalcol.a);
    }
    else if (_Fillblend == 4) {
        return  float4(finalcol.rgb, fxcol.a * finalcol.a);
    }
    else if (_Fillblend == 5) {
        fixed3 v = dot(finalcol.rgb, unity_ColorSpaceGrey);
        return  float4(v, finalcol.a);
    }
    else {
        return finalcol;
    }
}
// float4x4 getviewAngle4x4(float a)
// {
//     float r = -radians(a % 360.0);
//     float cv = cos(r);
//     float sv = sin(r);
//     // 矩阵其实可以先算好，除非摄像机会随时变化高度
//     float4x4 mat = float4x4(1.0, 0.0, 0.0, 0.0,
//                             0.0, cv, sv, 0.0,
//                             0.0, -sv, cv, 0.0,
//                             0.0, 0.0, 0.0, 1.0);
//     return mat;
// }
float4 GetNewWorldPos(float4 v, float a)
{
    // 这里感觉还有可优化空间。估计会多次改动
    //  //模型顶点坐标转世界空间坐标
    //  float4 worldPos = mul(unity_ObjectToWorld, i.pos);
    //  //世界空间顶点坐标转观察空间坐标
    //  float4 viewPos = mul(UNITY_MATRIX_V, worldPos);
    //  //观察空间坐标转裁剪空间坐标
    //  float4 clipPos = mul(UNITY_MATRIX_P, viewPos);
    if (a == 0)
    {
        return mul(unity_ObjectToWorld, v);
    }
    else
    {
        float r = -radians(a % 360.0);
        float cv = cos(r);
        float sv = sin(r);
        float4x4 CompensationheightMat = float4x4(1.0, 0.0, 0.0, 0.0,
                                                  0.0, 1.0 / cv, 0, 0.0,
                                                  0.0, 0, 1, 0.0,
                                                  0.0, 0.0, 0.0, 1.0);
        float4x4 offsetAnglemat = float4x4(1.0, 0.0, 0.0, 0.0,
                                           0.0, cv, sv, 0.0,
                                           0.0, -sv, cv, 0.0,
                                           0.0, 0.0, 0.0, 1.0);
        float4 pos = mul(offsetAnglemat, mul(unity_ObjectToWorld, float4(0, 0, 0, 1)));
        float4x4 offsetPosmat = float4x4(1.0, 0.0, 0.0, 0.0,
                                         0.0, 1.0, 0.0, -pos.y + unity_ObjectToWorld[1][3],
                                         0.0, 0.0, 1.0, -pos.z + unity_ObjectToWorld[2][3],
                                         0.0, 0.0, 0.0, 1.0);
        float4 pos0 = mul(offsetPosmat, mul(offsetAnglemat, mul(unity_ObjectToWorld , mul(CompensationheightMat, v))));

        return pos0;
    }
}
// float4 ShadowProjectPos(float4 vertPos, float4 _Dir, float4 localsrcv)
// {
//     // _Dir.xyz 是方向, _Dir.w = 阈值
//     // float4 shadowPos = ;

//     // 得到顶点的世界空间坐标
//     float4 worldPos = mul(unity_ObjectToWorld, vertPos);
//     // float4 shadowPos = worldPos;
//     // 灯光方向
//     float3 Dir = normalize(_Dir.xyz);

//     // 阴影的世界空间坐标（低于地面的部分不做改变）
//     worldPos.z = unity_ObjectToWorld[2].w + localsrcv.z;
//     worldPos.xy = worldPos.xy - Dir.xy * max(0, worldPos.z - _Dir.w) / Dir.z;

//     // //阴影的世界空间坐标（低于地面的部分不做改变）
//     // shadowPos.y = min(worldPos .y , _Dir.w);
//     // shadowPos.xz = worldPos .xz - Dir.xz * max(0 , worldPos .y - _Dir.w) / Dir.y;

//     return worldPos;
// }
float _CBLocalPow;

float4 ZMXSBloomOutfrag(float4 _col, fixed4 mask)
{
    fixed4 col = saturate(_col);

#ifndef YF_POST_BLOOM
    return col;
#else
    if (_CBLocalBanBool == 1)
    {
        return col;
    }
    if (_CBGlobalBool == 1)
    {
        fixed4 c = 0;
        // c.a = col.a;
        // float cc = dot(col.rgb, float3(0.2126729f, 0.7151522f, 0.0721750f));
        // c.rgb += col.rgb * cc;
        // c.rgb += mask.rgb * 2.0;
        // return col + c * 2.0;
        c.rgb = mask.rgb * 2.0; //_CBLocalPow
        return  col + c;
    }
    else
    {
        return col;
    }
#endif
}

v2f vertKWL(appdata v)
{
    v2f o = (v2f)0; 
    float _viewAngle = _CustomLightDir.w;
    float4 worldPos = GetNewWorldPos(v.vertex, _viewAngle);
    o.vertex = UnityWorldToClipPos(worldPos);
    o.worldPos = worldPos.xyz;

    // 重新计算深度值
    float4 verticalClipPos = UnityObjectToClipPos(v.vertex);
    o.vertex.z = verticalClipPos.z / verticalClipPos.w * o.vertex.w; 
    o.uv.xy = TRANSFORM_TEX(v.uv0, _MainTex); 
    o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
    o.color = v.color;
    return o;
}

v2f vertOuline(appdata v)
{
    v2f o = (v2f)0;

    float _viewAngle = _CustomLightDir.w;
    float4 worldPos = GetNewWorldPos(v.vertex, _viewAngle);
    // //顶点和法线转到观察空间
    o.vertex = mul(UNITY_MATRIX_V, worldPos);
    float3 normal = normalize(mul((float3x3)UNITY_MATRIX_MV, v.normal));
    // 对z分量进行处理，防止内凹的模型扩张顶点后出现背面面片遮挡正面面片的情况
    normal.z = -0.5;
    // 在观察空间下把模型顶点沿着法线方向向外扩张一段距离
    o.vertex = o.vertex + float4(normal, 0) * _Outline * 0.1 *  DT_CHAR_OUTLine(v);//v.color.r;//描邊粗細
    // 转换到裁剪空间
    o.vertex = mul(UNITY_MATRIX_P, o.vertex);
    // 重新计算深度值
    float4 verticalClipPos = UnityObjectToClipPos(v.vertex);
    o.vertex.z = verticalClipPos.z / verticalClipPos.w * o.vertex.w;
    o.color = v.color;
    return o;
}
fixed4 fragOuline(v2f i) : SV_Target
{
    fixed4 finalColor = fixed4(0, 0, 0, 1);
    return blendmodel(finalColor);
}

#endif // YF_ZMXS_3D_MODEL_UNITY_CG_INCLUDED
