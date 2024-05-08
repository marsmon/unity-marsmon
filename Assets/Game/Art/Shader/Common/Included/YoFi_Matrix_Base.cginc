//***********************************************************************************
// 特效中各种矩阵相关的方法
// #include "YoFi_Matrix_Base.cginc"

// ==vertex 变换用矩阵
// ==UV 变换用矩阵

// ==计算用的python 脚本
//***********************************************************************************
#ifndef YF_DATA_MATRIX_BASE_UNITY_CG_INCLUDED
#define YF_DATA_MATRIX_BASE_UNITY_CG_INCLUDED
#include "UnityCG.cginc"
#include "YoFi_Common_Base.cginc" 



#if DT_EDITORMODE
    #define DT_Sin(v) sin(v)
    #define DT_Cos(v) cos(v)
    #define DT_SinV(v) sin(v)
    #define DT_CosV(v) cos(v)
#else
    #define DT_Sin(v) (tex2D(PowDataDite,DTFloat2(v, 0.25)).g * 2.0 + -1.0)
    #define DT_Cos(v) (tex2D(PowDataDite,DTFloat2(v, 0.75)).g * 2.0 + -1.0)

    #define DT_SinV(v) (tex2Dlod(PowDataDite,DTFloat4(v, 0.25, 0.0, 0.0)).g * 2.0 + -1.0)
    #define DT_CosV(v) (tex2Dlod(PowDataDite,DTFloat4(v, 0.75, 0.0, 0.0)).g * 2.0 + -1.0)
#endif

// *********************vertex 变换用矩阵*********************
// 从右往左的执行顺序 |位移||旋转y||旋转x||旋转z||缩放|
#define DT_MAT_TV_M(pos)  DTFloat4x4(/*0*/1.0, 0.0, 0.0, pos##.x, /*1*/0.0, 1.0, 0.0, pos##.y,  /*2*/0.0, 0.0, 1.0, pos##.z, /*3*/0.0, 0.0, 0.0, 1.0)
#define DT_MAT_TV_S(sca)  DTFloat4x4(/*0*/sca##.x, 0.0, 0.0, 0.0, /*1*/0.0, sca##.y, 0.0, 0.0,  /*2*/0.0, 0.0, sca##.z, 0.0, /*3*/0.0, 0.0, 0.0, 1.0)


#define DT_MAT_TV_Rx(d) DTFloat4x4(     /*0*/1.0, 0.0, 0.0, 0.0,\
                                     /*1*/0.0, DT_CosV(d), -DT_SinV(d), 0.0,\
                                     /*2*/0.0, DT_SinV(d), DT_CosV(d), 0.0,\
                                     /*3*/0.0, 0.0, 0.0, 1.0)

#define DT_MAT_TV_Ry(d) DTFloat4x4(     /*0*/DT_CosV(d), 0.0, DT_SinV(d), 0.0,\
                                     /*1*/0.0, 1.0, 0.0, 0.0,\
                                     /*2*/-DT_SinV(d), 0.0, DT_CosV(d), 0.0,\
                                     /*3*/0.0, 0.0, 0.0, 1.0)

#define DT_MAT_TV_Rz(d) DTFloat4x4(     /*0*/DT_CosV(d), -DT_SinV(d), 0.0, 0.0,\
                                     /*1*/DT_SinV(d), DT_CosV(d), 0.0, 0.0,\
                                     /*2*/0.0, 0.0, 1.0, 0.0,\
                                     /*3*/0.0, 0.0, 0.0, 1.0)

#define DT_MAT_TV_ROT(rot) mul(DT_MAT_TV_Ry(rot##.y),mul(DT_MAT_TV_Rx(rot##.x),DT_MAT_TV_Rz(rot##.z)))

#define DT_VERT_TRAMSFORM_All(p,r,s) mul(DT_MAT_TV_M(p),mul ( DT_MAT_TV_ROT(r), DT_MAT_TV_S(s) ))

#define DT_MAT_TV_M_O(pos)  DTFloat4x4(/*0*/1.0, 0.0, 0.0, pos##.x, /*1*/0.0, 1.0, 0.0, pos##.y,  /*2*/0.0, 0.0, 1.0, 0.0, /*3*/0.0, 0.0, 0.0, 1.0)
#define DT_MAT_TV_S_O(sca)  DTFloat4x4(/*0*/sca##.x, 0.0, 0.0, 0.0, /*1*/0.0, sca##.y, 0.0, 0.0,  /*2*/0.0, 0.0, 0.0, 0.0, /*3*/0.0, 0.0, 0.0, 1.0)

#define DT_VERT_TRAMSFORM(p,r,s) mul(DT_MAT_TV_M_O(p), mul(DT_MAT_TV_Rz(r), DT_MAT_TV_S_O(s) ))



// *********************UV 变换用矩阵*********************
#define DT_UV_mul(mat,uv) mul(mat,DTFloat3(uv,1)).xy

//从右往左的执行顺序 |缩放||旋转||位移|
//原因，先位移改变原来的0,0 ,再选择，最后缩放，这样才能保证缩放的时候回到中心
//==============================================================
// UV 变化矩阵， 带C的都是以UV中心做变化的 CTSR 围绕UV[0.5,0.5]做位移，缩放，旋转


//==============================================================
// UV 变化矩阵 缩放变化矩阵
// | x ,0.0,0.0|
// |0.0, y ,0.0|
// |0.0,0.0,1.0| 
#define DT_MAT_UV_TS(s,speed)       \
    DTFloat2 t = 1.0 / s##.xy;          \
    \
    DTFloat col1 = floor (speed / t.x);\
    DTFloat row = floor (speed - col1 * s##.x);\
    \
    DTFloat2 p = DTFloat2(col1,row);\
    DTFloat3x3 mat = DTFloat3x3 (   \
        t.x,    0.0,    p.x,  \
        0.0,    t.y,    p.y,  \
        0.0,    0.0,    1.0)

// DTFloat col = floor (_Speed / _SizeT.x);
//         DTFloat row = floor (_Speed - col * _SizeT.x);
//==============================================================
// UV 围绕中心 旋转变化矩阵 
// |0.5,0.0,0.5| |cos,-sin,0.0| |2.0,0.0,-1.0|
// |0.0,0.5,0.5|*|sin, cos,0.0|*|0.0,2.0,-1.0|
// |0.0,0.0,1.0| |0.0, 0.0,1.0| |0.0,0.0, 1.0|
#define DT_MAT_UV_CR(r)                                     \
    DTFloat2 csv = DTFloat2(DT_Sin(r),DT_Cos(r));                 \
    DTFloat3x3 mat = DTFloat3x3 (                                 \
        csv.y,  -csv.x,   (-csv.y + csv.x + 1.0) * 0.5,     \
        csv.x,   csv.y,   (-csv.x - csv.y + 1.0) * 0.5,     \
        0.0,     0.0,     1.0)
//==============================================================
// UV 围绕中心 缩放变化矩阵
// |0.5,0.0,0.5| | x ,0.0,0.0| |2.0,0.0,-1.0|
// |0.0,0.5,0.5|*|0.0, y ,0.0|*|0.0,2.0,-1.0|
// |0.0,0.0,1.0| |0.0,0.0,1.0| |0.0,0.0, 1.0|
#define DT_MAT_UV_CS(sx,sy)                             \
    half3x3 mat = half3x3(                          \
        sx,  0.0,     (1.0 - sx) * 0.5,       \
        0.0,    sy,   (1.0 - sy) * 0.5,       \
        0.0,    0.0,     1.0)

//==============================================================
// UV 移动 旋转 缩放变化矩阵
// |0.5,0.0,0.5| |sx ,0.0,0.0| |cos,-sin,0.0| |2.0,0.0,-1.0| |1.0,0.0,px |
// |0.0,0.5,0.5|*|0.0,sy ,0.0|*|sin, cos,0.0|*|0.0,2.0,-1.0|*|0.0,1.0,py |
// |0.0,0.0,1.0| |0.0,0.0,1.0| |0.0, 0.0,1.0| |0.0,0.0, 1.0| |0.0,0.0,1.0|
// 推算过程
// csr.x = sx*sin ; csr.y = sy*sin ; csr.z = sx*cos ; csr.w = sy*cos ;
// cpsr.x = csr.x * py ; cpsr.y = csr.y * px  ; cpsr.z = csr.z * px  ; cpsr.w = csr.w * py ;
//
// sx*cos,         -sx*sin,        -0.5*sx*cos + -0.5*sx*-sin + 0.5 +px,
// sy*sin,         sy*cos,         -0.5*sy*sin + -0.5*sy*cos + 0.5 +py,
// 0,              0,              1.0
#define DT_MAT_UV_C0TRS(p,r,s)                                  \
    DTFloat2 csv = DTFloat2(DT_Sin(r),DT_Cos(r));                     \
    DTFloat4 csr = csv.xxyy * s##.xyxy;                            \
    DTFloat4 cpsr = csr * p##.yxxy ;                               \
    DTFloat3x3 mat = DTFloat3x3(                                      \
        csr.z,  -csr.x, (2*cpsr.z-2*cpsr.x-csr.z+csr.x+1)*0.5,  \
        csr.y,  csr.w,  (2*cpsr.y+2*cpsr.w-csr.y-csr.w+1)*0.5,  \
        0.0,  0.0, 1.0)

//==============================================================
//  UV 旋转始终围绕中心 移动 缩放变化矩阵
//  |0.5,0.0,0.5| |1.0,0.0,px | |sx ,0.0,0.0| |cos,-sin,0.0| |2.0,0.0,-1.0|
//  |0.0,0.5,0.5|*|0.0,1.0,py |*|0.0,sy ,0.0|*|sin, cos,0.0|*|0.0,2.0,-1.0|
//  |0.0,0.0,1.0| |0.0,0.0,1.0| |0.0,0.0,1.0| |0.0, 0.0,1.0| |0.0,0.0, 1.0|
// 推算过程
// csr.x = sx*sin ; csr.y = sy*sin ; csr.z = sx*cos ; csr.w = sy*cos ;
// cpsr.x = csr.x * py ; cpsr.y = csr.y * px  ; cpsr.z = csr.z * px  ; cpsr.w = csr.w * py ;
//
// sx*cos,        -sx*sin,      -0.5*sx*cos + -0.5*sx*-sin + 0.5*sx*px   + 0.5,
// sy*sin,         sy*cos,      -0.5*sy*sin + -0.5*sy*cos + 0.5*sy*py   + 0.5,
// 0,              0,           1.0
#define DT_MAT_UV_C1TRS(p,r,s)                              \
    DTFloat2 csv = DTFloat2(DT_Sin(r),DT_Cos(r));                 \
    DTFloat4 csr = csv.xxyy * s##.xyxy;                        \
    DTFloat2 cps = p##.xy * s##.xy;                            \
    DTFloat3x3 mat = DTFloat3x3(                                  \
        csr.z, -csr.x,  (1 - csr.z + csr.x + cps.x) * 0.5,  \
        csr.y,  csr.w,  (1 - csr.y - csr.w + cps.y) * 0.5,  \
        0.0,  0.0, 1.0)


#endif // YF_DATA_MATRIX_BASE_UNITY_CG_INCLUDED



// *****计算用的python 脚本*********************************************************************************************************************
/* #脚本开始
a0 = [
    0.5,0,0.5,
    0,0.5,0.5,
    0,0,1
]
a1 = [
    2,0,-1,
    0,2,-1,
    0,0,1
]
r0 = [
    "cos","-sin",0,
    "sin","cos",0,
    0,0,1
]
s0 = [
    "sx",0,0,
    0,"sy",0,
    0,0,1
]
p0 = [
    1,0,"px",
    0,1,"py",
    0,0,1
]
data = [a0,s0,p0,r0,a1] #这里可以组合矩阵内容
def isnumber(s):
    try:
        ss = DTFloat(s)
        return True
    except:
        return False
def MulPM(a,b):
    if a == 0 or b == 0 or a == "0" or b == "0":
        return "0"
    if a == 1 or a == "1" or a == "1.0" :
        return str(b) 
    if b == 1 or b == "1" or b == "1.0" :
        return str(a)
    try:
        c = DTFloat(a) * DTFloat(b)
    except:
        c = "%s*%s" %(str(a),str(b))
    return str(c)
def MulPadd(a,b):
    if a == "0" and b == "0":
        return "0"
    if a == "0" :
        return str(b)
    if b == "0" :
        return str(a)
    try:
        c = DTFloat(a) + DTFloat(b)
    except:
        c = "%s+%s" %(str(a),str(b))
    return str(c)
def getMatData(a1,b1,a2,b2,a3,b3):
    c1 = MulPM(a1,b1)
    c2 = MulPM(a2,b2)
    c3 = MulPM(a3,b3)
    s = MulPadd(c1,c2)
    s = MulPadd(s,c3)
    return s
def xunhuancheng(listmat,c,index):    
    if index >= len(listmat):
        return c
    else:
        c = MulPM(c,listmat[index])
        c = xunhuancheng(listmat,c,index + 1)
    return c
def panduanchengfa(s):
    try:
        l = str(s).split('*')
        l2 = []
        for i in l:
            if isnumber(i):
                l2.insert(0,DTFloat(i))
            else:            
                l2.append(i)
        return xunhuancheng(l2,1,0)
    except:
        return s
def panduanADD(s):
    try:
        l = str(s).split('+')
        l2 = ""
        for u in l:
            l2 = l2 + panduanchengfa(u) + " + "
        return l2[:-3]
    except:
        return panduanchengfa(s)
def mul(a,b):
    c = [0,0,0, 0,0,0, 0,0,0]    
    c[0] = getMatData(a[0],b[0],a[1],b[3],a[2],b[6])
    c[1] = getMatData(a[0],b[1],a[1],b[4],a[2],b[7])
    c[2] = getMatData(a[0],b[2],a[1],b[5],a[2],b[8])

    c[3] = getMatData(a[3],b[0],a[4],b[3],a[5],b[6])
    c[4] = getMatData(a[3],b[1],a[4],b[4],a[5],b[7])
    c[5] = getMatData(a[3],b[2],a[4],b[5],a[5],b[8])

    c[6] = getMatData(a[6],b[0],a[7],b[3],a[8],b[6])
    c[7] = getMatData(a[6],b[1],a[7],b[4],a[8],b[7])
    c[8] = getMatData(a[6],b[2],a[7],b[5],a[8],b[8])
    for i in range(len(c)):
        c[i] = panduanADD(c[i])    
    return c
def mulList(listmat,c,index):    
    if index >= len(listmat):
        return c
    else:
        c = mul(c,listmat[index])
        c = mulList(listmat,c,index + 1)
    return c
def printMat(mat):
    print("===========")
    print(mat[0] + ",\t\t" + mat[1] + ",\t\t" + mat[2] + ",")
    print(mat[3] + ",\t\t" + mat[4] + ",\t\t" + mat[5] + ",")
    print(mat[6] + ",\t\t" + mat[7] + ",\t\t" + mat[8] )
def main():
    c = [1,0,0, 0,1,0, 0,0,1]
    printMat(mulList(data,c,0))
main()
#脚本end */
// ************************************************************************************************************************************

// *********************UV 变换用矩阵***************************************************************************************************************
// //地图块算法 DT_TFD_fXpXrXv(p,r,s,index,tile)  DT_MATRIX_UV_FCMSR
// #define DT_TFD_fXpXrXv(p,r,s,i,t)    \
//     DTFloat2 uvMatDataP = p;\
//     DT_TFD_rXs(r,s);\
//     DTFloat tiled = 1.0 / t;\
//     DTFloat2 frame;\
//     frame.x = (int)i % t * tiled;\
//     frame.y = floor((int)i / t) * tiled
// #define DT_MATRIX_UV_FCMSR   DTFloat3x3 (                                                                 \
//     uvMatDataRXS.z * tiled,   uvMatDataRXS.x * tiled,  (-uvMatDataRXS.z - uvMatDataRXS.x + uvMatDataP.x + 1) * 0.5 * tiled + frame.x,   \
//     -uvMatDataRXS.y * tiled,  uvMatDataRXS.w * tiled,   (uvMatDataRXS.y - uvMatDataRXS.w + uvMatDataP.y + 1) * 0.5 * tiled + frame.y,   \
//     0,      0,      -1)
// #ifdef DT_UV_Frame_Loop_On
//     #define DT_UV_Frame_LC frac
// #else
//     #define DT_UV_Frame_LC saturate
// #endif
//右下为0 左上为结尾
// 6 7 8
// 3 4 5
// 0 1 2
// #define DT_MATRIX_UV_FT(index,_tile) DTFloat3x3(\
// /*0*/1.0/_tile.x, 0, (uint)index % _tile.x * 1.0/_tile.x,           \
// /*1*/0,1.0/_tile.y,+ floor((uint)index / _tile.x) * 1.0/_tile.y ,   \
// /*2*/0,0,1)
//右上为0 左下为结尾
// 0 1 2
// 3 4 5
// 6 7 8
// #define DT_MATRIX_UV_F(index,_tile) DTFloat3x3(\
// /*0*/1.0/_tile.x, 0, (uint)index % _tile.x * 1.0/_tile.x,\
// /*1*/0, 1.0/_tile.y, (1 - 1.0/_tile.y ) + floor((uint)index / _tile.x) * -1.0/_tile.y,\
// /*2*/0, 0, 1)
// ************************************************************************************************************************************