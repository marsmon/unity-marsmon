
// #define DT_BUTTON
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System;

using YF.Art.PostProcessing;

namespace DiteScripts.YF.EditoShaderGUI.Div
{
    internal partial class ShaderGUIBase : ShaderGUI
    {
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            this.DefaultBGcol = GUI.backgroundColor;
            FindProperties(props);

            Material material = materialEditor.target as Material;
            this.m_MaterialEditor = materialEditor;
            this.initMaterialData(material);
            if (!this.isSameShader)
            {
                this.ErrorHelpBox(material);
                return;
            }
            this.DefGUIWidth();
            //Draw MainProp
            this.DrawMainPropGUI(materialEditor, material);
            this.DrawBloomGUI(materialEditor, material, cbv);
            //自定的新属性
            if (!this.AddNewCustomProperty(materialEditor, props))
            {
                //原始的内容
                base.OnGUI(materialEditor, props);
            }
            //清理插件
            if (GUILayout.Button("清理材质球", GUILayout.Height(30)))
            {
                this.ClearMaterial(material);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            materialEditor.SetDefaultGUIWidths();
            HelpOnGUI("");
        }
    }
    // bloom
    internal partial class ShaderGUIBase  
    {
        CustomBloomValue cbv;
        private void FindPropertyCustomBloom(MaterialProperty[] props,out CustomBloomValue d)
        {
            d = new CustomBloomValue();
            d.Threshhold = FindProperty("_CBLocalThreshhold", props, false);
            d.color = FindProperty("_CBLocalColor", props, false);
            d.light = FindProperty("_CBLocalLight", props, false);
            d.LocalBool = FindProperty("_CBLocalBanBool", props, false);
        }
        bool isbloomView = true;
        void DrawBloomGUI(MaterialEditor materialEditor, Material materia, CustomBloomValue __data)
        {
            if (GameObject.FindObjectOfType<YFPostProcessing>() == null) { 
                return;
            }
            this.isbloomView = EditorGUILayout.Foldout(this.isbloomView, "YF Bloom");
            if (this.isbloomView)
            {                
                this.DrawBoxColor_Start();
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
                    // Debug.Log(__data.IsGlobalBool());
                    if (__data.IsGlobalBool())
                    {
                        lg = Mathf.Max(0f, lg);
                        __data.SetValue(__data.LocalBool, localbool);
                        __data.SetValue(__data.Threshhold, threshhold);
                        __data.SetValue(__data.light, lg);
                        __data.SetValue(__data.color, col);
                    }
                }
                // this.DrawBoxEColor();
                 this.DrawBoxColor_End();
            }
        }
    }

    //===========================================================================================
    //                                      ShaderGUIBaseUtil
    //===========================================================================================
    #region  ShaderGUIBaseUtil
    public enum AllShaderName
    {
        None = -1,
        YoFi_Effect_Particle_Base = 0,
        YoFi_Effect_ColorMask_Base = 1,
        YoFi_Effect_Dissolution_Base,
        YoFi_Effect_Distort_Base,
       Distort_Particle,


    }
    public enum AllShaderNameGUID
    {
        GUID_00000000000000000000000000000000 = -1,
        GUID_210be49b330e14c4eae96131e45ed5c1 = 0,
        GUID_b31bcfefb30f2684d82ced76b7ae0bf2 = 1,
        GUID_50c7ac9c667727f44974b1f6e95472bc,
        GUID_0799d73dc9852d740890492de486904f,
        GUID_16eae8dfb50f70d4792b96e20c2ca5eb
    }

    public enum MaterialSettingFlagsEnum
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
    #endregion
    //===========================================================================================
    //                                      ShaderGUIBase_Private
    //===========================================================================================
    #region ShaderGUIBasePrivate
    internal partial class ShaderGUIBase
    {
        void initMaterialData(Material material)
        {
            LoadMode(material);
            UpdateMaterBase(material);
        }
        protected virtual void InitShaderMode(int value)
        {
            this.ShaderMode = value;
        }
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

            // AllShaderName n;
            AllShaderNameGUID nguid;

            if (!Enum.TryParse<AllShaderNameGUID>(Sguid, out nguid))
            {
                isSameShader = false;
            }
            else
            {
                isSameShader = true;
            }
            this.ShaderMode = (int)nguid;
            this.nowmode = (int)mode.floatValue;
            this.renderTypeMode = (RenderTypeMode)(nowmode >> 0 & 0x7);
            this._shaderBlend = (nowmode >> 3 & 0x1); // 0 add 1 bnlend

        }

        void UpdateMaterBase(Material material)
        {
            this.UpdateMode(material);

            if (this.renderTypeMode == RenderTypeMode.Opaque)
            {
                srcBlend.floatValue = (int)UnityEngine.Rendering.BlendMode.One;
                dstBlend.floatValue = (int)UnityEngine.Rendering.BlendMode.Zero;
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            }
            else
            {
                if (this._shaderBlend < 2)
                {
                    srcBlend.floatValue = this._shaderBlend < 1 ? (float)UnityEngine.Rendering.BlendMode.SrcAlpha : (float)UnityEngine.Rendering.BlendMode.SrcAlpha;
                    dstBlend.floatValue = this._shaderBlend < 1 ? (float)UnityEngine.Rendering.BlendMode.One : (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                }
            }
        }
        /// <summary>
        /// 刷新数据到Material的“Mode"属性。
        /// </summary>
        /// <param name="material"></param>
        protected virtual void UpdateMode(Material material)
        {
            int m0 = (int)mode.floatValue;
            RenderTypeMode rtm = (RenderTypeMode)(m0 >> 0 & 0x3);
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

            zWrite.floatValue = (float)this._zWrite;
            cull.floatValue = (float)_cull;
            colorMask.floatValue = (float)_colorMask;
            srcBlend.floatValue = this._srcBlend;
            dstBlend.floatValue = this._dstBlend;

            int m1 = (int)this.renderTypeMode << 0; //##
            int m2 = (int)this._shaderBlend << 3; // ## 1位 0 ADD 1 blend

            mode.floatValue = (float)(m1 + m2);
        }



        void DefGUIWidth()
        {
            EditorGUIUtility.fieldWidth = 50;//Mathf.Max(50,EditorGUIUtility.currentViewWidth * 0.01f);
            EditorGUIUtility.labelWidth = Mathf.Max(115, EditorGUIUtility.currentViewWidth * 0.39f);
        }
        /// <summary>
        /// 刷新Shader的RenderType.必须手动点击。不然更新会延迟
        /// </summary>
        /// <param name="material"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sy"></param>
        /// <param name="s">true 红，false 绿</param>
        /// <returns></returns>
        protected GUIStyle setButtonFronCol(string sy, bool s)
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

        void ErrorHelpBox(Material material)
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
        }
        private void ClearMaterial(Material o)
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
    }
    #endregion
    //===========================================================================================
    //                                      ShaderGUIBaseProperty
    //===========================================================================================
    #region ShaderGUIBaseProperty
    /*
    Shader._mode 二进制转整数记录
    32位， ####,####,####,####,####,####,[# blend][### RenderTypeMode]
      RenderTypeMode: 留3位，共可拥有7个类型
      blend: 0 add 1 bnlend
    
    */
    internal partial class ShaderGUIBase
    {
        protected readonly string[] culldata = new string[3] { "Cull Off", "Cull Front", "Cull Back" };
        protected readonly string[] culldata2 = new string[3] { "N", "F", "B" };
        protected readonly string[] fxblend = new string[3] { "Add", "Blend", "Multiply" }; //MULTIPLY
        protected readonly string[] IsONOFF = new string[2] { "Off", "On" };
        protected readonly string[] MaterialBlendMode = new string[] { "Additive", "Alpha Blend", "Nomal" };

        protected readonly string[] uvNode0 = new string[] { "None", "Move", "Rotation", "Size" }; //,"Frame Loop"
        protected readonly string[] uvNode1_1 = new string[] { "Frame Rate", "Frame ", "Particle Custom.y" };
        protected readonly string[] uvNode1_2 = new string[] { "Frame Rate", "Frame ", "Particle Custom.x" };

        protected const string channelStr = " Tex(RGBA)";

        protected MaterialEditor m_MaterialEditor;
        protected int nowmode;

        int ShaderMode = -1;
        //比对GUID是否一样
        bool isSameShader;
        Color DefaultBGcol;
        int _shaderBlend = 0;
        int isBillborad = 0;
        RenderTypeMode renderTypeMode = 0;
        // int ShaderMode = -1;

        int _zWrite = 0;
        int _cull = 0;
        int _colorMask = 14;
        int _srcBlend = 1;
        int _dstBlend = 1;

        protected MaterialProperty mode = null;
        MaterialProperty cull = null;
        MaterialProperty zWrite = null;
        MaterialProperty srcBlend = null;
        MaterialProperty dstBlend = null;
        MaterialProperty colorMask = null;
    }
    #endregion
    //===========================================================================================
    //                                      ShaderGUIBaseVirtual
    //===========================================================================================
    #region ShaderGUIBaseVirtual
    internal partial class ShaderGUIBase
    {
        protected virtual void HelpOnGUI(string HelpBox){
            if(HelpBox == "") return;
            EditorGUILayout.HelpBox(HelpBox,MessageType.Info);
        }
        protected virtual void CustomOnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {

        }
        /// <summary>
        /// 自定义新界面的函数
        /// </summary>
        /// <param name="materialEditor"></param>
        /// <param name="props"></param>
        /// <returns>True 代表使用新的界面，如果false，则直接使用源生界面</returns>
        protected virtual bool AddNewCustomProperty(MaterialEditor materialEditor, MaterialProperty[] props)
        {
#if Dite_Div
            int debugUalue = (int)mode.floatValue;
            string binaryString = Convert.ToString(debugUalue, 2).PadLeft(32, '0');
            char[] a = binaryString.ToCharArray();//   .Split();
            string ss = String.Empty;
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0 && i % 4 == 0)
                {
                    ss += " ,";
                }
                ss += a[i];
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Debug Mode \n" + ss, new GUIStyle("AC BoldHeader"), GUILayout.Height(55));
            EditorGUILayout.EndHorizontal();
#endif
            // materialEditor.PropertiesDefaultGUI(npl.ToArray());//zTest
            return false;
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

        public virtual void FindProperties(MaterialProperty[] props)
        {
            mode = FindProperty("_mode", props);
            cull = FindProperty("_Cull", props);
            zWrite = FindProperty("_ZWrite", props);
            colorMask = FindProperty("_ColorMask", props);
            srcBlend = FindProperty("_SrcBlend", props);
            dstBlend = FindProperty("_DstBlend", props);
            this.FindPropertyCustomBloom(props,out cbv);
        }

        protected virtual void DrawBoxColor_Start()
        {
            Color c = new Color(0.5f, 1f, 1f, 0.35f);
            GUIStyle sg = new GUIStyle("GroupBox");
            GUI.backgroundColor = c;
            EditorGUILayout.BeginVertical(sg);
            this.SetDefaultBG();
        }
        protected virtual void DrawBoxColor_End()
        {
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Shader 主要属性的设置Gui
        /// </summary>
        /// <param name="materialEditor"></param>
        /// <param name="material"></param>
        protected virtual void DrawMainPropGUI(MaterialEditor materialEditor, Material material)
        {
            this._cull = (int)cull.floatValue;
            this._zWrite = (int)zWrite.floatValue;
            this._colorMask = (int)colorMask.floatValue;
            this._srcBlend = (int)srcBlend.floatValue;
            this._dstBlend = (int)dstBlend.floatValue;

            GUIContent RenderTypeGC = EditorGUIUtility.TrTextContent("Render Type", " Transparent:3000+\n AlphaTest: 2450+\n Opaque: 2500+");
            GUIContent BlendypeGC = EditorGUIUtility.TrTextContent("Render Blend", " Add or Alpha blend");
            GUIContent ZGC = EditorGUIUtility.TrTextContent("Z" + (_zWrite > 0 ? "√" : "×"), "ZWrite on/off(红关,绿开)");
            GUIContent CGC = EditorGUIUtility.TrTextContent("C", "Cull 开关剔除的面,\n  Cn:不剔除,双面(默认),\n  Cf:剔除正面\n  Cb:剔除背面");

            EditorGUIUtility.labelWidth = 0f;
            this.DrawBoxColor_Start();
            int buttonW = 40;

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.LabelField(" [" + (AllShaderName)this.ShaderMode + "] ", new GUIStyle("AC BoldHeader"), GUILayout.Height(25));
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
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                this.ButtomGroup_Increment(buttonW);

                if (GUILayout.Button((_colorMask == 15 ? "RGBA" : "RGB"), GUILayout.MaxWidth(buttonW))) { _colorMask ^= (1 << 0); }

                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                this.UpdateMode(material);
            }
            this.DrawBoxColor_End();
        }

        protected virtual void ButtomGroup_Increment(int buttonW)
        {
            return;
        }
    }
    #endregion
}
