using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using YF.Art.PostProcessing;


namespace YF.Art
{
    class SkillEditDiteWindow : EditorWindow
    {

        static SkillEditDite tag;

        static ColorSpace setColorspace = ColorSpace.Linear;
        string u3dVersion = "2019.4.30";// "Version of the runtime: 2019.4.30f1c3"; 
        Vector2 fenbianlv = new Vector2(1920, 1080);
        Camera basecamera;
        static int index = 2;
        bool ishasAndroid = true;
        static SkillEditDiteWindow window;
        static UnityEngine.Object mymima;
        string mimaStr = "";

        [MenuItem("Tools/Skill Tools Window")]
        public static void ShowWindow()
        {
            YFToolsInit.isrun = true;
            if (EditorWindow.HasOpenInstances<SkillEditDiteWindow>())
            {
                return;
            }
            window = GetWindow<SkillEditDiteWindow>();
            window.titleContent = new GUIContent("YoFi Effect Tools");
            window.position = new Rect(50, 50, 750, 350);
            window.Show();
            if (CheckError())
            {
                InitObject();
            }
        }


        void OnInspectorUpdate()
        {
            Repaint();
        }

        static bool CheckError()
        {
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget != UnityEditor.BuildTarget.Android) return false;
            if (UnityEditor.PlayerSettings.colorSpace != setColorspace) return false;
            var tt = UnityEditor.Rendering.EditorGraphicsSettings.GetTierSettings(UnityEditor.BuildTargetGroup.Android, Graphics.activeTier);
            if (!tt.hdr) return false;
            return true;
        }

        void unLock()
        {
            EditorGUI.BeginChangeCheck();
            mymima = EditorGUILayout.ObjectField(mymima, typeof(UnityEngine.Object), true);
            string g = EditorGUILayout.TextField("密码");
            if (EditorGUI.EndChangeCheck())
            {
                if (mymima != null)
                {
                    string gu = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(mymima));
                    if (gu == g)
                    {
                        YFToolsInit.isrun = false;
                    }
                }

            }

        }

        bool ErrorGUI()
        {
            Color c = GUI.backgroundColor;
            string v0 = Application.unityVersion.ToString().Replace("Version of the runtime: ", "");
            bool canrun = true;
            string errortxt = "Dite Error: ";
#if !UNITY_ANDROID
            errortxt += "\n  ◆ 平台错误:请切换平台为 [ 安卓 ]。";
            canrun = false;
#endif
            if (PlayerSettings.colorSpace != setColorspace)
            {
                errortxt += "\n  ◆ 色域空间未设置为 [ " + setColorspace.ToString() + " ]";
                canrun = false;
            }
            var tt = UnityEditor.Rendering.EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Android, Graphics.activeTier);
            if (!tt.hdr)
            {
                errortxt += "\n  ◆ 请检查 [ HDR 未打开  ]。";
                canrun = false;
            }
            if (!canrun)
            {
                GUI.backgroundColor = Color.red;
                if (UnityEditor.Handles.GetMainGameViewSize() != fenbianlv)
                {
                    errortxt += string.Format("\n  ◆ 分辨率请设置成  [ {0} x {1} ]。", fenbianlv.x, fenbianlv.y);
                }

                if (!v0.StartsWith(u3dVersion))
                {
                    errortxt += "\n  ◆ Unity3d的版本不对,应为 [ 2019.4.30系列的版本 ]。";
                }

                if (GUILayout.Button("修复错误"))
                {
                    ishasAndroid = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
                    if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                    {
                        ishasAndroid = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                    }
                    if (ishasAndroid)
                    {
                        PlayerSettings.colorSpace = setColorspace;
                        if (!tt.hdr)
                        {
                            UnityEditor.Rendering.TierSettings t1 = UnityEditor.Rendering.EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Android, UnityEngine.Rendering.GraphicsTier.Tier1);
                            t1.hdr = true;
                            UnityEditor.Rendering.EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Android, UnityEngine.Rendering.GraphicsTier.Tier1, t1);
                            UnityEditor.Rendering.TierSettings t2 = UnityEditor.Rendering.EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Android, UnityEngine.Rendering.GraphicsTier.Tier2);
                            t2.hdr = true;
                            UnityEditor.Rendering.EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Android, UnityEngine.Rendering.GraphicsTier.Tier2, t2);
                            UnityEditor.Rendering.TierSettings t3 = UnityEditor.Rendering.EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Android, UnityEngine.Rendering.GraphicsTier.Tier3);
                            t3.hdr = true;
                            UnityEditor.Rendering.EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Android, UnityEngine.Rendering.GraphicsTier.Tier3, t3);
                        }
                    }
                }
                if (!ishasAndroid)
                {
                    errortxt = "Dite Error: \n  ◆◆ 请检查 [ 安卓 模块未安装 ]。" + errortxt.Replace("Dite Error: ", "");
                }
                errortxt += "\n ★★★ 请解决所有的错误 ★★★ \n修正需要时间,点击后等待一段时间，看窗口是否有变化，没有再点一次。";
                EditorGUILayout.HelpBox(errortxt, MessageType.Error);
            }
            else
            {
                GUI.backgroundColor = Color.yellow;
                bool ise = false;
                string errortxt0 = "Dite Warning: ";
                if (UnityEditor.Handles.GetMainGameViewSize() != fenbianlv)
                {
                    errortxt0 += string.Format("\n  ◆ 分辨率请设置成  [ {0} x {1} ]。", fenbianlv.x, fenbianlv.y);
                    ise = true;
                }
                if (!v0.StartsWith(u3dVersion))
                {
                    errortxt0 += "\n  ◆ Unity3d的版本不对,应为 [ " + u3dVersion + "系列的版本 ]。";
                    ise = true;
                }
                if (ise)
                {
                    EditorGUILayout.HelpBox(errortxt0, MessageType.Warning);
                }
            }
            GUI.backgroundColor = c;
            this.Repaint();
            return canrun;

        }

        public static void InitObject()
        {
            var t = Transform.FindObjectOfType<SkillEditDite>() as SkillEditDite;
            if (t == null)
            {
                GameObject g = new GameObject();
                g.AddComponent<SkillEditDite>();
                g.transform.position = Vector3.zero;
                g.transform.rotation = Quaternion.identity;
                SetCamera();
            }

        }

        void OnGUI()
        {
            // Debug.Log(position);
            if (tag == null)
            {
                tag = Transform.FindObjectOfType<SkillEditDite>() as SkillEditDite;
            }
            EditorGUILayout.SelectableLabel("掌门下山 Skill 辅助工具", "LODLevelNotifyText");
            if (!this.ErrorGUI()) return;

            if (tag == null)
            {
                EditorGUILayout.HelpBox("场景中未发现需要的脚本  SkillEditDite  ;", MessageType.Info);
                if (GUILayout.Button("创建"))
                {
                    InitObject();
                    // GameObject g = new GameObject();
                    // g.AddComponent<SkillEditDite>();
                    // g.transform.position = Vector3.zero;
                    // g.transform.rotation = Quaternion.identity;
                    // this.SetCamera();
                }
                return;
            }

            {

                EditorGUILayout.BeginVertical("BOX");
                EditorGUI.BeginChangeCheck();

                // tag.AttackMode = (YF.Art.SKILL_MODE)EditorGUILayout.EnumPopup(new GUIContent("攻击模式:"), tag.AttackMode);
                tag.AttackMode = (YF.Art.SKILL_MODE)EditorGUILayout.Popup(new GUIContent("攻击模式:"), (int)tag.AttackMode, new string[] { "自身出发范围的技能", "目标身上范围的技能" });
                // tag.bg = SceneBG; 
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(new Object[] { this, tag }, "SkillEditDite [Dite Tools]");
                    if (tag.AttackMode == SKILL_MODE.TAG)
                    {
                        tag.showMode = SKILL_EDIT_MOD.RANGE;
                        tag.Node.transform.position = tag.TagMode0 == null ? new Vector3(3, 0, 0) : tag.TagMode0.position;
                        tag.TagFar = 300;
                        tag.distance1 = 200;
                    }
                    else
                    {
                        tag.showMode = SKILL_EDIT_MOD.LINE;
                        tag.distance1 = 800;
                        tag.width = 200;
                    }
                    tag.setMode();
                    tag.CheckMeshNode();
                }

                EditorGUI.BeginChangeCheck();
                if (tag.AttackMode == SKILL_MODE.SELF || tag.AttackMode == SKILL_MODE.TAG)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        float size = 20;
                        EditorGUILayout.BeginVertical(GUILayout.Width(80));
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("7", GUILayout.Width(size), GUILayout.Height(size)))
                        {
                            index = 7;
                        }
                        if (GUILayout.Button("0", GUILayout.Width(size), GUILayout.Height(size)))
                        {
                            index = 0;
                        }
                        if (GUILayout.Button("1", GUILayout.Width(size), GUILayout.Height(size)))
                        {
                            index = 1;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("6", GUILayout.Width(size), GUILayout.Height(size)))
                        {
                            index = 6;
                        }
                        if (GUILayout.Button(" ", GUILayout.Width(size), GUILayout.Height(size)))
                        {

                            // EditorUtility.DisplayDialog("Dite Print Error", "色域空间未设置为  Linear", "OK");
                        }
                        if (GUILayout.Button("2", GUILayout.Width(size), GUILayout.Height(size)))
                        {
                            index = 2;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("5", GUILayout.Width(size), GUILayout.Height(size)))
                        {
                            index = 5;

                        }
                        if (GUILayout.Button("4", GUILayout.Width(size), GUILayout.Height(size)))
                        {
                            index = 4;
                        }
                        if (GUILayout.Button("3", GUILayout.Width(size), GUILayout.Height(size)))
                        {
                            index = 3;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                        this.setRoleDir(index);
                    }
                    {
                        EditorGUIUtility.labelWidth = 80;
                        EditorGUILayout.BeginVertical();
                        if (tag.AttackMode == SKILL_MODE.TAG)
                        {
                            tag.showMode = SKILL_EDIT_MOD.RANGE;
                            tag.Node.transform.position = tag.TagMode0 == null ? new Vector3(4, 0, 0) : tag.TagMode0.position;
                            tag.TagFar = EditorGUILayout.FloatField("目标距离", tag.TagFar);
                        }
                        else
                        {
                            tag.showMode = (SKILL_EDIT_MOD)(EditorGUILayout.Popup(new GUIContent("范围模式:"),
                                (int)tag.showMode - 1, new string[] { "直线攻击", "圆范围攻击", "扇形攻击" }) + 1);
                        }

                        tag.color = EditorGUILayout.ColorField("范围标注颜色:", tag.color);
                        switch (tag.showMode)
                        {
                            case SKILL_EDIT_MOD.CUSTOM:
                                tag.width = EditorGUILayout.FloatField("宽:", tag.width);
                                tag.distance1 = EditorGUILayout.FloatField("长:", tag.distance1);
                                break;
                            case SKILL_EDIT_MOD.LINE:
                                tag.width = EditorGUILayout.FloatField("宽:", tag.width);
                                tag.distance1 = EditorGUILayout.FloatField("长:", tag.distance1);
                                break;
                            case SKILL_EDIT_MOD.RANGE:
                                tag.distance1 = EditorGUILayout.FloatField("半径:", tag.distance1);
                                break;
                            case SKILL_EDIT_MOD.SECTOR:
                                tag.distance1 = EditorGUILayout.FloatField("半径:", tag.distance1);
                                tag.angle = EditorGUILayout.FloatField("开口角度:", tag.angle);
                                break;
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(new Object[] { this, tag }, "SkillEditDite [Dite Tools]");
                    tag.changeDir(index);
                    tag.setMode();
                    tag.CheckMeshNode();
                }

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("设置摄像机"))
                    {
                        SetCamera();
                    }
                }
                if (GUILayout.Button(new GUIContent("角色显示开关", "可以关闭中心显示的角色，方便显示其他角色"), GUILayout.Width(80)))
                {
                    this.setRoleView();
                }
                Color dc = GUI.backgroundColor;
                GUI.backgroundColor = YFToolsInit.isrun ? dc : Color.red;
                if (GUILayout.Button(new GUIContent(YFToolsInit.isrun ? "开" : "关"), GUILayout.Width(10)))
                {

                    isunlock = !isunlock;
                }
                GUI.backgroundColor = dc;
                EditorGUILayout.EndHorizontal();
                if (isunlock)
                {
                    unLock();
                }
                string helpTxt = @"特别注意：
　1: 【不允许使用中文字符】，包括不限于(对象，贴图，材质球，目录等)。
　2: 每个技能的资源是独立的。【不允许公用】，相同的资源，需要复制，并且要注意使用【新复制出来的资源】并放到【对应的效果目录】下。
　3: 所有预制体的【Root必须清零】，并且是一个Empty[只有Transform组件]对象。
制作时:
　　自身效果的预制体【放到“Self_Root”】下制作。
　　目标效果的预制体【放到“Tag_Root”】下制作。
　4: 除飞行物特效外，效果Root节点的锚点都应和角色锚点重合，【偏移做在Root的子节点上】。
　5: 技能朝向默认是向：【正Z方向】。
　6: 只能使用提供的shader。【不要使用任何系统默认的贴图、模型和shader】。
　7: 优先使用[粒子系统]，复杂动画使用[Animator系统],【不能使用[Animation系统]】,同预制体下，使用动画时候，应尽可能合并动画，减少动画文件";
                EditorGUILayout.HelpBox(helpTxt, MessageType.Info);
            }
        }
        bool isunlock = false;
        void setRoleDir(int index)
        {
            if (tag == null) return;
            if (tag.bg0 == null) return;
            if (tag.bg0.transform.Find("Role") == null) return;
            if (tag.bg0.transform.Find("Role").gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer == null) return;
            // tag.bg0.transform.hideFlags = HideFlags.None; 
            tag.bg0.transform.Find("Role").gameObject.GetComponent<SpriteRenderer>().flipX = index > 3;
        }
        void setRoleView()
        {
            if (tag == null) return;
            if (tag.bg0.transform.Find("Role") == null) return;
            tag.bg0.transform.Find("Role").gameObject.SetActive(!tag.bg0.transform.Find("Role").gameObject.activeInHierarchy);
        }
        static void SetCamera()
        {
            if (Camera.main == null)
            {
                GameObject gg = new GameObject("Main Camera");
                gg.AddComponent<Camera>();
                gg.tag = "MainCamera";
            }
            var c = Camera.main;
            Matrix4x4 matr = Matrix4x4.Rotate(Quaternion.Euler(41.3f, 0, 0));
            Matrix4x4 matp = Matrix4x4.Translate(new Vector3(0, 0, -50f));
            c.transform.position = (matr * matp).MultiplyPoint3x4(Vector3.zero);//new Vector3(0f, 25.03f, -28.48f);
            c.transform.rotation = Quaternion.Euler(41.3f, 0, 0);
            c.orthographic = true;
            c.orthographicSize = 5.4f;
            c.farClipPlane = 100;
            YFPostProcessing g;
            if (!c.gameObject.TryGetComponent(out g))
            {
                g = c.gameObject.AddComponent<YFPostProcessing>();
            }
            setBloom(g);
        }

        static void setBloom(YFPostProcessing g)
        {
            g.hdr = HDRBloomMode.On;
            g.sepBlurSpread = 3;
            g.bloomIntensity = 2;
            g.bloomThreshold = 1;
            g.bloomThresholdColor = Color.white;
            g.bloomBlurIterations = 5;
            g.screenBlendMode = BloomScreenBlendMode.Add;
            g.quality = BloomQuality.High;
            g.isBloomRun = true;
        }

        static void setisrun()
        {
            YFToolsInit.isrun = !YFToolsInit.isrun;
        }
    }


    public class YFToolsInit
    {

        [InitializeOnLoadMethod]
        static void Start()
        {
            SkillEditDiteWindow.ShowWindow();
            EditorApplication.update -= OnSkillToolsUpdateEditor;
            EditorApplication.update += OnSkillToolsUpdateEditor;
            // SceneView.duringSceneGui += OnSceneGUI;
        }
        public static bool isrun = true;

        static void OnSkillToolsUpdateEditor()
        {
            if (!isrun) return;
            if (!EditorWindow.HasOpenInstances<SkillEditDiteWindow>())
            {
                SkillEditDiteWindow.ShowWindow();
            }
            var gg = Selection.activeObject as GameObject;
            if (gg == null)
            {
                return;
            }
            else
            {
                if (gg.tag == "EditorOnly")
                {
                    Selection.activeObject = null;
                }
            }
        }
    }
}