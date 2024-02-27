
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class FBXMaker
{
    public static bool loopTransform(string parentPath, Transform prefabTransform, Transform subTransform, Dictionary<string, string> boneNameDict, int deep) {
        for (int i = 0; i < subTransform.childCount; i++)
        {
            Transform subChildTransform = subTransform.GetChild(i);

            string transPath = parentPath + "/" + subChildTransform.name;

            if (!subChildTransform.name.StartsWith("Bip")) {
                // 自定义骨骼（需要统计）
                string preTransPath = string.Empty;
                if (boneNameDict.TryGetValue(subChildTransform.name, out preTransPath)) {
                    if (!preTransPath.Equals(transPath)) {
                        UnityEngine.Debug.Log("发现重复的骨骼名称 boneName:" + subChildTransform.name + " preTransPath:" + preTransPath + " transPath:" + transPath);
                    }
                } else {
                    boneNameDict.Add(subChildTransform.name, transPath);
                }
            }

            Transform prefabChildTransform = prefabTransform.Find(subChildTransform.name);
            if (prefabChildTransform == null) {
                // 同步创建骨骼节点
                GameObject prefabChildGameObject = new GameObject(subChildTransform.name);
                prefabChildGameObject.transform.localPosition = subChildTransform.localPosition;
                prefabChildGameObject.transform.localEulerAngles = subChildTransform.localEulerAngles;
                prefabChildGameObject.transform.localRotation = subChildTransform.localRotation;
                prefabChildGameObject.transform.parent = prefabTransform;
                prefabChildTransform = prefabChildGameObject.transform;
            }

            if (!loopTransform(transPath, prefabChildTransform, subChildTransform, boneNameDict, deep+1)) {
                return false;
            }
        }

        return true;
    }

    [MenuItem("FBXMaker/BoneCombine")]
    public static void fbxBoneCombine() {
        string fullPath = "Assets/Resources/Demo/Fbx/";

        if (!Directory.Exists(fullPath)) {
            UnityEngine.Debug.Log("文件夹不存在");
            return;
        }

        GameObject prefab = new GameObject("BoneBase");
        prefab.AddComponent<SkinnedMeshRenderer>();

        GameObject bip001 = null;

        Dictionary<string, string> boneNameDict = new Dictionary<string, string>();

        // 遍历所有FBX(获得完整骨骼信息)
        DirectoryInfo direction = new DirectoryInfo(fullPath);
        FileInfo[] files = direction.GetFiles();
        foreach (var file in files) {
            if (file.Extension == ".meta") {
                continue;
            }
            // UnityEngine.Debug.Log("file.FullName:"+file.FullName);
                
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine(fullPath, file.Name));
            // UnityEngine.Debug.Log("asset.name:"+asset.name+" type:"+asset.GetType());

            GameObject obj = GameObject.Instantiate(asset);

            Transform subBip001 = obj.transform.Find("Bip001");
            if (!subBip001) {
                UnityEngine.Debug.LogError("未找到骨骼根节点 Bip001");
                return;
            }

            if (bip001 == null) {
                bip001 = new GameObject("Bip001");
                bip001.transform.localPosition = subBip001.localPosition;
                bip001.transform.localEulerAngles = subBip001.localEulerAngles;
                bip001.transform.localRotation = subBip001.localRotation;
                bip001.transform.parent = prefab.transform;
            }

            bool ret = loopTransform("Bip001", bip001.transform, subBip001, boneNameDict, 0);
            if (!ret) {
                return;
            }

            GameObject.DestroyImmediate(obj);
            // break;
        }

        PrefabUtility.SaveAsPrefabAsset(prefab, "Assets/Resources/Demo/BoneBase.prefab");
        GameObject.DestroyImmediate(prefab);
        AssetDatabase.Refresh();

        UnityEngine.Debug.Log("骨骼信息合并完毕");
    }

    static int curveTypeToInt(Type type) {
        int typeCode = 0;
        if (type == typeof(UnityEngine.Transform)) {
            typeCode = 1;
        } else {
            UnityEngine.Debug.LogError("curveTypeToInt 缺少处理 type:" + type);
        }
        return typeCode;
    }

    static void writeInt(FileStream fs, int i) {
        fs.Write(BitConverter.GetBytes(i), 0, 4);
    }

    static void writeFloat(FileStream fs, float f) {
        fs.Write(BitConverter.GetBytes(f), 0, 4);
    }

    static void writeString(FileStream fs, string str) {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
        fs.Write(bytes, 0, bytes.Length);
    }

    [MenuItem("FBXMaker/AnimCombine")]
    public static void fbxAnimCombine() {
        string fullPath = "Assets/Resources/Demo/Anim/";
        string outPath = "Assets/Resources/Demo/AnimData/";

        if (!Directory.Exists(fullPath)) {
            UnityEngine.Debug.Log("文件夹不存在");
            return;
        }

        AssetDatabase.StartAssetEditing();

        // 遍历所有FBX(获得完整骨骼信息)
        DirectoryInfo direction = new DirectoryInfo(fullPath);
        FileInfo[] files = direction.GetFiles();
        foreach (var file in files) {
            if (file.Extension == ".meta") {
                continue;
            }

            // Object[] objects = AssetDatabase.LoadAllAssetsAtPath(Path.Combine(fullPath, file.Name));
            // foreach (var obj in objects)
            // {
            //     UnityEngine.Debug.Log("name:" + obj.name + " type:" + obj.GetType());
            // }

            var asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(Path.Combine(fullPath, file.Name));
            FileStream fs = new FileStream(Path.Combine(outPath, Path.GetFileNameWithoutExtension(file.Name) + ".bytes"), FileMode.OpenOrCreate);
            
            // 数据版本
            writeInt(fs, 1);

            EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(asset);

            // 数据长度
            writeInt(fs, curveBindings.Length);

            foreach (var curveBinding in curveBindings) {
                // path
                writeString(fs, curveBinding.path);
                // type
                writeInt(fs, curveTypeToInt(curveBinding.type));
                // propertyName
                writeString(fs, curveBinding.propertyName);

                AnimationCurve animationCurve = AnimationUtility.GetEditorCurve(asset, curveBinding);
                
                // 数据长度
                writeInt(fs, animationCurve.length);

                for (int i = 0; i < animationCurve.length; i++)
                {
                    Keyframe keyframe = animationCurve[i];

                    writeFloat(fs, keyframe.time);
                    writeFloat(fs, keyframe.value);
                    writeFloat(fs, keyframe.inTangent);
                    writeFloat(fs, keyframe.outTangent);
                    writeFloat(fs, keyframe.inWeight);
                    writeFloat(fs, keyframe.outWeight);
                    writeInt(fs, Convert.ToInt32(keyframe.weightedMode));
                    writeInt(fs, Convert.ToInt32(keyframe.tangentMode));
                }
            }

            fs.Flush();
            fs.Dispose();
        }

        AssetDatabase.StopAssetEditing();
        AssetDatabase.Refresh();
        
        UnityEngine.Debug.Log("动画信息处理完毕");
    }
}
