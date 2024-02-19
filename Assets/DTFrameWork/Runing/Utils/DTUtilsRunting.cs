using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DT.FrameWork
{

    //texture相关
    public static partial class DTUtilsRunting
    { 
        /// <summary>
        /// 获取图片的宽高
        /// </summary> 
        public static Vector2Int getTextureSize(this Texture tex)
        {
            return new Vector2Int(tex.width, tex.height);
        }
    }
    //数学相关
    public static partial class DTUtilsRunting
    {
        /// <summary>
        /// 判断数字的符号，如果是0返回0
        /// </summary> 
        public static float DTSign(this int value)
        {
            if (float.Parse(value.ToString("F5")) == 0.0f) return 0.0f;
            return Mathf.Sign(value);
        }
        /// <summary>
        /// 判断数字的符号，如果是0返回0
        /// </summary> 
        public static float DTSign(this float value)
        {
            if (float.Parse(value.ToString("F5")) == 0.0f) return 0.0f;
            return Mathf.Sign(value);
        }
    }

    //其他相关
    public static partial class DTUtilsRunting
    {
        static string MyName = "DTS";
        public static string logSplit = " :: ";
        private static string comberstr(params object[] arg)
        {
            string printStr = string.Empty;
            foreach (var item in arg)
            {
                if (item is float)
                {
                    printStr += ((float)item).ToString("F4");
                }
                else if (item is double)
                {
                    printStr += ((double)item).ToString("F4");
                }
                else
                {
                    printStr += item.ToString();
                }
                printStr += logSplit;
            }
            return printStr.Substring(0, printStr.Length - logSplit.Length);
        } 
        public static void DTLog(this string fristView, params object[] arg)
        {
            string p = comberstr(arg);//String.Join(" :: ", arg);
            if (string.IsNullOrEmpty(p)) return;
            if (string.IsNullOrEmpty(fristView))
            {
                fristView = MyName;
            }
            // Debug.Log($"{fristView} Log: >> {p} >> [Dite Log]");
        }
        public static void DTLogWarning(this string fristView, params object[] arg)
        {
            string p = comberstr(arg);//String.Join(" :: ", arg);
            if (string.IsNullOrEmpty(p)) return;
            if (string.IsNullOrEmpty(fristView))
            {
                fristView = MyName;
            }
            Debug.LogWarning($"{fristView} Warning: >> {p} >> [Dite Warning]");
        }
        public static void DTLogError(this string fristView, params object[] arg)
        {
            string p = comberstr(arg);//String.Join(" :: ", arg);
            if (string.IsNullOrEmpty(p)) return;
            if (string.IsNullOrEmpty(fristView))
            {
                fristView = MyName;
            }
            Debug.LogError($"{fristView} Error: >> {p} >> [Dite Error]");
        }

        public static string[] GetSortingLayername()
        {
            string[] layerNames = new string[SortingLayer.layers.Length];
            for (int i = 0; i < layerNames.Length; i++)
            {
                layerNames[i] = SortingLayer.layers[i].name;
            }
            return layerNames;
        }

    }
}
