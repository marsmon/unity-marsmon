
using System;
using System.Reflection;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace YF.Art.YFShaderEditor
{
    using YFut = YFUtilFun;

    
    public enum AllShaderNameGUID
    {
        GUID_210be49b330e14c4eae96131e45ed5c1 = 0,
        GUID_b31bcfefb30f2684d82ced76b7ae0bf2 = 1,
        GUID_50c7ac9c667727f44974b1f6e95472bc,
        GUID_0799d73dc9852d740890492de486904f,
        GUID_ca337594b0119c2478238181b329bd75,
        GUID_94fc445624049124493e3477a48d76ac

    }

    #region  ShaderGUI
    internal partial class YoFi_FXShader_GUIBase : ShaderGUI
    {
        public static Color DefaultBGcol;
        MaterialProperty[] m_props;
        YFfxModel viewSelect = YFfxModel.TIMEDT;
        YF_MatRenderProperty BaseRenderData = new YF_MatRenderProperty();
        YF_MainMatTexProperty MainMode = new YF_MainMatTexProperty();
        YF_DistMatTexProperty DistMode = new YF_DistMatTexProperty();
        YF_FNLMatTexProperty FNLMode = new YF_FNLMatTexProperty();
        YF_ColorMaskMatTexProperty ColMaskMode = new YF_ColorMaskMatTexProperty();
        YF_DissolutionMatTexProperty DissMode = new YF_DissolutionMatTexProperty();
        YF_SproutMatTexProperty SproutMode = new YF_SproutMatTexProperty();
        YF_GroundMatTexProperty GroundMode = new YF_GroundMatTexProperty();
        YF_BloomMatTexProperty BloomMode = new YF_BloomMatTexProperty();
        Shader m_shader;
        int selectGridIdx;
        int oldmode;
        List<YFfxModel> selectGridStrsInd = new List<YFfxModel>();
        List<string> selectGridStrs = new List<string>();
        bool issame = false;
        bool m_isFrist = true;

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            String Sguid = "GUID_" + AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newShader));
            AllShaderNameGUID nguid;
            issame = Enum.TryParse<AllShaderNameGUID>(Sguid, out nguid);
            material.shader = newShader;
            m_isFrist = true;
            if (!issame)
            {
                return;
            }
            base.AssignNewShaderToMaterial(material, oldShader, newShader);
        }


#if UNITY_2021_1_OR_NEWER
        public override void ValidateMaterial(Material material)
        {
            if (material.shader != m_shader)
            {
                BaseRenderData.CheckShaderKeys(material);
                m_shader = material.shader;
            }
            base.ValidateMaterial(material);
        }
        void _OnStart(Material material){}
#else
        public virtual void ValidateMaterial(Material material)
        {
            if(material == null) return;
            if (material.shader != m_shader)
            {
                BaseRenderData.CheckShaderKeys(material);
                m_shader = material.shader;
            } 
        }
        void _OnStart(Material material)
        {
            ValidateMaterial(material);
        }
#endif

        int GetShaderMode(Shader newShader)
        {
            MaterialProperty shadermode = FindProperty("_Shadermode", m_props, false);
            String Sguid = "GUID_" + AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newShader));
            AllShaderNameGUID nguid;
            issame = Enum.TryParse<AllShaderNameGUID>(Sguid, out nguid);

            if (shadermode == null) return -1;
            return (int)shadermode.floatValue;
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            m_props = props;
            DefaultBGcol = GUI.backgroundColor;
            Material material = materialEditor.target as Material;

            if (m_isFrist)
            {
                _OnStart(material);
                m_isFrist = false;
            }
            // AddNewCustomProperty(materialEditor, props);
            Color c = new Color(0.5f, 1f, 1f, 1f);
            Color c0 = GUI.backgroundColor;
            BaseRenderData.isv = true;
            MainMode.isv = true;
            BaseRenderData.MatBGVolor = c;
            MainMode.MatBGVolor = c;
            int ShaderMode = GetShaderMode(material.shader);
            if (!this.issame)
            {
                String Sguid = "GUID_" + AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material.shader));
                if (ShaderMode > -1)
                {
                    string errorstr = @"严重错误:!!!
GUID 不同,请重新导入,或使用GUID对应的shader!
默认的GUID:
    {0}
当前的GUID:
    {1}
请检查!!!!";
                    EditorGUILayout.HelpBox(string.Format(errorstr, (AllShaderNameGUID)ShaderMode, Sguid), MessageType.Error);

                    return;
                }
                else
                {
                    string errorstr = @"严重错误:!!!
该shader存在严重错误,输入未录入shader
当前的GUID:
    {0}
请检查!!!!";
                    EditorGUILayout.HelpBox(string.Format(errorstr, Sguid), MessageType.Error);

                    return;
                }
            }
            //正式的界面
            EditorGUILayout.BeginVertical();
            BaseRenderData.DrawPropGUI(materialEditor, props);
            // MainMode
            MainMode.DrawPropGUI(materialEditor, props);
            if (btnGroup(this.GetMatMode()))
            {
                GUI.backgroundColor = c;
                EditorGUILayout.BeginVertical(new GUIStyle("GroupBox"));
                if (selectGridStrs.Count > 1)
                {
                    this.selectGridIdx = GUILayout.Toolbar(this.selectGridIdx, selectGridStrs.ToArray(), GUILayout.Height(32));
                }
                else if (selectGridStrs.Count > 0)
                {
                    EditorGUILayout.LabelField(this.selectGridStrs[0], new GUIStyle("AC BoldHeader"), GUILayout.Height(26));
                }
                GUI.backgroundColor = c0;
                EditorGUILayout.BeginVertical("Box");
                if (this.selectGridIdx < this.selectGridStrsInd.Count)
                {
                    switch (this.selectGridStrsInd[this.selectGridIdx])
                    {
                        case YFfxModel.Fresnel:
                            if (BaseRenderData.m_Fresnel_ON.value)
                            {
                                FNLMode.DrawPropGUI(materialEditor, props);
                            }
                            break;
                        case YFfxModel.Distort:
                            if (BaseRenderData.m_Distort_ON.value)
                            {
                                DistMode.DrawPropGUI(materialEditor, props);
                            }
                            break;
                        case YFfxModel.ColorMask:
                            if (BaseRenderData.m_ColorMask_ON.value)
                            {
                                ColMaskMode.DrawPropGUI(materialEditor, props);
                            }
                            break;
                        case YFfxModel.Dissolution:
                            if (BaseRenderData.m_Dissolution_ON.value)
                            {
                                DissMode.DrawPropGUI(materialEditor, props);
                            }
                            break;
                        case YFfxModel.GroundClip:
                            if (BaseRenderData.m_isGroundClip.value)
                            {
                                GroundMode.DrawPropGUI(materialEditor, props);
                            }
                            break;
                        case YFfxModel.Sprout:
                            if (BaseRenderData.m_Sprout_ON.value)
                            {
                                SproutMode.DrawPropGUI(materialEditor, props);
                            }
                            break;
                        default:
                            if (BloomMode.GetIsRun())
                            {
                                BloomMode.DrawPropGUI(materialEditor, props);
                            }
                            break;
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            if (GUILayout.Button("清理材质球", GUILayout.Height(30)))
            {
                // Material material = materialEditor.target as Material;
                material.CleanMaterial(this.GetMatMode());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            materialEditor.SetDefaultGUIWidths();
            materialEditor.RenderQueueField();
            materialEditor.EnableInstancingField();
            materialEditor.DoubleSidedGIField();

            // EditorGUILayout.Space(20);
            // base.OnGUI(materialEditor, props);

        }
        protected virtual bool AddNewCustomProperty(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            // int debugUalue = this.GetMatMode();
            // debugUalue.ToBitViewGUI();
            return false;
        }
        int GetMatMode()
        {
            return (int)FindProperty("_mode", m_props).floatValue;
        }
        void SetDefaultBG()
        {
            GUI.backgroundColor = DefaultBGcol;
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
        bool btnGroup(int modeid)
        {
            if (oldmode == modeid)
            {
                return this.selectGridStrsInd.Count > 0;
            }
            oldmode = modeid;
            var collection = System.Enum.GetValues(typeof(YFfxModel));
            selectGridStrs.Clear();
            selectGridStrsInd.Clear();

            foreach (YFfxModel item in collection)
            {
                if (item == YFfxModel.ALPHATEST || item == YFfxModel.TIMEDT)
                {
                    continue;
                }
                int tmp = modeid >> ((int)item) & 0x1;
                if (tmp.ToBool())
                {
                    switch (item)
                    {
                        case YFfxModel.Fresnel:
                            selectGridStrs.Add("FNL");
                            break;
                        case YFfxModel.Distort:
                            selectGridStrs.Add("扰动");
                            break;
                        case YFfxModel.ColorMask:
                            selectGridStrs.Add("遮罩");
                            break;
                        case YFfxModel.Dissolution:
                            selectGridStrs.Add("溶解");
                            break;
                        case YFfxModel.GroundClip:
                            selectGridStrs.Add("地面Y");
                            break;
                        case YFfxModel.Sprout:
                            selectGridStrs.Add("生长");
                            break;
                        default:
                            break;
                    }

                    selectGridStrsInd.Add(item);
                }
            }

            if (BloomMode.GetIsRun())
            {
                selectGridStrs.Add("Bloom");
                selectGridStrsInd.Add(YFfxModel.TIMEDT);
            }
            return this.selectGridStrsInd.Count > 0;
        }
    }
    #endregion 



     
    #region MatBaseProperty
    public abstract class YF_BaseMatProperty : ShaderGUI
    {
        Color DefaultBGcol;
        public Color MatBGVolor;
        bool isFrist = true;
        protected MaterialEditor m_materialEditor;
        protected MaterialProperty[] m_props;
        protected Material m_material;
        public bool isv = false;
        public YF_BaseMatProperty()
        {
            isFrist = true;
        }        
        public virtual void DrawPropGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            this.m_material = materialEditor.target as Material;
            if (this.m_material == null)
            {
                Debug.LogError("MatBaseProperty.m_materialEditor.target == null! DiteError");
                return;
            }
            this.m_materialEditor = materialEditor;
            this.m_props = props;
            this.DefaultBGcol = GUI.backgroundColor;
            if (isFrist)
            {
                this._OnStart();
                isFrist = false;
            }
            this._FindProperty();
            EditorGUIUtility.labelWidth = 0f;
            if (isv)
            {
                GUIStyle sg = new GUIStyle("GroupBox");
                GUI.backgroundColor = this.MatBGVolor;
                EditorGUILayout.BeginVertical(sg);
            }
            else
            {
                EditorGUILayout.BeginVertical();
            }
            this.DefGUIColor();
            YFut.SetGUIWidth();
            EditorGUI.BeginChangeCheck();
            this._OnDrawPropGUI();
            if (EditorGUI.EndChangeCheck())
            {
                this._OnChandeEndDrawPropGUI();
            }
            this._OnUpdate();
            EditorGUILayout.EndVertical();
            this.DefGUIColor();
        }
        protected virtual void _OnStart() { }
        protected virtual void _OnUpdate() { }
        protected virtual void _OnDrawPropGUI()
        {
            this._DrawGUI();
        }
        protected virtual void DefGUIColor()
        {
            GUI.backgroundColor = DefaultBGcol;
        }

        protected abstract void _FindProperty();
        protected abstract void _DrawGUI();
        protected abstract void _OnChandeEndDrawPropGUI();

    }
    #endregion 



    
    #region -MatRenderProperty
    class YF_MatRenderProperty : YF_BaseMatProperty
    {
        protected override void _OnStart()
        {
            this.CheckShaderKeys(m_material);
            // this.CacheRenderersUsingThisMaterial(m_material);
        }
        protected override void _OnChandeEndDrawPropGUI()
        {
            this.UpdateProperty(this.local_RenderType);
        }
        protected override void _FindProperty()
        {
            mp_mode = FindProperty("_mode", m_props);
            mp_Cull = FindProperty("_Cull", m_props, false);
            mp_ZWrite = FindProperty("_ZWrite", m_props, false);
            mp_ColorMask = FindProperty("_ColorMask", m_props);
            mp_SrcBlend = FindProperty("_SrcBlend", m_props);
            mp_DstBlend = FindProperty("_DstBlend", m_props);
            mp_IsClipGround = FindProperty("_ClipGroundbitSw", m_props, false); //_ClipGroundbitSw _IsClipGround
            mp_ClipValue = FindProperty("_ClipValue", m_props, false);
            this.GetData((int)mp_mode.floatValue);
        }
        public int GetSelMode()
        {
            if (mp_mode == null)
            {
                return 0;
            }
            return (int)mp_mode.floatValue;
        }
        public RenderTypeMode local_RenderType;
        #region mode的2进制结构 当前23位了 总共32位 
        public RenderTypeMode m_RenderType; static readonly int ID_RenderType = 0; static readonly uint OX_RenderType = 0x3;// ###
        public MaterialBlend m_Blendmode; static readonly int ID_Blend = 2; static readonly uint OX_Blend = 0x3;// #
        public float m_ClipValue;
        public BlendMode m_SrcBlend;
        public BlendMode m_DstBlend;
        public CullMode m_Cull; static readonly int ID_Cull = 4; static readonly uint OX_Cull = 0x3;// ##
        public int m_Zwrite; static readonly int ID_Zwrite = 6; static readonly uint OX_Zwrite = 0x1;// #
        // public int m_ColorMask; static readonly int ID_ColorMask = 7; static readonly uint OX_ColorMask = 0x1;// #
        public YFIntBtn m_ColorMask = new YFIntBtn(7, 0x1, "RGBA", "Color Mask,RT 需要通道时候必须开启", "", "RGB");
        public YFbooleraKeyBtn m_ALPHATEST_ON = new YFbooleraKeyBtn(8, "_ALPHATEST_ON", "", "", "");
        public YFbooleraKeyBtn m_TIMEDT_ON = new YFbooleraKeyBtn(9, "_TIMEDT_ON", "T", "自动时间", "T:自动时间 |");
        public YFbooleraKeyBtn m_Fresnel_ON = new YFbooleraKeyBtn(10, "_FRESNEL_ON", "菲", "菲尼尔", "菲:菲尼尔 |");
        public YFbooleraKeyBtn m_Distort_ON = new YFbooleraKeyBtn(11, "_DISTOR_ON", "扰", "扰动", "Dt:扰动 |");
        public YFbooleraKeyBtn m_ColorMask_ON = new YFbooleraKeyBtn(12, "_COLORMASK_ON", "遮", "混合颜色遮罩 ColorMask", "遮:ColorMask |");
        public YFbooleraKeyBtn m_Dissolution_ON = new YFbooleraKeyBtn(13, "_DISSOLUTION_ON", "溶", "溶解", "Ds:溶解 |");
        public YFbooleraKeyBtn m_Sprout_ON = new YFbooleraKeyBtn(14, "_SPROUT_ON", "生", "模拟藤蔓生长", "生:生长 |");
        public YFbooleraBtn m_isGroundClip = new YFbooleraBtn(15, "地", "地下裁切", "地:地下裁切 |");
        #endregion 

        MaterialProperty mp_mode; MaterialProperty mp_Cull; MaterialProperty mp_ZWrite;
        MaterialProperty mp_SrcBlend; MaterialProperty mp_DstBlend; MaterialProperty mp_ColorMask; MaterialProperty mp_IsClipGround;// MaterialProperty mp_CBLocalBanBool;
        MaterialProperty mp_ClipValue;
        int keyCache = 0;

        List<ParticleSystemRenderer> m_RenderersUsingThisMaterial = new List<ParticleSystemRenderer>();
        public void CheckShaderKeys(Material mat)
        {
            mat.CheckShaderKeys(ref this.keyCache);
        }
        protected override void _DrawGUI()
        {
            GUIContent RenderTypeGC, BlendypeGC, CGC;
            YFut.CreatGUIContent(out RenderTypeGC, "Render Type", " Transparent:3000+\n AlphaTest: 2450+\n Opaque: 2500+");
            YFut.CreatGUIContent(out BlendypeGC, "Render Blend", " Add or Alpha blend");
            YFut.CreatGUIContent(out CGC, "C", "Cull 开关剔除的面,\n  Cn:不剔除,双面(默认),\n  Cf:剔除正面\n  Cb:剔除背面");
            // EditorGUILayout.LabelField("Base Property", new GUIStyle("AC BoldHeader"), GUILayout.Height(26));

            this.local_RenderType = (RenderTypeMode)EditorGUILayout.EnumPopup(RenderTypeGC, this.m_RenderType);
            this.UpdateProperty(this.local_RenderType);
            if (this.local_RenderType != RenderTypeMode.Opaque)
                this.m_Blendmode = (MaterialBlend)EditorGUILayout.Popup(BlendypeGC, (int)this.m_Blendmode, new string[] { "Additive", "Alpha Blend" });
            else
                EditorGUILayout.Popup(BlendypeGC, 0, new string[] { "Opaque" });
            if (m_ALPHATEST_ON.value)
                this.m_ClipValue = EditorGUILayout.Slider("  Cull Value: ", this.m_ClipValue, 0.0f, 1.0f);

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 1;
            EditorGUILayout.Space();
            this.m_Cull = (CullMode)EditorGUILayout.Popup(CGC, (int)this.m_Cull, new string[3] { "Cull Off", "Cull Front", "Cull Back" }, GUILayout.Width(80));
            this.m_ColorMask.DrawGUI(48, this.m_ColorMask.value > 0 ? " RGBA" : "RGB");
            GUIContent psgc = new GUIContent("PS", "设置ParticleSystems Renderer 的 VertexStreams");
            if (GUILayout.Button(psgc, GUILayout.Width(40)))
            {
                DoVertexStreamsArea(m_material);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Load Module:");
            EditorGUIUtility.labelWidth = 1;
            int buttonW = 22;
            YFut.TooSmallToWrap(300);
            this.m_TIMEDT_ON.DrawGUI(buttonW);
            this.m_Fresnel_ON.DrawGUI(buttonW);
            this.m_Distort_ON.DrawGUI(buttonW);            
            this.m_ColorMask_ON.DrawGUI(buttonW);
            this.m_Dissolution_ON.DrawGUI(buttonW);
            this.m_Sprout_ON.DrawGUI(buttonW);
            this.m_isGroundClip.DrawGUI(buttonW);

            string h = "Module:  " + m_TIMEDT_ON.helpbox + m_Fresnel_ON.helpbox + m_Distort_ON.helpbox +
                m_ColorMask_ON.helpbox + m_Dissolution_ON.helpbox + m_Sprout_ON.helpbox + m_isGroundClip.helpbox +
                "ALPHATEST_" + (m_ALPHATEST_ON.value ? "ON" : "OFF");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox(h, MessageType.None);

            // btnGroup(m_props);
        }

        void UpdateProperty(RenderTypeMode _RenderType)
        {
            bool isc = (this.m_RenderType != _RenderType);
            this.m_RenderType = _RenderType;
            switch (this.m_RenderType)
            {
                case RenderTypeMode.Opaque:
                    this.m_Blendmode = MaterialBlend.Opaque;
                    this.m_Zwrite = 1;
                    this.m_ALPHATEST_ON.value = true;
                    this.m_Cull = CullMode.Back;
                    break;
                case RenderTypeMode.AlphaTest:
                    this.m_Zwrite = 1;
                    this.m_ALPHATEST_ON.value = true;
                    this.m_Blendmode = this.m_Blendmode.Equals(MaterialBlend.Opaque) ? MaterialBlend.Alpha_Blend : this.m_Blendmode;
                    if (isc) { this.m_Cull = CullMode.Back; }
                    break;
                case RenderTypeMode.Transparent:
                default:
                    this.m_ALPHATEST_ON.value = false;
                    this.m_Zwrite = 0;
                    this.m_Blendmode = this.m_Blendmode.Equals(MaterialBlend.Opaque) ? MaterialBlend.Alpha_Blend : this.m_Blendmode;
                    if (isc) { this.m_Cull = CullMode.Back; }
                    break;
            }
            m_ALPHATEST_ON.SetKey();
            this.SetBlendtoSRCDst();
            mp_mode.floatValue = (float)this.SetData();
            // ========================================================
            switch (this.m_RenderType)
            {
                case RenderTypeMode.Opaque:
                    m_material.SetOverrideTag("RenderType", "Opaque");
                    m_material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                    break;
                case RenderTypeMode.AlphaTest:
                    m_material.SetOverrideTag("RenderType", "TransparentCutout");
                    m_material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    break;
                case RenderTypeMode.Transparent:
                default:
                    m_material.SetOverrideTag("RenderType", "Transparent");
                    m_material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
            }
            mp_Cull.floatValue = (float)this.m_Cull;
            mp_ZWrite.floatValue = (float)this.m_Zwrite;
            mp_SrcBlend.floatValue = (float)this.m_SrcBlend;
            mp_DstBlend.floatValue = (float)this.m_DstBlend;
            mp_ColorMask.floatValue = (float)this.m_ColorMask.value + 14;
            if (mp_ClipValue != null)
            {
                mp_ClipValue.floatValue = (float)this.m_ClipValue;
            }
            if (mp_IsClipGround != null)
            {
                mp_IsClipGround.floatValue = this.m_isGroundClip.value ? 1f : 0f;
            }
        }

        void SetBlendtoSRCDst()
        {
            switch (this.m_Blendmode)
            {
                case MaterialBlend.Additive:
                    this.m_SrcBlend = UnityEngine.Rendering.BlendMode.SrcAlpha;
                    this.m_DstBlend = UnityEngine.Rendering.BlendMode.One;
                    break;
                case MaterialBlend.Alpha_Blend:
                    this.m_SrcBlend = UnityEngine.Rendering.BlendMode.SrcAlpha;
                    this.m_DstBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                    break;
                case MaterialBlend.Opaque:
                default:
                    this.m_SrcBlend = UnityEngine.Rendering.BlendMode.One;
                    this.m_DstBlend = UnityEngine.Rendering.BlendMode.Zero;
                    break;
            }
        }
        void GetData(int mode)
        {
            this.m_RenderType = (RenderTypeMode)YFUtilFun.GetINTData(mode, ID_RenderType, OX_RenderType);
            this.m_Blendmode = (MaterialBlend)YFUtilFun.GetINTData(mode, ID_Blend, OX_Blend);
            this.SetBlendtoSRCDst();
            this.m_Cull = (CullMode)YFUtilFun.GetINTData(mode, ID_Cull, OX_Cull);
            this.m_Zwrite = YFUtilFun.GetINTData(mode, ID_Zwrite, OX_Zwrite);

            this.m_ColorMask.ImportValue(mode);
            //bool
            this.m_ALPHATEST_ON.ImportValue(mode, this.m_material);
            this.m_TIMEDT_ON.ImportValue(mode, this.m_material);
            this.m_Fresnel_ON.ImportValue(mode, this.m_material);
            this.m_Distort_ON.ImportValue(mode, this.m_material);
            this.m_ColorMask_ON.ImportValue(mode, this.m_material);
            this.m_Dissolution_ON.ImportValue(mode, this.m_material);
            this.m_Sprout_ON.ImportValue(mode, this.m_material);
            this.m_isGroundClip.ImportValue(mode);
            if (this.mp_ClipValue != null)
            {
                this.m_ClipValue = this.mp_ClipValue.floatValue;
            }
        }
        int SetData()
        {
            int modeTmp = 0;//4194304;
            YFUtilFun.SetINTData(ID_RenderType, OX_RenderType, (int)this.m_RenderType, ref modeTmp);
            YFUtilFun.SetINTData(ID_Blend, OX_Blend, (int)this.m_Blendmode, ref modeTmp);
            YFUtilFun.SetINTData(ID_Cull, OX_Cull, (int)this.m_Cull, ref modeTmp);
            YFUtilFun.SetINTData(ID_Zwrite, OX_Zwrite, this.m_Zwrite, ref modeTmp);

            // UtilFun.SetINTData(ID_ColorMask, OX_ColorMask, this.m_ColorMask - 14, ref modeTmp);
            this.m_ColorMask.ExportIntData(ref modeTmp);
            //bool
            this.m_ALPHATEST_ON.ExportIntData(ref modeTmp);
            this.m_TIMEDT_ON.ExportIntData(ref modeTmp);
            this.m_Fresnel_ON.ExportIntData(ref modeTmp);
            this.m_Distort_ON.ExportIntData(ref modeTmp);
            this.m_ColorMask_ON.ExportIntData(ref modeTmp);
            this.m_Dissolution_ON.ExportIntData(ref modeTmp);
            this.m_Sprout_ON.ExportIntData(ref modeTmp);
            this.m_isGroundClip.ExportIntData(ref modeTmp);
            return modeTmp;
        }

        void CacheRenderersUsingThisMaterial(Material material)
        {
            m_RenderersUsingThisMaterial.Clear();

            ParticleSystemRenderer[] renderers = Resources.FindObjectsOfTypeAll(typeof(ParticleSystemRenderer)) as ParticleSystemRenderer[];
            foreach (ParticleSystemRenderer renderer in renderers)
            {
                var go = renderer.gameObject;
                if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                    continue;

                if (renderer.sharedMaterial == material)
                    m_RenderersUsingThisMaterial.Add(renderer);
            }
        }

        ParticleSystemRenderer GetParticle(Material material)
        {
            var go = Selection.activeGameObject;
            if (go == null)
            {
                return null;
            }
            if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                return null;
            ParticleSystemRenderer r = go.GetComponent<ParticleSystemRenderer>() as ParticleSystemRenderer;
            if (r.sharedMaterial == material)
            {
                return r;
            }
            else
            {
                return null;
            }
        }
        void DoVertexStreamsArea(Material material)
        {
            List<ParticleSystemVertexStream> rendererStreams = new List<ParticleSystemVertexStream>();
            // streams.Add(ParticleSystemVertexStream.Position);
            // streams.Add(ParticleSystemVertexStream.Normal);
            // streams.Add(ParticleSystemVertexStream.Color);
            // streams.Add(ParticleSystemVertexStream.UV);
            // streams.Add(ParticleSystemVertexStream.Custom1XYZW);
            // streams.Add(ParticleSystemVertexStream.Custom2XYZW);
            ParticleSystemRenderer r = GetParticle(material);
            if (r != null)
            {
                r.GetActiveVertexStreams(rendererStreams);
                rendererStreams.Remove(ParticleSystemVertexStream.Custom1XYZW);
                rendererStreams.Remove(ParticleSystemVertexStream.Custom2XYZW); 
                if (rendererStreams.Count < 5)
                {
                    rendererStreams.Add(ParticleSystemVertexStream.Custom1XYZW);
                }
                else
                {
                    if (rendererStreams[4] != ParticleSystemVertexStream.Custom1XYZW)
                    {
                        rendererStreams.Insert(4, ParticleSystemVertexStream.Custom1XYZW);
                    }
                }
                if (rendererStreams.Count < 6)
                {
                    rendererStreams.Add(ParticleSystemVertexStream.Custom2XYZW);
                }
                else
                {
                    if (rendererStreams[5] != ParticleSystemVertexStream.Custom2XYZW)
                    {
                        rendererStreams.Insert(5, ParticleSystemVertexStream.Custom2XYZW);
                    }
                }
                r.SetActiveVertexStreams(rendererStreams);
            }
            // foreach (ParticleSystemRenderer renderer in m_RenderersUsingThisMaterial)
            // {
            //     if (renderer != null)
            //     {
            //         renderer.SetActiveVertexStreams(streams); 
            //     } 
            // }
        }
    }
    #endregion 



    
    #region -MatTexProperty
    internal abstract class YF_MatTexProperty : YF_BaseMatProperty
    {
        protected class SwitchGroup
        {
            public int ColorBlend = 0;
            public float ColorBlendvalue = 0.5f;
            public int AplBlend = 0;
            public float AplBlendvalue = 0.5f;
            public bool IsClip = false;
            public bool IsFlip = false;
            public bool IsGray = false;
            public bool[] isview = new bool[5] { false, false, false, false, false };


            public void GetData(int bitv, Vector4 valuedata)
            {
                ColorBlend = (int)(bitv >> ((int)EnumSwitchGroup.ColBlend) & 0x3);
                AplBlend = (int)(bitv >> ((int)EnumSwitchGroup.AplBlend) & 0x3);
                IsClip = (bitv >> ((int)EnumSwitchGroup.IsClip) & 0x1).ToBool();
                IsFlip = (bitv >> ((int)EnumSwitchGroup.IsFlip) & 0x1).ToBool();
                IsGray = (bitv >> ((int)EnumSwitchGroup.IsGray) & 0x1).ToBool();

                ColorBlendvalue = valuedata.x;
                AplBlendvalue = valuedata.y;
            }
            public void ExportData(ref int bitv, ref Vector4 valuedata)
            {
                // int Maxlan = 7; 7f
                int d = 0;
                d += ColorBlend << ((int)EnumSwitchGroup.ColBlend);
                d += AplBlend << ((int)EnumSwitchGroup.AplBlend);
                d += ((int)IsClip.ToInt()) << ((int)EnumSwitchGroup.IsClip);
                d += ((int)IsFlip.ToInt()) << ((int)EnumSwitchGroup.IsFlip);
                d += ((int)IsGray.ToInt()) << ((int)EnumSwitchGroup.IsGray);
                YFut.SetINTData(0, 0x7f, d, ref bitv);
                valuedata.x = ColorBlendvalue;
                valuedata.y = AplBlendvalue;
            }
        }

        protected override void _OnChandeEndDrawPropGUI()
        {
            animGUI.SetData(ref m_v1, ref animbitSw);
            _switchGroup.ExportData(ref srcbitSw, ref m_v2);
            int d = 0;
            d = (srcbitSw << 13) + animbitSw;
            if (mpb_value0 != null) { mpb_value0.vectorValue = m_color; }
            if (mpb_value1 != null) { mpb_value1.vectorValue = m_v1; }
            if (mpb_value2 != null) { mpb_value2.vectorValue = m_v2; }
            if (mpb_value3 != null) { mpb_value3.vectorValue = m_v3; }

            if (mpb_bitSw != null) { mpb_bitSw.floatValue = (float)d; }
        }
        protected override void _FindProperty()
        {
            modeid = (int)FindProperty("_mode", m_props).floatValue;
            mpb_tex = FindProperty("_" + tagName + "Tex", m_props, false);
            mpb_value0 = FindProperty("_" + tagName + "Value0", m_props, false);//颜色属性
            mpb_value1 = FindProperty("_" + tagName + "Value1", m_props, false);//动画属性
            mpb_value2 = FindProperty("_" + tagName + "Value2", m_props, false);
            mpb_value3 = FindProperty("_" + tagName + "Value3", m_props, false);
            mpb_bitSw = FindProperty("_" + tagName + "bitSw", m_props, false);
            // getdata
            int bit = 0;
            if (mpb_value0 != null) { m_color = (Color)mpb_value0.vectorValue; }
            if (mpb_value1 != null) { m_v1 = mpb_value1.vectorValue; }
            if (mpb_value2 != null) { m_v2 = mpb_value2.vectorValue; }
            if (mpb_value3 != null) { m_v3 = mpb_value3.vectorValue; }
            if (mpb_bitSw != null) { bit = (int)mpb_bitSw.floatValue; }
            srcbitSw = bit >> 13;
            animbitSw = bit & 0x1fff;
            if (mpb_bitSw != null)
            {
                _switchGroup.GetData(srcbitSw, m_v2);
                if (mpb_value1 != null)
                {
                    animGUI.GetData(m_v1, animbitSw, modeid);
                }
            }
        }

        protected string tagName;
        protected int buttonW;
        protected SwitchGroup _switchGroup = new SwitchGroup();
        public int modeid; //MaterialProperty mode; 
        public MaterialProperty mpb_tex;
        public MaterialProperty mpb_value0; protected Color m_color;
        public MaterialProperty mpb_value1; protected Vector4 m_v1;
        public MaterialProperty mpb_value2; protected Vector4 m_v2;
        public MaterialProperty mpb_value3; protected Vector4 m_v3;
        public MaterialProperty mpb_bitSw; protected int animbitSw; protected int srcbitSw;
        protected YF_CustomAnimGUI animGUI = new YF_CustomAnimGUI();
        public void IsButtomGUI(GUIContent lable, ref bool isv, int w = 65)
        { 
            if (GUILayout.Button(lable, YFut.CbtnGS("button",isv),GUILayout.MaxWidth(w)))
            {
                isv = !isv;
            } 
        }

        public void PopGUI(string[] view, ref int bv, ref float v)
        {
            int w = 65;
            if (bv == 1)
            {
                w = 20;
            }
            // EditorGUILayout.BeginHorizontal();
            bv = EditorGUILayout.Popup(bv, view, GUILayout.MinWidth(w));
            if (bv == 1)
            {
                EditorGUIUtility.fieldWidth = 25;
                v = EditorGUILayout.Slider(v, 0.0f, 1.0f);
            }
            // EditorGUILayout.EndHorizontal();
        }

        public void PopGUIMax(string[] view, ref int bv, int maxw)
        {
            bv = EditorGUILayout.Popup(bv, view, GUILayout.MaxWidth(maxw));
        }
        public void PopGUI(string[] view, ref int bv)
        {
            bv = EditorGUILayout.Popup(bv, view);
        }
        public int getbitData()
        {
            return (int)mpb_bitSw.floatValue;
        }
        public YF_MatTexProperty()
        {
            buttonW = 20;
        }

    }
    #endregion 



     
    #region ---BloomMatTexProperty
    class YF_BloomMatTexProperty : YF_BaseMatProperty
    {
        MaterialProperty mbp_threshhold; MaterialProperty mbp_color;
        MaterialProperty mbp_localBool; MaterialProperty mbp_light;
        float threshhold; Color color; bool localBool; float light;
        public YF_BloomMatTexProperty() { }
        protected override void _FindProperty()
        {
            mbp_threshhold = FindProperty("_CBLocalThreshhold", m_props, false);
            mbp_color = FindProperty("_CBLocalColor", m_props, false);
            mbp_light = FindProperty("_CBLocalLight", m_props, false);
            mbp_localBool = FindProperty("_CBLocalBanBool", m_props, false);
            if (mbp_threshhold != null)
            {
                threshhold = mbp_threshhold.floatValue;
            }
            if (mbp_color != null)
            {
                color = mbp_color.colorValue;
            }
            if (mbp_localBool != null)
            {
                localBool = mbp_localBool.floatValue > 0;
            }
            if (mbp_light != null)
            {
                light = mbp_light.floatValue;
            }
        }
        protected override void _OnChandeEndDrawPropGUI()
        {
            if (mbp_threshhold != null)
            {
                mbp_threshhold.floatValue = threshhold;
            }
            if (mbp_color != null)
            {
                mbp_color.colorValue = color;
            }
            if (mbp_localBool != null)
            {
                mbp_localBool.floatValue = localBool ? 1f : 0f;
            }
            if (mbp_light != null)
            {
                mbp_light.floatValue = light;
            }
        }
        public bool GetIsRun()
        { 
            //Assembly-CSharp.dll
            var t = Type.GetType("YF.Art.PostProcessing.YFPostProcessing,Assembly-CSharp"); 
            if(t == null) return false;
            return GameObject.FindObjectOfType(t) != null;
            // return GameObject.FindObjectOfType<YF.Art.PostProcessing.YFPostProcessing>() != null;
 
        }
        protected override void _DrawGUI()
        {
            if (!this.GetIsRun())
            {
                return;
            }
            this.DefGUIColor();
            YFut.SetGUIWidth();
            EditorGUIUtility.labelWidth = 100;
            EditorGUILayout.BeginHorizontal();
            localBool = EditorGUILayout.Toggle(new GUIContent("独立关闭", "开启Bloom下想单独关闭当前材质球的Bloom效果。可打开这个开关。常用在UI上"), localBool);
            EditorGUILayout.LabelField(!localBool ? "Bloom状态开启中" : "Bloom状态关闭中", new GUIStyle("TextArea"));
            EditorGUILayout.EndHorizontal();
            color = EditorGUILayout.ColorField(new GUIContent("颜色", "独立的发光颜色,无HDR"), color);
            threshhold = EditorGUILayout.Slider(new GUIContent("Threshhold", "独立阈值"), threshhold, 0f, 1f);
            light = EditorGUILayout.FloatField(new GUIContent("亮度", "独立的亮度强度"), light);
        }
    }
    #endregion 

 
 
    #region ---COLORMASKMatTexProperty
    class YF_ColorMaskMatTexProperty : YF_DefMatTexProperty
    {
        public YF_ColorMaskMatTexProperty()
        {
            tagName = "ColorMask";
            viewname = "Color Mask ";
            buttonW = 20;
        }
        protected override void SwitchGroupProGUI()
        {
            base.SwitchGroupProGUI();
            EditorGUILayout.BeginVertical();
            bool v = EditorGUIUtility.currentViewWidth < 400;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            IsButtomGUI(new GUIContent("Use Mask Alpha", "维持Mask.a"), ref _switchGroup.IsClip, 125);
            IsButtomGUI(new GUIContent("IsGray", "IsGray"), ref _switchGroup.IsGray);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("混合模式:",GUILayout.Width(80),GUILayout.ExpandWidth(false));
            EditorGUILayout.Space();
            PopGUI(new string[4] { "Add", "Blend", "Multiply", "Only Alpha" }, ref _switchGroup.AplBlend, ref _switchGroup.AplBlendvalue);
            EditorGUILayout.EndHorizontal();
            switch (_switchGroup.AplBlend)
            {
                case 0:
                    EditorGUILayout.LabelField(" Mask.rgba + Main.rgba", new GUIStyle("TextArea"));
                    break;
                case 1:
                    EditorGUILayout.LabelField(" Mask↑↓Blend", new GUIStyle("TextArea"));
                    break;
                case 2:
                    EditorGUILayout.LabelField(" Mask.rgba * Main.rgba", new GUIStyle("TextArea"));
                    break;
                case 3:
                    EditorGUILayout.LabelField(" (Main.rgb,Main.a * Mask.a)", new GUIStyle("TextArea"));
                    break;
            }
            EditorGUILayout.EndVertical();
        }
    }
    #endregion 


     
    #region --DefMatTexProperty
    class YF_DefMatTexProperty : YF_MatTexProperty
    {
        protected string viewname;
        protected string  tagViewName;
        public YF_DefMatTexProperty() { }
        public YF_DefMatTexProperty(string _tagname, string _viewname)
        {
            tagName = _tagname;
            viewname = _viewname;
            tagViewName = "";
            buttonW = 20;
        }
        protected override void _DrawGUI()
        {
            EditorGUIUtility.labelWidth = 0f;
            GUI.backgroundColor = this.MatBGVolor;
            EditorGUILayout.BeginVertical();
            this.DefGUIColor();
            // EditorGUILayout.LabelField(viewname + " Property", new GUIStyle("AC BoldHeader"), GUILayout.Height(26));
            YFut.SetGUIWidth();
            Value0ProViewGUI();
            TexProViewGUI();
            EditorGUILayout.BeginVertical("Box");
            Value1ProViewGUI();
            EditorGUIUtility.labelWidth = -1;
            SwitchGroupProGUI();
            Value2ProViewGUI();
            Value3ProViewGUI();
            EditorGUILayout.EndVertical();
            YFut.SetGUIWidth();
            // EditorGUILayout.HelpBox(/*srcbitSw.ToBitView()*/ this.getbitData().ToBitView(), MessageType.None);
            EditorGUILayout.EndVertical();
        }

        protected virtual void SwitchGroupProGUI()
        {
            if (mpb_bitSw == null) return;
            // 示例
            // EditorGUILayout.BeginVertical("box");
            // bool v = EditorGUIUtility.currentViewWidth < 350;
            // if (v)
            // {
            //     EditorGUILayout.LabelField("Use Property");
            //     EditorGUILayout.BeginHorizontal();
            //     EditorGUILayout.Space();
            // }
            // else
            // {
            //     EditorGUILayout.BeginHorizontal();
            //     EditorGUILayout.LabelField("Use Property", GUILayout.MaxWidth(85));
            //     EditorGUILayout.Space();
            // }
            //  IsButtomGUI(new GUIContent(" IsClip " + (_switchGroup.IsClip ? "●" : "○"), "IsClip"), ref _switchGroup.IsClip);
            // IsButtomGUI(new GUIContent(" IsFlip " + (_switchGroup.IsFlip ? "●" : "○"), "IsFlip"), ref _switchGroup.IsFlip);
            // IsButtomGUI(new GUIContent(" IsGray " + (_switchGroup.IsGray ? "●" : "○"), "IsGray"), ref _switchGroup.IsGray);
            // EditorGUILayout.EndHorizontal();
            // if (!v)
            // {
            //     EditorGUILayout.BeginHorizontal();
            // }
            // PopGUI(new string[3] { "Color Add", "Color Blend", "Color Multiply" }, ref _switchGroup.ColorBlend, ref _switchGroup.ColorBlendvalue);
            // PopGUI(new string[3] { "Alpha Add", "Alpha Blend", "Alpha Multiply" }, ref _switchGroup.AplBlend, ref _switchGroup.AplBlendvalue);
            // if (!v)
            // {
            //     EditorGUILayout.EndHorizontal();
            // }
            // EditorGUILayout.EndVertical();
        }

        protected virtual void Value0ProViewGUI()
        {
            if (mpb_value0 == null) return;
            EditorGUIUtility.labelWidth = 0;
            m_color = EditorGUILayout.ColorField(new GUIContent(tagViewName + " Color"), m_color, true, true, false);
        }
        protected virtual void TexProViewGUI()
        {
            if (mpb_tex == null) return;
            m_materialEditor.YFDrawDefaultShaderProp(mpb_tex, tagViewName + " Tex(RGBA)");
            if (mpb_value2 != null)
            {
                EditorGUIUtility.labelWidth = -1;
                m_v2.z = EditorGUILayout.FloatField("Brightness", m_v2.z);
                YFut.SetGUIWidth();
            }
        }
        protected virtual void Value1ProViewGUI()
        {
            if (mpb_value1 == null || mpb_bitSw == null) return;
            EditorGUILayout.BeginHorizontal();
            animGUI.DrawGUI();
            EditorGUILayout.EndHorizontal();
        }
        protected virtual void Value2ProViewGUI()
        {
            if (mpb_value2 == null) return;
        }
        protected virtual void Value3ProViewGUI()
        {
            if (mpb_value3 == null) return;

        }
    }
    #endregion 


    
    #region DissolutionMatTexProperty
    class YF_DissolutionMatTexProperty : YF_DefMatTexProperty
    {
        protected int dissVmode = 9;
        protected bool dissIsSrcApl = false;
        public MaterialProperty mpb_bitSw2; protected int srcbitSw2;
        public YF_DissolutionMatTexProperty()
        {
            tagName = "Diss";
            viewname = "溶解";
            tagViewName = "Dissolution";
            buttonW = 20;
        }

        protected override void _DrawGUI()
        {
            EditorGUIUtility.labelWidth = 0f;
            GUI.backgroundColor = this.MatBGVolor;
            EditorGUILayout.BeginVertical();
            this.DefGUIColor();
            // EditorGUILayout.LabelField(viewname + " Property", new GUIStyle("AC BoldHeader"), GUILayout.Height(26));
            YFut.SetGUIWidth();
            TexProViewGUI();
            EditorGUILayout.BeginVertical("box");
            Value1ProViewGUI();
            EditorGUIUtility.labelWidth = -1;
            EditorGUILayout.Space();

            SwitchGroupProGUI();
            Value2ProViewGUI();
            Value0ProViewGUI();
            Value3ProViewGUI();

            SwitchGroupProGUI0();
            EditorGUILayout.EndVertical();
            YFut.SetGUIWidth();
            // EditorGUILayout.HelpBox(/*srcbitSw.ToBitView()*/ this.getbitData().ToBitView(), MessageType.None);
            EditorGUILayout.EndVertical();
        }

        protected override void Value0ProViewGUI()
        {
            return;
        }

        protected override void Value2ProViewGUI()
        {
            base.Value2ProViewGUI();
            EditorGUIUtility.labelWidth = -1;
            this.dissVmode = this.dissVmode.DrawValueAndParticleValue2("Diss Value", " ", 25 * 3, 50, ref m_v2.x, true);

            m_v2.y = EditorGUILayout.Slider("Line width", m_v2.y, 0f, 1.0f);
            m_v2.w = EditorGUILayout.FloatField("Line Brightness", m_v2.w);
            if (mpb_value0 != null)
                m_color = EditorGUILayout.ColorField(new GUIContent("Line Color"), m_color, true, true, false);
            YFut.SetGUIWidth();
        }
        protected void SwitchGroupProGUI0()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Line Blend", GUILayout.Width(100));
            PopGUI(new string[3] { "Color Add", "Color Blend", "Color Multiply" }, ref _switchGroup.ColorBlend);
            EditorGUILayout.EndHorizontal();
        }
        protected override void SwitchGroupProGUI()
        {
            if (mpb_bitSw == null) return;
            // EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(); 
            int w = (int)Mathf.Min(65f,EditorGUIUtility.currentViewWidth * 0.15f); 
            IsButtomGUI(new GUIContent("IsClip", "IsClip"), ref _switchGroup.IsClip,w);
            IsButtomGUI(new GUIContent("IsFlip", "IsFlip"), ref _switchGroup.IsFlip,w);
            IsButtomGUI(new GUIContent("IsGray", "IsGray"), ref _switchGroup.IsGray,w);
            IsButtomGUI(new GUIContent("SrcApl", "IsSrcApl"), ref dissIsSrcApl,w);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        protected override void _FindProperty()
        {

            modeid = (int)FindProperty("_mode", m_props).floatValue;
            mpb_tex = FindProperty("_" + tagName + "Tex", m_props, false);
            mpb_value0 = FindProperty("_" + tagName + "Value0", m_props, false);//颜色属性
            mpb_value1 = FindProperty("_" + tagName + "Value1", m_props, false);//动画属性
            mpb_value2 = FindProperty("_" + tagName + "Value2", m_props, false);
            mpb_value3 = FindProperty("_" + tagName + "Value3", m_props, false);
            mpb_bitSw = FindProperty("_" + tagName + "bitSw", m_props, false);
            mpb_bitSw2 = FindProperty("_" + tagName + "bitSw2", m_props, false);
            // getdata
            int bit = 0;
            if (mpb_value0 != null) { m_color = (Color)mpb_value0.vectorValue; }
            if (mpb_value1 != null) { m_v1 = mpb_value1.vectorValue; }
            if (mpb_value2 != null) { m_v2 = mpb_value2.vectorValue; }
            if (mpb_value3 != null) { m_v3 = mpb_value3.vectorValue; }
            if (mpb_bitSw != null) { bit = (int)mpb_bitSw.floatValue; }
            if (mpb_bitSw2 != null) { srcbitSw2 = (int)mpb_bitSw2.floatValue; }

            srcbitSw = bit >> 13;
            animbitSw = bit & 0x1fff;
            if (mpb_bitSw != null)
            {
                Vector4 tmp = Vector4.zero;
                tmp.x = m_color.a;
                _switchGroup.GetData(srcbitSw, tmp);
                if (mpb_value1 != null)
                {
                    animGUI.GetData(m_v1, animbitSw, modeid);
                }
                dissVmode = srcbitSw2 >> 0 & 0xF;
                dissIsSrcApl = (srcbitSw2 >> 4 & 0x1).ToBool();
            }
        }
        protected override void _OnChandeEndDrawPropGUI()
        {
            srcbitSw2 = dissVmode << 0;
            srcbitSw2 += ((int)dissIsSrcApl.ToInt()) << 4;
            animGUI.SetData(ref m_v1, ref animbitSw);
            Vector4 tmp = Vector4.zero;
            tmp.x = m_color.a;
            _switchGroup.ExportData(ref srcbitSw, ref tmp);
            m_color.a = tmp.x;
            int d = 0;
            d = (srcbitSw << 13) + animbitSw;
            if (mpb_value0 != null) { mpb_value0.vectorValue = m_color; }
            if (mpb_value1 != null) { mpb_value1.vectorValue = m_v1; }
            if (mpb_value2 != null) { mpb_value2.vectorValue = m_v2; }
            if (mpb_value3 != null) { mpb_value3.vectorValue = m_v3; }

            if (mpb_bitSw != null) { mpb_bitSw.floatValue = (float)d; }
            if (mpb_bitSw2 != null) { mpb_bitSw2.floatValue = (float)srcbitSw2; }
        }


    }
    #endregion 

 
    #region ---DistMatTexProperty
    class YF_DistMatTexProperty : YF_DefMatTexProperty
    { 
        public YF_DistMatTexProperty()
        {
            tagName = "Dist";
            viewname = "扰动";
            tagViewName = "Distort";
        }

        protected override void SwitchGroupProGUI()
        {
            base.SwitchGroupProGUI();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal(); 
            EditorGUILayout.Space();
            IsButtomGUI(new GUIContent("IsFlip", "IsFlip"), ref _switchGroup.IsFlip);
            IsButtomGUI(new GUIContent("IsGray" , "IsGray"), ref _switchGroup.IsGray);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        protected override void Value1ProViewGUI()
        {
            if (mpb_value1 == null || mpb_bitSw == null) return;
            EditorGUILayout.BeginHorizontal();
            animGUI.DrawGUI(false);
            EditorGUILayout.EndHorizontal();
        }

    }
    #endregion 


    

    #region ---FNLMatTexProperty
    class YF_FNLMatTexProperty : YF_DefMatTexProperty
    {
        public YF_FNLMatTexProperty()
        {
            tagName = "FNL";
            viewname = "Fresnel";
            tagViewName = "Fresnel";
        }
        protected override void _DrawGUI()
        {
            EditorGUIUtility.labelWidth = 0f;
            GUI.backgroundColor = this.MatBGVolor;
            EditorGUILayout.BeginVertical("Box");
            this.DefGUIColor();
            // EditorGUILayout.LabelField(viewname + " Property", new GUIStyle("AC BoldHeader"), GUILayout.Height(26));
            YFut.SetGUIWidth();
            EditorGUILayout.BeginVertical();
            EditorGUIUtility.labelWidth = -1;
            SwitchGroupProGUI();    
            Value0ProViewGUI();
            Value2ProViewGUI();

            YFut.SetGUIWidth();
            // EditorGUILayout.HelpBox(/*srcbitSw.ToBitView()*/ this.getbitData().ToBitView(), MessageType.None);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
        protected override void _OnChandeEndDrawPropGUI()
        {
            animGUI.SetData(ref m_v1, ref animbitSw);
            Vector4 v = Vector4.zero;
            _switchGroup.ExportData(ref srcbitSw, ref v);
            int d = 0;
            d = (srcbitSw << 13) + animbitSw;
            if (mpb_value0 != null) { mpb_value0.vectorValue = m_color; }
            if (mpb_value1 != null) { mpb_value1.vectorValue = m_v1; }
            if (mpb_value2 != null) { mpb_value2.vectorValue = m_v2; }
            if (mpb_bitSw != null) { mpb_bitSw.floatValue = (float)d; }
        }
        protected override void _FindProperty()
        {
            modeid = (int)FindProperty("_mode", m_props).floatValue;
            mpb_tex = FindProperty("_" + tagName + "Tex", m_props, false);
            mpb_value0 = FindProperty("_" + tagName + "Value0", m_props, false);//颜色属性
            mpb_value1 = FindProperty("_" + tagName + "Value1", m_props, false);//动画属性
            mpb_value2 = FindProperty("_" + tagName + "Value2", m_props, false);
            mpb_bitSw = FindProperty("_" + tagName + "bitSw", m_props, false);
            // getdata
            int bit = 0;
            if (mpb_value0 != null) { m_color = (Color)mpb_value0.vectorValue; }
            if (mpb_value1 != null) { m_v1 = mpb_value1.vectorValue; }
            if (mpb_value2 != null) { m_v2 = mpb_value2.vectorValue; }
            if (mpb_bitSw != null) { bit = (int)mpb_bitSw.floatValue; }
            srcbitSw = bit >> 13;
            animbitSw = bit & 0x1fff;
            if (mpb_bitSw != null)
            {
                _switchGroup.GetData(srcbitSw, Vector4.zero);
                if (mpb_value1 != null)
                {
                    animGUI.GetData(m_v1, animbitSw, modeid);
                }
            }
        }
        protected override void Value0ProViewGUI()
        {
            if (mpb_value0 == null) return;
            m_color = EditorGUILayout.ColorField(new GUIContent("Fresnel Color"), m_color, true, true, false);
        }
        protected override void Value1ProViewGUI()
        {
            return;
        }
        protected override void SwitchGroupProGUI()
        {
            if (mpb_bitSw == null) return;
            // 示例
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(); 
            PopGUIMax(new string[4] { "View All", "Use SrcAlpha", "Use FNLAlpha", "Only FNLAlpha" }, ref _switchGroup.AplBlend,85);
            PopGUIMax(new string[3] { "Color Add", "Color Blend", "Color Multiply" }, ref _switchGroup.ColorBlend,85);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        protected override void Value2ProViewGUI()
        {
            base.Value2ProViewGUI();
            m_v2.z = EditorGUILayout.FloatField("Fresnel Brightness", m_v2.z);
            m_v2.x = EditorGUILayout.Slider("Fresnel EXP0", m_v2.x, -1.0f, 1.0f);
            m_v2.y = EditorGUILayout.Slider("Fresnel EXP1", m_v2.y, -1.0f, 1.0f);
            if (mpb_value1 != null || mpb_bitSw != null)
            {
                EditorGUILayout.BeginHorizontal();
                animGUI.DrawGUIOnlyDistU();
                EditorGUILayout.EndHorizontal();
            }
        }
    }
    #endregion 



     
    #region ---SproutMatTexProperty
    class YF_GroundMatTexProperty : YF_BaseMatProperty
    {
        MaterialProperty mp_Value;
        float value;
        public YF_GroundMatTexProperty() { }
        protected override void _FindProperty()
        {
            mp_Value = FindProperty("_ClipGroundValue", m_props, false);
            if (mp_Value != null)
            {
                value = mp_Value.floatValue;
            }

        }
        protected override void _OnChandeEndDrawPropGUI()
        {
            if (mp_Value != null)
            {
                mp_Value.floatValue = value;
            }
        }
        protected override void _DrawGUI()
        {
            this.DefGUIColor();
            // EditorGUILayout.LabelField("地面裁切 Property", new GUIStyle("AC BoldHeader"), GUILayout.Height(26));
            YFut.SetGUIWidth();
            EditorGUIUtility.labelWidth = 80;
            this.value = EditorGUILayout.Slider("地面接缝过度:", this.value, 0.0f, 1.0f);
            EditorGUILayout.HelpBox(@"Tips:世界坐标Y小于0的时候,将被裁切。", MessageType.Info);
        }
    }
    #endregion 

  
    #region ---MainMatTexProperty
    class YF_MainMatTexProperty : YF_DefMatTexProperty
    {
        public float m_Brightness; MaterialProperty mpb_Brightness;
        public Color m_TintColor; MaterialProperty mpb_TintColor;
        public YF_MainMatTexProperty()
        {
            tagName = "Main";
            viewname = "Main";
            tagViewName = "Main";
        }
        protected override void _OnChandeEndDrawPropGUI()
        {
            base._OnChandeEndDrawPropGUI();
            if (mpb_Brightness != null) { mpb_Brightness.floatValue = m_Brightness; }
            if (mpb_TintColor != null) { mpb_TintColor.colorValue = m_TintColor; }
        }
        protected override void _FindProperty()
        {
            mpb_TintColor = FindProperty("_TintColor", m_props, false);
            mpb_Brightness = FindProperty("_Brightness", m_props, false);
            base._FindProperty();
            if (mpb_Brightness != null) { m_Brightness = mpb_Brightness.floatValue; }
            if (mpb_TintColor != null) { m_TintColor = (Color)mpb_TintColor.colorValue; }
        }
        protected override void SwitchGroupProGUI()
        {
            return;
        }
        protected override void Value0ProViewGUI()
        {
            if (mpb_TintColor == null) return;
            EditorGUIUtility.labelWidth = 0;
            m_TintColor = EditorGUILayout.ColorField(new GUIContent("Tint Color"), m_TintColor, true, true, false);
        }
        protected override void TexProViewGUI()
        {
            base.TexProViewGUI();
            if (mpb_Brightness != null)
            {
                EditorGUIUtility.labelWidth = -1;
                m_Brightness = EditorGUILayout.FloatField("Brightness", m_Brightness);
            }
        }
    }
    #endregion 



     
    #region ---SproutMatTexProperty
    class YF_SproutMatTexProperty : YF_BaseMatProperty
    {
        MaterialProperty mp_SproutValue; MaterialProperty mp_SproutbitSw;
        Vector4 value; int bitSW;
        float SproutValue; float SproutNicety; int SproutDir; int SproutValueMode; int SproutNicetyMode;
        public YF_SproutMatTexProperty() { }
        protected override void _FindProperty()
        {
            mp_SproutValue = FindProperty("_SproutValue", m_props, false);
            mp_SproutbitSw = FindProperty("_SproutbitSw", m_props, false);
            if (mp_SproutValue != null)
            {
                value = mp_SproutValue.vectorValue;
            }
            if (mp_SproutbitSw != null)
            {
                bitSW = (int)mp_SproutbitSw.floatValue;
            }
            SproutValue = value.x;
            SproutNicety = value.y;
            SproutDir = bitSW >> 0 & 0xf;
            SproutValueMode = bitSW >> 4 & 0xf;
            SproutNicetyMode = bitSW >> 8 & 0xf;

        }
        protected override void _OnChandeEndDrawPropGUI()
        {
            this.bitSW = 0;
            this.bitSW += SproutDir << 0;
            this.bitSW += SproutValueMode << 4;
            this.bitSW += SproutNicetyMode << 8;
            this.value = new Vector4(SproutValue, SproutNicety);
            if (mp_SproutValue != null)
            {
                mp_SproutValue.vectorValue = value;
            }
            if (mp_SproutbitSw != null)
            {
                mp_SproutbitSw.floatValue = (float)bitSW;
            }
        }
        protected override void _DrawGUI()
        {
            this.DefGUIColor();
            // EditorGUILayout.LabelField("藤蔓生长 Property", new GUIStyle("AC BoldHeader"), GUILayout.Height(26));
            YFut.SetGUIWidth();
            this.SproutDir = EditorGUILayout.Popup("生长方向:", this.SproutDir, new string[4] { "U", "V", "-U", "-V" });
            this.SproutValueMode = this.SproutValueMode.DrawValueAndParticleValue("Sprout Value", ":", 120, 50, ref this.SproutValue, true);
            this.SproutNicetyMode = this.SproutNicetyMode.DrawValueAndParticleValue("Sprout Nicety", ":", 120, 50, ref this.SproutNicety);
            EditorGUILayout.HelpBox(@"Tips:
1.这个功能的原理:藤蔓生长是基于模型UV的,根据U或V的分布来溶解模型.
  2.Sprout Value可以控制溶解,也能使用粒子的custom value控制.
  3.Sprout Nicety可以略微的控制当前溶解处顶点的收口(位移).
  该功能对模型的要求较高。不明白的请问我.
            ", MessageType.Info);
        }
    }
    #endregion 




    #region YF_CustomAnimGUI
    internal class YF_CustomAnimGUI
    {
        static readonly string[] animModestr = { "动画模式", "Move", "Rotation" };
        public int modeid;
        Vector4 value;
        int data;
        public YFbooleraBtn isPolar = new YFbooleraBtn(0, "极", "开启极坐标", "开启极坐标");
        public YFbooleraBtn isauto = new YFbooleraBtn(1, "自动", "开启自动时间", "开启极坐标", "手动");
        public YFIntBtn animmode = new YFIntBtn(2, 0x3, "动画模式", "动画模式move或旋转", "");
        public YFIntBtn animmodeU = new YFIntBtn(4, 0xf, "U", "UV动画时候的值模式,一共9种", "");
        public YFIntBtn animmodeV = new YFIntBtn(8, 0xf, "V", "UV动画时候的值模式,一共9种", "");
        public YFbooleraBtn isDist = new YFbooleraBtn(12, "扰动", "开启当前图片UV的扰动", "开启当前图片UV的扰动", "开启扰动");
        // MaterialProperty m_bitdata;
        public YF_CustomAnimGUI() { }

        public void GetData(Vector4 vector, int d)
        {
            value = vector;
            // m_bitdata = null;
            this.data = d;
            _UpdataGetValue();
        }
        public void GetData(Vector4 vector, int d, int dv)
        {
            modeid = dv;
            value = vector;
            // m_bitdata = d;
            this.data = d;
            _UpdataGetValue();
        }
        public void SetData(ref Vector4 vector, ref int d)
        {
            _UpdataSetValue();
            vector = value;
            uint a = (uint)d;
            a &= ~(((uint)0x1FFF) << 0);
            d = (int)(((uint)data) << 0 | a);
        }
        void _UpdataGetValue()
        {
            isPolar.ImportValue(data);
            isauto.ImportValue(data);
            isDist.ImportValue(data);
            animmode.ImportValue(data);
            animmodeU.ImportValue(data);
            animmodeV.ImportValue(data);
        }
        void _UpdataSetValue()
        {
            isPolar.ExportIntData(ref data);
            isauto.ExportIntData(ref data);
            isDist.ExportIntData(ref data);
            animmode.ExportIntData(ref data);
            animmodeU.ExportIntData(ref data);
            animmodeV.ExportIntData(ref data);
        }
        public void DrawGUI(bool localisdist = true)
        {
            EditorGUIUtility.labelWidth = 1;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            int w = 25;
            EditorGUILayout.LabelField("UV Effect:");
            EditorGUILayout.Space();
            this.isPolar.DrawGUI(w);
            // Debug.Log(IsTimeExist);
            if ((modeid >> ((int)YFfxModel.TIMEDT) & 0x1).ToBool())
            {
                this.isauto.DrawGUI(w * 2);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            this.animmode.value = EditorGUILayout.Popup(":", this.animmode.value, animModestr, GUILayout.MinWidth(85));
            EditorGUIUtility.labelWidth = -1;
            switch (this.animmode.value)
            {
                case 0:
                    break;
                case 1:
                    this.animmodeU.value = this.animmodeU.value.DrawValueAndParticleValue("U", ":", 25, 50, ref this.value.x);
                    this.animmodeV.value = this.animmodeV.value.DrawValueAndParticleValue("V", ":", 25, 50, ref this.value.y);

                    this.value.x = (float)Math.Round(this.value.x, 1);
                    this.value.y = (float)Math.Round(this.value.y, 1);
                    break;
                case 2:
                    this.animmodeU.value = this.animmodeU.value.DrawValueAndParticleValue("Angle", ":", 55, 50, ref this.value.x);
                    this.value.x = (float)Math.Round(this.value.x, 1);
                    break;
            }
            EditorGUILayout.EndHorizontal();
            if (localisdist)
            {
                if ((modeid >> ((int)YFfxModel.Distort) & 0x1).ToBool())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(1, false);
                    this.isDist.DrawGUIMin(this.isDist.value ? 55 : -1);
                    if (this.isDist.value)
                    {
                        EditorGUILayout.Space(10, false);
                        EditorGUIUtility.labelWidth = 20;
                        this.value.z = EditorGUILayout.FloatField("U:", this.value.z);
                        this.value.w = EditorGUILayout.FloatField("V:", this.value.w);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
        public void DrawGUIOnlyDistU()
        {
            EditorGUIUtility.labelWidth = 1;

            if (modeid.ToBool((int)YFfxModel.Distort))//>> ((int)YFfxModel.Distort) & 0x1).ToBool())
            {
                EditorGUILayout.BeginHorizontal();
                // EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(1, false);
                this.isDist.DrawGUIMin(this.isDist.value ? 55 : -1);

                if (this.isDist.value)
                {
                    EditorGUILayout.Space(10, false);
                    EditorGUIUtility.labelWidth = 20;
                    this.value.z = EditorGUILayout.FloatField("U:", this.value.z);
                    EditorGUILayout.FloatField("V:", 0);
                }
                EditorGUILayout.EndHorizontal();
                // EditorGUILayout.EndHorizontal();
            }
        }
    }
    #endregion 

    
     
    #region YFShaderGUICustomBTNUI
    internal abstract class YF_ShaderGUICustomBTNUI
    {
        protected string name;
        protected uint v;
        int bitIndex;
        uint bitlen;
        public YF_ShaderGUICustomBTNUI() { }
        public YF_ShaderGUICustomBTNUI(int bitid, uint bitlen)//, params string[] n)
        {
            SetData(bitid, bitlen);
        }
        public abstract void DrawGUI(int Width);
        public virtual void ImportValue(int mode)
        {
            GetINTData(mode);
        }
        public virtual void ExportIntData(ref int mode)
        {
            mode = SetINTData(mode);
        }

        public virtual int ExportIntData(int mode)
        {
            return SetINTData(mode);
        }

        protected virtual void GetINTData(int mode)
        {
            this.v = (uint)(mode >> this.bitIndex & this.bitlen);
        }
        protected virtual void SetData(int bitid, uint bitlen)//, params string[] n)
        {
            this.bitIndex = bitid;
            this.bitlen = bitlen;
            v = 0;
        }
        protected int SetINTData(int mode)
        {
            uint a = (uint)mode;
            a &= ~(bitlen << bitIndex);
            return (int)(((uint)this.v) << bitIndex | a);
        }
    }
    #endregion

    #region YFIntBtn
    internal class YFIntBtn : YF_ShaderGUICustomBTNUI
    {
        public int value;
        protected string label;
        protected string label2;
        protected string tooltip;
        public string helpbox;
        public YFIntBtn() { }
        public YFIntBtn(int bitid, uint bitlen, params string[] str)
        {
            SetData(bitid, bitlen);
            label = str[0];
            tooltip = str[1];
            helpbox = str[2];
            value = (int)this.v;
            if (str.Length > 3)
            {
                label2 = str[3];
            }
            else
            {
                label2 = str[0];
            }
        }

        public override void DrawGUI(int Width = -1)
        {
            if (Width < 0)
            {
                this.value = YFut.Cbtn<int>(a => (a > 0 ? 0 : 1), new GUIContent(this.value > 0 ? label : label2, tooltip), this.value > 0, this.value);
            }
            else
            {
                this.value = YFut.Cbtn<int>(a => (a > 0 ? 0 : 1), new GUIContent(this.value > 0 ? label : label2, tooltip), this.value > 0, Width, this.value);
            }
            this.v = (uint)this.value;
        }
        public void DrawGUI(int Width, string str)
        {
            if (str != "")
            {
                this.label = str;
            }
            DrawGUI(Width);
        }
        public override void ImportValue(int mode)
        {
            base.ImportValue(mode);
            this.value = (int)this.v;
        }
        public override void ExportIntData(ref int mode)
        {
            this.v = (uint)this.value;
            mode = ExportIntData(mode);
        }
    }
    #endregion

    #region YFbooleraBtn
    internal class YFbooleraBtn : YF_ShaderGUICustomBTNUI
    {
        public bool value;
        public string label;
        protected string label2;
        protected string tooltip;
        public string helpbox;
        public YFbooleraBtn() { }
        public YFbooleraBtn(string _name, int bitid, params string[] str)
        {
            name = _name;
            _YFbooleraBtn(bitid, str);
        }

        public YFbooleraBtn(int bitid, params string[] str)
        {
            _YFbooleraBtn(bitid, str);
        }
        public bool Str2Value(string n)
        {
            if (n == name)
            {
                return this.value;
            }
            else
            {
                return false;
            }
        }
        public void _YFbooleraBtn(int bitid, params string[] str)
        {
            SetData(bitid, 0x1);
            label = str[0];
            tooltip = str[1];
            helpbox = str[2];
            value = this.v.ToBool();
            if (str.Length > 3)
            {
                label2 = str[3];
            }
            else
            {
                label2 = str[0];
            }
        }
        public override void DrawGUI(int Width = -1)
        {

            if (Width < 0)
            {
                this.value = this.value.Cbtn(this.value ? label : label2, tooltip);
            }
            else
            {
                this.value = this.value.Cbtn(Width, this.value ? label : label2, tooltip);
            }

        }
        public void DrawGUIMin(int Width = -1)
        {

            if (Width < 0)
            {
                this.value = this.value.Cbtn(this.value ? label : label2, tooltip);
            }
            else
            {
                this.value = this.value.CbtnMin(Width, this.value ? label : label2, tooltip);
            }

        }
        public override void ImportValue(int mode)
        {
            base.ImportValue(mode);
            this.value = this.v.ToBool();
        }

        public override void ExportIntData(ref int mode)
        {
            this.v = this.value.ToInt();
            mode = ExportIntData(mode);
        }
    }
    #endregion

    #region YFbooleraKeyBtn
    internal class YFbooleraKeyBtn : YFbooleraBtn
    {
        string key;
        bool iskey;
        Material material;
        public YFbooleraKeyBtn() { }
        public YFbooleraKeyBtn(string _name, int bitid, string key, params string[] str)
        {
            name = _name;
            _YFbooleraKeyBtn(bitid, key, str);
        }
        public YFbooleraKeyBtn(int bitid, string key, params string[] str)
        {
            _YFbooleraKeyBtn(bitid, key, str);
        }
        public void _YFbooleraKeyBtn(int bitid, string key, params string[] str)
        {
            SetData(bitid, 0x1);
            iskey = false;
            label = str[0];
            tooltip = str[1];
            helpbox = str[2];
            value = this.v.ToBool();
            this.key = key;
            if (str.Length > 3)
            {
                label2 = str[3];
            }
            else
            {
                label2 = str[0];
            }
        }

        public void ImportValue(int mode, Material _material)
        {
            GetINTData(mode);
            this.value = this.v > 0;
            this.material = _material;
            this.iskey = material.CheckShaderKeys(key);
            helpbox = this.iskey ? helpbox : string.Empty;
        }

        public void SetKey()
        {
            if (this.iskey)
            {
                if (this.value)
                {
                    this.material.EnableKeyword(key);
                }
                else
                {
                    this.material.DisableKeyword(key);
                }
            }
        }

        public override void DrawGUI(int Width)
        {
            if (this.iskey)
            {
                base.DrawGUI(Width);
            }
            this.SetKey();
        }
    }
    #endregion 


    #region YFUtilFun    
    public enum MaterialBlend
    {
        Additive = 0,
        Alpha_Blend,
        Opaque
    }
    public enum RenderTypeMode
    {   // Transparent AlphaTest Opaque
        Transparent = 0,
        AlphaTest = 1,
        Opaque = 2
    }
    public enum EnumSwitchGroup
    {
        ColBlend = 0, AplBlend = 2, IsClip = 4, IsFlip, IsGray
    }
    public enum YFfxModel
    {
        ALPHATEST = 8, TIMEDT = 9, Fresnel = 10, Distort = 11, ColorMask = 12,
        Dissolution = 13, GroundClip = 15, Sprout = 14
    }
    internal static class YFUtilFun
    {
        public static readonly string[] valuemode = { "C1.x", "C1.y", "C1.z", "C1.w", "C2.x", "C2.y", "C2.z", "C2.w", "Value" };

        public static int DrawValueAndParticleValue2(this int mode, string btnlabel, string valuelabel, int Btnwidth, int Popwidth, ref float floatValue, bool isSlider = false)
        {

            EditorGUILayout.BeginHorizontal();
            float w = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 25;
            int w2 = 10;
            if (mode > 8)
            {
                if (GUILayout.Button(btnlabel, GUILayout.Width(Btnwidth), GUILayout.ExpandWidth(false)))
                {
                    mode = 0;
                }
                EditorGUILayout.Space();
                if (isSlider)
                {
                    if (valuelabel == string.Empty)
                    {
                        EditorGUILayout.Space(w2);
                        floatValue = EditorGUILayout.Slider(floatValue, 0.0f, 1.0f);
                    }
                    else
                    {
                        floatValue = EditorGUILayout.Slider(valuelabel, floatValue, 0.0f, 1.0f);
                    }
                }
                else
                {
                    if (valuelabel == string.Empty)
                    {
                        EditorGUILayout.Space(w2);
                        floatValue = EditorGUILayout.FloatField(floatValue);
                    }
                    else
                    {
                        floatValue = EditorGUILayout.FloatField(valuelabel, floatValue);
                    }
                }
            }
            else
            {
                if (GUILayout.Button(btnlabel, GUILayout.Width(Btnwidth), GUILayout.ExpandWidth(false)))
                {
                    mode = 9;
                }
                if (valuelabel == string.Empty)
                {
                    EditorGUILayout.Space(w2);
                    mode = EditorGUILayout.Popup(mode, valuemode, GUILayout.MinWidth(Popwidth));
                }
                else
                {
                    mode = EditorGUILayout.Popup(valuelabel, mode, valuemode, GUILayout.MinWidth(Popwidth));
                }

            }
            EditorGUIUtility.labelWidth = w;
            EditorGUILayout.EndHorizontal();
            return mode;
        }
        public static int DrawValueAndParticleValue(this int mode, string btnlabel, string valuelabel, int Btnwidth, int Popwidth, ref float floatValue, bool isSlider = false)
        {

            EditorGUILayout.BeginHorizontal();
            float w = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 5;
            int w2 = 10;
            if (mode > 7)
            {
                if (GUILayout.Button(btnlabel, GUILayout.Width(Btnwidth), GUILayout.ExpandWidth(false)))
                {
                    mode = 0;
                }
                if (isSlider)
                {
                    if (valuelabel == string.Empty)
                    {
                        EditorGUILayout.Space(w2);
                        floatValue = EditorGUILayout.Slider(floatValue, 0.0f, 1.0f);
                    }
                    else
                    {
                        floatValue = EditorGUILayout.Slider(valuelabel, floatValue, 0.0f, 1.0f);
                    }
                }
                else
                {
                    if (valuelabel == string.Empty)
                    {
                        EditorGUILayout.Space(w2);
                        floatValue = EditorGUILayout.FloatField(floatValue);
                    }
                    else
                    {
                        floatValue = EditorGUILayout.FloatField(valuelabel, floatValue);
                    }
                }
            }
            else
            {
                if (GUILayout.Button(btnlabel, GUILayout.Width(Btnwidth), GUILayout.ExpandWidth(false)))
                {
                    mode = 8;
                }
                if (valuelabel == string.Empty)
                {
                    EditorGUILayout.Space(w2);
                    mode = EditorGUILayout.Popup(mode, valuemode, GUILayout.MinWidth(Popwidth));
                }
                else
                {
                    mode = EditorGUILayout.Popup(valuelabel, mode, valuemode, GUILayout.MinWidth(Popwidth));
                }

            }
            EditorGUIUtility.labelWidth = w;
            EditorGUILayout.EndHorizontal();
            return mode;
        }
        public static void ToBitViewGUI(this int debugUalue)
        {
            EditorGUILayout.LabelField("Debug Mode \n" + debugUalue.ToBitView(), new GUIStyle("AC BoldHeader"), GUILayout.Height(55));
        }

        public static string ToBitView(this int debugUalue)
        {
            string binaryString = Convert.ToString(debugUalue, 2).PadLeft(24, '0');
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
            //安全的MAX 长度说7f,ffff
            if ((int)(debugUalue >> 23) > 0)
            {
                ss += "\n 已经超过已知的安全长度";
            }
            return ss;
        }

        public static uint ToInt(this bool v)
        {
            return (uint)(v ? 1 : 0);
        }
        public static bool ToBool(this float v)
        {
            return (int)v > 0;
        }
        public static bool ToBool(this uint v)
        {
            return v > 0;
        }
        public static bool ToBool(this int v)
        {
            return v > 0;
        }
        public static bool ToBool(this int v, int index)
        {
            return (v >> index & 0x1).ToBool();
        }

        public static void YFDrawDefaultShaderProp(this MaterialEditor materialEditor, MaterialProperty prop, string label)
        {
            if (prop != null)
            {
                materialEditor.SetDefaultGUIWidths();
                materialEditor.DefaultShaderProperty(prop, label);
            }
        }
        public static void YFSetKeyWord(this Material material, bool v, string key)
        {
            if (v)
            {
                material.EnableKeyword(key);
            }
            else
            {
                material.DisableKeyword(key);
            }
        }

        public static int GetINTData(float v, int index, uint len)
        {
            uint a = (uint)v;
            a = a >> index & len;
            return (int)a;
        }
        public static bool GetINTData(float v, int index)
        {
            return GetINTData(v, index, 0x1) > 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">0</param>
        /// <param name="value">TRUE</param>
        /// <param name="len">0X1</param>
        /// <param name="v"></param>
        static void SetINTData(int index, int value, uint len, ref int v)
        {
            uint a = (uint)v;
            a &= ~(len << index);
            v = (int)(((uint)value) << index | a);
        }

        public static void SetINTData(int index, uint len, bool value, ref int v)
        {
            SetINTData(index, value ? 1 : 0, len, ref v);
        }
        public static void SetINTData(int index, uint len, int value, ref int v)
        {
            SetINTData(index, value, len, ref v);
        }
        public static GUIContent CreatGUIContent(string n, string help = "")
        {
            return EditorGUIUtility.TrTextContent(n, help);
        }

        public static void CreatGUIContent(out GUIContent src, string n, string help = "")
        {
            src = EditorGUIUtility.TrTextContent(n, help);
        }
        public static T Cbtn<T>(Func<T, T> method, GUIContent gUIContent, bool b, int w, T value)
        {
            T a = value;
            if (GUILayout.Button(gUIContent, CbtnGS("button", b), GUILayout.MaxWidth(w)))
            {
                a = method(value);
            }
            return a;
        }
        public static T Cbtn<T>(Func<T, T> method, GUIContent gUIContent, bool b, T value)
        {
            T a = value;
            if (GUILayout.Button(gUIContent, CbtnGS("button", b)))
            {
                a = method(value);
            }
            return a;
        }

        public struct btnStruct
        {
            public bool value;
            public string label;
            public string label2;
            public btnStruct(bool _m, string l1, string l2)
            {
                this.value = _m;
                label = l1;
                label2 = l2;
            }
        }
        public static bool Cbtn(this btnStruct d, int w)
        {
            return d.value.Cbtn(w, d.label, d.label2);
        }
        public static bool Cbtn(this bool value, int w, params string[] label)
        {
            if (GUILayout.Button(new GUIContent(label[0], label[1]), CbtnGS("button", value), GUILayout.MaxWidth(w)))
            {
                value = !value;
            }
            return value;
        }
        public static bool CbtnMin(this bool value, int w, params string[] label)
        {
            if (GUILayout.Button(new GUIContent(label[0], label[1]), CbtnGS("button", value), GUILayout.MinWidth(w)))
            {
                value = !value;
            }
            return value;
        }
        public static bool Cbtn(this bool value, params string[] label)
        {
            if (GUILayout.Button(new GUIContent(label[0], label[1]), CbtnGS("button", value)))
            {
                value = !value;
            }
            return value;
        }
        public static void Cbtn(GUIContent gUIContent, int w, ref bool value)
        {
            if (GUILayout.Button(gUIContent, CbtnGS("button", value), GUILayout.MaxWidth(w)))
            {
                value = !value;
            }
        }
        public static GUIStyle CbtnGS(string sy, bool s)
        {
            GUIStyle fgs = new GUIStyle(sy);
            Color c = s ? Color.green : Color.black;
            // fgs.fontStyle = FontStyle.Bold;
            fgs.hover.textColor = c;
            fgs.normal.textColor = c;
            return fgs;
        }

        public static void TooSmallToWrap(int minwidth = 300)
        {
            if (EditorGUIUtility.currentViewWidth < minwidth)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
        }
        public static void SetGUIWidth()
        {
            EditorGUIUtility.fieldWidth = 50;
            EditorGUIUtility.labelWidth = Mathf.Max(115, EditorGUIUtility.currentViewWidth * 0.39f);
            if (EditorGUIUtility.currentViewWidth < 300)
            {
                EditorGUIUtility.fieldWidth = 35;
            }
        }
        public static void SetGUIWidth(float currentViewWidth, float fieldWidth0, float labelWidth0, float fieldWidth1, float labelWidth1)
        {
            EditorGUIUtility.fieldWidth = fieldWidth0;
            EditorGUIUtility.labelWidth = labelWidth0;
            if (EditorGUIUtility.currentViewWidth < currentViewWidth)
            {
                EditorGUIUtility.fieldWidth = fieldWidth1;
                EditorGUIUtility.labelWidth = labelWidth1;
            }
        }
        public static void CleanMaterial(this Material o, int modeid)
        {
            Material mat = o as Material;
            if (mat == null)
            {
                return;
            }
            int dist = modeid >> ((int)YFfxModel.Distort) & 0x1;
            if (!dist.ToBool())
            {
                mat.SetTexture("_DistTex", null);
            }
            int colmask = modeid >> ((int)YFfxModel.ColorMask) & 0x1;
            if (!colmask.ToBool())
            {
                mat.SetTexture("_ColorMaskTex", null);
            }
            int diss = modeid >> ((int)YFfxModel.Dissolution) & 0x1;
            if (!diss.ToBool())
            {
                mat.SetTexture("_DissTex", null);
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
        public static void CleanMaterial(this Material o)
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
        private static void CleanMaterialShaderKeywords(SerializedProperty property, Material mat)
        {

            string s = "";
            for (int i = 0; i < mat.shaderKeywords.Length; i++)
            {
                s = s + mat.shaderKeywords[i].ToString() + " ";
            }
            mat.shaderKeywords = null;
        }

        private static void CleanMaterialSerializedProperty(SerializedProperty property, Material mat)
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
        public static readonly string[] YFFXkeys = new string[]{"_ALPHATEST_ON","_TIMEDT_ON","_FRESNEL_ON","_DTDISTOR_ON",
                                        "_DTCOLORMASK_ON","_DTDISSOLUTION_ON","_SPROUT_ON"};
        public static void CheckShaderKeys(this Material material, ref int keyCache)
        {
            if (material == null) return;
            string[] GlobalKeywords = material.shader.GetShaderKeyWords();
            for (int i = 0; i < YFFXkeys.Length; i++)
            {
                bool b = ((IList<string>)GlobalKeywords).Contains(YFFXkeys[i]);
                YFUtilFun.SetINTData(i, 0x1, b ? 1 : 0, ref keyCache);
            }
        }
        public static bool CheckShaderKeys(this Material material, string key)
        {
            if (material == null) return false;
            string[] GlobalKeywords = material.shader.GetShaderKeyWords();
            return ((IList<string>)GlobalKeywords).Contains(key);
        }
        public static string[] GetShaderKeyWords(this Shader shader)
        {
            MethodInfo getShaderGlobalKeywords = typeof(ShaderUtil).GetMethod("GetShaderGlobalKeywords", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])getShaderGlobalKeywords.Invoke(null, new object[] { shader });
        }
    }
    #endregion 


}