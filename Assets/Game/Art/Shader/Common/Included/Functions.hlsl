#ifndef FUNCTIONS
#define FUNCTIONS
#include "UnityCG.cginc"
/// 灰度
half greyscale(half3 input)
{
    return input.r * 0.299 + input.g * 0.587 + input.b * 0.114;
}

/// 正片叠底
half3 multiply(half3 color0, half3 color1)
{
    return color0 * color1;
}

/// 叠加
half3 overlay(half3 color0, half3 color1)
{
    #define OVERLAY_LESS(v0,v1) 2.0*v0*v1
    #define OVERLAY_GREATER(v0,v1) 1.0-2.0*((1-v0)*(1-v1))
    half flag = 1.0 - step(greyscale(color0), 0.5);
    return lerp(
        half3(OVERLAY_LESS(color0.r,color1.r), OVERLAY_LESS(color0.g,color1.g), OVERLAY_LESS(color0.b,color1.b)),
        half3(OVERLAY_GREATER(color0.r,color1.r), OVERLAY_GREATER(color0.g,color1.g), OVERLAY_GREATER(color0.b,color1.b)),
        flag);
}

/// 滤色
half3 screen(half3 color0, half3 color1)
{
    half3 one3 = half3(1,1,1);
    return one3 - (one3 - color0) * (one3 - color1);
}

/// 强光
half3 hardlight(half3 color0, half3 color1)
{
    #define HARDLIGHT(v0,v1) lerp(2.0*v0*v1, 1.0-2.0*(1-v0)*(1-v1), step(1.0-v1,0.5))
    return half3(HARDLIGHT(color0.r,color1.r), HARDLIGHT(color0.g,color1.g), HARDLIGHT(color0.b,color1.b));
}

/// 颜色减淡
half3 dodge(half3 color0, half3 color1)
{
    return color0 + (color0 * color1) / max(0.0001, (1 - color1));
}


/// 修改色相（Fast）
half3 changeHue(half3 aColor, half aHue)
{
    float angle = radians(aHue);
    float3 k = float3(0.57735, 0.57735, 0.57735);
    float cosAngle = cos(angle);
    //Rodrigues' rotation formula
    return aColor * cosAngle + cross(k, aColor) * sin(angle) + k * dot(k, aColor) * (1 - cosAngle);
}

/// 修改饱和度
half3 changeSaturation(half3 input, half saturation)
{
    float3 intensity = dot(input, half3(0.299, 0.587, 0.114)); // 获取亮度
    return saturate(lerp(intensity, input, saturation));
}

/// 修改HSVC
half3 changeHSBC(half3 origin, half4 hsbc)
{
    float hue = 360 * hsbc.r;
    float saturation = max(0, hsbc.g + 1);
    float brightness = hsbc.b + 1;
    float contrast = max(0, hsbc.a + 1);

    float3 outputColor = origin;
    outputColor.rgb = origin * brightness;
    outputColor.rgb = changeHue(outputColor.rgb, hue);
    outputColor.rgb = (outputColor.rgb - 0.5) * (contrast) + 0.5;
    float3 intensity = dot(outputColor.rgb, float3(0.299,0.587,0.114));
    outputColor.rgb = lerp(intensity, outputColor.rgb, saturation);

    return outputColor;
}
float3 RGBToHSV(float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 HSVToRGB(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
}


//
// https://www.ryanjuckett.com/photoshop-blend-modes-in-hlsl/
//******************************************************************************
//******************************************************************************

float BlendMode_Overlay(float base, float blend)
{
    return (base <= 0.5) ? 2*base*blend : 1 - 2*(1-base)*(1-blend);
}

float3 BlendMode_Overlay(float3 base, float3 blend)
{
    return float3(  BlendMode_Overlay(base.r, blend.r), 
                    BlendMode_Overlay(base.g, blend.g), 
                    BlendMode_Overlay(base.b, blend.b) );
}

float Color_GetLuminosity(float3 c)
{
    return 0.3*c.r + 0.59*c.g + 0.11*c.b;
    //return LinearRgbToLuminance(c);
}

//******************************************************************************
//******************************************************************************
float3 Color_SetLuminosity(float3 c, float lum)
{
    float d = lum - Color_GetLuminosity(c);
    c.rgb += float3(d,d,d);

    // clip back into legal range
    lum = Color_GetLuminosity(c);
    float cMin = min(c.r, min(c.g, c.b));
    float cMax = max(c.r, max(c.g, c.b));

    if(cMin < 0)
        c = lerp(float3(lum,lum,lum), c, lum / (lum - cMin));

    if(cMax > 1)
        c = lerp(float3(lum,lum,lum), c, (1 - lum) / (cMax - lum));

    return c;
}

//******************************************************************************
//******************************************************************************
float Color_GetSaturation(float3 c)
{
    return max(c.r, max(c.g, c.b)) - min(c.r, min(c.g, c.b));
}
//******************************************************************************
// Set saturation if color components are sorted in ascending order.
//******************************************************************************
float3 Color_SetSaturation_MinMidMax(float3 cSorted, float s)
{
    if(cSorted.z > cSorted.x)
    {
        cSorted.y = (((cSorted.y - cSorted.x) * s) / (cSorted.z - cSorted.x));
        cSorted.z = s;
    }
    else
    {
        cSorted.y = 0;
        cSorted.z = 0;
    }

    cSorted.x = 0;

    return cSorted;
}

//******************************************************************************
//******************************************************************************
float3 Color_SetSaturation(float3 c, float s)
{
    if (c.r <= c.g && c.r <= c.b)
    {
        if (c.g <= c.b)
            c.rgb = Color_SetSaturation_MinMidMax(c.rgb, s);
        else
            c.rbg = Color_SetSaturation_MinMidMax(c.rbg, s);
    }
    else if (c.g <= c.r && c.g <= c.b)
    {
        if (c.r <= c.b)
            c.grb = Color_SetSaturation_MinMidMax(c.grb, s);
        else
            c.gbr = Color_SetSaturation_MinMidMax(c.gbr, s);
    }
    else
    {
        if (c.r <= c.g)
            c.brg = Color_SetSaturation_MinMidMax(c.brg, s);
        else
            c.bgr = Color_SetSaturation_MinMidMax(c.bgr, s);
    }
    
    return c;
}
//******************************************************************************
// Creates a color with the hue of the blend color and the saturation and
// luminosity of the base color.
//******************************************************************************
float3 BlendMode_Hue(float3 base, float3 blend)
{
    return Color_SetLuminosity(Color_SetSaturation(blend, Color_GetSaturation(base)), Color_GetLuminosity(base));
}

//******************************************************************************
// Creates a color with the saturation of the blend color and the hue and
// luminosity of the base color. 
//******************************************************************************
float3 BlendMode_Saturation(float3 base, float3 blend)
{
    return Color_SetLuminosity(Color_SetSaturation(base, Color_GetSaturation(blend)), Color_GetLuminosity(base));
}

//******************************************************************************
// Creates a color with the hue and saturation of the blend color and the 
// luminosity of the base color.
//******************************************************************************
float3 BlendMode_Color(float3 base, float3 blend)
{
    return Color_SetLuminosity(blend, Color_GetLuminosity(base));
}


// 混合模式hue
float3 BlendHue( float3 s, float3 d )
{
    s = RGBToHSV(s);
    s.x = RGBToHSV(d).x;
    return HSVToRGB(s);
}
// 混合模式hue
float3 BlendColor( float3 s, float3 d )
{
    s = RGBToHSV(s);
    s.xy = RGBToHSV(d).xy;
    return HSVToRGB(s);
}


#endif