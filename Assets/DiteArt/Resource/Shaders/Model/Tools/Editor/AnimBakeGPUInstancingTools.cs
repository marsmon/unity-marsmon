using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

using UnityEditor;
using UnityEditorInternal;
// using DiteScripts.YF.Editor;
using UnityEditor.Animations;

namespace DiteScripts.YF.AnimMap.Editor
{
    partial class AnimBakeGPUInstancingTools : EditorWindow
    {
        [MenuItem("Dite Tools//Anim Map Bake Tools")]
        static void ShowWindow()
        {
            var window = GetWindow<AnimBakeGPUInstancingTools>();
            // window.Close();
            window.titleContent = new GUIContent("Anim Map Bake [Bones]");
            window.Show();
        }
        void OnGUI()
        {
            string helpbox = "操作说明：\n";
            helpbox = helpbox + "1.建好一个对象,包含需要导出的动画状态机和对应的模型。必须能正常播放的。\n";
            helpbox = helpbox + "2.先点击 Load Data,等待界面刷新 \n";
            helpbox = helpbox + "3.重写填写Export Path 和Export Name。必须是存在的目录，不存在会返回默认地址，名字会覆盖已经存在的（不改变GUID的覆盖）\n";
            helpbox = helpbox + "4.点击Export All。如果单独导出模型或贴图，可以点另外2个按钮  \n";
            helpbox = helpbox + "注意：\n";
            helpbox = helpbox + "动画一定要是1秒的倍数。可以是0.5秒也可以2秒。否在无法循环。\n";
            helpbox = helpbox + "都已经集群动画了，尽量减少动画的长度和符合动画的规则，最后播放时，所有动画都会被约束到1秒的倍数里。\n";
            EditorGUILayout.HelpBox(helpbox,MessageType.Info);
            EditorGUIUtility.labelWidth = 100;            
            Color defc = GUI.backgroundColor;
            activego = (GameObject)EditorGUILayout.ObjectField("Select Prefabs:", activego, typeof(GameObject), true);
            if (activego == null)
            {
                EditorGUILayout.HelpBox("请选择对象", MessageType.Info);
                return;
            }
            FrameRate = EditorGUILayout.IntField("Frame Rate:", FrameRate);
            EditorGUI.BeginChangeCheck();
            __exportpath = EditorGUILayout.TextField("Export Path:", __exportpath);
            __exportname = EditorGUILayout.TextField("Export name:", __exportname);
            if (EditorGUI.EndChangeCheck())
            {
                if (!Directory.Exists(Application.dataPath.Replace("Assets", "") + __exportpath))
                {
                    __exportpath = "";
                }
                if (string.IsNullOrEmpty(__exportpath)) __exportpath = "Assets";
                if (string.IsNullOrEmpty(__exportname)) __exportname = activego.name;
            }

            // MapExpand = EditorGUILayout.FloatField("Map Expand:", MapExpand);
            if (GUILayout.Button("Load Data"))
            {
                data.Ismax = true;
                data.LoadData(activego, this.FrameRate);
                if (string.IsNullOrEmpty(__exportname)) __exportname = activego.name;
                if (string.IsNullOrEmpty(__exportpath)) __exportpath = "Assets";//"Assets";
                isviewdata = true;
            }


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Mesh"))
            {
                data.CteatMesh();
                CreateAnimMapMesh();
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("Export AnimMap"))
            {
                data.CreatMap();
                CreateAnimMapTexture();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Export All"))
            {
                data.CteatMesh();
                data.CreatMap();
                CreateAnimMapMesh();
                CreateAnimMapTexture();
                AssetDatabase.Refresh();
            }
            if (activego == null)
            {
                isviewdata = false;
            }

            EditorGUILayout.Space(2);
            GUILayout.Button("", GUILayout.Height(2));

            if (isviewdata)
            {
                data.viewGUI();
            }
            GUILayout.Button("", GUILayout.Height(2));
            EditorGUILayout.Space(2);
            
            GUI.backgroundColor = defc;
        }


        void CreateAnimMapMesh()
        {
            string meshpath = __exportpath + "/" + __exportname + "_model.mesh";
            Mesh m = AssetDatabase.LoadAssetAtPath(meshpath, typeof(Mesh)) as Mesh;
            if (m == null)
            {
                AssetDatabase.CreateAsset(data.exportMesh, meshpath);
                EditorUtility.SetDirty(data.exportMesh);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(data.exportMesh));
            }
            else
            {
                m.Clear();
                m.vertices = data.exportMesh.vertices;
                m.normals = data.exportMesh.normals;
                m.triangles = data.exportMesh.triangles;
                m.uv = data.exportMesh.uv;
                List<Vector4> v = new List<Vector4>();
                if (data.exportMesh.HasVertexAttribute(VertexAttribute.TexCoord1))
                {
                    v.Clear();
                    data.exportMesh.GetUVs(1, v);
                    m.SetUVs(1, v);// = data.exportMesh.uv2;
                }
                if (data.exportMesh.HasVertexAttribute(VertexAttribute.TexCoord2))
                {
                    v.Clear();
                    data.exportMesh.GetUVs(2, v);
                    m.SetUVs(2, v);// = data.exportMesh.uv2;
                }
                if (data.exportMesh.HasVertexAttribute(VertexAttribute.Color))
                {
                    m.colors = data.exportMesh.colors;
                }
                m.RecalculateNormals();
                m.RecalculateBounds();
                // m.MarkModified();
                EditorUtility.SetDirty(m);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(m));
            }
        }

        void CreateAnimMapTexture()
        {
            string texpath = __exportpath + "/" + __exportname + "_Tex.asset";
            Texture2D m = AssetDatabase.LoadAssetAtPath(texpath, typeof(Texture2D)) as Texture2D;
            if (m == null)
            {
                AssetDatabase.CreateAsset(data.ExportTexture, texpath);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(data.ExportTexture));
            }
            else
            {
            #if UNITY_2019_1_OR_NEWER
                m = new Texture2D(data.ExportTexture.width, data.ExportTexture.height,m.format,false);
            #else
                m.Reinitialize(data.ExportTexture.width, data.ExportTexture.height);
            #endif
                m.SetPixels(data.ExportTexture.GetPixels());
                m.Apply();
                EditorUtility.SetDirty(m);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(m));
            }
            AssetDatabase.Refresh();
        }
    }




    /// <summary>
    /// Main Data
    /// </summary>
    partial class AnimBakeGPUInstancingTools
    {
        GameObject activego;
        int FrameRate = 30;
        AnimMapData data = new AnimMapData();
        bool isviewdata = false;
        static string __exportpath = "";
        static string __exportname = "";



    }

}