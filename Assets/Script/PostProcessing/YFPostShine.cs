using UnityEngine;

namespace YF.Art.PostProcessing
{
    public partial class YFPostProcessing
    {
        [Range(0f, 1.0f)] public float DayToNight = 0.5f;
        public Gradient SunColor;
        public RenderTexture NightLightMaskRT; 
        public int RenderTextureScale = 50;

        Material m_material;
        Material material
        {
            get
            {
                if (m_material == null)
                {
                    m_material = new Material(Shader.Find("Hidden/YoFi/ShineWarsShader"));
                }
                return m_material;
            }
        }

        int MaskID = 0;
        int SunColorID = 0;

        void StartShine()
        {
            this.MaskID = Shader.PropertyToID("_MaskTex");
            this.SunColorID = Shader.PropertyToID("_SunColor");

            this.InitRenderTexture();
        }

        private void InitRenderTexture() {
            this.NightLightMaskRT = new RenderTexture(Screen.width * RenderTextureScale / 100, Screen.height * RenderTextureScale / 100, 0, RenderTextureFormat.ARGB32);
            this.NightLightMaskRT.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;
            this.NightLightMaskRT.name = "NightLightMaskRT";

            if (transform.childCount <= 0) {
                UnityEngine.Debug.LogWarning("InitRenderTexture Fail childCount is 0");
                return;
            }

            var camera = transform.GetChild(0).GetComponent<Camera>();
            if (camera == null) {
                UnityEngine.Debug.LogWarning("InitRenderTexture Fail camera is nil");
                return;
            }

            camera.targetTexture = NightLightMaskRT;
        }

        private void OnRenderImageShine(RenderTexture src, RenderTexture dest)
        {
            if (this.NightLightMaskRT != null) {
                this.material.SetTexture(this.MaskID, this.NightLightMaskRT);
                this.material.SetColor(this.SunColorID, SunColor.Evaluate(DayToNight));
                Graphics.Blit(src, dest, this.material);
            } else {
                Graphics.Blit(src, dest);
            }
        }
    }
}