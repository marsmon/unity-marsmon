using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace YF.Art
{
    public static class ClearMaterial
    {
        public static void Clear(Material o)
        {
            Material mat = o as Material;
            if (mat == null)
            {
                return;
            }
            SerializedObject psSource = new SerializedObject(mat);

            SerializedProperty ShaderKeywords = psSource.FindProperty("m_ShaderKeywords");
            CleanMaterialShaderKeywords(ShaderKeywords, mat);

            SerializedProperty emissionProperty = psSource.FindProperty("m_SavedProperties");
            SerializedProperty texEnvs = emissionProperty.FindPropertyRelative("m_TexEnvs");
            SerializedProperty floats = emissionProperty.FindPropertyRelative("m_Floats");
            SerializedProperty colos = emissionProperty.FindPropertyRelative("m_Colors");
            CleanMaterialSerializedProperty(texEnvs, mat);
            CleanMaterialSerializedProperty(floats, mat);
            CleanMaterialSerializedProperty(colos, mat);




            psSource.ApplyModifiedProperties();

            EditorUtility.SetDirty(o);
            Debug.Log("清理完成!!");
        }

        private static void CleanMaterialShaderKeywords(SerializedProperty property, Material mat)
        {

            string s = "";
            for (int i = 0; i < mat.shaderKeywords.Length; i++)
            {
                s = s + mat.shaderKeywords[i].ToString() + " ";
            }
            mat.shaderKeywords = null;
        }

        private static void CleanMaterialSerializedProperty(SerializedProperty property, Material mat)
        {
            for (int j = property.arraySize - 1; j >= 0; j--)
            {
                string propertyName = property.GetArrayElementAtIndex(j).FindPropertyRelative("first").stringValue;
                if (!mat.HasProperty(propertyName))
                {
                    property.DeleteArrayElementAtIndex(j);
                }
            }
        }
    }
}