using UnityEngine;
using System.Collections;
using System.IO;

namespace YF.Art.FXRenderStudio
{
    public class ScreenRecordingToolController : MonoBehaviour
    {

        [HideInInspector]public Camera cam;
        [HideInInspector]public bool defrender;
        [HideInInspector]public string imageFormat;
        // [HideInInspector]public bool isEnabledAlpha;
        [HideInInspector]public Vector2 resolution;
        [HideInInspector]public int frameCount;
        [HideInInspector]public string fileName;
        [HideInInspector]public string filePath;
        [HideInInspector]public int rangeStart;
        [HideInInspector]public int rangeEnd;
        [HideInInspector]public int frame;
        [HideInInspector]public int defcamclearflage;
        [HideInInspector] public Color bgcol;
        [HideInInspector] public Color renderingbgcolor;
        
        [HideInInspector]public int masklayer;
        [HideInInspector]public int oldmasklayer;


        

        void Awake()
        {
            if (cam = null)
                cam = Camera.main;
        }

        // Use this for initialization
        void Start()
        {
            bgcol = cam.backgroundColor;
            defcamclearflage = (int)cam.clearFlags;
            oldmasklayer = cam.cullingMask;
            Time.captureFramerate = frameCount;
            cam.clearFlags = CameraClearFlags.SolidColor;
            if(this.defrender){
                cam.backgroundColor =  this.renderingbgcolor;
            }else{
                cam.backgroundColor =  new Color(0,0,0,0);
            }
            
            cam.cullingMask = masklayer;
        }
        private void OnDestroy() {
            if(cam == null){
                return;
            }
            cam.clearFlags = (CameraClearFlags)defcamclearflage;
            cam.backgroundColor =  bgcol;
            cam.cullingMask = oldmasklayer;
        }
	
        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 生成序列图
        /// </summary>
        public void TakeSequenceScreenShot()
        {
            
            StartCoroutine(WaitTakeSequenceScreenShot());
        }

        IEnumerator WaitTakeSequenceScreenShot()
        {
            yield return new WaitForEndOfFrame();
            Shader.SetGlobalFloat("_ImageExporterRenderShaderADDMASK",0);
            Shader.SetGlobalFloat("_ImageExporterRenderShaderBlendMASK",1);
            TakeScreenShot(true);
            Shader.SetGlobalFloat("_ImageExporterRenderShaderADDMASK",1);
            Shader.SetGlobalFloat("_ImageExporterRenderShaderBlendMASK",0);
            TakeScreenShot(false);
            Shader.SetGlobalFloat("_ImageExporterRenderShaderADDMASK",0);
            Shader.SetGlobalFloat("_ImageExporterRenderShaderBlendMASK",0);
            
        }
        void TakeScreenShot(bool a){
            int ss = 1;
            if(a){
                ss = 0;
            }
            Material material = new Material(Shader.Find("Dite/IScreenRecordingToolWindow/ImageExportEditorShader"));
            int resWidthN = (int)resolution.x;
            int resHeightN = (int)resolution.y;

            RenderTexture rt = new RenderTexture(resWidthN, resHeightN,24);
            RenderTexture rt2 = new RenderTexture(resWidthN, resHeightN,24);
            cam.targetTexture = rt;

            TextureFormat _texFormat = TextureFormat.ARGB32;

            Texture2D tex = new Texture2D(resWidthN, resHeightN, _texFormat, false);
            
            if(this.defrender){
                cam.Render();
                Graphics.Blit(rt, rt2,material,2);
            }
            else
            {
                cam.Render();
                Graphics.Blit(rt, rt2,material,ss);
            }
            

            RenderTexture.active = rt2;

            tex.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
            tex.Apply();

            //清空rendertexture
            cam.targetTexture = null;
            RenderTexture.active = null; 

            string fp;
            string suffix;
            byte[] bytes;
            bytes = tex.EncodeToPNG();
            // int frame = Time.frameCount - 2;
            if(this.defrender){
                fp = "/";
                suffix = "_" + this.frame.ToString("00") +".png";
            }
            else{
                fp = "/blend/";
                suffix = "_blend_"+ this.frame.ToString("00") +".png";
                if (a){
                    fp = "/add/";
                    suffix = "_add_"+ this.frame.ToString("00") +".png";
                }  
            }
           
            File.WriteAllBytes(filePath + fp + fileName + suffix, bytes);
        }
    }
}