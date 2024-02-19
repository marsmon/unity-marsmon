using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DT.FrameWork
{
    public static class DTUtilsEditor
    {
        public static string MoveAddFolderFile(this string oldPath, string newFolder,string newname)
        {
            if (!File.Exists(oldPath))
                return string.Empty;
            string tmp = Path.GetDirectoryName(oldPath);
            string newPath = Path.Combine(tmp,newFolder);
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);
            string ex = Path.GetExtension(oldPath);
            string fileName = Path.GetFileName(oldPath).Replace(ex,newname+ex);
            string destinationPath = Path.Combine(newPath, fileName);
            File.Move(oldPath, destinationPath);
            return destinationPath;
        }
        public static void MoveFile(this string oldPath, string newPath)
        {
            if (!File.Exists(oldPath))
                return;
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);

            string fileName = Path.GetFileName(oldPath);
            string destinationPath = Path.Combine(newPath, fileName);
            File.Move(oldPath, destinationPath);
        }
        public static void FindSelfFile(this string filename, ref MonoScript self)
        {
            if (self != null)
            {
                if (self.name == filename.Split('.')[0])
                    return;
            }
            self = null;
            string[] collection = AssetDatabase.FindAssets(filename.Split('.')[0] + " t:Script");
            foreach (var item in collection)
            {
                string t = AssetDatabase.GUIDToAssetPath(item);
                if (t.EndsWith(filename))
                {
                    self = AssetDatabase.LoadAssetAtPath<MonoScript>(t);
                    return;
                }
            }
            Debug.Log(1111);
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
