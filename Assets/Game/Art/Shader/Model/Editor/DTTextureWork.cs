using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
namespace YF.Art
{ 
    public struct DTTMateData
    {
        public string key;
        public MaterialProperty.PropType type;
        public float floatValue;
        public Vector4 vectorValue;
        public Texture2D textureValue;
        public Color colorValue;

        public DTTMateData(string key, float value)
        {
            this.key = key;
            this.type = MaterialProperty.PropType.Float;
            this.floatValue = value;
            this.vectorValue = default(Vector4);
            this.textureValue = null;
            this.colorValue = default(Color);
        }
        public DTTMateData(string key, bool value)
        {
            this.key = key;
            this.type = MaterialProperty.PropType.Float;
            this.floatValue = value ? 1f : 0f;
            this.vectorValue = default(Vector4);
            this.textureValue = null;
            this.colorValue = default(Color);
        }
        public DTTMateData(string key, Vector4 value)
        {
            this.key = key;
            this.type = MaterialProperty.PropType.Vector;
            this.floatValue = default(float);
            this.vectorValue = value;
            this.textureValue = null;
            this.colorValue = default(Color);
        }
        public DTTMateData(string key, Texture2D value)
        {
            this.key = key;
            this.type = MaterialProperty.PropType.Texture;
            this.floatValue = default(float);
            this.vectorValue = default(Vector4);
            this.textureValue = value;
            this.colorValue = default(Color);
        }
        public DTTMateData(string key, Color value)
        {
            this.key = key;
            this.type = MaterialProperty.PropType.Color;
            this.floatValue = default(float);
            this.vectorValue = default(Vector4);
            this.textureValue = null;
            this.colorValue = value;
        }
        public void setValue(Material mate)
        {
            switch (this.type)
            {
                case MaterialProperty.PropType.Float:
                    mate.SetFloat(this.key, this.floatValue);
                    break;
                case MaterialProperty.PropType.Vector:
                    mate.SetVector(this.key, this.vectorValue);
                    break;
                case MaterialProperty.PropType.Texture:
                    if(this.textureValue)
                        mate.SetTexture(this.key, this.textureValue);
                    break;
                case MaterialProperty.PropType.Color:
                    mate.SetColor(this.key, this.colorValue);
                    break;
            }
        }
    }
    public static class DTTextureWork
    {


        public static void dtprint<T>(T obj)
        {
            Debug.Log($"{nameof(obj)}   ==>  {obj}");
        }

        // MaterialProperty.PropType
        /// <summary>
        /// 处理图片
        /// </summary>
        /// <param name="sourceTexture"></param>
        /// <param name="shader"></param>
        /// <param name="otherTex"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static Texture2D TexturWork(this Texture sourceTexture, Shader shader, List<DTTMateData> otherTex, int pass = 0)
        {
            if (shader == null)
            {
                Debug.LogError($"未找到{shader}.");
                return null;
            }
            Material mate = new Material(shader);
            foreach (var item in otherTex)
            {
                item.setValue(mate);
            }

            int scaledWidth = sourceTexture.width;
            int scaledHeight = sourceTexture.height;

            Texture2D tex = new Texture2D(scaledWidth, scaledHeight);
            // 将原始纹理的像素数据拷贝到缩放后的纹理
            RenderTexture renderTexture = RenderTexture.GetTemporary(scaledWidth, scaledHeight);
            RenderTexture.active = renderTexture;

            Graphics.Blit(sourceTexture, renderTexture, mate, pass);
            // 从RenderTexture中读取像素数据并应用到缩放后的纹理
            tex.ReadPixels(new Rect(0, 0, scaledWidth, scaledHeight), 0, 0);
            tex.Apply();
            // 释放临时的RenderTexture
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
            return tex;
        }

        /// <summary>
        /// 把Texture2D 输出成PNG
        /// </summary>
        /// <param name="srctex">输出的Texture2D</param>
        /// <param name="ExportPath">输出的绝对路径，包含文件名和扩展名</param>
        public static void SavePng(this Texture2D srctex, string ExportPath)
        {
            byte[] bytes;
            try
            {
                bytes = srctex.EncodeToPNG();
            }
            finally
            {
                Debug.LogWarning($"{srctex} 不可读写,自动转制成 内存里的Texture2D!!");
                bytes = LoadTex(srctex).EncodeToPNG();
            }
            File.WriteAllBytes(ExportPath, bytes);
            AssetDatabase.Refresh();
        }

        public static string GetTexPath(this Texture srctex,string kuozhan = "")
        {
            var path = AssetDatabase.GetAssetPath(srctex);
            path = System.IO.Path.GetFullPath(path);
            var ex = System.IO.Path.GetExtension(path);
            if(kuozhan != ""){
                path = path.Replace(ex, $"_{kuozhan}.png");
            }else{
                path = path.Replace(ex, ".png");
            }
            return path;
        }

        public static Texture2D LoadTex(Texture sourceTexture)
        {
            int scaledWidth = sourceTexture.width;
            int scaledHeight = sourceTexture.height;
            // 创建缩放后的纹理
            Texture2D tex = new Texture2D(scaledWidth, scaledHeight);
            // 将原始纹理的像素数据拷贝到缩放后的纹理
            RenderTexture renderTexture = RenderTexture.GetTemporary(scaledWidth, scaledHeight);
            RenderTexture.active = renderTexture;
            Graphics.Blit(sourceTexture, renderTexture);
            // 从RenderTexture中读取像素数据并应用到缩放后的纹理
            tex.ReadPixels(new Rect(0, 0, scaledWidth, scaledHeight), 0, 0);
            tex.Apply();
            // 释放临时的RenderTexture
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(renderTexture);
            return tex;
        }
        
    }
}

