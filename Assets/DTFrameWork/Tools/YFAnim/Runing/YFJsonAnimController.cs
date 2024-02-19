// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using DT.FrameWork;
// namespace YF.Art
// {  
//     [RequireComponent(typeof(MeshFilter))]
//     [RequireComponent(typeof(MeshRenderer))]
// #if UNITY_EDITOR
//     [ExecuteInEditMode]
// #endif    
//     public partial class YFJsonAnimController : MonoBehaviour
//     {


//         public TextAsset json;
//         public Texture2D texture;
//         [SerializeField] Vector4 FrameRect = new Vector4(0f, 0f, 0f, 0f);
//         [SerializeField] Vector4 FrameData = new Vector4(0f, 0f, 0f, 0f); 

//         bool ischange = false;
//         Matrix4x4 m_FrameMatrix;
         
//         private void Awake()
//         {
//             this.init();
//         }
//         private void LateUpdate()
//         {
//             // this._UpdateFrame();
//         }
        
//         private Matrix4x4 jsonDataToUVOffset(Vector2 texsize, int x, int y, int w, int h, Vector2 off)
//         {
//             Vector4 FrameData;
//             FrameData.x = off.x ;
//             FrameData.y = (h - Mathf.Abs(off.y)) * off.y.DTSign(); 
//             FrameData.z = w;
//             FrameData.w = h;
//             FrameData *= 0.01f;
//             Vector4 FrameRect;
//             FrameRect.x = x / texsize.x;
//             FrameRect.y = (texsize.y - h - y) / texsize.y; 
//             FrameRect.z = w / texsize.x ;
//             FrameRect.w = h / texsize.y ; 
//             Matrix4x4 FrameMatrix = Matrix4x4.identity;
//             FrameMatrix.SetRow(0,new Vector4(FrameRect.z,0,FrameRect.x,1));
//             FrameMatrix.SetRow(1,new Vector4(0,FrameRect.w,FrameRect.y,1));

//             FrameMatrix.SetRow(2,new Vector4(FrameData.z,0,FrameData.x,1));
//             FrameMatrix.SetRow(3,new Vector4(0,FrameData.w,FrameData.y,1)); 
//             return FrameMatrix;
//         }

//         public virtual void _UpdateFrame()
//         {
//             m_FrameMatrix = this.jsonDataToUVOffset(texture.getTextureSize(), x, y, w, h, offsetData);        
//             meshrenderer.GetPropertyBlock(mpb); 
//             mpb.SetMatrix("DT_FrameRect",m_FrameMatrix); 
//             meshrenderer.SetPropertyBlock(mpb);
//         }
//         public virtual void _UpdateFrame(Matrix4x4 matx){
//             if(matx != m_FrameMatrix){
//                 m_FrameMatrix = matx;
//                 meshrenderer.GetPropertyBlock(mpb);
//                 mpb.SetMatrix("DT_FrameRect",m_FrameMatrix);
//                 meshrenderer.SetPropertyBlock(mpb);
//                 "Matrix4x4 change".DTLog("jjfdslkjfs");
//             }
//         }


//         public void setRenderLayer()
//         {
//             meshrenderer.sortingLayerName = SortingLayer.layers[this.sortingLayerID].name;// this.sortingLayerName; 
//             meshrenderer.sortingLayerID = SortingLayer.layers[this.sortingLayerID].id;// this.sortingLayer;
//             meshrenderer.sortingOrder = this.sortingOrder;
//         }


//     }

// }