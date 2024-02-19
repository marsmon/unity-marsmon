using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiteScripts.YF.Tools.Editor
{
    class ViewDependencies : EditorWindow
    {
        // [MenuItem("Dites Tools/View Dependencies Tools")]
        [MenuItem("Dite Tools/Other/查看依赖关系工具")]
        static void ShowWindow()
        {
            Rect rect = new Rect(new Vector2(0, 0), new Vector2(300, 500));
            var window = GetWindow<ViewDependencies>();
            window.titleContent = new GUIContent("依赖关系查询");
            window.position = rect;
            window.Show();
        }
        bool isRun = false;
        List<UnityEngine.Object> selectgo = new List<UnityEngine.Object>();
        Dictionary<UnityEngine.Object, List<UnityEngine.Object>> data = new Dictionary<UnityEngine.Object, List<UnityEngine.Object>>();
        Vector2[] pos0;
        List<Material> m = new List<Material>();
        List<UnityEngine.Object> uo = new List<UnityEngine.Object>();
        Vector2 pos = Vector2.zero;
        void OnGUI()
        {
            if (GUILayout.Button("我依赖谁"))
            {
                selectgo.Clear();
                data.Clear();
                UnityEngine.Object[] gg = Selection.objects;
                selectgo = new List<UnityEngine.Object>(gg);
                m = GetAllmat();

                for (int i = 0; i < selectgo.Count; i++)
                {
                    getother(selectgo[i]);
                }
            }
            if (GUILayout.Button("那些材质球依赖了本贴图 [贴图快捷通道]"))
            {
                selectgo.Clear();
                data.Clear();
                UnityEngine.Object[] gg = Selection.objects;
                selectgo = new List<UnityEngine.Object>(gg);

                m = GetAllmat();
                for (int i = 0; i < selectgo.Count; i++)
                {
                    getTEX2Mat(selectgo[i]);

                }
            }
            if (GUILayout.Button("谁依赖我[会卡顿,不要在意细节]"))
            {
                string content = string.Format("确认要执行吗？\n 需要遍历整个工程，\n速度较慢，有0.1%卡死可能。\n不在意的点确认。");
                if (EditorUtility.DisplayDialog("二次确认", content, "确定", "取消"))
                {
                    selectgo.Clear();
                    data.Clear();
                    UnityEngine.Object[] gg = Selection.objects;
                    selectgo = new List<UnityEngine.Object>(gg);
                    // m = GetAllmat();
                    uo = GetAllPrefabs();
                    for (int i = 0; i < selectgo.Count; i++)
                    {
                        getguanlian(selectgo[i]);
                    }
                }
            }

            EditorGUIUtility.labelWidth = 30;
            EditorGUILayout.HelpBox("选择要查看依赖关系的对象后，根据自己的需要点击上面的按钮,\n [谁依赖我] 功能会因为工程内容过多而时间较长。", MessageType.Info);

            if (data.Count > 0)
            {
                pos = EditorGUILayout.BeginScrollView(pos, GUILayout.Height(350));
                pos0 = new Vector2[data.Count];
                EditorGUILayout.BeginVertical();

                foreach (var d in data)
                {
                    EditorGUILayout.ObjectField(d.Key, typeof(UnityEngine.Object), false);
                    foreach (var mat in d.Value)
                    {
                        EditorGUILayout.ObjectField("  ", mat, typeof(UnityEngine.Object), false);
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
        }

        bool getother(UnityEngine.Object s)
        {
            UnityEngine.Object[] a = GetDependencies(s);
            if (data.ContainsKey(s))
            {
                data[s] = new List<UnityEngine.Object>(a);
            }
            else
            {
                data.Add(s, new List<UnityEngine.Object>(a));
            }
            return false;
        }

        bool getguanlian(UnityEngine.Object g)
        {
            if (g != null)
            {
                List<UnityEngine.Object> mmm = new List<UnityEngine.Object>();
                foreach (var mm in uo)
                {
                    if (mm == null)
                    {
                        continue;
                    }
                    if (mm.GetType() == typeof(DefaultAsset))
                    {
                        continue;
                    }
                    if (GetTextur2dToMaterialGUID(mm, g))
                    {
                        mmm.Add(mm);
                    }
                }
                if (!data.ContainsKey(g))
                {
                    data.Add(g, mmm);
                }
                else
                {
                    data[g] = mmm;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        bool getTEX2Mat(UnityEngine.Object s)
        {
            var g = s as Texture2D;
            if (g != null)
            {
                List<UnityEngine.Object> mmm = new List<UnityEngine.Object>();
                foreach (var mm in m)
                {
                    if (mm.GetType() == typeof(DefaultAsset))
                    {
                        continue;
                    }
                    if (GetTextur2dToMaterialGUID(mm, g))
                    {
                        mmm.Add(mm);
                    }
                }
                if (!data.ContainsKey(g))
                {
                    data.Add(g, mmm);
                }
                else
                {
                    data[g] = mmm;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        List<UnityEngine.Object> GetAllPrefabs()
        {
            List<UnityEngine.Object> a0 = new List<UnityEngine.Object>();
            string[] a = AssetDatabase.FindAssets("t: Object");
            foreach (var s in a)
            {
                a0.Add(((UnityEngine.Object)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(UnityEngine.Object))));
            }
            return a0;
        }

        List<Material> GetAllmat()
        {
            List<Material> a0 = new List<Material>();
            string[] a = AssetDatabase.FindAssets("t: Material");
            foreach (var s in a)
            {
                a0.Add(((Material)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(Material))));
            }
            return a0;
        }



        UnityEngine.Object[] GetDependencies(UnityEngine.Object go)
        {
            List<UnityEngine.Object> g = new List<UnityEngine.Object>();
            string[] objects = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(go));
            foreach (string s in objects)
            {
                g.Add(AssetDatabase.LoadAssetAtPath(s, typeof(UnityEngine.Object)));
            }
            return g.ToArray();
        }

        bool GetTextur2dToMaterialGUID(UnityEngine.Object go, UnityEngine.Object t)
        {
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(t));
            string[] objects = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(go));
            foreach (string s in objects)
            {
                if (guid == AssetDatabase.AssetPathToGUID(s))
                {
                    return true;
                }
            }
            return false;
        }

        bool GetTextur2dToMaterialGUID(Material go, Texture2D t)
        {
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(t));
            string[] objects = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(go));
            foreach (string s in objects)
            {
                if (guid == AssetDatabase.AssetPathToGUID(s))
                {
                    return true;
                }
            }
            return false;
        }
    }
}