//***********************************************************************************
// 特效和UI有关的方法
// #include "YFShaderUIFxBase.cginc"

//== UI 特效裁切
//***********************************************************************************

#ifndef YF_DATA_UTFX_BASE_UNITY_CG_INCLUDED
    #define YF_DATA_UTFX_BASE_UNITY_CG_INCLUDED

    #include "UnityCG.cginc"
    #include "YoFi_Common_Base.cginc"


    //=============================================================UI 特效裁切==================================================================
    DTFloat4 _ScrollViewRect; //UI裁切用的的属性。输入世界坐标。必须在正交摄像机下


    //ui上遇到scrollview 时，特效在UI上裁切不显示的方法
    //黑帮目前使用的是平面坐标 既从顶点函数 UnityObjectToClipPos(v.vertex);出来的顶点
    // 方式1
    //     通过获取RECT的4个顶点的极限坐标来裁切
    //     RectTransform rectTransform = go.GetComponent<RectTransform>();
    //     Vector3[] corners = new Vector3[4];
    //     rectTransform.GetWorldCorners(corners);
    //     DTFloat minX = corners[0].x;
    //     DTFloat minY = corners[0].y;
    //     DTFloat maxX = corners[2].x;
    //     DTFloat maxY = corners[2].y;
    //     return new Vector4(minX, minY, maxX, maxY);
    //  方式2
    // //通过获取RECT的中心点以及向[X,Y]边衍生的距离来确定范围。计算比较繁琐，效率有点低
    // //这里的变量可以是常量。因为都是直接从引擎读取并且游戏立项后都不会再改动了的
    // // ===========================================================================
    // // 计算过程
    // // vector2 pos0 = rectTransform.anchoredPosition;
    // // vector2 piont = rectTransform.pivot;
    // // vector2 size = rectTransform.rect;
    // DTFloat osize = Camera.main.orthographicSize;//正交相机的size 默认是5，最好是改成比例是1:1
    // DTFloat viewsetHight = 1080f; //Canvas里设置的相机画布的高度
    // DTFloat PerPixels = 100; //Canvas里设置的每像素比例。

    // DTFloat bfenbiPerPixels = 1.0f / PerPixels; 
    // 每像素与单位M之间的比例关系 0.925925925f<=1920*1080 
    // DTFloat bili = osize / (viewsetHight * bfenbiPerPixels * 0.5f);
    // // ===========================================================================
    // Vector2 p = rectTransform.pivot * 2.0f - Vector2.one ;
    // Vector4 pos = Vector3.one;
    // pos.z = rectTransform.rect.width * 0.5f;
    // pos.w = rectTransform.rect.height * 0.5f;
    // pos.x = rectTransform.anchoredPosition.x - p.x * pos.z;
    // pos.y = rectTransform.anchoredPosition.y - p.y * pos.w;               
    // return  pos * bfenbiPerPixels * bili;
    // DTFloat b = 1 - ceil(saturate(_v.z * _v.w));
    inline DTFloat DT_UIscrollviewClip( DTFloat4 pos, DTFloat4 _v){
        // _v = DTFloat4(0,-10,10,-1); //测试代码
        DTFloat result = sign(_v.z - _v.x) * sign( _v.w - _v.y); //* sign(v.x + v.y + v.z + v.w); 
        if (result == 0){
            return 1;
        }
        #if 1
            
            result = step(_v.x, pos.x) * step(pos.x, _v.z) * step(_v.y, pos.y) * step(pos.y, _v.w);
        #else
            
            DTFloat2 r = ceil(saturate(_v.zw - abs(_v.xy - pos.xy)));
            result = r.x * r.y;
        #endif
        return result; 
    }


    inline DTFloat DT_UIscrollviewClip( DTFloat4 pos){
        return DT_UIscrollviewClip(pos,_ScrollViewRect);
    }

#endif // YF_DATA_UTFX_BASE_UNITY_CG_INCLUDED

