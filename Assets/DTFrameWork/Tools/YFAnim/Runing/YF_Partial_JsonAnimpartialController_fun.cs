// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using DT.FrameWork;
// namespace YF.Art
// {
//     #region  protected
//     public partial class YFJsonAnimController
//     {
//         public virtual void setTexture()
//         {
//             if (this.texture != null)
//             {
//                 this.mpb.Clear();
//                 meshrenderer.GetPropertyBlock(mpb);
//                 mpb.SetTexture("_MainTex", this.texture);
//                 meshrenderer.SetPropertyBlock(mpb);
//             }
//         }
//         public virtual void Reset()
//         {
//             isinit = false;
//         }
//         protected virtual void init()
//         {
//             if (isinit) return;
//             this.checkMaterial();
//             // this.setTexture();
//             isinit = true;
//         }
//     }
//     #endregion
//     #region  private
//     public partial class YFJsonAnimController
//     {
        


//         private void checkMaterial()
//         {
//             Shader s = Shader.Find(shaderfile);
//             if (meshrenderer.sharedMaterial == null)
//             {
//                 meshrenderer.sharedMaterial = this.BaseMaterial;
//             }
//             else
//             {
//                 if (meshrenderer.sharedMaterial.shader != s)
//                 {
//                     meshrenderer.sharedMaterial = this.BaseMaterial;
//                 }
//             }
//         }
//     }
//     #endregion

//     #region property
//     public partial class YFJsonAnimController : MonoBehaviour
//     {
//         [HideInInspector]
//         public Material BaseMaterial;
//         public int sortingLayerID;
//         public int sortingOrder;
//         bool isinit = false;
//         const string shaderfile = "Dite/Util/YFJsonAnim";


//         MeshRenderer m_meshrenderer;
//         MeshRenderer meshrenderer
//         {
//             get
//             {
//                 if (m_meshrenderer == null)
//                 {
//                     m_meshrenderer = GetComponent<MeshRenderer>() as MeshRenderer;
//                 }
//                 return m_meshrenderer;
//             }
//         }
//         MaterialPropertyBlock m_materialPropertyBlock;
//         MaterialPropertyBlock mpb
//         {
//             get
//             {
//                 if (m_materialPropertyBlock == null)
//                 {
//                     m_materialPropertyBlock = new MaterialPropertyBlock();
//                 }
//                 return m_materialPropertyBlock;
//             }
//             set
//             {
//                 m_materialPropertyBlock = value;
//             }
//         }
//     }
//     #endregion
//     #region  Editor
// #if UNITY_EDITOR
//     public partial class YFJsonAnimController : MonoBehaviour
//     {
//         public int x = 0;
//         public int y = 0;
//         public int w = 0;
//         public int h = 0;
//         public Vector2 offsetData = new Vector2(0f, 0f);

//         private void OnEnable()
//         {
//             init();
//         }
//         // private void Update() {
//         //     this._UpdateFrame();
//         // }
//         private void OnValidate()
//         {
//             this._UpdateFrame(this.jsonDataToUVOffset(texture.getTextureSize(), x, y, w, h, offsetData));
//         }
//     }
// #endif
//     #endregion
// }