// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using DT.FrameWork;

// namespace YF.Art
// {
//     using DTU = DTUtilsEditor;
//     [CustomEditor(typeof(YFJsonAnimController))]
//     [CanEditMultipleObjects]
//     public class YF_AnimControllerInspector : Editor
//     {
//         const string ClassFileName = "YFJsonAnimController.cs";
//         SerializedProperty m_json;
//         SerializedProperty m_texture;
//         SerializedProperty m_sortingOrder;
//         SerializedProperty m_sortingLayerID;

//         protected MonoScript selfFile;

//         private void OnEnable()
//         {
//             m_json = serializedObject.FindProperty("json");
//             m_texture = serializedObject.FindProperty("texture");
//             m_sortingOrder = serializedObject.FindProperty("sortingOrder");
//             m_sortingLayerID = serializedObject.FindProperty("sortingLayerID");
//             // m_GameObjectProp = serializedObject.FindProperty("m_MyGameObject");
//         }
//         private void OnInspectorUpdate()
//         {
//             this.Repaint();
//         }

//         private void OnSelectionChange()
//         {
//             EditorUtility.SetDirty(target);
//         }

//         static bool addsetting = true;
//         public override void OnInspectorGUI()
//         {
//             ClassFileName.FindSelfFile(ref selfFile);
//             YFJsonAnimController tag = target as YFJsonAnimController;
//             if (tag == null) return;
//             GUI.enabled = false;
//             EditorGUILayout.ObjectField("Script", selfFile, typeof(MonoScript), false);
//             GUI.enabled = true;
//             serializedObject.Update();            
//             int layerValue = m_sortingLayerID.intValue;
//             EditorGUI.BeginChangeCheck();
//             {
//                 EditorGUILayout.BeginHorizontal();
//                 EditorGUILayout.PropertyField(m_json, new GUIContent("Json Flie"));
//                 if (GUILayout.Button("Reload", GUILayout.Width(60)))
//                 {
//                     tag.Reset();
//                 }
//                 EditorGUILayout.EndHorizontal();
//                 EditorGUILayout.PropertyField(m_texture, new GUIContent("Json Texture"));
//                 addsetting = EditorGUILayout.Foldout(addsetting, "Additional Settings");
//                 if (addsetting)
//                 {
//                     layerValue = EditorGUILayout.Popup("Sorting Layer", layerValue, DTU.GetSortingLayername());
//                     EditorGUILayout.PropertyField(m_sortingOrder, new GUIContent("Order In Value"));
//                 }
//                 if (EditorGUI.EndChangeCheck())
//                 {
//                     if (tag.texture != null)
//                     {
//                         tag.setTexture();
//                     }
//                     m_sortingLayerID.intValue = layerValue;
//                     serializedObject.ApplyModifiedProperties();
//                     tag.setRenderLayer();
//                 }
//             }
//             if(GUILayout.Button("测试")){
//                 tag._UpdateFrame();
                
//             }
//             EditorGUILayout.Space(20);
//             base.OnInspectorGUI();
//         }
//     }
// }