using UnityEngine;

namespace YF.Art.PostProcessing
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("YoFi/YF Post Processing")]
    public partial class YFPostProcessing : MonoBehaviour
    {
        public bool isBloomRun = false; // 是否开启Bloom后效
        private bool _isBloomRun = false;
        public bool isShineRun = false; // 是否开启日照后效

        private void Awake()
        {
            CheckResources();
        }

        protected void Start()
        {
            CheckResources();

            StartBloom();
            StartShine();
        }

        void OnEnable()
        {
            CheckResources();
        }

        void OnValidate()
        {
            if (CheckResources())
            {
                this._UpdateSetShaderData_Bloom();
            }
        }

        public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            this.OnPrepareBloom();

            if (!this._isBloomRun && !this.isShineRun) {
                Graphics.Blit(source, destination);
                return;
            }

            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            if (this._isBloomRun && this.isShineRun) {
                var temp = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);

                // 处理Bloom
                this.OnRenderImageBloom(source, temp);
                // 处理日夜
                this.OnRenderImageShine(temp, destination);

                RenderTexture.ReleaseTemporary(temp);
            } else {
                if (this._isBloomRun) {
                    // 处理Bloom
                    this.OnRenderImageBloom(source, destination);
                }
                if (this.isShineRun) {
                    // 处理Bloom
                    this.OnRenderImageShine(source, destination);
                }
            }

            
        }

        public bool CheckResources()
        {
            CheckSupport(false);

            isSupported &= CheckResourcesBloom();

            if (!isSupported)
                ReportAutoDisable();

            return isSupported;
        }
    }

    // PostEffectsBase
    public partial class YFPostProcessing
    {
        protected bool supportHDRTextures = true;
        protected bool isSupported = true;
        protected bool supportDX11 = false;
        private bool doHdr = false;
        
        protected void NotSupported()
        {
            enabled = false;
            isSupported = false;
            return;
        }

        protected void ReportAutoDisable()
        {
            Debug.LogWarning("The image effect " + ToString() + " has been disabled as it's not supported on the current platform.");
        }

        protected bool CheckSupport(bool needDepth)
        {
            isSupported = true;
            supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
            supportDX11 = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
            
            if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
            {
                NotSupported();
                return false;
            }

            if (needDepth)
                GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;

            return true;
        }

        protected Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
        {
            if (!s)
            {
                Debug.Log("Missing shader in " + ToString());
                enabled = false;
                return null;
            }

            if (s.isSupported && m2Create && m2Create.shader == s)
                return m2Create;

            if (!s.isSupported)
            {
                NotSupported();
                Debug.LogError("cfc The shader " + s.ToString() + " on effect " + ToString() + " is not supported on this platform!");
                return null;
            }
            else
            {
                m2Create = new Material(s);
                m2Create.hideFlags = HideFlags.DontSave;
                if (m2Create)
                    return m2Create;
                else return null;
            }
        }
    }
}
