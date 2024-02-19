using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using YF.Art.EdWindowUilts;
using UnityEditorInternal;


namespace YF.Art.FXRenderStudio
{
    public class animtordata{
        public Animator animator;
        public bool Isbake;
    }
    public class IScreenRecordingToolWindow : EditorWindow
    {        
        ScreenRecordingToolNAME GUI_NAME = new ScreenRecordingToolNAME();
        
        private string productName
        {
            get
            {
                return Application.productName;
            }
        }
        // private List<GameObject> renderobj =new List<GameObject>();
        
        // private bool isEnabled;
        private Camera cam;
        private Vector2 resolution;
        private bool isStartRender;
        private int frameRate = 30;
        private string[] frameCountStrArry = new string[]{ "10","15","24","25","30","60" };
        private int[] frameCountIntArry = new int[]{ 10,15,24,25,30,60 };
        private bool defRender = true;
        private Color bgcolor = Color.black;
        // private bool isEnabledAlpha;
        private string fileName="screenShot";
        private string filePath = "";
        private int rangeStart = 1;
        private int rangeEnd = 100;
        private float progress;
        private bool isEndStop;
        private bool A = true;
        private bool B = false;

        private int masklayer = 0;

        // private Color bgcolor = Color.black;
        // static Vector2 sizeaa;
        // private RenderTexture rendertex;
        private Rect rc;
        private float jiange_dite =0f;


        private List<Animation> animobj = new List<Animation>();
        private List<animtordata> antoobj = new List<animtordata>();
        private List<ParticleSystem> parobj = new List<ParticleSystem>();
        private List<Transform> renderobj = new List<Transform>();
        private List<Transform> oldrenderobj = new List<Transform>();
        bool Ispreview = false;
        bool previewPlay = false;
        private float time;
        float m_PreviousTime = 0 ;     
        bool IsHeloview =false;   

        [MenuItem("Dite_Tools/特效渲染[YoFi]")]
        static void ShowWindow()
        {
            IScreenRecordingToolWindow window = (IScreenRecordingToolWindow)EditorWindow.GetWindow(typeof(IScreenRecordingToolWindow),false);
            window.minSize = new Vector2(300, 480);
            // ImageExporterEditorWindow.sizeaa = window.minSize;
            window.titleContent = new GUIContent(ScreenRecordingToolNAME.NAME_windowTitle);
            window.Show();
            EditorPrefs.SetInt("IScreenRecordingToolWindow_srcframeCount",Time.captureFramerate);
            
        }

        private void Awake() {            
            this.oldrenderobj.Clear();
            this.renderobj.Clear();
            this.getobjforrender(Selection.transforms.ToList()); 
        }

        void OnEnable()
        {            
            this.GUI_NAME.init();
            Application.runInBackground = true;
           
            Time.captureFramerate = frameRate;

            if (EditorPrefs.HasKey(productName + "_masklayer"))
                masklayer = EditorPrefs.GetInt(productName + "_masklayer");
            else
                masklayer = 0;

            if (EditorPrefs.HasKey(productName + "_frameCount"))
                frameRate = EditorPrefs.GetInt(productName + "_frameCount");
            else
                frameRate = 30;

            if (EditorPrefs.HasKey(productName + "_resolutionX"))
                resolution.x = EditorPrefs.GetInt(productName + "_resolutionX");
            else
                resolution.x = 1280;

            if (EditorPrefs.HasKey(productName + "_resolutionY"))
                resolution.y = EditorPrefs.GetInt(productName + "_resolutionY");
            else
                resolution.y = 720;

            if (EditorPrefs.HasKey(productName + "_fileName"))
                fileName = EditorPrefs.GetString(productName + "_fileName");
            else
                fileName = "screenShot";

            if (EditorPrefs.HasKey(productName + "_filePath"))
                filePath = EditorPrefs.GetString(productName + "_filePath"); 
            else
                filePath = Application.dataPath;

            if (EditorPrefs.HasKey(productName + "_rangeStart"))
                rangeStart = EditorPrefs.GetInt(productName + "_rangeStart");
            else
                rangeStart = 0;

            if (EditorPrefs.HasKey(productName + "_rangeEnd"))
                rangeEnd = EditorPrefs.GetInt(productName + "_rangeEnd");
            else
                rangeEnd = 100;
            if (!this.isStartRender)
                this.getobjforrender(Selection.transforms.ToList());   
        }

        void OnDisable()
        {             
            EPUilts.SetEP(productName + "_fileName", fileName);
            EPUilts.SetEP(productName + "_frameCount", frameRate);
            EPUilts.SetEP(productName + "_resolutionX", (int)resolution.x);
            EPUilts.SetEP(productName + "_resolutionY", (int)resolution.y);
            EPUilts.SetEP(productName + "_filePath", filePath);
            EPUilts.SetEP(productName + "_rangeStart", rangeStart);
            EPUilts.SetEP(productName + "_rangeEnd", rangeEnd);
            EPUilts.SetEP(productName + "_masklayer", masklayer);
        }
        private void OnSelectionChange() {
            if(!this.isStartRender){
                this.getobjforrender(Selection.transforms.ToList());

            this.renderobj.Clear();
            this.renderobj = Selection.transforms.ToList();            
            for (int i = 0; i < this.renderobj.Count; i++){
                if(this.oldrenderobj.Exists(t => t == this.renderobj[i]))
                {
                    this.renderobj.RemoveAt(i);
                }
            }
            this.oldrenderobj.AddRange(this.renderobj);
            }
        }

        private void OnDestroy() {
            if(!this.isStartRender){
                this.getobjforrender(this.oldrenderobj);            
                this.runOBj(0.0f);
            }
            Time.captureFramerate = EditorPrefs.GetInt("IScreenRecordingToolWindow_srcframeCount");
        }
        void OnInspectorUpdate()
        {
            Repaint();
        }         

        void OnGUI()
        {   
            

            string helpdata ="感谢原作者： 李红伟\t原工具名:taecgLibrary\n改进者：缔特 （Dite） \t2021 4 28 v2.10";
            if(GUILayout.Button("╮(￣ ▽￣)╭ =====特效渲染工具===== ╮(￣▽ ￣)╭",GUILayout.Height(30))){
                Debug.Log("╮(￣ ▽￣)╭=====特效渲染工具=====╮(￣▽ ￣)╭\n" + helpdata);
                IsHeloview = !IsHeloview;
            }
            EditorGUILayout.BeginHorizontal(); 
            EditorGUILayout.LabelField(" ");
            if(GUILayout.Button("特效预览工具",GUILayout.MaxWidth(150))){
                Ispreview = !Ispreview;
            }  
            string helpbutton = IsHeloview?"关闭帮助":"开启帮助";
            if(GUILayout.Button(helpbutton,GUILayout.MaxWidth(88))){  
                // if(!IsHeloview)  
                //     EditorUtility.DisplayDialog("提示：",helpdata ,"OK",null);        
                IsHeloview = !IsHeloview;    
                
            }
            EditorGUILayout.EndHorizontal();
            
            if(Ispreview)       
            { 
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal();
                float _time = this.time * this.frameRate;
                // "控制器:",
                EditorGUILayout.LabelField("控制器:",GUILayout.MaxWidth(40),GUILayout.ExpandWidth(false));
                _time = EditorGUILayout.IntSlider((int)_time,this.rangeStart,this.rangeEnd);
                if(!this.previewPlay){
                    this.time = _time / this.frameRate;
                }
                if (GUILayout.Button("⊙", GUILayout.Width(20),GUILayout.ExpandWidth(false))){
                    this.time = this.frame2time(this.rangeStart);
                }   
                EditorGUILayout.EndHorizontal();
                // this.time = _time / this.frameCount;
                
                string psbut = this.previewPlay ? "Stop":"Play";
                if(GUILayout.Button(psbut)){
                    this.previewPlay = !this.previewPlay;
                    if(this.previewPlay){
                        m_PreviousTime = (float)EditorApplication.timeSinceStartup;
                    }
                }    
                EditorGUILayout.EndHorizontal();
                // 
                if(IsHeloview){
                    EditorGUILayout.HelpBox("特效预览工具可以方便查看大部分特效。使用方法：选择需要预览的对象（可以是最高父级），然后拖动滑杆或点击play即可。",MessageType.Info) ;
                }
                EditorGUILayout.EndVertical();
                 
            }else{
                this.getobjforrender(this.oldrenderobj);
                this.runOBj(0.0f);
            }
            
            EditorGUILayout.Space(5);
            int oo = (int)((rangeEnd - rangeStart) * progress+rangeStart);
             

            this.xiahuaxian();
            //设置渲染相机
            {
                cam = EditorGUILayout.ObjectField(GUI_NAME.NAME_selectedCamera, cam, typeof(Camera),true)as Camera;
                if (cam == null)
                {
                    cam = Camera.main;
                }
                
                if(this.IsHeloview){EditorGUILayout.BeginVertical("box");} else{EditorGUILayout.BeginVertical();} 
                this.masklayer = this.EditorGUILayout_MaskField(this.masklayer,"Render Layer");                                      
                if(IsHeloview){ 
                    EditorGUILayout.HelpBox("[重要！！！]设置渲染目标的Layer，通过Layer来区分渲染的内容",MessageType.Warning); 
                }              
                EditorGUILayout.EndVertical();
            }
            this.xiahuaxian();
            //设置辨率
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(GUI_NAME.NAME_setResolution);
                resolution = EditorGUILayout.Vector2Field(GUIContent.none, resolution);
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("默认尺寸"))
                    resolution = new Vector2(640, 640);
                if (GUILayout.Button("获取当前视图尺寸"))
                    resolution = Handles.GetMainGameViewSize();
            }
            
            
            // 时间线模块
            {
                EditorGUI.BeginChangeCheck();
                {
                    //设置帧率
                    this.frameRate = EditorGUILayout.IntPopup(GUI_NAME.NAME_setFrame, this.frameRate, frameCountStrArry, frameCountIntArry);
                }                    
                if(EditorGUI.EndChangeCheck()){
                    int oldframeRate = 30;
                    if (EditorPrefs.HasKey(productName + "_frameCount"))
                        oldframeRate = EditorPrefs.GetInt(productName + "_frameCount");                        
                    this.rangeStart = (int)((float)rangeStart / (float)oldframeRate * (float)this.frameRate);
                    this.rangeEnd = (int)((float)rangeEnd / (float)oldframeRate * (float)this.frameRate);
                    EditorPrefs.SetInt(productName + "_frameCount",this.frameRate);
                }
                //帧数范围
                {
                    EditorGUILayout.BeginHorizontal();
                    GUIStyle style = new GUIStyle("textfield");
                    EditorGUILayout.PrefixLabel(GUI_NAME.NAME_setFrameRange);

                    rangeStart = EditorGUILayout.IntField(rangeStart, style);
                    rangeEnd = EditorGUILayout.IntField(rangeEnd, style);

                    EditorGUILayout.EndHorizontal();
                }
            }
            this.xiahuaxian();
            {
                EditorGUILayout.BeginHorizontal();
                GUIStyle style = new GUIStyle("textfield");
                EditorGUILayout.LabelField(GUI_NAME.NAME_setFileName,GUILayout.Width(120),GUILayout.ExpandWidth(false));
                fileName = EditorGUILayout.TextField(fileName, style);
                if (string.IsNullOrEmpty(fileName)){
                    fileName = "screenShot";
                }
                EditorGUILayout.EndHorizontal();
            }
            
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(GUI_NAME.NAME_setSavePath,GUILayout.Width(120),GUILayout.ExpandWidth(false));
                string aass = filePath;
                if (GUILayout.Button("...", GUILayout.Width(80),GUILayout.ExpandWidth(true)))
                {
                    filePath = EditorUtility.OpenFolderPanel("", filePath, "");
                }
                if (GUILayout.Button("⊙", GUILayout.Width(20),GUILayout.ExpandWidth(false)))
                {    
                    Debug.Log(filePath);                    
                    filePath = Application.dataPath;
                }
                if(filePath == ""){
                    filePath = aass;
                } 
                //打开文件夹
                {
                    if (GUILayout.Button("打开导出文件夹",GUILayout.Width(80),GUILayout.ExpandWidth(true)))
                    {
                        Application.OpenURL("file://" + filePath);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal("box");
                // EditorGUILayout.PrefixLabel("       ");
                GUIStyle style02 = new GUIStyle("label");
                style02.fontStyle = FontStyle.Italic;
                EditorGUILayout.LabelField(filePath, style02);
                EditorGUILayout.EndHorizontal();
            }
                 
            if(IsHeloview){
                EditorGUILayout.BeginVertical("box");
            }
            this.xiahuaxian();
            // EditorGUILayout.Space(-18);
            // 模式改变
            {
                this.defRender = EditorGUILayout.Toggle("截屏模式(无通道)(源模式)",this.defRender);
                if(this.defRender)
                {   
                    this.bgcolor = EditorGUILayout.ColorField("背景色",this.bgcolor);  
                    if(IsHeloview){          
                        EditorGUILayout.HelpBox("截屏模式下：\n只做屏幕截图[含序列帧],但不再考虑A通道问题,部分默认shader会通道显示不正确或丢失\n推荐把背景色设置为不透明来获得正确的显示效果！",MessageType.Warning);
                    }
                }
                else{
                    if(IsHeloview){     
                        EditorGUILayout.HelpBox("非截屏模式下：\n1 如需得到正确的通道，需要使用YOFI提供的自定义Shader。\n2 导出的图片会分ADD和BLEND 对应的是AE种ADD 和 NORMAL 2种混合模式！",MessageType.Info);
                    }
                }
                if(IsHeloview){
                    EditorGUILayout.EndVertical();
                }
                this.xiahuaxian();
                EditorGUILayout.BeginVertical("BOX");
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Toggle(A ,"单帧导出",EditorStyles.toolbarButton, GUILayout.Height(20)))
                {
                    this.IsPathExists();
                    A=true;
                    B=false;
                }
                if(GUILayout.Toggle(B ,"序列导出",EditorStyles.toolbarButton, GUILayout.Height(20))) 
                {
                    this.IsPathExists();
                    A=false;
                    B=true;
                }
                EditorGUILayout.EndHorizontal();
            }
            if(A)
            {
                //导出当前截图
                {
                    
                    EditorGUILayout.BeginHorizontal();  

                    if (GUILayout.Button(GUI_NAME.NAME_takeScreenShot, GUILayout.Height(30)))
                    {
                        int cc = (int)cam.clearFlags;
                        Color cc2 = cam.backgroundColor;
                        int cl = cam.cullingMask;
                        cam.backgroundColor =  new Color(0,0,0,0);
                        if(this.defRender){
                            cam.backgroundColor =  this.bgcolor;
                        }
                        
                        cam.clearFlags = CameraClearFlags.SolidColor;
                        cam.cullingMask = this.masklayer;
                        if(!this.defRender){
                            Shader.SetGlobalFloat("_ImageExporterRenderShaderADDMASK",0);
                            Shader.SetGlobalFloat("_ImageExporterRenderShaderBlendMASK",1);
                            TakeScreenShot(true);
                            Shader.SetGlobalFloat("_ImageExporterRenderShaderADDMASK",1);
                            Shader.SetGlobalFloat("_ImageExporterRenderShaderBlendMASK",0);
                            TakeScreenShot(false);
                            Shader.SetGlobalFloat("_ImageExporterRenderShaderADDMASK",0);
                            Shader.SetGlobalFloat("_ImageExporterRenderShaderBlendMASK",0);
                        }
                        else{
                            TakeScreenShot(true,true);
                        }
                        cam.cullingMask =cl;
                        cam.clearFlags = (CameraClearFlags)cc;
                        cam.backgroundColor =  cc2;
                        Debug.Log(string.Format("截图成功"));
                    }                      
                    EditorGUILayout.EndHorizontal();                 
                    EditorGUILayout.HelpBox(GUI_NAME.NAME_takeScreenShotDescribution, MessageType.None);
                }
            }
            if(B)
            //导出序列图
            {
                {
                    EditorGUILayout.BeginVertical();    
                    //如果处于运行状态则显示进度条
                    if (EditorApplication.isPlaying  && this.isStartRender)
                    {
                        GUIContent st = new  GUIContent(EditorGUIUtility.IconContent("PlayButton On"));                       
                        st.text = "    停止输出    ";                        
                        GUIContent st0 = new  GUIContent(EditorGUIUtility.IconContent("PauseButton On"));                        
                        if(!EditorApplication.isPaused )
                        {
                            st0 = new  GUIContent(EditorGUIUtility.IconContent("PauseButton"));
                            st0.text = "    暂停    ";
                        }
                        else{
                            st0 = new  GUIContent(EditorGUIUtility.IconContent("PlayButton"));
                            st0.text = "    开始渲染    ";
                        }
                        
                        EditorGUILayout.BeginHorizontal();                        
                        if (GUILayout.Button(st0, GUILayout.Height(30)))
                        {                            
                            EditorApplication.isPaused = !EditorApplication.isPaused;
                        }
                        if (GUILayout.Button(st, GUILayout.Height(30)))
                        {
                            EditorApplication.isPlaying = false;
                            EditorApplication.isPaused = false;
                            this.isStartRender = false;
                            
                        }
                        EditorGUILayout.EndHorizontal();
                        this.rc = EditorGUILayout.GetControlRect();
                        EditorGUI.ProgressBar(new Rect(this.rc.x, this.rc.y,this.rc.width, this.rc.height*2), (float)progress , "正在导出第 ["+ oo +"] 序列图..."+(progress * 100) + "%");
                    }
                    else
                    {
                        //开始导出                   
                        GUIContent st = new  GUIContent(EditorGUIUtility.IconContent("PlayButton"));
                        st.text = "    渲染准备    " ;

                        if (GUILayout.Button(st, GUILayout.Height(30)))
                        {                 
                            if(EditorApplication.isPlaying && !this.isStartRender){
                                EditorApplication.isPlaying = false;
                                Event evt = Event.current;
                                Vector2 mousePos = evt.mousePosition;
                                EditorUtility.DisplayDialog("提示：","    当前处于Play运行状态，必须先退出Play状态后再次开启。\n谢谢！" ,"OK",null);
                            }
                            else{
                                this.time = 0;
                                EditorApplication.isPaused = true;
                                EditorApplication.isPlaying = true;
                                this.isStartRender = true;
                            }
                            
                        }
                        EditorGUILayout.HelpBox(GUI_NAME.NAME_startExportDescribution, MessageType.None);
                    }           
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
            
            
            
            EditorGUILayout.HelpBox(helpdata,MessageType.None);
            GUILayout.Space(10);

            
            
            
            Repaint();
        }
        
        
        void Update()
        {
            // Debug.Log("Update"); !EditorApplication.isPlaying &&
            if(!this.isStartRender){ 
                if(this.previewPlay){
                    float framedelta = this.time2frame(this.time) + this.TimeUpdate();
                    if(framedelta >= this.rangeStart && framedelta <= this.rangeEnd)
                    {
                        this.time = this.frame2time(framedelta);
                        this.runOBj( this.time);
                    }
                    if(framedelta > this.rangeEnd)
                    {
                        this.time = this.frame2time( this.rangeStart);
                    }
                }else{
                    this.runOBj(this.time);
                }   
                //===============================        
                // if(this.previewPlay){
                //     // float delta = (float)EditorApplication.timeSinceStartup - m_PreviousTime;
                //     // this.time = this.time + 0.01f;
                //     float _delta = (float)this.TimeUpdate();
                //     this.time += _delta;
                //     if(this.time > ((float)this.rangeStart - 1.0f) / (float)this.frameCount  
                //                     && this.time < ((float)this.rangeEnd - 1.0f) / (float)this.frameCount )
                //     { 
                //         this.runOBj( this.time);
                //     } 
                //     if(this.time >= ((float)this.rangeEnd - 1.0f) / (float)this.frameCount ){
                //         this.time = this.rangeStart * (1.0f / this.frameCount);
                //     }
                //     m_PreviousTime = (float)EditorApplication.timeSinceStartup;
                //     this.runOBj(this.time);
                // }else{
                //     this.runOBj(this.time);
                // }                
                //=============================== 
            }
            // Debug.Log(this.time);
            if(EditorApplication.isPlaying){
                if(!this.isStartRender){
                    Time.captureFramerate = EditorPrefs.GetInt("IScreenRecordingToolWindow_srcframeCount");
                }
                else{
                    Time.captureFramerate = this.frameRate;
                }
            }
            
            
            if(this.isStartRender){
                this.getobjforrender(this.oldrenderobj);            
                this.runOBj(0.0f);
                this.oldrenderobj.Clear();
            }            
            if (EditorApplication.isPlaying && !EditorApplication.isPaused && this.isStartRender)
            {
                int frame = Time.frameCount - 2;
                if (frame >= this.rangeStart && frame <= this.rangeEnd)
                {
                    ScreenRecordingToolController sc = GameObject.FindObjectOfType<ScreenRecordingToolController>();
                    if (sc == null)
                    {
                        GameObject _obj = new GameObject("ScreenShotController");
                        sc = _obj.AddComponent<ScreenRecordingToolController>();
                    }
                    sc.cam = cam;
                    sc.defrender = this.defRender;
                    sc.renderingbgcolor = this.bgcolor;
                    sc.imageFormat = ".png";
                    sc.resolution = resolution;
                    sc.frameCount = frameRate;
                    sc.fileName = fileName;
                    sc.masklayer = masklayer;
                    sc.filePath = filePath;
                    sc.rangeStart = rangeStart;
                    sc.rangeEnd = rangeEnd;
                    sc.frame = frame;
                    sc.TakeSequenceScreenShot();
                    progress = (frame - rangeStart) / (float)(rangeEnd - rangeStart);
                }
                else if (frame > rangeEnd)
                {
                    EditorApplication.isPlaying = false;
                }
            }
            else if(EditorApplication.isPaused) {
            }
            else
            {
                this.isStartRender = false;
                progress = 0f;
            }
            
        }
        
        private int EditorGUILayout_MaskField(int layermask,string labname){
            int ss = InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layermask);
            EditorGUILayout.BeginHorizontal();
            ss = EditorGUILayout.MaskField(labname,ss,InternalEditorUtility.layers);
            if (GUILayout.Button("⊙", GUILayout.Width(20),GUILayout.ExpandWidth(false)))
            {                        
                // Debug.Log(InternalEditorUtility.GetAssetsFolder());
                ss = InternalEditorUtility.LayerMaskToConcatenatedLayersMask(cam.cullingMask);
            }
            EditorGUILayout.EndHorizontal();
            return InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(ss).value;
        }

        void TakeScreenShot(bool a,bool b = false)
        {
            int ss = 1;
            if(a){
                ss = 0;
            }
            Material material = new Material(Shader.Find("Dite/IScreenRecordingToolWindow/ImageExportEditorShader"));
            int resWidthN = (int)resolution.x;
            int resHeightN = (int)resolution.y;
            RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
            RenderTexture rt2 = new RenderTexture(resWidthN, resHeightN, 24);
            cam.targetTexture = rt;

            TextureFormat _texFormat = TextureFormat.ARGB32;            
            Texture2D tex = new Texture2D(resWidthN, resHeightN, _texFormat, false);
            if(b){
                cam.Render();
                Graphics.Blit(rt, rt2,material,2);
            }
            else{
                cam.Render();
                Graphics.Blit(rt, rt2,material,ss);
            }
            

            RenderTexture.active = rt2;

            tex.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
            cam.targetTexture = null;
            RenderTexture.active = null; 
            byte[] bytes;
            string suffix;
            bytes = tex.EncodeToPNG();

            suffix = "_blend.png";
            if (a){
                suffix = "_add.png"; 
            }                
            if(b){
                suffix = ".png";
            }
            string _outfileName = fileName;
            
            File.WriteAllBytes(filePath + "/" + _outfileName + suffix, bytes);
            
        }
        void IsPathExists(){
            // EditorPrefs.GetString(productName + "_filePath"); EditorPrefs.GetString(productName + "_fileName")
            if (!Directory.Exists(EditorPrefs.GetString(productName + "_filePath") +"\\add")){
                Directory.CreateDirectory(EditorPrefs.GetString(productName + "_filePath") + "\\add");
            }
            if (!Directory.Exists(EditorPrefs.GetString(productName + "_filePath") +"\\blend")){
                Directory.CreateDirectory(EditorPrefs.GetString(productName + "_filePath") + "\\blend");
            }
        }

        private void runOBj(float time){
            foreach(ParticleSystem p in this.parobj){
                if (p ==null){
                    continue;
                }
                if(!p.isPlaying){
                    p.useAutoRandomSeed = false;
                }
                p.Simulate(time,false,true,true);
            }
            foreach(Animation p in this.animobj){
                if (p ==null){
                    continue;
                }
                if(!EditorApplication.isPlaying){
                    p.clip.SampleAnimation(p.gameObject, time);
                }
                else{
                    string ss = p.clip.name;
                    p.Play(ss);
                    p[ss].time = time;
                    p.Sample();
                    p.Stop();
                }
                
                
            }
            foreach(animtordata p in this.antoobj){
                if (p.animator ==null){
                    continue;
                }                
                if(!EditorApplication.isPlaying){
                    this.bakeAnimator(p);
                    p.animator.playbackTime = time;
                    p.animator.Update(0);
                }
                else{
                    AnimatorStateInfo stateInfo = p.animator.GetCurrentAnimatorStateInfo(0); 
                    p.animator.Play(stateInfo.fullPathHash, 0, time);                    
                }
                          
            }
        }
    

        void getobjforrender(List<Transform> obj){
            this.antoobj.Clear();
            this.animobj.Clear();
            this.parobj.Clear();            
            
            foreach(Transform i in obj){
                if(i!=null){
                    Animator[] a = i.GetComponentsInChildren<Animator>();                    
                    List<Animator> df = a.ToList();
                    foreach(Animator at in df){
                        animtordata s = new animtordata();
                        s.animator = at;
                        this.antoobj.Add(s);
                    }

                    Animation[] an = i.GetComponentsInChildren<Animation>();
                    List<Animation> dfa = an.ToList();
                    this.animobj.AddRange(dfa);

                    ParticleSystem[] pa = i.GetComponentsInChildren<ParticleSystem>();
                    List<ParticleSystem> pdf = pa.ToList();
                    this.parobj.AddRange(pdf);
                }
            }
            this.antoobj = this.quchongfu<animtordata>(this.antoobj);
            this.animobj = this.quchongfu<Animation>(this.animobj);
            this.parobj = this.quchongfu<ParticleSystem>(this.parobj);
        }
        private List<T> quchongfu<T>(List<T> o){
            List<T> s = new List<T>();
            foreach(T i in o){
                if(!s.Contains(i))
                {
                    s.Add(i);
                }
            }
            return s;
        }

        // 输出的是帧
        private float TimeUpdate(){
            // float delta = (float)EditorApplication.timeSinceStartup - m_PreviousTime;
            // m_PreviousTime = (float)EditorApplication.timeSinceStartup;
            // return delta;
            float delta = (float)EditorApplication.timeSinceStartup - m_PreviousTime;
            if(delta >= (1.0f / (float)this.frameRate)){
                // Debug.Log(delta);
                m_PreviousTime = (float)EditorApplication.timeSinceStartup - (delta - (1.0f / (float)this.frameRate));
                return 1.0f;
            }
            return 0.0f;
        }
        private float frame2time(float frame){
            float time = frame / (float)this.frameRate;
            return time;
        }

        private float time2frame(float time){
            float frame = time * (float)this.frameRate;
            return frame;
        }

        private void bakeAnimator(animtordata ator){
            if(ator == null){return;}
            if(ator.animator == null){return;}
            if(ator.Isbake){return;}

            int frameCount = ( int)(((this.rangeEnd - this.rangeStart) * this.frameRate) +  2); 
            ator.animator.Rebind(); 
            ator.animator.StopPlayback();
            ator.animator.recorderStartTime =  this.frame2time(this.rangeStart); 
            ator.animator.recorderStopTime =  this.frame2time(this.rangeEnd); 
            ator.animator.StartRecording(frameCount);
            for (var i =  0; i < frameCount -  1; i++)
            {
                // 记录每一帧
                ator.animator.Update( 1.0f / (float)this.frameRate); 
            }            
            // 完成记录
            ator.animator.StopRecording(); 
            ator.animator.StartPlayback();
            ator.Isbake=  true;
            // this.time = ator.animator.recorderStopTime;
        }

        private void xiahuaxian()
        {
            GUIStyle style_ss2 = new GUIStyle("AC BoldHeader"); //" ProjectBrowserBottomBarBg "PlayerSettingsPlatform" ""ProfilerRightPane"" PreToolbar "LODRendererRemove"
            style_ss2.fixedHeight = 2.5f ;  
            EditorGUILayout.LabelField(GUIContent.none, style_ss2);
            EditorGUILayout.Space(-18);            
        }
    }

}