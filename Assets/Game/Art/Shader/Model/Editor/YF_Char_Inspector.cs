using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace YF.Art
{
    public class YF_Char_BaseGUI : ShaderGUI
    {
        protected MaterialProperty mainTex = null;
        const string editshader = "YoFi/Editor/GGS_Char_Simple_Edit";
        const string Runshader = "YoFi/GGS_Char_Simple";
        const string UIShader = "YoFi/GGS_Char_Simple_Graphic";
        bool isnew = false;
        static float angle = 0;
        static Vector3 dir = Vector3.forward;
        string shadremode = "Edit";

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            Color c = GUI.backgroundColor;
            GUI.backgroundColor = Color.red * 0.5f;
            EditorGUILayout.BeginVertical("Box");
            GUI.backgroundColor = c;
            Material material = materialEditor.target as Material;
            if (material.shader != Shader.Find(UIShader))
            {
                if (GUILayout.Button($"切換Shader [{shadremode}]"))
                {
                    isnew = (material.shader != Shader.Find(editshader));
                    if (!isnew)
                    {
                        materialEditor.SetShader(Shader.Find(Runshader));
                    }
                    else
                    {
                        materialEditor.SetShader(Shader.Find(editshader));
                    }
                    isnew = !isnew;
                }
                Help();
                if (material.shader == Shader.Find(editshader))
                {
                    shadremode = "Edit";
                    if (GUILayout.Button("合并本shader贴图"))
                    {
                        this.Init(props);
                    }
                }
                else
                {
                    shadremode = "Run";
                }
            }

            SetGlobalPorp();
            CustomBaseProp(material);
            EditorGUILayout.EndVertical();
            base.OnGUI(materialEditor, props);
        }
        void CustomBaseProp(Material material)
        {
            if (GUILayout.Button("Clear Cache Tex"))
            {
                ClearMaterial.Clear(material);
            }
        }
        void SetGlobalPorp()
        {
            EditorGUI.BeginChangeCheck();
            angle = EditorGUILayout.FloatField("场景偏移角度:", angle);
            dir = EditorGUILayout.Vector3Field("自定义灯光方向", dir);
            if (EditorGUI.EndChangeCheck())
            {
                Shader.SetGlobalVector("_CustomLightDir", new Vector4(dir.x, dir.y, dir.z, angle));
            }
            if (GUILayout.Button("设置全局属性[调试用]"))
            {
                Light l = Transform.FindObjectOfType<Light>();
                dir = l.transform.forward;
                Shader.SetGlobalVector("_CustomLightDir", new Vector4(dir.x, dir.y, dir.z, angle));
            }
        }
        void Help()
        {
            string tips = "Tips：\n 1.  绝对不能使用PSD 格式作为贴图.\n";
            tips += " 2.  [设置全局属性[调试用]按钮的作用为：获取场景中唯一灯光的方向并赋值。(如果有多个灯光，会随机获取请自重).\n";
            tips += " 3.  编辑模式下，点击合并贴图，会在主贴图目录下生成后缀为[#####_combine.png]新的图片,拖放进运行模式的图槽内即可观看结果.\n";
            tips += " 4.  提交:运行时的贴图，主色图一张，暗部图一张，bloom图一张(bloom图2个模式共用，未改名).";
            EditorGUILayout.HelpBox(tips, MessageType.Info);
        }

        public void Init(MaterialProperty[] props)
        {
            List<DTTMateData> data = new List<DTTMateData>();
            data.Add(new DTTMateData("_ILMTex", GetTex("_ILMTex", props)));
            data.Add(new DTTMateData("_DetailTex", GetTex("_DetailTex", props)));
            data.Add(new DTTMateData("_EmissionTex", GetTex("_EmissionTex", props)));
            Shader shader = Shader.Find("Hidden/DT/YFCharBaseCombineTexData");

            Texture2D MainTex = GetTex("_BaseTex", props);
            var outputPath = MainTex.GetTexPath("combine");
            
            if (MainTex != null)
                MainTex.TexturWork(shader, data).SavePng(Path.Combine(Path.GetDirectoryName(outputPath), Path.GetFileName(outputPath).ToLower()));
            else
                Debug.Log("MainTex 未找到! ");
            Texture2D DarkTex = GetTex("_SssTex", props);
            var darkOutputPath = DarkTex.GetTexPath("combine");
            if (DarkTex != null)
                DarkTex.TexturWork(shader, data).SavePng(Path.Combine(Path.GetDirectoryName(darkOutputPath), Path.GetFileName(darkOutputPath).ToLower()));
            else
                Debug.Log("DarkTex 未找到! ");
        }


        Texture2D GetTex(string item, MaterialProperty[] props)
        {
            MaterialProperty texTmp = FindProperty(item, props, false);
            if (texTmp != null)
            {
                return (Texture2D)(texTmp.textureValue);
            }
            else
            {
                return null;
            }
        }

        bool iscanrun;

        string getPath(string item, MaterialProperty[] props)
        {
            MaterialProperty texTmp = FindProperty(item, props, false);
            var tex = texTmp.textureValue;

            string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(tex).Replace("/", "\\"));
            // Debug.Log(path);
            if (path.ToLower().EndsWith(".psd"))
            {
                Debug.LogError("不能使用Psd" + AssetDatabase.GetAssetPath(tex));
                iscanrun = false;
                return "None";
            }
            if (tex == null)
            {
                path = "None";
            }
            return path;
        }
    }

}