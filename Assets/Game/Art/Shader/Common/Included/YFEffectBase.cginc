
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
// YoFiUnityCG
#ifndef YF_EFFECT_BASE_DITE_INCLUDED
#define YF_EFFECT_BASE_DITE_INCLUDED
#include "UnityCG.cginc"


//星火include
// #include "BattleSkillUnityCG.cginc"
#include "YFBaseData.cginc"

#ifdef FOG_ON
#define FOG_LINEAR
#endif

#include "YFEffectCGForYFPost.cginc" 

#ifndef EnableComputeSkillMode
#define EnableComputeSkillMode 1
#endif 
// 设置时间 可改其他变量来控制时间，关闭是1执行：YF_SHADER_WORKTIME * speed
#ifdef _TIMEDT_ON
// #define DT_AUTOTIME(speed,isauto) (YF_SHADER_WORKTIME * ##isauto + 1.0 - ##isauto ) * speed
#define DT_AUTOTIME(speed,isauto) (YF_SHADER_WORKTIME * ##isauto + 1.0 - ##isauto ) * (floor(speed * 100) * 0.01)
#else
#define DT_AUTOTIME(speed,isauto) speed
#endif

#ifdef XH_HBWORK_DT
#undef DT_OutFragColor
    //星火专用怒气通道 EnableComputeSkillMode怒气宏
#define DT_OutFragColor(col) col * EnableComputeSkillMode 
#else	
#define DT_OutFragColor(col) col
#endif

#ifdef _ALPHATEST_ON
#define DT_ALPHATESTCLIP(a) clip(a - 0.5);
#else
#define DT_ALPHATESTCLIP(a)
#endif  




// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// public Vector4 GetWorldCorners(GameObject go)
// {
//     RectTransform rectTransform = go.GetComponent<RectTransform>();
//     Vector3[] corners = new Vector3[4];
//     rectTransform.GetWorldCorners(corners);
//     DTFloat minX = corners[0].x;
//     DTFloat minY = corners[0].y;
//     DTFloat maxX = corners[2].x;
//     DTFloat maxY = corners[2].y;
//     return new Vector4(minX, minY, maxX, maxY);
// }
// DTFloat4 _ScrollViewRect; //UI裁切用的的属性。输入世界坐标。必须在正交摄像机下

// ************************************************************************************************************************************
// ************************************************************************************************************************************

sampler2D _MainTex;
sampler2D _Effect0Tex;
sampler2D _Effect1Tex;
DTFloat4 _MainTex_ST;
DTFloat4 _Effect0Tex_ST;
DTFloat4 _Effect1Tex_ST;


UNITY_INSTANCING_BUFFER_START( DTProps )
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _MainColor)
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _MainValue)
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _MainAnimData)
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _MainBase)

    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect0Color)
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect0Value)
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect0AnimData)
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect0Base)

    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect1Color)
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect1Value)
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect1AnimData)
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect1Base)

    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _FresnelColor)
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _FresnelValue)
    UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _FresnelBase)

    // UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect2Color)
    // UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect2Value)
    // UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect2AnimData)
    // UNITY_DEFINE_INSTANCED_PROP( DTFloat4, _Effect2Base)

UNITY_INSTANCING_BUFFER_END( DTProps )

// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
//常用属性
// FXCommonlyTexBaseON
struct FXCommonlyTexBase
{
        //[0] //颜色    
        //[1] //x:u/rot,y:v,z:frame,w: eeee,ffff,dddd,cbaa e:tilesx f:tilesy d:FrameVMode c:isauto b:isframe a:(move/rot/size)
        //[2] //效果需要的第1输入值 v0,v1,v2,v3
        //[3] //x:   y:   z: bbbb,bbbb,aaaa,aaaa a:F_Max b:F_Min  w: _dcb,aaaa d:_IsDist,c:_Flip,b:_Clip,a:_Blend
	DTFloat4 main_Color;
	DTFloat4x4 main_Value;
	DTFloat4 fx0_Color;
	DTFloat4x4 fx0_Value; //效果需要的第1输入值 v0,v1,v2,v3

#ifdef DTMultichannel_I        
	DTFloat4 fx1_Color;
    DTFloat4x4 fx1_Value;     /*预留效果值*/
#endif

        //[0] 颜色  
        //[1] x:法线计算后输入的方向，默认0，y:v0 z:v1 w:效果的强度
        //[2] IsDist,_Flip,_Clip,_Blend
	DTFloat3x4 fnl_Value;
};

    // vColor (定点色) uvbase (基础UV) uv0 uv1 
    // vcv(TEXCOORD0.zw,TEXCOORD1.xy) 粒子里custom里会用到
	//  v2f_DT i
// #define DT_SETVertexDATA()                   \
//     DTFloat4 vColor = i.color ;              \
//     DTFloat2 uvbase = i.basedata.xy;         \
//     DTFloat2 uv0 = i.uv.xy;                  \
//     DTFloat2 uv1 = i.uv.zw;                  \
//     DTFloat4 worldpos = i.worldPos;          \
//     DTFloat4 vcv = DTFloat4(i.basedata.zw,i.fogCoord.zw)

	

    //##_c ##_a ##_v ##_b 
#define DT_SETTexDATA_TT(name0) FXFrameData name0##FD = getFXFrameDataData(dt.name0##_Value)

    //##_c ##_v ##_b
#define DT_SETFnlDATA(name0)                   \
    DTFloat4 name0##_c = dt.name0##_Value[0];  \
    DTFloat4 name0##_v = dt.name0##_Value[1];  \
    DTFloat4 name0##_b = dt.name0##_Value[2]

#define DT_SETTexDATA(name0)                   \
    DTFloat4 name0##_c = dt.name0##_Value[0];  \
    DTFloat4 name0##_a = dt.name0##_Value[1];  \
    DTFloat4 name0##_v = dt.name0##_Value[2];  \
    DTFloat4 name0##_b = dt.name0##_Value[3]


struct FXFrameData
{
	DTFloat4 color;
	DTFloat4 value;
	DTFloat2 value2; //x:Tex亮度值,非特效值 y:空缺

	int animtype;
	int isAuto;
	int isframe;

	DTFloat2 uv;
	DTFloat2 size;
	DTFloat rot;

        
	DTFloat Ftime; //x:
	uint Ftimemode; //y: 0 Frame 1:auto(frame rate) 2 particle cutom.x/y
	uint FTilesx; //y: MAX15
	uint FTilesy; //y: MAX15
	uint FRangeMin; //z: MAX255
	uint FRangeMax; //z: MAX255
	uint isPolar; //极坐标

	int isValueType;
	int isDist;
	int isFilp;
	int isClip;
	int blend;
};



inline FXFrameData getFXFrameDataData(in DTFloat4x4 v)
{
	FXFrameData o = (FXFrameData) 0;
        //[0] color [1] animdata [2] value [3] base
	o.color = v[0];
	o.value = v[2];

	o.animtype = (uint) v[1].w >> 0 & 0x3;
	o.isAuto = (uint) v[1].w >> 2 & 0x1;
	o.isframe = (uint) v[1].w >> 3 & 0x1;
	o.isPolar = (uint) v[1].w >> 4 & 0x1;

	o.uv = v[1].xy;
	o.size = v[1].xy;
	o.rot = v[1].x;
	o.Ftime = v[1].z;

	o.Ftimemode = (uint) v[1].w >> 5 & 0x7;
	o.FTilesx = (uint) v[1].w >> 8 & 0xf;
	o.FTilesy = (uint) v[1].w >> 12 & 0xf;
	o.FRangeMin = (uint) v[3].z >> 8 & 0xff;
	o.FRangeMax = (uint) v[3].z >> 0 & 0xff;
        

	o.isValueType = (uint) v[3].w >> 7 & 0xf; // 是否启用顶点属性 15 0b1111
	o.isDist = (uint) v[3].w >> 6 & 0x1; // 是否扰动 1 0b1 
	o.isFilp = (uint) v[3].w >> 5 & 0x1; // 是否反相 1 0b1
	o.isClip = (uint) v[3].w >> 4 & 0x1; // 是否clip 1 0b1
	o.blend = (uint) v[3].w >> 0 & 0xf; //混合模式  15 0b1111    

	o.value2 = v[3].xy;
	return o;
}

// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
//EffectBaseFunON
inline DTFloat2 MoveUV(DTFloat2 _uv, FXFrameData data)
{
	data.uv = DT_AUTOTIME(data.uv, data.isAuto);
	data.uv  =  data.uv * -1;
	if (data.uv.x == 0 && data.uv.y == 0)
	{
		return _uv;
	}
	return dt_moveUV(_uv, data.uv.xy);
}

    
inline DTFloat2 RotUV(DTFloat2 _uv, FXFrameData data) //特殊优化，实际运行时候用贴图代替计算
{
	data.rot = DT_AUTOTIME(data.rot, data.isAuto) * -1;
	DTFloat2 uv = dt_rotateUV(frac(_uv), data.rot);
	return saturate(uv);
}

inline DTFloat2 ScaUV(DTFloat2 _uv, FXFrameData data)
{
        // DTFloat2 uv = dt_scaleUV(frac(_uv),data.size);  
	DTFloat2 uv = dt_scaleUV(_uv, data.size);
	return uv;
}

inline DTFloat2 FrameUV(DTFloat2 _uv, FXFrameData data)
{
	if (data.isframe == 0)
	{
		return _uv;
	}
	int startv = data.FRangeMin;
	int count = data.FRangeMax - data.FRangeMin - 1;
	DTFloat _Speed;
	if (data.Ftimemode == 0)
	{
		data.Ftime = YF_SHADER_WORKTIME * data.Ftime;
		_Speed = data.Ftime % count + startv;
	}
	else
	{
		_Speed = data.Ftime * count + startv;
	}
	DTFloat2 _SizeT = DTFloat2(max(data.FTilesx, 1), max(data.FTilesy, 1));
	DTFloat sourceX = 1.0 / _SizeT.x;
	DTFloat sourceY = 1.0 / _SizeT.y;

	DTFloat col = floor(_Speed / _SizeT.x);
	DTFloat row = floor(_Speed - col * _SizeT.x);
	return (_uv + DTFloat2(row, -col)) * DTFloat2(sourceX, sourceY) + DTFloat2(0, -sourceY);
}

    // inline DTFloat2 DTPolar(DTFloat2 UV)
    // {
    //     #if DT_EDITORMODE
    //         return _DTPolar0(UV);
    //     #else
    //         return _DTPolar1(UV);
    //     #endif  
    // }

inline DTFloat4 GetTexData(sampler2D tex, DTFloat2 uv, FXFrameData data)
{
	DTFloat4 fragcolor;
	DTFloat2 _uv = uv;
	if (data.isPolar == 1)
	{
		_uv = dt_Polar(uv);
	}
	if (data.animtype == 0)
	{
		_uv = _uv;
	}
	else if (data.animtype == 1)
	{
		_uv = MoveUV(_uv, data);
	}
	else if (data.animtype == 2)
	{
		_uv = RotUV(_uv, data);
	}
	else
	{
		_uv = ScaUV(_uv, data);
	}


	if (data.isframe > 0)
	{
		return tex2D(tex, FrameUV(frac(_uv), data), ddx(uv) * 0.1, ddy(uv) * 0.1);
	}
	else
	{
		return tex2D(tex, _uv);
	}
}
    
    
    // ************************************************************************************************************************************
    // ************************************************************************************************************************************
    // ************************************************************************************************************************************
    // ************************************会用到的一些方法*********************************************************************************
    // ************************************************************************************************************************************
    // ************************************************************************************************************************************
    // ************************************************************************************************************************************
    // ************************************************************************************************************************************

    // inline DTFloat4 UnityDiteMaskWork(DTFloat4 srcCol,DTFloat _mask, DTFloat alpblend){}
    //dissolution 溶解
    //srcCol 输入的源色
    //_mask x:v0,y:v1,z:pow,w:light
    //_color 溢出的颜色
inline DTFloat4 UnityDiteDissolutionWork(DTFloat4 srcCol, DTFloat _mask, DTFloat4 _maskValue, DTFloat4 _color, DTFloat _flip, DTFloat _clip, DTFloat _blendmode)
{
	DTFloat mask = dt_Flip(_mask, _flip);
	DTFloat2 m = dt_ClipMode(dt_DissNormal(mask, _maskValue.x, _maskValue.y), _clip);
	DTFloat3 c;
	DTFloat a = _color.a * step(0.0000001, _maskValue.y);
	if (_blendmode == 1)
	{ //BLEND 
		c = lerp(srcCol.rgb, _color.rgb * 2.0, a);
	}
	else if (_blendmode == 2)
	{ //MULTIPLY
		c = srcCol.rgb * (_color.rgb * 2.0 * a + abs(1.0 - a));
	}
	else
	{ //Add
		c = srcCol.rgb + _color.rgb * 2.0 * a;
	}
	DTFloat4 e;
	e.rgb = lerp(saturate(c) * _maskValue.w, srcCol.rgb, m.x) * m.y;
	e.a = m.y * srcCol.a;
        // e.rgb = m.y;
        // e.a = 1;
	return e;
        // return _UnityDiteDissolutionWork(srcCol,_mask,_maskValue,_color,_flip,_clip,_blendmode);
}
inline DTFloat4 UnityDiteColorMaskWork(DTFloat4 _srcCol, DTFloat4 _mask, DTFloat vx, int _isclip, int _isblend)
{
	DTFloat4 mask = _mask;
	mask.a = lerp(_mask.a, dt_getGray(_mask), _isclip);
	fixed4 col = 0;
	float blend = (uint) _isblend >> 0 & 0x7; //混合模式  15 0b1111    dddddd 
	float maska = (uint) _isblend >> 3 & 0x1;

	if (blend == 1)
	{
		col = lerp(_srcCol, mask, vx);
	}
	else if (blend == 2)
	{
		col =  _srcCol * mask;
	}
	else if (blend == 3)
	{
		DTFloat4 c = _srcCol;
		c.a *= mask.a;
		col =  c;
	}
	else
	{
		col =  saturate(_srcCol + mask);
	}
	col.a = lerp(col.a,mask.a,maska);
	return col;
}


inline DTFloat4 UnityDiteFNRwork(DTFloat4 srcCol, DTFloat4 fc, DTFloat4 fv, DTFloat4 fb, DTFloat2 dissdata, DTFloat2 pow)
{
	DTFloat nv = saturate(fv.x);
	DTFloat2 _FNLv = fv.yz * 2;
	DTFloat4 _color = fc;
	DTFloat _intensity = fv.w;

	int isapl = fb.z;
	int blend = fb.w;
	if (fb.x > 0)
	{
		nv = dt_DistortUV(nv, dissdata, pow);
	}

	DTFloat4 e;
	DTFloat fnlv = dt_FNRwork(nv, _FNLv.x, _FNLv.y);
	DTFloat3 c;
	if (isapl == 3)
	{
		srcCol.a = fnlv;
		return srcCol;
	}

	if (blend == 1)
	{ //BLEND 
		c = lerp(srcCol.rgb, _color.rgb * _color.a * _intensity, fnlv);
	}
	else if (blend == 2)
	{ //MULTIPLY
		c = srcCol.rgb * lerp(1.0, (_color.rgb * _color.a * _intensity), fnlv);
	}
	else
	{ //Add
		c = srcCol.rgb + _color.rgb * _color.a * _intensity * fnlv;
	}
	e.rgb = c;
	if (isapl == 0)
	{
		e.a = saturate(srcCol.a + fnlv);
		return e;
	}
	else if (isapl == 1)
	{
		e.a = srcCol.a;
		return e;
	}
	else
	{
		e.a = fnlv;
		return e;
	}
}
inline DTFloat2 UnityDiteDistortUVWork(DTFloat2 _uv, DTFloat v, DTFloat2 pow)
{
	return dt_DistortUV(_uv, v, pow);
}
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
struct appdata_t_DT
{
	DTFloat4 vertex : POSITION;
	DTFloat4 color : COLOR;
	DTFloat4 texcoords0 : TEXCOORD0;
	DTFloat4 texcoords1 : TEXCOORD1;
	DTFloat4 texcoords2 : TEXCOORD2;
	DTFloat3 normal : NORMAL;
    // DTFloat4 tangent : TANGENT;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f_DT
{
	DTFloat4 vertex : SV_POSITION;
	DTFloat4 color : COLOR;
	DTFloat4 basedata : TEXCOORD0;
	DTFloat4 uv : TEXCOORD1;
	DTFloat4 fogCoord : TEXCOORD2;
	DTFloat4 worldPos : TEXCOORD3;
	DTFloat4 pCutsomCoord2 : TEXCOORD4;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

#ifndef DT_surf
#define DT_surf dtsurf
    //方便调整最后输出使用
inline void dtsurf(v2f_DT vd, inout FXCommonlyTexBase s)
{
	s.main_Value[0] = UNITY_ACCESS_INSTANCED_PROP(DTProps, _MainColor);
	s.main_Value[1] = UNITY_ACCESS_INSTANCED_PROP(DTProps, _MainAnimData); //DTFloat4(_SpeedU,_SpeedV,1,1);
	s.main_Value[2] = UNITY_ACCESS_INSTANCED_PROP(DTProps, _MainValue); //DTFloat4(_Level,0,0,0);        
	s.main_Value[3] = UNITY_ACCESS_INSTANCED_PROP(DTProps, _MainBase); //DTFloat4(_SpeedU,_SpeedV,1,1);
        
	s.fx0_Value[0] = UNITY_ACCESS_INSTANCED_PROP(DTProps, _Effect0Color);
	s.fx0_Value[2] = UNITY_ACCESS_INSTANCED_PROP(DTProps, _Effect0Value); //DTFloat4(_Level,0,0,0);
	s.fx0_Value[1] = UNITY_ACCESS_INSTANCED_PROP(DTProps, _Effect0AnimData); //DTFloat4(_SpeedU,_SpeedV,1,1);
	s.fx0_Value[3] = UNITY_ACCESS_INSTANCED_PROP(DTProps, _Effect0Base); //DTFloat4(_SpeedU,_SpeedV,1,1);
#ifdef DTMultichannel_I   
	s.fx1_Value[0] = UNITY_ACCESS_INSTANCED_PROP( DTProps,_Effect1Color);
	s.fx1_Value[2] = UNITY_ACCESS_INSTANCED_PROP( DTProps,_Effect1Value);//DTFloat4(_Level,0,0,0);
	s.fx1_Value[1] = UNITY_ACCESS_INSTANCED_PROP( DTProps,_Effect1AnimData);//DTFloat4(_SpeedU,_SpeedV,1,1);
	s.fx1_Value[3] = UNITY_ACCESS_INSTANCED_PROP( DTProps,_Effect1Base);//DTFloat4(_SpeedU,_SpeedV,1,1);
#endif
	// s.fx2_Value[0] = UNITY_ACCESS_INSTANCED_PROP( DTProps,_Effect2Value1);
	// s.fx2_Value[2] = UNITY_ACCESS_INSTANCED_PROP( DTProps,_Effect2Value);//DTFloat4(_Level,0,0,0);
	// s.fx2_Value[1] = UNITY_ACCESS_INSTANCED_PROP( DTProps,_Effect2AnimData);//DTFloat4(_SpeedU,_SpeedV,1,1);
	// s.fx2_Value[3] = UNITY_ACCESS_INSTANCED_PROP( DTProps,_Effect2Base);//DTFloat4(_SpeedU,_SpeedV,1,1);


	s.fnl_Value[0] = UNITY_ACCESS_INSTANCED_PROP(DTProps, _FresnelColor);
	s.fnl_Value[1] = UNITY_ACCESS_INSTANCED_PROP(DTProps, _FresnelValue); //DTFloat4(vd.fogCoord.y,_FresnelEXP0,_FresnelEXP1,_FresnelBright);
	s.fnl_Value[1].x = vd.fogCoord.y;
	s.fnl_Value[2] = UNITY_ACCESS_INSTANCED_PROP(DTProps, _FresnelBase); //DTFloat4(0,0,_FnlIsclip,_FnlBlend);
}
#endif
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************

//主要结构体
v2f_DT vertBase(appdata_t_DT v)
{
	v2f_DT o;
	UNITY_INITIALIZE_OUTPUT(v2f_DT, o);
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
	// o.vertex = DTCambillborad(v.vertex);
	o.vertex = UnityObjectToClipPos(v.vertex);
    
	o.color = v.color;
	o.basedata = v.texcoords0;

	o.uv.xy = TRANSFORM_TEX(v.texcoords0.xy, _MainTex);
	o.uv.zw = TRANSFORM_TEX(v.texcoords0.xy, _Effect0Tex);

	UNITY_TRANSFER_FOG(o, o.vertex); //o.fogCoord.x
	o.fogCoord.y = abs(dot(UnityObjectToWorldNormal(v.normal), dt_WorldViewDir(v.vertex)));
	o.fogCoord.zw = v.texcoords1.xy;
    // o.pCutsomCoord2 = DTFloat4(v.texcoords1.zw,v.texcoords2.xy);
    // 
	o.worldPos = mul(unity_ObjectToWorld, v.vertex);
	return o;
}
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// DTFloat4 vColor = i##.color ;         \
//     DTFloat2 uvbase = i##.basedata.xy;         \
//     DTFloat2 uv0 = i##.uv.xy;                  \
//     DTFloat2 uv1 = i##.uv.zw;                  \
//     DTFloat4 worldpos = i##.worldPos;          \
//     DTFloat4 vcv = DTFloat4(i##.basedata.zw,i##.fogCoord.zw)




#ifndef FXRenderStudio
#define FXRenderStudio 0
#endif
DTFloat _ImageExporterRenderShaderADDMASK;
DTFloat _ImageExporterRenderShaderBlendMASK;
uint _DstBlend;

inline void getBlendforRender(inout DTFloat4 fragcolor)
{
#if FXRenderStudio
	if (_DstBlend > 1)
	{
		fragcolor *= (1 - _ImageExporterRenderShaderBlendMASK);
	}
	else
	{
		fragcolor *= (1 - _ImageExporterRenderShaderADDMASK);
	}
#else
    fragcolor = fragcolor;
#endif
}

inline DTFloat4 LastExportColor(DTFloat4 fragcolor, v2f_DT i)
{
	fragcolor = saturate(fragcolor);
    DT_ALPHATESTCLIP(fragcolor.a) 
	fragcolor *= DT_UIscrollviewClip(i.worldPos);
	getBlendforRender(fragcolor);   
	fragcolor *= dt_ClipInWorldY(i.worldPos.y);
	return BloomOutfrag(fragcolor);
} 

// inline DTFloat4 LastExportColor(DTFloat4 fragcolor, FXCommonlyTexBase dt, v2f_DT i)
// {
// 	fragcolor = saturate(fragcolor);
//     DT_ALPHATESTCLIP(fragcolor.a) 
// 	fragcolor *= DT_UIscrollviewClip(i.worldPos);
// 	getBlendforRender(fragcolor); 
// 	fragcolor *= dt_ClipInWorldY(i.worldPos.y);
// 	return BloomOutfrag(saturate(fragcolor));
// }



// Debug.Log(nowmode >> 12 & 0xF);



fixed _IsNewColorMask;
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// 普通
DTFloat4 fragNone(v2f_DT i) : SV_Target
{
	UNITY_SETUP_INSTANCE_ID(i);
	DTFloat4 fragcolor = 0;
	FXCommonlyTexBase dt;
	UNITY_INITIALIZE_OUTPUT(FXCommonlyTexBase, dt);
    DT_surf(i, dt);   
    // DT_SETVertexDATA(); // vColor VCustomValue uvbase uv0 uv1
	DTFloat4 vColor = i.color ;              
    DTFloat2 uvbase = i.basedata.xy;         
    DTFloat2 uv0 = i.uv.xy;                  
    DTFloat2 uv1 = i.uv.zw;                  
    DTFloat4 worldpos = i.worldPos;          
    DTFloat4 vcv = DTFloat4(i.basedata.zw,i.fogCoord.zw);

    DT_SETTexDATA_TT(main); //##_c ##_a ##_v ##_b
       
	if (mainFD.Ftimemode == 2)
	{
        // o.Ftime = v[1].z; 
        // o.Ftimemode =  (uint)v[1].w >> 4 & 0xf;
		mainFD.Ftime = vcv.x * 1.1;
	}
    
	fragcolor = GetTexData(_MainTex, uv0, mainFD); // tex2D(_MainTex,uv0);
	fragcolor.rgb *= vColor.rgb * mainFD.color.rgb * 2.0 * mainFD.value2.x/*亮度*/;
#ifdef ZMXS
	//全是为过去莫名其妙的买单
	if(_IsNewColorMask < 1 ) {
		fragcolor.rgb *= vColor.rgb;
	}
	//全是为过去莫名其妙的买单
#endif
	fragcolor.a = saturate(fragcolor.a * vColor.a * mainFD.color.a);
    // fragcolor.xyz = DTFloat3(uv0.xy,0);
#ifdef _FRESNEL_ON
        DT_SETFnlDATA(fnl);//##_c ##_v ##_b
		// DTFloat4 fnl_c = dt.fnl_Value[0];
    	// DTFloat4 fnl_v = dt.fnl_Value[1];
    	// DTFloat4 fnl_b = dt.fnl_Value[2];
        fragcolor = UnityDiteFNRwork(fragcolor,fnl_c,fnl_v,fnl_b,0,0);
#endif
    
	fragcolor = LastExportColor(fragcolor,  i);
    // fragcolor.a = saturate(fragcolor.a);
	UNITY_APPLY_FOG(i.fogCoord, col);
	return DT_OutFragColor(fragcolor);
}
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// 扰动
DTFloat4 fragDistort(v2f_DT i) : SV_Target
{
	UNITY_SETUP_INSTANCE_ID(i);
	DTFloat4 fragcolor;
	FXCommonlyTexBase dt;
	UNITY_INITIALIZE_OUTPUT(FXCommonlyTexBase, dt);
    DT_surf(i, dt);
    // DT_SETVertexDATA(); // vColor VCustomValue uvbase uv0 uv1
	DTFloat4 vColor = i.color ;              
    DTFloat2 uvbase = i.basedata.xy;         
    DTFloat2 uv0 = i.uv.xy;                  
    DTFloat2 uv1 = i.uv.zw;                  
    DTFloat4 worldpos = i.worldPos;          
    DTFloat4 vcv = DTFloat4(i.basedata.zw,i.fogCoord.zw);

    DT_SETTexDATA_TT(main); //##_c ##_a ##_v ##_b
    DT_SETTexDATA_TT(fx0);
	if (mainFD.Ftimemode == 2)
	{
        // o.Ftime = v[1].z; 
        // o.Ftimemode =  (uint)v[1].w >> 4 & 0xf;
		mainFD.Ftime = vcv.x * 1.1;
	}
	if (fx0FD.Ftimemode == 2)
	{
        // o.Ftime = v[1].z; 
        // o.Ftimemode =  (uint)v[1].w >> 4 & 0xf;
		fx0FD.Ftime = vcv.y * 1.1;
	}
    
	DTFloat4 tex1 = GetTexData(_Effect0Tex, uv1, fx0FD) * fx0FD.value2.x/*亮度*/;
	DTFloat2 Mdistuv = UnityDiteDistortUVWork(uv0, tex1.r, fx0FD.value.xy);
	fragcolor = GetTexData(_MainTex, Mdistuv, mainFD); //tex2D(_MainTex,Mdistuv);
	fragcolor.rgb *= mainFD.color.rgb * 2.0 * mainFD.value2.x * vColor.rgb/*亮度*/;
	fragcolor.a = saturate(fragcolor.a * vColor.a * mainFD.color.a);
#ifdef _FRESNEL_ON
        DT_SETFnlDATA(fnl);//##_c ##_v ##_b
		// DTFloat4 fnl_c = dt.fnl_Value[0];
    	// DTFloat4 fnl_v = dt.fnl_Value[1];
    	// DTFloat4 fnl_b = dt.fnl_Value[2];
        fragcolor = UnityDiteFNRwork(fragcolor,fnl_c,fnl_v,fnl_b,tex1.r,fx0FD.value.xy);
#endif
	fragcolor = LastExportColor(fragcolor,  i);

	UNITY_APPLY_FOG(i.fogCoord, col);
	return DT_OutFragColor(fragcolor);
}
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// 遮罩
DTFloat4 fragColMask(v2f_DT i) : SV_Target
{
	UNITY_SETUP_INSTANCE_ID(i);
	DTFloat4 fragcolor;
	FXCommonlyTexBase dt;
	UNITY_INITIALIZE_OUTPUT(FXCommonlyTexBase, dt);
    DT_surf(i, dt);

    // DT_SETVertexDATA(); // vColor VCustomValue uvbase uv0 uv1	
	DTFloat4 vColor = i.color ;              
    DTFloat2 uvbase = i.basedata.xy;         
    DTFloat2 uv0 = i.uv.xy;                  
    DTFloat2 uv1 = i.uv.zw;                  
    DTFloat4 worldpos = i.worldPos;          
    DTFloat4 vcv = DTFloat4(i.basedata.zw,i.fogCoord.zw);

    DT_SETTexDATA_TT(main); //##_c ##_a ##_v ##_b
    DT_SETTexDATA_TT(fx0);
	if (mainFD.Ftimemode == 2)
	{
        // o.Ftime = v[1].z; 
        // o.Ftimemode =  (uint)v[1].w >> 4 & 0xf;
		mainFD.Ftime = vcv.x * 1.1;
	}
	if (fx0FD.Ftimemode == 2)
	{
        // o.Ftime = v[1].z; 
        // o.Ftimemode =  (uint)v[1].w >> 4 & 0xf;
		fx0FD.Ftime = vcv.y * 1.1;
	}

	fragcolor = GetTexData(_MainTex, uv0, mainFD);
	fragcolor.rgb *= mainFD.color.rgb * 2.0 *  mainFD.value2.x/*亮度*/;
	fragcolor.a = saturate(fragcolor.a * mainFD.color.a);
    
	DTFloat4 tex1 = GetTexData(_Effect0Tex, uv1, fx0FD) * fx0FD.color * fx0FD.value2.x/*亮度*/;
#ifdef ZMXS
	//全是为过去莫名其妙的买单
	if(_IsNewColorMask < 1){
		tex1 *= 0.5;
	}
	//全是为过去莫名其妙的买单
#endif
    // inline DTFloat4 UnityDiteColorMaskWork(DTFloat4 _srcCol,DTFloat4 _mask,DTFloat vx,int _isclip,int _isblend)
	fragcolor = UnityDiteColorMaskWork(fragcolor, tex1, fx0FD.value.x, fx0FD.isClip, fx0FD.blend);
	fragcolor *= vColor;
    // fragcolor = saturate(fragcolor);

#ifdef _FRESNEL_ON
        DT_SETFnlDATA(fnl);//##_c ##_v ##_b
		// DTFloat4 fnl_c = dt.fnl_Value[0];
    	// DTFloat4 fnl_v = dt.fnl_Value[1];
    	// DTFloat4 fnl_b = dt.fnl_Value[2];
        fragcolor = UnityDiteFNRwork(fragcolor,fnl_c,fnl_v,fnl_b,0,0);
#endif
	fragcolor = LastExportColor(fragcolor,  i);

	UNITY_APPLY_FOG(i.fogCoord, col);
	return DT_OutFragColor(fragcolor);
}

// DTFloat4 fragColMask_Diss(v2f_DT i) : SV_Target
// {
// 	UNITY_SETUP_INSTANCE_ID(i);
// 	DTFloat4 fragcolor;
// 	FXCommonlyTexBase dt;
// 	UNITY_INITIALIZE_OUTPUT(FXCommonlyTexBase, dt);
//     DT_surf(i, dt);

//     // DT_SETVertexDATA(); // vColor VCustomValue uvbase uv0 uv1	
// 	DTFloat4 vColor = i.color ;              
//     DTFloat2 uvbase = i.basedata.xy;         
//     DTFloat2 uv0 = i.uv.xy;                  
//     DTFloat2 uv1 = i.uv.zw;                  
//     DTFloat4 worldpos = i.worldPos;          
//     DTFloat4 vcv = DTFloat4(i.basedata.zw,i.fogCoord.zw);

//     DT_SETTexDATA_TT(main); //##_c ##_a ##_v ##_b
//     DT_SETTexDATA_TT(fx0);
	
// 	if (mainFD.Ftimemode == 2)
// 	{
// 		mainFD.Ftime = vcv.x * 1.1;
// 	}
// 	if (fx0FD.Ftimemode == 2)
// 	{
// 		fx0FD.Ftime = vcv.y * 1.1;
// 	}

	

// 	fragcolor = GetTexData(_MainTex, uv0, mainFD);
// 	fragcolor.rgb *= mainFD.color.rgb * 2.0 * mainFD.value2.x/*亮度*/;
// 	fragcolor.a = saturate(fragcolor.a * mainFD.color.a);
	
// 	DTFloat4 tex1 = GetTexData(_Effect0Tex, uv1, fx0FD);
// 	tex1 *= fx0FD.color * fx0FD.value2.x/*亮度*/; 
	
// 	fragcolor = UnityDiteColorMaskWork(fragcolor, tex1, fx0FD.value.x, fx0FD.isClip, fx0FD.blend);

// 	#ifdef DTMultichannel_I
// 		DT_SETTexDATA_TT(fx1);
// 		if (fx1FD.isValueType == 1)
// 		{
// 			fx1FD.value.x = i.color.a;
// 			vColor.a = 1;
// 		}
// 		else if (fx1FD.isValueType == 2)
// 		{
// 			fx1FD.value.x = saturate(vcv.z);
// 		}
// 		else
// 		{
// 			fx1FD.value.x = fx1FD.value.x;
// 		}	
// 		DTFloat4 tex2 = GetTexData(_Effect1Tex, uv1, fx1FD) * fx1FD.value2.x/*亮度*/;
// 		fragcolor = UnityDiteDissolutionWork(fragcolor, saturate(tex2.r), fx1FD.value, fx1FD.color, fx1FD.isFilp, fx1FD.isClip, fx1FD.blend);
// 	#endif
    

// 	// DTFloat4 tex2 = GetTexData(_Effect1Tex, uv1, fx1FD) * fx1FD.value2.x/*亮度*/;
// 	// tex2 *= fx1FD.color * 0.5 * fx1FD.value2.x/*亮度*/;
// 	// return   fx1FD.value2.x ;
// 	// DTFloat4 tex2 = GetTexData(_Effect1Tex, uv1);

//     // inline DTFloat4 UnityDiteColorMaskWork(DTFloat4 _srcCol,DTFloat4 _mask,DTFloat vx,int _isclip,int _isblend)

	
// 	fragcolor *= vColor;
//     // fragcolor = saturate(fragcolor);

// #ifdef _FRESNEL_ON
//         DT_SETFnlDATA(fnl);//##_c ##_v ##_b
// 		// DTFloat4 fnl_c = dt.fnl_Value[0];
//     	// DTFloat4 fnl_v = dt.fnl_Value[1];
//     	// DTFloat4 fnl_b = dt.fnl_Value[2];
//         fragcolor = UnityDiteFNRwork(fragcolor,fnl_c,fnl_v,fnl_b,0,0);
// #endif

// 	fragcolor = LastExportColor(fragcolor,  i);

// 	UNITY_APPLY_FOG(i.fogCoord, col);
// 	return DT_OutFragColor(fragcolor);
// }
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// 溶解
DTFloat4 fragDiss(v2f_DT i) : SV_Target
{
	UNITY_SETUP_INSTANCE_ID(i);
	DTFloat4 fragcolor;
	FXCommonlyTexBase dt;
	UNITY_INITIALIZE_OUTPUT(FXCommonlyTexBase, dt);
    DT_surf(i, dt);
	
    // DT_SETVertexDATA(); // vColor VCustomValue uvbase uv0 uv1
	DTFloat4 vColor = i.color ;              
    DTFloat2 uvbase = i.basedata.xy;         
    DTFloat2 uv0 = i.uv.xy;                  
    DTFloat2 uv1 = i.uv.zw;                  
    DTFloat4 worldpos = i.worldPos;          
    DTFloat4 vcv = DTFloat4(i.basedata.zw,i.fogCoord.zw);

    DT_SETTexDATA_TT(main); //##_c ##_a ##_v ##_b
    DT_SETTexDATA_TT(fx0);

	if (fx0FD.isValueType == 1)
	{
		fx0FD.value.x = i.color.a;
		vColor.a = 1;
	}
	else if (fx0FD.isValueType == 2)
	{
		fx0FD.value.x = saturate(vcv.z);
	}
	else
	{
		fx0FD.value.x = fx0FD.value.x;
	}
	if (mainFD.Ftimemode == 2)
	{
		mainFD.Ftime = vcv.x * 1.1;
	}
	if (fx0FD.Ftimemode == 2)
	{
		fx0FD.Ftime = vcv.y * 1.1;
	}

	fragcolor = GetTexData(_MainTex, uv0, mainFD);
	fragcolor.rgb *= mainFD.color.rgb * 2.0 * mainFD.value2.x/*亮度*/;
	fragcolor.a = saturate(fragcolor.a * mainFD.color.a);

	DTFloat4 tex1 = GetTexData(_Effect0Tex, uv1, fx0FD) * fx0FD.value2.x/*亮度*/;
	fragcolor = UnityDiteDissolutionWork(fragcolor, saturate(tex1.r), fx0FD.value, fx0FD.color, fx0FD.isFilp, fx0FD.isClip, fx0FD.blend); //.y, fx0_b.z, fx0_b.w);
    // fragcolor = UnityDiteDissolutionWork(fragcolor,tex1,fx0_v,fx0_b);        
	fragcolor *= vColor;
    // fragcolor = saturate(fragcolor);

#ifdef _FRESNEL_ON
        DT_SETFnlDATA(fnl);//##_c ##_v ##_b
        fragcolor = UnityDiteFNRwork(fragcolor,fnl_c,fnl_v,fnl_b,0,0);
#endif
	fragcolor = LastExportColor(fragcolor,  i);

	UNITY_APPLY_FOG(i.fogCoord, col);
	return DT_OutFragColor(fragcolor);
}

// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// ************************************************************************************************************************************
// UI Shader normal
    // inline DTFloat4 GetTexData(sampler2D tex,DTFloat2 uv)
    // {
    //     DTFloat4 fragcolor;
    //     DTFloat2 _uv = uv;
    //     return tex2D(tex,FrameUV(frac(_uv), data),ddx(uv)*0.1,ddy(uv)*0.1);
        
    // }

// DTFloat4 fragUITex(v2f_DT i) : SV_Target
// {
//     UNITY_SETUP_INSTANCE_ID( i );
//     DTFloat4 fragcolor;


//     fragcolor.a = saturate(fragcolor.a);
    
//     fragcolor = LastExportColor(fragcolor);

//     UNITY_APPLY_FOG(i.fogCoord, col);
//     return DT_OutFragColor(fragcolor);
// }

#endif // YF_EFFECT_BASE_DITE_INCLUDED

