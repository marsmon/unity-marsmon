using UnityEngine;

namespace YF.Art.PostProcessing
{
    public enum HDRBloomMode
    {
        Auto = 0,
        On = 1,
        Off = 2,
    }

    public enum BloomScreenBlendMode
    {
        Screen = 0,
        Add = 1,
    }

    public enum BloomQuality
    {
        Cheap = 0,
        High = 1,
    }

    [System.Serializable]
    public partial class YFPostProcessing
    {
        public BloomScreenBlendMode screenBlendMode = BloomScreenBlendMode.Add;
        public HDRBloomMode hdr = HDRBloomMode.On;
        public float sepBlurSpread = 3f;
        public BloomQuality quality = BloomQuality.High;
        //Shader property
        public float bloomIntensity = 2f;
        public float bloomThreshold = 1f;
        public Color bloomThresholdColor = Color.white;
        [Range(1, 10)] public int bloomBlurIterations = 5;
        ////对应Shader property data.xy = offset, data.z = bloomIntensity,data.w = bloomThreshold
        private Vector4 data = Vector4.one;
        //Shader property
        RenderTextureFormat rtFormat;
        Vector2Int IsNew = Vector2Int.zero;
        Vector2Int rt2 = Vector2Int.one;
        Vector2Int rt4 = Vector2Int.one;
        float widthOverHeight = -1f;
        float oneOverBaseSize = 1.0f / 512.0f;

        public Shader screenBlendShader;
        protected Material screenBlend;

        [ContextMenu("Reset Bloom")]
        public virtual void ResetBloom()
        {
            IsNew = Vector2Int.zero;// new Vector2Int(-1,-1);
            rt2 = Vector2Int.one;
            rt4 = Vector2Int.one;
            widthOverHeight = 0f;
            oneOverBaseSize = 1.0f / 512.0f;
            screenBlendMode = BloomScreenBlendMode.Add;
            sepBlurSpread = 2.5f;
            quality = BloomQuality.High;
            bloomIntensity = 0.5f;
            bloomThreshold = 0.5f;
            bloomThresholdColor = Color.white;
            bloomBlurIterations = 2;
        }

        [ContextMenu("下山 Bloom")]
        public virtual void ZMSXBloom()
        {
            IsNew = Vector2Int.zero;
            rt2 = Vector2Int.one;
            rt4 = Vector2Int.one;
            widthOverHeight = 0f;
            oneOverBaseSize = 1.0f / 512.0f;

            screenBlendMode = BloomScreenBlendMode.Add;
            sepBlurSpread = 3.0f;
            quality = BloomQuality.High;
            bloomIntensity = 2.0f;
            bloomThreshold = 1.0f;//0.4F
            bloomThresholdColor = Color.white;
            bloomBlurIterations = 5;
        }


        int sp_ColorBuffer = 0; // 这是什么
        int sp_CBData = 0; // 这是什么
        int sp_CBThreshholdCol = 0; // 这是什么
        int sp_CBGlobalBool = 0; // 这是什么

        private void StartBloom()
        {
            this.sp_ColorBuffer = Shader.PropertyToID("_ColorBuffer");
            this.sp_CBData = Shader.PropertyToID("_CBData");
            this.sp_CBThreshholdCol = Shader.PropertyToID("_CBThreshholdCol");
            this.sp_CBGlobalBool = Shader.PropertyToID("_CBGlobalBool");
        }

        public bool CheckResourcesBloom() {
            if (screenBlendShader == null)
                return false;

            screenBlend = CheckShaderAndCreateMaterial(screenBlendShader, screenBlend);
        
            return true;
        }

        public void OnPrepareBloom()
        {
            if (this.isBloomRun) {
                if (!this._isBloomRun) {
                    Shader.SetGlobalFloat("_CBGlobalBool", 1f);
                    this._isBloomRun = true;
                }
            } else {
                if (this._isBloomRun) {
                    Shader.SetGlobalFloat("_CBGlobalBool", 0f);
                    this._isBloomRun = false;
                }
            }
        }

        public virtual void OnRenderImageBloom(RenderTexture source, RenderTexture destination)
        {
            this.GetDoHdr(source);
            // 更新向Shader的数据，不过_Data的XY需要实时更新，所以这里没有赋值。
            // 这样写说为了只new一次Vector4出来。而不是重复New
            this._UpdateSetShaderData_Bloom();
            screenBlend.SetTexture(this.sp_ColorBuffer, source);

            // 更新一些计算需要的数据，如果source的大小不改变则不在更新数据
            this.UpdateRenderTexSizeData(source.width, source.height);
            RenderTexture quarterRezColor = RenderTexture.GetTemporary(rt4.x, rt4.y, 0, rtFormat);
            RenderTexture secondQuarterRezColor = RenderTexture.GetTemporary(rt4.x, rt4.y, 0, rtFormat);
            // 递减缩小RT,为了减少模糊时的计算量 
            downsample_(source, quarterRezColor);
            // 获取bloom Mask
            cutColorsThresholding_(quarterRezColor, secondQuarterRezColor);
            // 模糊结果
            blurring_(quarterRezColor, secondQuarterRezColor);
            // 合并结果
            GetRenderImageResult_(secondQuarterRezColor, destination);
            RenderTexture.ReleaseTemporary(quarterRezColor);
            RenderTexture.ReleaseTemporary(secondQuarterRezColor);
        }


        public virtual void _UpdateSetShaderData_Bloom()
        {
            this.data.w = this.bloomThreshold;//Mathf.Min(this.bloomThreshold,0.9f);
            this.data.z = this.bloomIntensity; 
            screenBlend.SetVector(this.sp_CBData, this.data);
            screenBlend.SetColor(this.sp_CBThreshholdCol, this.bloomThresholdColor);
        }


        public virtual void UpdateRenderTexSizeData(int w, int h)
        {
            rtFormat = (doHdr) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.Default;
            if (IsNew.x != w || IsNew.y != h)
            {
                rt2 = new Vector2Int(w / 2, h / 2);
                rt4 = new Vector2Int(w / 4, h / 4);
                widthOverHeight = (1.0f * w) / (1.0f * h);
                oneOverBaseSize = 1.0f / 512.0f;
                IsNew = new Vector2Int(w, h);
            }
        }

        public virtual void downsample_(RenderTexture from, RenderTexture to)
        {
            // to = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
            RenderTexture halfRezColorDown = RenderTexture.GetTemporary(rt2.x, rt2.y, 0, rtFormat);
            if (quality > BloomQuality.Cheap)
            {
                Graphics.Blit(from, halfRezColorDown, screenBlend, 2);
                RenderTexture rtDown4 = RenderTexture.GetTemporary(rt4.x, rt4.y, 0, rtFormat);
                Graphics.Blit(halfRezColorDown, rtDown4, screenBlend, 2);
                Graphics.Blit(rtDown4, to, screenBlend, 6);
                RenderTexture.ReleaseTemporary(rtDown4);
            }
            else
            {
                Graphics.Blit(from, halfRezColorDown);
                Graphics.Blit(halfRezColorDown, to, screenBlend, 6);
            }
            RenderTexture.ReleaseTemporary(halfRezColorDown);
        }

        public virtual void cutColorsThresholding_(RenderTexture from, RenderTexture to)
        {
            Graphics.Blit(from, to, screenBlend, 12);
        }

        public virtual void blurring_(RenderTexture from, RenderTexture to)
        {
            // blurring
            bloomBlurIterations = Mathf.Max(1, Mathf.Min(10, bloomBlurIterations));

            for (int iter = 0; iter < bloomBlurIterations; iter++)
            {
                float spreadForPass = (1.0f + (iter * 0.25f)) * sepBlurSpread;
                RenderTexture blur4 = RenderTexture.GetTemporary(rt4.x, rt4.y, 0, rtFormat);
                this.data.x = (spreadForPass / widthOverHeight) * oneOverBaseSize;
                this.data.y = spreadForPass * oneOverBaseSize;
                // screenBlend.SetVector(this.shader_Data, this.data);
                Graphics.Blit(to, blur4, screenBlend, 13);
                Graphics.Blit(blur4, to, screenBlend, 14);
                RenderTexture.ReleaseTemporary(blur4);
                //如果说高质量的话，把每次的结果混合，混合
                if (quality > BloomQuality.Cheap)
                {
                    Graphics.Blit(to, from);
                }
            }

            if (quality > BloomQuality.Cheap)
            {
                Graphics.SetRenderTarget(to);
                GL.Clear(false, true, Color.black); // Clear to avoid RT restore
                Graphics.Blit(from, to, screenBlend, 6);
            }
        }

        public virtual void GetRenderImageResult_(RenderTexture from, RenderTexture to)
        {
            int blendPass = (int)((doHdr) ? BloomScreenBlendMode.Add : screenBlendMode);

            if (quality > BloomQuality.Cheap)
            {
                RenderTexture halfRezColorUp = RenderTexture.GetTemporary(rt2.x, rt2.y, 0, rtFormat);
                Graphics.Blit(from, halfRezColorUp);
                Graphics.Blit(halfRezColorUp, to, screenBlend, blendPass);
                RenderTexture.ReleaseTemporary(halfRezColorUp);
            }
            else
            {
                Graphics.Blit(from, to, screenBlend, blendPass);
            }
        }

        public virtual void GetDoHdr(RenderTexture source)
        {
            doHdr = false;
            if (hdr == HDRBloomMode.Auto)
                doHdr = source.format == RenderTextureFormat.ARGBHalf &&  GetComponent<Camera>().allowHDR;
            else
            {
                doHdr = hdr == HDRBloomMode.On;
            }
            
            doHdr = doHdr && supportHDRTextures;
        }
    }

}