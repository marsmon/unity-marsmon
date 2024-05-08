
// #define DT_BUTTON
#define ZMXS
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Rendering;
using YF.Art.PostProcessing;

namespace DiteScripts.YF.EditoShaderGUI
{
#if true
    public enum AllShaderName
    {
        YoFi_Effect_Particle_Base = 0,
        YoFi_Effect_ColorMask_Base = 1,
        YoFi_Effect_Dissolution_Base,
        YoFi_Effect_Distort_Base,
        YoFi_Effect_ColorMask_Dissolution
    }
    public enum AllShaderNameGUID
    {
        GUID_210be49b330e14c4eae96131e45ed5c1 = 0,
        GUID_b31bcfefb30f2684d82ced76b7ae0bf2 = 1,
        GUID_50c7ac9c667727f44974b1f6e95472bc,
        GUID_0799d73dc9852d740890492de486904f,
        GUID_ca337594b0119c2478238181b329bd75
    }

    public enum DTColorChannel
    {
        RGBA = 0,
        RGB_A,
        RG_B_A,
        R_G_B_A,
        R,
        G,
        B,
        A,
        RG,
        RGB,
    }


    internal class YFEffectBaseGUI : ShaderGUI
    {
        string[] culldata = new string[3] { "Cull Off", "Cull Front", "Cull Back" };
        string[] culldata2 = new string[3] { "N", "F", "B" };
        string[] fxblend = new string[3] { "Add", "Blend", "Multiply" }; //MULTIPLY
        string[] fxblend2 = new string[4] { "Add", "Blend", "Multiply", "Only Alpha" }; //MULTIPLY
        string[] IsONOFF = new string[2] { "Off", "On" };
        //比对GUID是否一样
        bool isSameShader;

        enum MaterialSettingFlagsEnum
        {
            None = 0, // Custom name for "Nothing" option
            Zwrite_On = 1 << 0,
            AutoTime_On = 1 << 1,
            Fresnel_On = 1 << 2,

            All = ~0, // Custom name for "Everything" option
        }
        public enum RenderTypeMode
        {   // Transparent AlphaTest Opaque
            Transparent = 0,
            AlphaTest = 1,
            Opaque = 2
        }
        MaterialEditor m_MaterialEditor;
        Color DefaultBGcol;
        int _shaderBlend = 0;
        int isBillborad = 0;
        RenderTypeMode renderTypeMode = 0;
        bool IS_TIMEDT_ON;
        bool IS_FRESNEL_ON;
        bool isUIclip;
        bool IsPolar;
        int ShaderMode = 0;
        bool isDissMode = false;
        int _zWrite;
        int _cull;
        int _colorMask;
        int _srcBlend;
        int _dstBlend;

        int tmpbq;
        int nowmode;

        string[] MaterialBlendMode = new string[] { "Additive", "Alpha Blend", "Nomal" };

        MaterialProperty mode = null;
        MaterialProperty cull = null;
        MaterialProperty zWrite = null;
        MaterialProperty srcBlend = null;
        MaterialProperty dstBlend = null;
        MaterialProperty colorMask = null;
        MaterialProperty billboradSize = null; //_BillboradSize


        TexDataForShader MainTexData = new TexDataForShader();
        TexDataForShader FX0TexData = new TexDataForShader();
        TexDataForShader FX1TexData = new TexDataForShader();

        MaterialProperty fnlColor = null;
        MaterialProperty fnlvalue = null;
        MaterialProperty fnlBase = null;
        string st;

        string channelStr = " Tex(RGBA)";
        string channelStr1 = " Tex(RGBA)";
        string channelStr2 = " Tex(RGBA)";

        bool ismainView = true;
        bool isbloomView = true;
        bool isEffect0View = true;
        bool isEffect1View = true;


        public void FindProperties(MaterialProperty[] props)
        {
            mode = FindProperty("_mode", props);
            cull = FindProperty("_Cull", props);
            zWrite = FindProperty("_ZWrite", props);
            colorMask = FindProperty("_ColorMask", props);
            srcBlend = FindProperty("_SrcBlend", props);
            dstBlend = FindProperty("_DstBlend", props);

            this.FindPropertyCustomBloom(props,out this.cbv );

            billboradSize = FindProperty("_BillboradSize", props, false);

            this.MainTexData = this.__SetTexDate("Main", props);
            this.FX0TexData = this.__SetTexDate("Effect0", props);
            this.FX1TexData = this.__SetTexDate("Effect1", props);

            


            //FNL
            fnlvalue = FindProperty("_FresnelValue", props);
            fnlColor = FindProperty("_FresnelColor", props);
            fnlBase = FindProperty("_FresnelBase", props);
        }


        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        MaterialProperty isClipGround = null;
        MaterialProperty isClipGroundBlur = null;
        MaterialProperty isNewColorMask = null;
        MaterialProperty isCustomBloomMask = null;

        void AddNewCustomProperty(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            List<MaterialProperty> npl = new List<MaterialProperty>();

            isClipGround = FindProperty("_IsClipGround", props, false);
            if (this.isClipGround != null)
            {
                npl.Add(this.isClipGround);// 8H6H 地面绝对等于0 
            }

            isClipGroundBlur = FindProperty("_IsClipGroundBlur", props, false);
            if (this.isClipGroundBlur != null)
            {
                npl.Add(this.isClipGroundBlur);// 8H6H 地面绝对等于0 
            }

            isCustomBloomMask = FindProperty("_CustomBloomTag", props, false);
            if (this.isCustomBloomMask != null)
            {
                npl.Add(this.isCustomBloomMask);// 8H6H 地面绝对等于0 
            }

#if ZMXS
            EditorGUILayout.LabelField("Bug 处理");
            isNewColorMask = FindProperty("_IsNewColorMask", props, false);
            if (this.isNewColorMask != null)
            {
                npl.Add(this.isNewColorMask);// 8H6H 地面绝对等于0 
            }
#endif



            materialEditor.PropertiesDefaultGUI(npl.ToArray());//zTest

        }


        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            String Sguid = "GUID_" + AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newShader));
            AllShaderNameGUID nguid;
            if (!Enum.TryParse<AllShaderNameGUID>(Sguid, out nguid))
            {
                isSameShader = false;
            }
            else
            {
                isSameShader = true;
            }
            material.shader = newShader;
            if (!isSameShader)
            {
                return;
            }
            this.handupdate(material);
        }
        private void handupdate(Material material)
        {
            int _nowmode = (int)material.GetFloat("_mode");
            RenderTypeMode rm = (RenderTypeMode)(_nowmode >> 8 & 0x3);
            int shaderBlend = (_nowmode >> 12 & 0xF);
            switch (rm)
            {
                case RenderTypeMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    // material.EnableKeyword("_DTBILLBORAD_UI"); 
                    this.isBillborad = 0;
                    this._zWrite = 1;
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.GeometryLast;
                    break;
                case RenderTypeMode.AlphaTest:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.EnableKeyword("_ALPHATEST_ON");
                    // material.EnableKeyword("_DTBILLBORAD_UI"); 
                    this.isBillborad = 0;
                    int sbl = shaderBlend < 1 ? (int)UnityEngine.Rendering.BlendMode.SrcAlpha : (int)UnityEngine.Rendering.BlendMode.SrcAlpha;
                    int dbl = shaderBlend < 1 ? (int)UnityEngine.Rendering.BlendMode.One : (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                    material.SetInt("_SrcBlend", sbl);
                    material.SetInt("_DstBlend", dbl);
                    material.SetInt("_ZWrite", 1);
                    this._zWrite = 1;
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    break;
                case RenderTypeMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.DisableKeyword("_ALPHATEST_ON");
                    // material.EnableKeyword("_DTBILLBORAD_UI"); 
                    this.isBillborad = 0;
                    int sbl2 = shaderBlend < 1 ? (int)UnityEngine.Rendering.BlendMode.SrcAlpha : (int)UnityEngine.Rendering.BlendMode.SrcAlpha;
                    int dbl2 = shaderBlend < 1 ? (int)UnityEngine.Rendering.BlendMode.One : (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                    material.SetInt("_SrcBlend", sbl2);
                    material.SetInt("_DstBlend", dbl2);
                    material.SetInt("_ZWrite", 0);
                    this._zWrite = 0;
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
                default:
                    break;
            }
            Debug.Log("刷新完成!");
        }
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {

            this.DefaultBGcol = GUI.backgroundColor;
            float defw = EditorGUIUtility.labelWidth;
            FindProperties(props);
            Material material = materialEditor.target as Material;
            this.m_MaterialEditor = materialEditor;

            this.LoadMode(material);
            if (!this.isSameShader)
            {
                String Sguid = "GUID_" + AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material.shader));
                string errorstr = @"严重错误:!!!
GUID 不同,请重新导入,或使用GUID对应的shader!
默认的GUID:
    {0}
当前的GUID:
    {1}
请检查!!!!";
                EditorGUILayout.HelpBox(string.Format(errorstr, (AllShaderNameGUID)this.ShaderMode, Sguid), MessageType.Error);

                return;
            }
            this.UpdateMaterBase(material);
            this.DefGUIWidth();
            //Draw MainProp
            this.DrawMainPropGUI(materialEditor, material);


            this.ismainView = EditorGUILayout.Foldout(this.ismainView, "Main Tex");
            if (this.ismainView)
            {
                //Draw Main
                this.DrawTexGUI(materialEditor, material, this.MainTexData, channelStr);
            }
            //Draw FXTex0
            // this.DrawTexGUI(materialEditor, material, this.FX0TexData, channelStr1);

            // //Draw FXTex1
            // this.DrawTexGUI(materialEditor, material, this.FX1TexData, channelStr2,"w");

            switch ((AllShaderName)this.ShaderMode)
            {
                case AllShaderName.YoFi_Effect_Particle_Base:
                    this.isDissMode = false;
                    st = "普通";
                    channelStr = " Tex(RGBA)";
                    channelStr1 = "EffectTex1 Tex(RGBA)";
                    break;
                case AllShaderName.YoFi_Effect_ColorMask_Base:
                    st = "遮罩";
                    this.isDissMode = false;
                    channelStr = " Tex(RGB,A/开启灰度)";
                    channelStr1 = "EffectTex1 Tex(RGB,A/开启灰度)";
                    this.isEffect0View = EditorGUILayout.Foldout(this.isEffect0View, "Effect0 Tex");
                    if (this.isEffect0View)
                    {
                        this.DrawTexGUI(materialEditor, material, this.FX0TexData, channelStr1);
                        this.DrawColorMaskGUI(materialEditor, material, this.FX0TexData);
                    }
                    break;
                case AllShaderName.YoFi_Effect_Dissolution_Base:
                    st = "溶解";
                    this.isDissMode = false;
                    channelStr = " Tex(R)";
                    channelStr1 = "EffectTex1 Tex(R)";
                    this.isEffect0View = EditorGUILayout.Foldout(this.isEffect0View, "Effect0 Tex");
                    if (this.isEffect0View)
                    {
                        this.DrawTexGUI(materialEditor, material, this.FX0TexData, channelStr1);
                        this.DrawDissolutionGUI(materialEditor, material, this.FX0TexData);
                    }
                    break;
                case AllShaderName.YoFi_Effect_Distort_Base:
                    st = "扰动";
                    this.isDissMode = true;
                    channelStr = " Tex(R)";
                    channelStr1 = "EffectTex1 Tex(R)";
                    this.isEffect0View = EditorGUILayout.Foldout(this.isEffect0View, "Effect0 Tex");
                    if (this.isEffect0View)
                    {
                        this.DrawTexGUI(materialEditor, material, this.FX0TexData, channelStr1);
                        this.DrawDisturbanceGUI(materialEditor, material, this.FX0TexData);
                    }
                    break;
                case AllShaderName.YoFi_Effect_ColorMask_Dissolution:
                    st = "遮罩+溶解";
                    this.isDissMode = false;
                    channelStr1 = "ColorMask Tex(RGB,A/开启灰度)";
                    channelStr2 = "溶解 Tex Tex(R)";
                    // this.DrawBoxSColor();
                    this.isEffect0View = EditorGUILayout.Foldout(this.isEffect0View, "Color Mask");
                    if (this.isEffect0View)
                    {
                        EditorGUILayout.BeginVertical("box");
                        this.DrawTexGUI(materialEditor, material, this.FX0TexData, channelStr1);
                        this.DrawColorMaskGUI(materialEditor, material, this.FX0TexData);
                        EditorGUILayout.EndVertical();
                    }
                    this.isEffect0View = EditorGUILayout.Foldout(this.isEffect0View, "溶解 Tex");
                    if (this.isEffect0View)
                    {
                        EditorGUILayout.BeginVertical("box");
                        this.DrawTexGUI(materialEditor, material, this.FX1TexData, channelStr1);
                        this.DrawDissolutionGUI(materialEditor, material, this.FX1TexData);
                        EditorGUILayout.EndVertical();
                    }
                    this.SetDefaultBG();
                    // this.DrawColorMaskDissGUI(materialEditor, material, this.FX0TexData, this.FX1TexData);
                    break;

                default:
                    EditorGUILayout.HelpBox("没有找到可以使用的Shader文件/类型", MessageType.Error);
                    break;
            }
            st = "  [" + ((AllShaderName)this.ShaderMode).ToString() + "]" + st;
            //Draw FNL
            this.DrawFNLGUI(materialEditor, material);

            this.DrawBloomGUI(materialEditor, material, cbv);
            // }
            //自定的新属性
            this.AddNewCustomProperty(materialEditor, props);
            if (GUILayout.Button("清理材质球", GUILayout.Height(30)))
            {
                this.run(material);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            materialEditor.SetDefaultGUIWidths();

            // materialEditor.EnableInstancingField();
            // materialEditor.RenderQueueField();
            // materialEditor.DoubleSidedGIField();



            // EditorGUIUtility.labelWidth = defw;
            // materialEditor.PropertiesDefaultGUI(props);       
            // base.OnGUI(materialEditor,props);
        }



        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //YF_Effect_Particle
        void DrawYFEffectParticle(MaterialEditor materialEditor, Material materia, TexDataForShader __data)
        {
            this.DrawBoxSColor();
            this.DefGUIWidth();
            {
                //该项无用。预留模板
            }
            // this.DrawBoxEColor();
        }

        void DrawColorMaskDissGUI(MaterialEditor materialEditor, Material material, TexDataForShader __data1, TexDataForShader __data2)
        {
            DrawColorMaskGUI(materialEditor, material, __data1);

            DrawDissolutionGUI(materialEditor, material, __data2);
        }

        /// <summary>
        /// 绘制ColorMask面板
        /// </summary>
        /// <param name="materialEditor"></param>
        /// <param name="material"></param>
        /// <param name="__data0"></param>
        /// <param name="__data1"></param>
        void DrawColorMaskGUI(MaterialEditor materialEditor, Material material, TexDataForShader __data1)
        {
            Color c1 = __data1.getColor();
            Vector4 v1 = __data1.getValue0();
            Vector4 b1 = __data1.getBaseData();
            this.DrawBoxSColor();
            this.DefGUIWidth();

            int isValueType = (int)b1.w >> 7 & 0xf; // 是否启用顶点属性 15 0b1111
            int isDist = (int)b1.w >> 6 & 0x1; // 是否扰动 1 0b1 
            int isFilp = (int)b1.w >> 5 & 0x1; // 是否反相 1 0b1
            int isClip = (int)b1.w >> 4 & 0x1; // 是否clip 1 0b1
            int blend = (int)b1.w >> 0 & 0x7; //混合模式   0b111
            int isMaskA = (int)b1.w >> 3 & 0x1; //是否保持Mask通道 1 0b1
            bool maska = isMaskA > 0 ? true : false;

            GUIContent b1zgc = EditorGUIUtility.TrTextContent("", "使用灰度为A通道 或 自身的A通道");
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.BeginHorizontal();
                isClip = EditorGUILayout.Popup(b1zgc, isClip, new string[] { "Alpha", "灰度Alpha" }, GUILayout.MaxWidth(80), GUILayout.MinWidth(50));
                c1 = EditorGUILayout.ColorField(new GUIContent(" Mask Color"), c1, true, true, true);
                EditorGUILayout.EndHorizontal();
                maska = EditorGUILayout.Toggle("维持Mask.a", maska);
                EditorGUILayout.BeginHorizontal();
                blend = EditorGUILayout.Popup(blend, fxblend2, GUILayout.MaxWidth(80), GUILayout.MinWidth(50));
                switch (blend)
                {
                    case 0:
                        EditorGUILayout.LabelField(" Mask.rgba + Main.rgba", new GUIStyle("TextArea"));
                        break;
                    case 1:
                        EditorGUIUtility.labelWidth = 100;
                        v1.x = EditorGUILayout.Slider(" Mask↑↓Blend", v1.x, 0.0f, 1.0f);
                        this.DefGUIWidth();
                        break;
                    case 2:
                        EditorGUILayout.LabelField(" Mask.rgba * Main.rgba", new GUIStyle("TextArea"));
                        break;
                    case 3:
                        EditorGUILayout.LabelField(" (Main.rgb,Main.a * Mask.a)", new GUIStyle("TextArea"));
                        break;
                }
                EditorGUILayout.EndHorizontal();


            }
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("DrawColorMaskGUI Mode");
                int ma = maska ? 1 : 0;
                b1.w = blend + (ma << 3) + (isClip << 4) + (isFilp << 5) + (isDist << 6) + (isValueType << 7);
                __data1.SetColor(c1);
                __data1.SetValue0(v1);
                __data1.SetBaseData(b1);
            }
            this.DrawBoxEColor();
            // this.DrawLine();       
        }
        /// <summary>
        /// 绘制溶解面板
        /// </summary>
        /// <param name="materialEditor"></param>
        /// <param name="material"></param>
        /// <param name="__data0"></param>
        /// <param name="__data1"></param>
        void DrawDissolutionGUI(MaterialEditor materialEditor, Material material, TexDataForShader __data1)
        {

            this.DrawBoxSColor();
            this.DefGUIWidth();
            // Vector4 v0 = __data0.getValue0();
            // Vector4 b0 = __data0.getBaseData();
            // b0.w = 1;
            Vector4 v1 = __data1.getValue0();
            Color col = __data1.getColor();
            Vector4 b1 = __data1.getBaseData();

            int isValueType = (int)b1.w >> 7 & 0xf; // 是否启用顶点属性 15 0b1111
            int isDist = (int)b1.w >> 6 & 0x1; // 是否扰动 1 0b1 
            int isFilp = (int)b1.w >> 5 & 0x1; // 是否反相 1 0b1
            int isClip = (int)b1.w >> 4 & 0x1; // 是否clip 1 0b1
            int blend = (int)b1.w >> 0 & 0xf; //混合模式  15 0b1111



            string[] pn = new string[] { "溶解值", "Particle Color.a", "Particle Custom.z" };//,"G 通道辅助"};

            // GUIContent isValueTypeGC  = EditorGUIUtility.TrTextContent(pn[(int)b0.w] ,"particle custom.xy[TEXCOORD0.zw]");
            GUIContent isValueTypelGC = EditorGUIUtility.TrTextContent("溶解值模式", "particle custom.xy[TEXCOORD0.zw]");
            GUIContent IsdistGC = EditorGUIUtility.TrTextContent(isDist > 0 ? "扰动 ●" : "扰动 ○", "是否参与扰动");
            GUIContent IsfilpGC = EditorGUIUtility.TrTextContent(isFilp > 0 ? "Flip ●" : "Flip ○", "是否反相");
            GUIContent IsclipGC = EditorGUIUtility.TrTextContent(isClip > 0 ? "Clip ●" : "Clip ○", "是否硬裁切");
            GUIContent blendGC = EditorGUIUtility.TrTextContent(fxblend[blend], "描边与源的混合模式");
            GUIContent cgc = EditorGUIUtility.TrTextContent("描边颜色");
            GUIContent pgc = EditorGUIUtility.TrTextContent("溶解范围的细节", "主要作用是收缩过度,但并非裁切,\n裁切无效的属性范围 1 - 32 ,\n如细节就是硬边,请开启下面的Clip而不是使用这个数值");
            EditorGUI.BeginChangeCheck();
            {
                isValueType = EditorGUILayout.Popup(isValueTypelGC, isValueType, pn);
                switch (isValueType)
                {
                    case 0:
                        v1.x = EditorGUILayout.Slider("溶解值", v1.x, 0f, 1f);
                        break;
                    default:
                        break;
                }
                v1.y = EditorGUILayout.Slider("描边宽度", v1.y, 0f, 1f);
                col = EditorGUILayout.ColorField(cgc, col, true, true, true);
                v1.w = EditorGUILayout.FloatField("描边亮度", v1.w);
                //关闭了的功能，暂时不开启POW 细化细节
                // v1.z = (float)EditorGUILayout.IntSlider(pgc,(int)v1.z,1,10);
                int buttonW = 80;
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (this.isDissMode)
                {
                    if (GUILayout.Button(IsdistGC, this.setButtonFronCol("button", isDist < 1), GUILayout.MaxWidth(80), GUILayout.MinWidth(50)))
                    { isDist = 1 - isDist; }
                }
                if (GUILayout.Button(IsfilpGC, this.setButtonFronCol("button", isFilp < 1), GUILayout.MaxWidth(buttonW), GUILayout.MinWidth(50)))
                { isFilp = 1 - isFilp; }
                if (GUILayout.Button(IsclipGC, this.setButtonFronCol("button", isClip < 1), GUILayout.MaxWidth(buttonW), GUILayout.MinWidth(50)))
                { isClip = 1 - isClip; }
                blend = EditorGUILayout.Popup(blend, fxblend, GUILayout.MaxWidth(80), GUILayout.MinWidth(50));
                EditorGUILayout.EndHorizontal();
                string hbs = @"Tips: 溶解贴图只读取贴图的 R(红) 通道";
                EditorGUILayout.HelpBox(hbs, MessageType.Info);
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("DrawDissolutionGUI Mode");
                b1.w = blend + (isClip << 4) + (isFilp << 5) + (isDist << 6) + (isValueType << 7);
                __data1.SetValue0(v1);
                __data1.SetColor(col);
                __data1.SetBaseData(b1);

            }
            this.DrawBoxEColor();
        }
        /// <summary>
        /// 绘制扰动面板
        /// </summary>
        /// <param name="materialEditor"></param>
        /// <param name="material"></param>
        /// <param name="__data0"></param>
        /// <param name="__data1"></param>
        void DrawDisturbanceGUI(MaterialEditor materialEditor, Material material, TexDataForShader __data1)
        {
            this.DrawBoxSColor();
            this.DefGUIWidth();
            Vector4 v1 = __data1.getValue0();
            EditorGUI.BeginChangeCheck();
            {
                v1.x = EditorGUILayout.Slider("扰动强度 U", v1.x, -5f, 5f);
                v1.y = EditorGUILayout.Slider("扰动强度 V", v1.y, -5f, 5f);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                string hbs = "Tips: 扰动贴图只读取贴图的 R(红) 通道";
                EditorGUILayout.HelpBox(hbs, MessageType.Info);
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("DrawDisturbanceGUI Mode");
                __data1.SetValue0(v1);
            }
            this.DrawBoxEColor();
        }


        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //*****************************************************************************************************************************

        /// <summary>
        /// Shader 主要属性的设置Gui
        /// </summary>
        /// <param name="materialEditor"></param>
        /// <param name="material"></param>
        void DrawMainPropGUI(MaterialEditor materialEditor, Material material)
        {
            this._cull = (int)cull.floatValue;
            this._zWrite = (int)zWrite.floatValue;
            this._colorMask = (int)colorMask.floatValue;
            this._srcBlend = (int)srcBlend.floatValue;
            this._dstBlend = (int)dstBlend.floatValue;
            string[] gonggaoban = new string[] { "F", "V", "U" };
            GUIContent RenderTypeGC = EditorGUIUtility.TrTextContent("Render Type", " Transparent:3000+\n AlphaTest: 2450+\n Opaque: 2500+");
            GUIContent BlendypeGC = EditorGUIUtility.TrTextContent("Render Blend", " Add or Alpha blend");
            GUIContent ZGC = EditorGUIUtility.TrTextContent("Z" + (_zWrite > 0 ? "√" : "×"), "ZWrite on/off(红关,绿开)");
            GUIContent CGC = EditorGUIUtility.TrTextContent("C", "Cull 开关剔除的面,\n  Cn:不剔除,双面(默认),\n  Cf:剔除正面\n  Cb:剔除背面");    //"Cull"+( culldata[_cull + 3] )  
            // GUIContent CGC2 = EditorGUIUtility.TrTextContent("C"+( culldata2[_cull] )  ,"Cull 开关剔除的面,\n  Cn:不剔除,双面(默认),\n  Cf:剔除正面\n  Cb:剔除背面");    //"Cull"+( culldata[_cull + 3] )  
            GUIContent tGC = EditorGUIUtility.TrTextContent("T" + (this.IS_TIMEDT_ON ? "√" : "×"), "自动增加的时间开关");
            GUIContent fGC = EditorGUIUtility.TrTextContent("F" + (this.IS_FRESNEL_ON ? "√" : "×"), "菲尼尔开关");
            // GUIContent UIGC = EditorGUIUtility.TrTextContent("UI"+( this.isUIclip? "√":"×" ),"UI上是否裁切");
            // GUIContent POLARGC = EditorGUIUtility.TrTextContent("极"+( this.IsPolar? "√":"×" ),"是否开启极坐标");
            GUIContent IsBGC = EditorGUIUtility.TrTextContent("", "公告板开关,F是之前的弹道\nV是永远面向镜头(类似粒子),U:是UI使用的切片暂时没用");

            EditorGUIUtility.labelWidth = 0f;
            this.DrawBoxSColor();
            int buttonW = 40;


            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.LabelField(" " + st, new GUIStyle("AC BoldHeader"), GUILayout.Height(25));

                EditorGUIUtility.labelWidth = 80;
                this.DefGUIWidth();
                EditorGUILayout.BeginHorizontal();
                this.renderTypeMode = (RenderTypeMode)EditorGUILayout.EnumPopup(RenderTypeGC, this.renderTypeMode);
                string uopddd = "Update";
                float uopdddw = 55;
                if (EditorGUIUtility.currentViewWidth < 300)
                {
                    uopddd = "U";
                    uopdddw = 35;
                }
                if (GUILayout.Button(uopddd, GUILayout.Width(uopdddw))) { this.handupdate(material); }
                EditorGUILayout.EndHorizontal();
                if (this.renderTypeMode == RenderTypeMode.Opaque)
                {
                    EditorGUILayout.Popup(BlendypeGC, 2, new string[] { "Additive", "Alpha Blend", "None" });
                }
                else
                {
                    this._shaderBlend = EditorGUILayout.Popup(BlendypeGC, this._shaderBlend, new string[] { "Additive", "Alpha Blend" });
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Other Setting", GUILayout.Width(80));
                EditorGUILayout.Space();
                // this.isBillborad = EditorGUILayout.Popup(IsBGC, this.isBillborad, new string[] { "Bil None", "Bi FX Billborad", "Bi VC Billborad" }, GUILayout.MaxWidth(buttonW));
                if (this.renderTypeMode != RenderTypeMode.Opaque)
                {
                    EditorGUIUtility.labelWidth = 1;
                    _cull = EditorGUILayout.Popup(CGC, _cull, culldata, GUILayout.Width(buttonW));


                    if (GUILayout.Button(ZGC, this.setButtonFronCol("button", this._zWrite == 0), GUILayout.MaxWidth(buttonW)))
                    { this._zWrite = 1 - this._zWrite; }
                }
                else
                {
                    EditorGUILayout.Popup(CGC, 0, culldata, GUILayout.Width(buttonW));
                    if (GUILayout.Button(ZGC, this.setButtonFronCol("button", _zWrite == 0), GUILayout.MaxWidth(buttonW))) { }
                }
                if (EditorGUIUtility.currentViewWidth < 450)
                { //|| uvanim == 4
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                if (GUILayout.Button(tGC, this.setButtonFronCol("button", !this.IS_TIMEDT_ON), GUILayout.MaxWidth(buttonW)))
                {
                    this.IS_TIMEDT_ON = !this.IS_TIMEDT_ON;
                }
                if (GUILayout.Button(fGC, this.setButtonFronCol("button", !this.IS_FRESNEL_ON), GUILayout.MaxWidth(buttonW)))
                {
                    this.IS_FRESNEL_ON = !this.IS_FRESNEL_ON;
                }
                // if(GUILayout.Button(POLARGC,this.setButtonFronCol("button",!this.IsPolar ) ,GUILayout.MaxWidth(buttonW)))
                // { 
                //     this.IsPolar = !this.IsPolar ;
                // }

                // if(GUILayout.Button(IsBGC,this.setButtonFronCol("button",this.isBillborad == 0 ) ,GUILayout.MaxWidth(buttonW)))
                // { 
                //     this.isBillborad = 1 - this.isBillborad ;                            
                // }

                //保留属性,特效制作暂时用不到
                if (GUILayout.Button((_colorMask == 15 ? "RGBA" : "RGB"), GUILayout.MaxWidth(buttonW))) { _colorMask ^= (1 << 0); }

                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                this.UpdateMode(material);

            }
            EditorGUILayout.HelpBox("Bi:公告板,Cull:正反面裁切,Z:Zwrite,T:自动时间,F:菲尼尔 ", MessageType.None);
            this.DrawBoxEColor();
        }


        /// <summary>
        /// 绘制贴图的GUI
        /// </summary>
        /// <param name="materialEditor"></param>
        /// <param name="materia"></param>
        /// <param name="__data"></param>
        void DrawTexGUI(MaterialEditor materialEditor, Material materia, TexDataForShader __data, string channlemode, string axis = "y")
        {
            if (!__data.IsTex)
            {
                return;
            }
            bool ismain = __data.name == "Main";
            this.DrawBoxSColor();
            this.DefGUIWidth();
            Color c = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            if (__data.IsColor)
            {
                c = __data.color.colorValue;
            }
            Vector4 value = __data.getValue0();
            Vector4 adata = __data.getAnimData();
            Vector4 _baseData = __data.getBaseData();

            EditorGUI.BeginChangeCheck();
            {
                if (ismain)
                {
                    GUIContent colorGC = EditorGUIUtility.TrTextContent(" Tint Color");
                    c = EditorGUILayout.ColorField(colorGC, c, true, true, true); //////dddddd
                }
                materialEditor.SetDefaultGUIWidths();
                if (ismain)
                {
                    materialEditor.DefaultShaderProperty(__data.Tex, " " + __data.name + " Tex(RGBA)");
                }
                else
                {
                    materialEditor.DefaultShaderProperty(__data.Tex, " " + channlemode);
                }

                this.DefGUIWidth();
                _baseData.x = EditorGUILayout.FloatField("Brightness(亮度)", _baseData.x);

                this.DrawUVanimWindowGUI(materialEditor, ref adata, ref _baseData, axis, ismain);
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("DrawMainTexGUI Mode");
                __data.SetValue0(value);
                __data.SetAnimData(adata);
                __data.SetBaseData(_baseData);
                __data.SetColor(c);
            }
            this.DrawBoxEColor();
        }

        /// <summary>
        /// UV动画的总显示GUI
        /// </summary>
        /// <param name="materialEditor"></param>
        /// <param name="adata">动画数据</param>
        void DrawUVanimWindowGUI(MaterialEditor materialEditor, ref Vector4 adata, ref Vector4 abase, string axis = "y", bool ismain = false)
        {
            string[] ssz = new string[] { "None", "Move", "Rotation", "Size" }; //,"Frame Loop"
            string[] ssss = new string[] { "Frame Rate", "Frame ", "Particle Custom." + axis };

            if (ismain)
            {
                ssss = new string[] { "Frame Rate", "Frame ", "Particle Custom.x" };
            }

            float scrollX = adata.x;
            float scrollY = adata.y;
            float scrollZ = adata.z;
            int RangeData = (int)abase.z;


            // int isValueType = (int)abase.w >> 7 & 0xf; // 是否启用顶点属性 15 0b1111
            // int isDist = (int)abase.w >> 6 & 0x1; // 是否扰动 1 0b1 
            // int isFilp = (int)abase.w >> 5 & 0x1; // 是否反相 1 0b1
            // int isClip = (int)abase.w >> 4 & 0x1; // 是否clip 1 0b1
            // int blend= (int)abase.w >> 0 & 0xf; //混合模式  15 0b1111

            int scrollW = (int)adata.w;

            this.IsPolar = (scrollW >> 4 & 0x1) > 0 ? true : false;
            int F_timemode = scrollW >> 5 & 0x7;
            int F_Tilesx = scrollW >> 8 & 0xf;
            int F_Tilesy = scrollW >> 12 & 0xf;

            int F_Max = RangeData >> 0 & 0xff;
            int F_Min = RangeData >> 8 & 0xff;


            int AnimType = scrollW & 0x3;
            int isAuto = this.IS_TIMEDT_ON ? (scrollW >> 2 & 0x1) : 0;
            int ISFAnimType = scrollW >> 3 & 0x1;

            string sp = isAuto > 0 ? "Speed" : "Offset";
            GUIContent POLARGC = EditorGUIUtility.TrTextContent("极" + (this.IsPolar ? "√" : "×"), "是否开启极坐标");
            GUIContent useTimeGC = EditorGUIUtility.TrTextContent(isAuto > 0 ? "UV 自动" : "UV 手动", "N:无动画,M:偏移UV,R:旋转UV\n同一动画不可共存");
            GUIContent IsFrameGC = EditorGUIUtility.TrTextContent(ISFAnimType > 0 ? "序列 On" : "序列 Off", "开启序列帧动画模式\nTiles是切片的横列,最多15x15\nStart和End是开始和结束帧,开始永远不会超过结束[所以无法倒放]\n  FrameRates 是帧率\n  Frame是0-1的拖动播放,开始到结束\n custom和粒子是一样的意思");
            // EditorGUIUtility.labelWidth = 100;
            this.SetDefaultBG();
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(POLARGC, this.setButtonFronCol("button", !this.IsPolar), GUILayout.MaxWidth(30)))
                {
                    this.IsPolar = !this.IsPolar;
                }
                if (AnimType > 0)
                {
                    if (GUILayout.Button(useTimeGC, GUILayout.MaxWidth(80), GUILayout.ExpandWidth(false)))
                    {
                        isAuto = 1 - isAuto;
                        isAuto = this.IS_TIMEDT_ON ? isAuto : 0;
                    }
                }
                else
                {
                    GUILayout.Button("UV 动画", GUILayout.MaxWidth(80), GUILayout.ExpandWidth(false));
                }
                AnimType = EditorGUILayout.Popup(AnimType, ssz, GUILayout.Width(100), GUILayout.ExpandWidth(AnimType == 0));
                EditorGUIUtility.labelWidth = 100;
                if (EditorGUIUtility.currentViewWidth < 450)
                { //|| uvanim == 4
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                Vector2 scroll;
                switch (AnimType)
                {
                    case 0: //None
                            // EditorGUILayout.TextArea(" ");
                        break;
                    case 1: //Move                           
                        EditorGUILayout.LabelField(sp, GUILayout.Width(50));
                        scroll = new Vector2(scrollX, scrollY);
                        scroll = EditorGUILayout.Vector2Field("", scroll);
                        scrollX = scroll.x;
                        scrollY = scroll.y;
                        break;
                    case 2: //Rotation
                        scrollX = EditorGUILayout.FloatField(sp, scrollX);
                        break;
                    case 3: //Size
                        EditorGUILayout.LabelField("size[待测]", GUILayout.Width(50));
                        scroll = new Vector2(scrollX, scrollY);
                        scroll = EditorGUILayout.Vector2Field("", scroll);
                        scrollX = scroll.x;
                        scrollY = scroll.y;
                        // EditorGUILayout.TextArea("暂未开发");
                        break;

                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(IsFrameGC, GUILayout.Width(80), GUILayout.ExpandWidth(ISFAnimType < 1)))
                {
                    ISFAnimType = 1 - ISFAnimType;
                }


                if (ISFAnimType > 0)
                {
                    F_timemode = EditorGUILayout.Popup(F_timemode, ssss, GUILayout.Width(100), GUILayout.ExpandWidth(false));
                    if (EditorGUIUtility.currentViewWidth < 450)
                    { //|| uvanim == 4
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                    if (F_timemode == 0)
                    {
                        scrollZ = (float)EditorGUILayout.IntField("Frame Rate", (int)scrollZ);
                    }
                    else if (F_timemode == 1)
                    {
                        scrollZ = EditorGUILayout.Slider("Frame", scrollZ, 0, 1);
                    }
                    else
                    {
                        EditorGUILayout.TextArea(ssss[F_timemode] + "是当前驱动属性");
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    EditorGUIUtility.labelWidth = 45;
                    F_Tilesx = EditorGUILayout.IntField("Tiles X", F_Tilesx);
                    F_Tilesy = EditorGUILayout.IntField("Tiles Y", F_Tilesy);
                    if (EditorGUI.EndChangeCheck())
                    {
                        F_Max = F_Tilesx * F_Tilesy;
                    }
                    if (EditorGUIUtility.currentViewWidth < 450)
                    { //|| uvanim == 4
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }
                    F_Min = EditorGUILayout.IntField("Start", F_Min);
                    F_Max = EditorGUILayout.IntField("End", F_Max);
                }

                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    m_MaterialEditor.RegisterPropertyChangeUndo("DrawUVanimWindowGUI Mode");
                    F_Tilesx = Mathf.Min(15, Mathf.Max(F_Tilesx, 1));
                    F_Tilesy = Mathf.Min(15, Mathf.Max(F_Tilesy, 1));
                    F_Max = Mathf.Min(255, Mathf.Max(F_Max, 1));
                    F_Min = Mathf.Min(F_Min, Mathf.Max(F_Max - 1, 0));
                    F_timemode = Mathf.Min(F_timemode, 15);

                    scrollW = AnimType + (isAuto << 2) + (ISFAnimType << 3) + (F_timemode << 5) + (this.IsPolar ? (1 << 4) : (0 << 4)) + (F_Tilesx << 8) + (F_Tilesy << 12);
                    RangeData = F_Max + (F_Min << 8);

                    abase = new Vector4(abase.x, abase.y, RangeData, abase.w);
                    adata = new Vector4(scrollX, scrollY, scrollZ, scrollW);
                }
            }
        }

        /// <summary>
        /// 菲尼尔的GUI界面
        /// </summary>
        /// <param name="materialEditor"></param>
        void DrawFNLGUI(MaterialEditor materialEditor, Material material)
        {
            this.DefGUIWidth();
            if (this.IS_FRESNEL_ON)
            {
                Color col = fnlColor.colorValue;
                Vector4 v = fnlvalue.vectorValue;
                Vector4 b = fnlBase.vectorValue;
                this.DrawBoxSColor();
                float ex0 = v.y;
                float ex1 = v.z;
                float light = v.w;

                float isdis = b.x;
                float apl = b.z;
                float blend = b.w;

                string[] fnlapltype = new string[] { "View All", "Use SrcAlp", "Use FnlAlp", "Only FnlAlp" };

                GUIContent aplGC = EditorGUIUtility.TrTextContent(" " + fnlapltype[(int)apl], "是否保留菲尼尔A通道,如果贴图没通道 推荐 关闭");
                GUIContent blendGC = EditorGUIUtility.TrTextContent(" " + fxblend[(int)blend], "菲尼尔与源色的融合模式!不包含A通道");
                GUIContent isdisGC = EditorGUIUtility.TrTextContent(" 扰动 " + IsONOFF[(int)isdis], "是否被扰动");

                GUIContent fnlcolGC = EditorGUIUtility.TrTextContent(" Fresnel Color");
                GUIContent fnlbright = EditorGUIUtility.TrTextContent(" Fresnel Brightness");


                EditorGUI.BeginChangeCheck();
                {
                    col = EditorGUILayout.ColorField(fnlcolGC, col, true, true, true);
                    light = EditorGUILayout.FloatField(fnlbright, light);
                    ex0 = EditorGUILayout.Slider(" Fresnel EXP0", ex0, -1f, 1.0f);
                    ex1 = EditorGUILayout.Slider(" Fresnel EXP1", ex1, -1f, 1.0f);

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (this.isDissMode)
                        {
                            if (GUILayout.Button(isdisGC, GUILayout.MaxWidth(80), GUILayout.MinWidth(50)))
                            { isdis = 1 - isdis; }
                        }
                        apl = (float)EditorGUILayout.Popup((int)apl, fnlapltype, GUILayout.MaxWidth(80), GUILayout.MinWidth(50));

                        if (apl < 3)
                        {
                            blend = (float)EditorGUILayout.Popup((int)blend, fxblend, GUILayout.MaxWidth(80), GUILayout.MinWidth(50));
                        }
                        else
                        {
                            GUILayout.Button("--", GUILayout.MaxWidth(80), GUILayout.MinWidth(50));
                        }

                    }
                    EditorGUILayout.EndHorizontal();

                    // if(GUILayout.Button(isdisGC,GUILayout.MaxWidth(60)))  { isdis = 1 - isdis; }

                    v.y = ex0;
                    v.z = ex1;
                    v.w = light;

                    b.x = isdis;
                    b.z = apl;
                    b.w = blend;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    m_MaterialEditor.RegisterPropertyChangeUndo("FNL Mode");
                    fnlColor.colorValue = col;
                    fnlvalue.vectorValue = v;
                    fnlBase.vectorValue = b;
                }
                string hbs = "Tips: 菲尼尔的 Render Type推荐 AlphaTest / Opaque";
                EditorGUILayout.HelpBox(hbs, MessageType.Info);
                // this.SetDefaultBG();
                this.DrawBoxEColor();
            }
        }

        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //*****************************************************************************************************************************

        /// <summary>
        /// 从Material的“Mode"属性读取一些主要的设置。
        /// </summary>
        /// <param name="material"></param>
        void LoadMode(Material material)
        {
            string shaderName = material.shader.name.Replace('/', '_');
            // AllShaderNameGUID
            Shader s = material.shader;// 
            String Sguid = "GUID_" + AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(s));

            AllShaderName n;
            AllShaderNameGUID nguid;
            if (!Enum.TryParse<AllShaderName>(shaderName, out n))
            {
                n = 0;
            }
            if (!Enum.TryParse<AllShaderNameGUID>(Sguid, out nguid))
            {
                isSameShader = false;
            }
            else
            {
                isSameShader = true;
            }
            this.nowmode = (int)mode.floatValue;
            this.ShaderMode = nowmode >> 0 & 0xff;
            this.renderTypeMode = (RenderTypeMode)(nowmode >> 8 & 0x3);
            this.IS_FRESNEL_ON = (nowmode >> 10 & 0x1) > 0;
            this.IS_TIMEDT_ON = (nowmode >> 11 & 0x1) > 0;
            this._shaderBlend = (nowmode >> 12 & 0xF); // 0 add 1 bnlend
            // Debug.Log(nowmode >> 12 & 0xF);
            this.isBillborad = (nowmode >> 16 & 0xF); //是否开启公告板对应各种公告板模式
            // this.IsPolar = (nowmode >> 20 & 0x1) > 0; 
            if (this.ShaderMode != (int)n)
            {
                int tmp = nowmode >> 8 << 8;
                mode.floatValue = tmp + (int)n;
                this.ShaderMode = (int)n;
            }
        }

        void UpdateMaterBase(Material material)
        {
            // this.UpdateMode(material);

            if (this.renderTypeMode == RenderTypeMode.Opaque)
            {

                srcBlend.floatValue = (int)UnityEngine.Rendering.BlendMode.One;
                dstBlend.floatValue = (int)UnityEngine.Rendering.BlendMode.Zero;
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                // this._shaderBlend = 2;
            }
            else
            {
                if (this._shaderBlend < 2)
                {
                    srcBlend.floatValue = this._shaderBlend < 1 ? (float)UnityEngine.Rendering.BlendMode.SrcAlpha : (float)UnityEngine.Rendering.BlendMode.SrcAlpha;
                    dstBlend.floatValue = this._shaderBlend < 1 ? (float)UnityEngine.Rendering.BlendMode.One : (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                }
            }
            //new string[]{"None","FX Billborad","VC Billborad","UI Clip"}

            // if(this.IsPolar){
            //     material.EnableKeyword("_POLAR_ON");
            // }
            // else{
            //     material.DisableKeyword("_POLAR_ON");
            // } 
            this.isBillborad = 0;
            switch (this.isBillborad)
            {
                case 0:
                    material.DisableKeyword("_DTBILLBORAD_FX");
                    material.DisableKeyword("_DTBILLBORAD_VC");
                    break;
                case 1:
                    material.EnableKeyword("_DTBILLBORAD_FX");
                    material.DisableKeyword("_DTBILLBORAD_VC");
                    // material.DisableKeyword("_DTBILLBORAD_UI");
                    break;
                case 2:
                    material.DisableKeyword("_DTBILLBORAD_FX");
                    material.EnableKeyword("_DTBILLBORAD_VC");
                    // material.DisableKeyword("_DTBILLBORAD_UI");
                    break;
            }
            if (IS_TIMEDT_ON)
            {
                material.EnableKeyword("_TIMEDT_ON");
            }
            else
            {
                material.DisableKeyword("_TIMEDT_ON");
            }
            if (IS_FRESNEL_ON)
            {
                material.EnableKeyword("_FRESNEL_ON");
            }
            else
            {
                material.DisableKeyword("_FRESNEL_ON");
            }
        }



        /// <summary>
        /// 刷新数据到Material的“Mode"属性。
        /// </summary>
        /// <param name="material"></param>
        void UpdateMode(Material material)
        {
            int m = (int)mode.floatValue;
            RenderTypeMode rtm = (RenderTypeMode)(m >> 8 & 0x3);
            if (rtm != this.renderTypeMode)
            {
                switch (this.renderTypeMode)
                {
                    case RenderTypeMode.Opaque:
                        material.SetOverrideTag("RenderType", "");
                        material.DisableKeyword("_ALPHATEST_ON");
                        srcBlend.floatValue = (int)UnityEngine.Rendering.BlendMode.One;
                        dstBlend.floatValue = (int)UnityEngine.Rendering.BlendMode.Zero;
                        this._zWrite = 1;
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.GeometryLast;
                        break;
                    case RenderTypeMode.AlphaTest:
                        material.SetOverrideTag("RenderType", "TransparentCutout");
                        material.EnableKeyword("_ALPHATEST_ON");
                        srcBlend.floatValue = (int)UnityEngine.Rendering.BlendMode.One;
                        dstBlend.floatValue = (int)UnityEngine.Rendering.BlendMode.Zero;
                        this._zWrite = 1;
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                        break;
                    case RenderTypeMode.Transparent:
                        material.SetOverrideTag("RenderType", "Transparent");
                        material.DisableKeyword("_ALPHATEST_ON");
                        srcBlend.floatValue = (int)UnityEngine.Rendering.BlendMode.SrcAlpha;
                        dstBlend.floatValue = (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                        this._zWrite = 0;
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        break;
                    default:
                        break;
                }
            }
            // if(this.IsPolar){
            //     material.EnableKeyword("_POLAR_ON");
            // }
            // else{
            //     material.DisableKeyword("_POLAR_ON");
            // } 
            switch (this.isBillborad)
            {
                case 0:
                    material.DisableKeyword("_DTBILLBORAD_FX");
                    material.DisableKeyword("_DTBILLBORAD_VC");
                    break;
                case 1:
                    material.EnableKeyword("_DTBILLBORAD_FX");
                    material.DisableKeyword("_DTBILLBORAD_VC");
                    // material.DisableKeyword("_DTBILLBORAD_UI");
                    break;
                case 2:
                    material.DisableKeyword("_DTBILLBORAD_FX");
                    material.EnableKeyword("_DTBILLBORAD_VC");
                    // material.DisableKeyword("_DTBILLBORAD_UI");
                    break;
            }

            if (IS_TIMEDT_ON)
            {
                material.EnableKeyword("_TIMEDT_ON");
            }
            else
            {
                material.DisableKeyword("_TIMEDT_ON");
            }
            if (IS_FRESNEL_ON)
            {
                material.EnableKeyword("_FRESNEL_ON");
            }
            else
            {
                material.DisableKeyword("_FRESNEL_ON");
            }

            zWrite.floatValue = (float)this._zWrite;
            cull.floatValue = (float)_cull;
            colorMask.floatValue = (float)_colorMask;
            srcBlend.floatValue = this._srcBlend;
            dstBlend.floatValue = this._dstBlend;

            int m1 = this.ShaderMode << 0;
            int m2 = (int)this.renderTypeMode << 8;
            int m3 = ((this.IS_FRESNEL_ON ? 1 : 0) << 10);
            int m4 = ((this.IS_TIMEDT_ON ? 1 : 0) << 11);
            int m5 = (int)this._shaderBlend << 12;
            int m6 = (int)this.isBillborad << 16;
            // int m7 = ((this.IsPolar ? 1 : 0 ) << 20);
            mode.floatValue = (float)(m1 + m2 + m3 + m4 + m5 + m6); //+m7
        }

        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //*****************************************************************************************************************************
        //*****************************************************************************************************************************

        void DefGUIWidth()
        {
            EditorGUIUtility.fieldWidth = 50;//Mathf.Max(50,EditorGUIUtility.currentViewWidth * 0.01f);
            EditorGUIUtility.labelWidth = Mathf.Max(115, EditorGUIUtility.currentViewWidth * 0.39f);

        }
        void DrawBoxSColor()
        {
            Color c;
            switch (this.ShaderMode)
            {
                case 0:
                    c = new Color(0.5f, 1f, 1.2f, 0.5f);
                    break;
                case 1:
                    c = new Color(1.6f, 0.5f, 0.8f, 0.5f);
                    break;
                case 2:
                    c = new Color(1.5f, 0.53f, 0.53f, 0.5f);
                    break;
                case 3:
                    c = new Color(0.167f, 0.9f, 0.67f, 0.5f);
                    break;
                default:
                    c = new Color(0.5f, 1f, 1f, 0.35f);
                    break;
            }

            GUIStyle sg = new GUIStyle("GroupBox");
            GUI.backgroundColor = c;
            // sg.normal.background = EditorGUIUtility.FindTexture("mini btn act");
            EditorGUILayout.BeginVertical(sg);

            this.SetDefaultBG();
        }
        void DrawBoxSColor2(Color c)
        {
            GUIStyle sg = new GUIStyle("GroupBox");
            GUI.backgroundColor = c;
            EditorGUILayout.BeginVertical(sg);

            this.SetDefaultBG();
        }
        void DrawBoxEColor()
        {
            EditorGUILayout.EndVertical();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sy"></param>
        /// <param name="s">true 红，false 绿</param>
        /// <returns></returns>
        GUIStyle setButtonFronCol(string sy, bool s)
        {
            GUIStyle fgs = new GUIStyle(sy);
            fgs.fontStyle = FontStyle.Bold;
            fgs.hover.textColor = Color.green;
            fgs.normal.textColor = Color.green;
            if (s)
            {
                fgs.normal.textColor = Color.red;
                fgs.hover.textColor = Color.red;
            }
            return fgs;
        }
        void SetDefaultBG()
        {
            GUI.backgroundColor = DefaultBGcol;
        }

        public TexDataForShader __SetTexDate(string name, MaterialProperty[] props)
        {
            TexDataForShader d = new TexDataForShader();
            d.Tex = FindProperty("_" + name + "Tex", props, false);
            d.IsTex = d.Tex != null;
            d.color = FindProperty("_" + name + "Color", props, false);
            d.IsColor = d.color != null;
            d.Value0 = FindProperty("_" + name + "Value", props, false);
            d.AnimData = FindProperty("_" + name + "AnimData", props, false);
            d.BaseData = FindProperty("_" + name + "Base", props, false);
            d.IsBaseData = d.BaseData != null;
            d.Value1 = FindProperty("_" + name + "Value1", props, false);
            d.Value2 = FindProperty("_" + name + "Value2", props, false);
            d.name = name;
            return d;
        }
        private void run(Material o)
        {
            Material mat = o as Material;
            if (mat == null)
            {
                return;
            }
            SerializedObject psSource = new SerializedObject(mat);

            SerializedProperty ShaderKeywords = psSource.FindProperty("m_ShaderKeywords");
            CleanMaterialShaderKeywords(ShaderKeywords, mat);

            SerializedProperty emissionProperty = psSource.FindProperty("m_SavedProperties");
            SerializedProperty texEnvs = emissionProperty.FindPropertyRelative("m_TexEnvs");
            SerializedProperty floats = emissionProperty.FindPropertyRelative("m_Floats");
            SerializedProperty colos = emissionProperty.FindPropertyRelative("m_Colors");
            CleanMaterialSerializedProperty(texEnvs, mat);
            CleanMaterialSerializedProperty(floats, mat);
            CleanMaterialSerializedProperty(colos, mat);


            psSource.ApplyModifiedProperties();

            EditorUtility.SetDirty(o);
            Debug.Log("清理完成!!");
        }

        private void CleanMaterialShaderKeywords(SerializedProperty property, Material mat)
        {

            string s = "";
            for (int i = 0; i < mat.shaderKeywords.Length; i++)
            {
                s = s + mat.shaderKeywords[i].ToString() + " ";
            }
            mat.shaderKeywords = null;
        }

        private void CleanMaterialSerializedProperty(SerializedProperty property, Material mat)
        {
            for (int j = property.arraySize - 1; j >= 0; j--)
            {
                string propertyName = property.GetArrayElementAtIndex(j).FindPropertyRelative("first").stringValue;
                if (!mat.HasProperty(propertyName))
                {
                    property.DeleteArrayElementAtIndex(j);
                }
            }
        }

        CustomBloomValue cbv ;
        private void FindPropertyCustomBloom(MaterialProperty[] props,out CustomBloomValue d)
        {
            d = new CustomBloomValue();
            d.Threshhold = FindProperty("_CBLocalThreshhold", props, false);
            d.color = FindProperty("_CBLocalColor", props, false);
            d.light = FindProperty("_CBLocalLight", props, false);
            d.LocalBool = FindProperty("_CBLocalBanBool", props, false);
            // return d;
        }

        void DrawBloomGUI(MaterialEditor materialEditor, Material materia, CustomBloomValue __data)
        {
            if (GameObject.FindObjectOfType<YFPostProcessing>() == null) { 
                return;
            }
            this.isbloomView = EditorGUILayout.Foldout(this.ismainView, "YF Bloom");
            if (this.isbloomView)
            {
                 

                this.DrawBoxSColor();
                this.DefGUIWidth();

                bool localbool;
                float threshhold;
                float lg;
                Color col;
                __data.GetValue(__data.LocalBool, out localbool);
                __data.GetValue(__data.Threshhold, out threshhold);
                __data.GetValue(__data.light, out lg);
                __data.GetValue(__data.color, out col);

                EditorGUI.BeginChangeCheck();
                EditorGUIUtility.labelWidth = 100;
                EditorGUILayout.BeginHorizontal();
                localbool = EditorGUILayout.Toggle(new GUIContent("独立关闭", "开启Bloom下想单独关闭当前材质球的Bloom效果。可打开这个开关。常用在UI上"), localbool);
                EditorGUILayout.LabelField(__data.IsGlobalBool() ? "全局Bloom状态开启中" : "全局Bloom状态关闭中", new GUIStyle("TextArea"));
                EditorGUILayout.EndHorizontal();
                col = EditorGUILayout.ColorField(new GUIContent("颜色", "独立的发光颜色,无HDR"), col);
                threshhold = EditorGUILayout.Slider(new GUIContent("Threshhold", "独立阈值"), threshhold, 0f, 1f);
                lg = EditorGUILayout.FloatField(new GUIContent("亮度", "独立的亮度强度"), lg);

                if (EditorGUI.EndChangeCheck())
                {
                    m_MaterialEditor.RegisterPropertyChangeUndo("DrawMainTexGUI Mode");
                    if (__data.IsGlobalBool())
                    {
                        lg = Mathf.Max(0f, lg);
                        __data.SetValue(__data.LocalBool, localbool);
                        __data.SetValue(__data.Threshhold, threshhold);
                        __data.SetValue(__data.light, lg);
                        __data.SetValue(__data.color, col);
                    }
                }
                this.DrawBoxEColor();
            }
        }

    }

    //*****************************************************************************************************************************
    //*****************************************************************************************************************************
    //*****************************************************************************************************************************
    //*****************************************************************************************************************************
    public class CustomBloomValue : CustomMaterialPropertyValue
    {
        public MaterialProperty Threshhold;
        public MaterialProperty color;
        public MaterialProperty light;
        public MaterialProperty LocalBool;

        public CustomBloomValue()
        {
            this.Threshhold = null;
            this.color = null;
            this.light = null;
            this.LocalBool = null;
        }

        public bool IsGlobalBool()
        {
            return Shader.GetGlobalFloat("_CBGlobalBool") > 0;
            // bool b = Camera.main.GetComponent<YFPostProcessing>()  != null;

        }
    }
    public class CustomMaterialPropertyValue
    {
        public void SetValue(MaterialProperty mp, bool v)
        {
            if (mp != null) { mp.floatValue = v ? 1f : 0f; }
        }
        public void GetValue(MaterialProperty mp, out bool v)
        {
            v = default(bool);
            if (mp != null) { v = mp.floatValue > 0; }
        }


        public void SetValue(MaterialProperty mp, float v)
        {
            if (mp != null) { mp.floatValue = v; }
        }
        public void GetValue(MaterialProperty mp, out float v)
        {
            v = default(float);
            if (mp != null) { v = mp.floatValue; }
        }
        public void SetValue(MaterialProperty mp, Color v)
        {
            if (mp != null) { mp.colorValue = v; }
        }
        public void GetValue(MaterialProperty mp, out Color v)
        {
            v = default(Color);
            if (mp != null) { v = mp.colorValue; }
        }

        public void SetValue(MaterialProperty mp, Vector4 v)
        {
            if (mp != null) { mp.vectorValue = v; }
        }
        public void GetValue(MaterialProperty mp, out Vector4 v)
        {
            v = default(Vector4);
            if (mp != null) { v = mp.vectorValue; }
        }
    }


    public class TexDataForShader
    {
        public string name;
        public MaterialProperty Tex; public bool IsTex;
        public MaterialProperty color; public bool IsColor;
        public MaterialProperty Value0;
        public MaterialProperty Value1;
        public MaterialProperty Value2;
        public MaterialProperty AnimData;
        public MaterialProperty BaseData; public bool IsBaseData;
        public TexDataForShader()
        {
            this.Tex = null; this.IsTex = false;
            this.color = null; this.IsColor = false;
            this.Value0 = null;
            this.AnimData = null;
            this.BaseData = null; this.IsBaseData = false;
            this.Value1 = null;
            this.Value2 = null;
        }
        public void SetValue0(Vector4 v)
        {
            if (this.Value0 != null)
            { this.Value0.vectorValue = v; }
        }
        public void SetValue1(Vector4 v)
        {
            if (this.Value1 != null)
            { this.Value1.vectorValue = v; }
        }
        public void SetValue2(Vector4 v)
        {
            if (this.Value2 != null)
            { this.Value2.vectorValue = v; }
        }
        public void SetAnimData(Vector4 v)
        {
            if (this.AnimData != null)
            { this.AnimData.vectorValue = v; }
        }
        public void SetBaseData(Vector4 v)
        {
            if (this.BaseData != null)
            { this.BaseData.vectorValue = v; }
        }
        public void SetColor(Color v)
        {
            if (this.color != null)
            { this.color.colorValue = v; }
        }
        public Vector4 getValue0()
        {
            if (this.Value0 == null) { return Vector4.zero; }
            else { return this.Value0.vectorValue; }
        }
        public Vector4 getValue1()
        {
            if (this.Value1 == null) { return Vector4.zero; }
            else { return this.Value1.vectorValue; }
        }
        public Vector4 getValue2()
        {
            if (this.Value2 == null) { return Vector4.zero; }
            else { return this.Value2.vectorValue; }
        }
        public Vector4 getAnimData()
        {
            if (this.AnimData == null) { return Vector4.zero; }
            else { return this.AnimData.vectorValue; }
        }
        public Vector4 getBaseData()
        {
            if (this.BaseData == null) { return Vector4.zero; }
            else { return this.BaseData.vectorValue; }
        }
        public Color getColor()
        {
            if (this.color == null) { return Color.white; }
            else { return this.color.colorValue; }
        }
    }

    //*****************************************************************************************************************************
    //*****************************************************************************************************************************
    //*****************************************************************************************************************************
    //*****************************************************************************************************************************


    public class CleanMaterial
    {
        private static bool CleanOneMaterial(Material _material)
        {
            // 收集材质使用到的所有纹理贴图
            HashSet<string> textureGUIDs = CollectTextureGUIDs(_material);

            string materialPathName = Path.GetFullPath(AssetDatabase.GetAssetPath(_material));

            StringBuilder strBuilder = new StringBuilder();
            using (StreamReader reader = new StreamReader(materialPathName))
            {
                Regex regex = new Regex(@"\s+guid:\s+(\w+),");
                string line = reader.ReadLine();
                while (null != line)
                {
                    if (line.Contains("m_Texture:"))
                    {
                        // 包含纹理贴图引用的行，使用正则表达式获取纹理贴图的guid
                        Match match = regex.Match(line);
                        if (match.Success)
                        {
                            string textureGUID = match.Groups[1].Value;
                            if (textureGUIDs.Contains(textureGUID))
                            {
                                strBuilder.AppendLine(line);
                            }
                            else
                            {
                                // 材质没有用到纹理贴图，guid赋值为0来清除引用关系
                                strBuilder.AppendLine(line.Substring(0, line.IndexOf("fileID:") + 7) + " 0}");
                                Debug.Log(textureGUID);
                            }
                        }
                        else
                        {
                            strBuilder.AppendLine(line);
                        }
                    }
                    else
                    {
                        strBuilder.AppendLine(line);
                    }

                    line = reader.ReadLine();
                }
            }

            using (StreamWriter writer = new StreamWriter(materialPathName))
            {
                writer.Write(strBuilder.ToString());
            }

            return true;
        }

        private static HashSet<string> CollectTextureGUIDs(Material _material)
        {
            HashSet<string> textureGUIDs = new HashSet<string>();
            for (int i = 0; i < ShaderUtil.GetPropertyCount(_material.shader); ++i)
            {
                if (ShaderUtil.ShaderPropertyType.TexEnv == ShaderUtil.GetPropertyType(_material.shader, i))
                {
                    Texture texture = _material.GetTexture(ShaderUtil.GetPropertyName(_material.shader, i));
                    if (null == texture)
                    {
                        continue;
                    }

                    string textureGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(texture));
                    if (!textureGUIDs.Contains(textureGUID))
                    {
                        textureGUIDs.Add(textureGUID);
                    }
                }
            }

            return textureGUIDs;
        }
    }
#endif
}
