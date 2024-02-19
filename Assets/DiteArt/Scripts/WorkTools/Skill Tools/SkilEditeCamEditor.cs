using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YF.Art
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class SkilEditeCamEditor : MonoBehaviour
    {
        const string armname = "skill editor child camera";
        [SerializeField] Camera m_childcamera;
        Camera childcamera
        {
            get
            {
                if (m_childcamera == null || m_childcamera.gameObject.name != armname)
                {
                    check();
                }
                m_childcamera.CopyFrom(this.gameObject.GetComponent<Camera>());
                return m_childcamera;
            }
        }



        void check()
        {
            Transform n = this.transform.Find(armname);
            if (n == null)
            {
                GameObject g = new GameObject(armname);
                g.hideFlags = HideFlags.HideAndDontSave;
                m_childcamera = g.AddComponent<Camera>();
                g.transform.SetParent(this.transform);
            }
            if (m_childcamera == null)
            {
                m_childcamera = n.gameObject.GetComponent<Camera>() as Camera;
                if (m_childcamera == null)
                    m_childcamera = n.gameObject.AddComponent<Camera>();
            }
            m_childcamera.transform.SetParent(this.transform);
        }
        private void OnEnable()
        {
            this.gameObject.hideFlags = HideFlags.DontSave;
            childcamera.hideFlags = HideFlags.None;

        }
        private void Update()
        {
            this.transform.position = new Vector3(0f, 16.9f, -17.93f);
            this.transform.rotation = Quaternion.Euler(41.3f, 0, 0);
            childcamera.hideFlags = HideFlags.None;
        }

        /// <summary>
        /// OnRenderImage is called after all rendering is complete to render image.
        /// </summary>
        /// <param name="src">The source RenderTexture.</param>
        /// <param name="dest">The destination RenderTexture.</param>
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            // childcamera.RenderWithShader(Shader.Find("Unlit/Color"), "");
            // RenderTexture mRender = new RenderTexture((int)mRect.width, (int)mRect.height, 0);
            // var currentRT = RenderTexture.active;
            // RenderTexture.active = childcamera.targetTexture;
            // childcamera.Render();
            // RenderTexture.active = currentRT;
            Graphics.Blit(src, dest);
        }
    }
}
