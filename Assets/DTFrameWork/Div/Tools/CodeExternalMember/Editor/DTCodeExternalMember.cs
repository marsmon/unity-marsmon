using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.Text;

namespace DT.FrameWork
{
    [InitializeOnLoad]
    public partial class DTCodeExternalMember
    {
        private static bool isCanRun = false;
        static DTCodeExternalMember()
        {
            // Debug.Log("Assets/DTFrameWork/config.json");
            isCanRun = true;
            // SpaceName = 
        }
        static string SpaceName = "DT.FrameWork";
        static string YFSpaceName = "YF.Art";

    }

    public partial class DTCodeExternalMember
    {
        private const string kResourcesTemplatePath = "Resources/ScriptTemplates";
        private const string m_key = "DT_FW_NAMESPACE";
        private const string EDITOREXTENNAME = "Inspector";

        public static string GetEditorname()
        {
            return EDITOREXTENNAME;
        }

        public static string GetEditorPrefsKey()
        {
            return m_key;
        }
        static string GetTemplatePath(string file)
        {
            string tmpPath = $"Assets/ScriptTemplates/{file}";
            if (File.Exists(tmpPath))
                return tmpPath;
            var basePath = Path.Combine(EditorApplication.applicationContentsPath, kResourcesTemplatePath);
            return Path.Combine(basePath, file);
        }

        static void CreateScript()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                GetTemplatePath("81-C# Script-NewBehaviourScript.cs.txt"),
                "NewBehaviourScript.cs");
            AssetDatabase.Refresh();
        }

        static void CreateShader()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                GetTemplatePath("84-Shader__Unlit Shader-NewUnlitShader.shader.txt"),
                "NewUnlitShader.shader");
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Create/DT c# Script", false, 30)]
        public static void Create_DTScript()
        {
            if (!isCanRun) return;
            EditorPrefs.SetString(m_key, SpaceName + ",Def");
            CreateScript();
        }
        [MenuItem("Assets/Create/DT c# Editor Script", false, 30)]
        public static void Create_DTScript2()
        {
            if (!isCanRun) return;
            EditorPrefs.SetString(m_key, SpaceName + ",Editor");
            CreateScript();
        }
    }

    #region AutoAddNameSpace
    /// <summary>
    /// 自动添加命名空间的脚本
    /// 其实是重写脚本内容
    /// </summary>
    public partial class AutoAddNameSpaceEditor : AssetModificationProcessor
    {
        private static void OnWillCreateAsset(string path)
        {
            string key = DTCodeExternalMember.GetEditorPrefsKey();
            if (!EditorPrefs.HasKey(key)) return;

            path = path.Replace(".meta", "");
            if (path.EndsWith(".cs"))
            {
                string text = "";
                text += File.ReadAllText(path);
                string className = GetClassName(text);
                if (string.IsNullOrEmpty(className)) return;
                string[] data = EditorPrefs.GetString(key).Split(',');
                string newText;
                switch (data[1])
                {
                    case "Editor":
                        if (path.IndexOf("/Editor/") < 0)
                        {
                            path = path.MoveAddFolderFile("Editor",
                                            path.IndexOf(DTCodeExternalMember.GetEditorname()) < 0 ? DTCodeExternalMember.GetEditorname() : string.Empty);
                        }
                        newText = GetNewEditorScriptContext(className, data[0]);
                        File.WriteAllText(path, newText);
                        break;
                    default:
                        newText = GetNewScriptContext(className, data[0]);
                        File.WriteAllText(path, newText);
                        break;
                }
                EditorPrefs.DeleteKey(key);
            }
            AssetDatabase.Refresh();
        }


        private static string GetNewEditorScriptContext(string className, string name)
        {
            var script = new ScriptBuildHelp();
            script.WriteUsing("System");
            script.WriteUsing("System.Collections");
            script.WriteUsing("System.Collections.Generic");
            script.WriteUsing("UnityEngine");
            script.WriteUsing("UnityEditor");

            script.WriteEmptyLine();
            script.WriteNamespace(name);

            script.IndentTimes++;

            string v = className.Replace(DTCodeExternalMember.GetEditorname(), string.Empty);
            script.WriteLine(script.GetIndent() + $"[CustomEditor(typeof({v}))]");

            script.WriteClass(className, "Editor");
            script.IndentTimes++;
            List<string> keys = new List<string>();
            keys.Add("override");
            keys.Add("void");
            script.WriteFunHead(keys, "OnInspectorGUI");
            script.IndentTimes++;
            script.WriteCurlyBrackets("base.OnInspectorGUI();");
            script.IndentTimes--;

            return script.ToString();
        }

        //获取新的脚本内容
        private static string GetNewScriptContext(string className, string name)
        {
            var script = new ScriptBuildHelp();
            script.WriteUsing("System");
            script.WriteUsing("System.Collections");
            script.WriteUsing("System.Collections.Generic");
            script.WriteUsing("UnityEngine");

            script.WriteEmptyLine();
            script.WriteNamespace(name);

            script.IndentTimes++;
            script.WriteClass(className, "MonoBehaviour");
            script.IndentTimes++;
            List<string> keys = new List<string>();
            keys.Add("void");
            script.WriteFun(keys, "Start");
            return script.ToString();
        }

        //获取类名
        private static string GetClassName(string text)
        {
            // 传统的方式
            //return CommonGetClassName(text);

            // 正则表达式的方式 
            return RexGetClassName(text);
        }

        /// <summary>
        /// 传统方法获取
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static string CommonGetClassName(string text)
        {
            string[] data = text.Split(' ');
            int index = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Contains("class"))
                {
                    index = i + 1;
                    break;
                }
            }

            if (data[index].Contains(":"))
            {
                return data[index].Split(':')[0];
            }
            else
            {
                return data[index];
            }

        }

        /// <summary>
        /// 正则表达式的方式 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static string RexGetClassName(string text)
        {

            string patterm = "public class ([A-Za-z0-9_]+)\\s*:\\s*MonoBehaviour"; // 例如：public class NewBehaviourScript : MonoBehaviour
            var match = Regex.Match(text, patterm);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return "";
        }
    }
    #endregion

    #region ScriptBuildHelp
    /// <summary>
    /// 辅助脚本内容的改写
    /// </summary>
    public class ScriptBuildHelp
    {
        public static string Public = "public";
        public static string Private = "private";
        public static string Protected = "protected";

        private StringBuilder _stringBuilder;
        private string _lineBrake = "\r\n";
        private int currentIndex = 0;
        public int IndentTimes { get; set; }

        /// <summary>
        /// 回到大括号中间，需要缩进的值
        /// </summary>
        private int _backNum
        {
            get { return (GetIndent() + "}" + _lineBrake).Length; }
        }

        public ScriptBuildHelp()
        {
            _stringBuilder = new StringBuilder();
            ResetData();
        }

        public void ResetData()
        {
            _stringBuilder.Clear();
            currentIndex = 0;
        }

        private void Write(string context, bool needIndent = true)
        {
            if (needIndent)
            {
                context = GetIndent() + context;
            }

            if (currentIndex == _stringBuilder.Length)
            {
                _stringBuilder.Append(context);
            }
            else
            {
                _stringBuilder.Insert(currentIndex, context);
            }

            currentIndex += context.Length;
        }

        public void WriteLine(string context, bool needIndent = false)
        {
            Write(context + _lineBrake, needIndent);
        }

        public string GetIndent()
        {
            string indent = "";
            for (int i = 0; i < IndentTimes; i++)
            {
                indent += "    ";
            }
            return indent;
        }

        /// <summary>
        /// 返回值为回到大括号中间，需要缩进的值
        /// </summary>
        /// <returns></returns>
        private int WriteCurlyBrackets()
        {
            var start = _lineBrake + GetIndent() + "{" + _lineBrake;
            var end = GetIndent() + "}" + _lineBrake;
            Write(start + end);
            return end.Length;
        }

        public int WriteCurlyBrackets(params string[] collection)
        {
            var start = _lineBrake + GetIndent() + "{" + _lineBrake;
            var end = GetIndent() + "}" + _lineBrake;
            var data = "";
            foreach (var item in collection)
            {
                data += GetIndent() + GetIndent() + item + _lineBrake;
            }
            Write(start + data + end);
            return end.Length;
        }

        public void WriteUsing(string nameSpaceName, bool isAnnotation = false)
        {
            WriteLine(isAnnotation ? $"//using {nameSpaceName};" : $"using {nameSpaceName};");
        }

        public void WriteEmptyLine()
        {
            WriteLine("");
        }

        public void WriteNamespace(string name)
        {
            Write("namespace " + name);
            WriteCurlyBrackets();
            BackToInsertContent();
        }

        public void WriteClass(string name, params string[] baseName)
        {
            StringBuilder temp = new StringBuilder();
            for (int i = 0; i < baseName.Length; i++)
            {
                temp.Append(baseName[i]);
                if (i != baseName.Length - 1)
                {
                    temp.Append(",");
                }
            }

            Write("public class " + name + " : " + temp + " ");
            WriteCurlyBrackets();
            BackToInsertContent();
        }

        public void WriteInterface(string name, params string[] baseName)
        {
            StringBuilder temp = new StringBuilder();
            for (int i = 0; i < baseName.Length; i++)
            {
                temp.Append(baseName[i]);
                if (i != baseName.Length - 1)
                {
                    temp.Append(",");
                }
            }

            Write("public interface " + name + " : " + temp + " ", true);
            WriteCurlyBrackets();
            BackToInsertContent();
        }

        public void WriteFunHead(List<string> keyName, string name, string othes = "", params string[] paraName)
        {
            WriteFunHead(name, Public, keyName, othes, paraName);
        }
        public void WriteFunHead(string name, string publicState = "public", List<string> keyName = null, string othes = "", params string[] paraName)
        {
            StringBuilder keyTemp = new StringBuilder();

            if (keyName != null)
            {
                for (int i = 0; i < keyName.Count; i++)
                {
                    keyTemp.Append(keyName[i]);
                    if (i != keyName.Count - 1)
                    {
                        keyTemp.Append(" ");
                    }
                }

            }

            StringBuilder temp = new StringBuilder();
            temp.Append(publicState + " " + keyTemp + " " + name + "()");
            if (paraName.Length > 0)
            {
                foreach (string s in paraName)
                {
                    temp.Insert(temp.Length - 1, s + ",");
                }
                temp.Remove(temp.Length - 2, 1);
            }

            temp.Append(" ");
            temp.Append(othes);

            Write(temp.ToString());
            // WriteCurlyBrackets();
        }
        public void WriteFun(List<string> keyName, string name, string othes = "", params string[] paraName)
        {
            WriteFun(name, Public, keyName, othes, paraName);
        }
        public void WriteFun(string name, string publicState = "public", List<string> keyName = null, string othes = "", params string[] paraName)
        {
            StringBuilder keyTemp = new StringBuilder();

            if (keyName != null)
            {
                for (int i = 0; i < keyName.Count; i++)
                {
                    keyTemp.Append(keyName[i]);
                    if (i != keyName.Count - 1)
                    {
                        keyTemp.Append(" ");
                    }
                }

            }

            StringBuilder temp = new StringBuilder();
            temp.Append(publicState + " " + keyTemp + " " + name + "()");
            if (paraName.Length > 0)
            {
                foreach (string s in paraName)
                {
                    temp.Insert(temp.Length - 1, s + ",");
                }
                temp.Remove(temp.Length - 2, 1);
            }

            temp.Append(" ");
            temp.Append(othes);

            Write(temp.ToString());
            WriteCurlyBrackets();
        }

        /// <summary>
        /// 设置光标位置,到大括号内插入内容
        /// </summary>
        /// <param name="num"></param>
        public void BackToInsertContent()
        {
            currentIndex -= _backNum;
        }

        /// <summary>
        /// 设置光标位置,到结束大括号外
        /// </summary>
        /// <param name="num"></param>
        public void ToContentEnd()
        {
            currentIndex += _backNum;
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
    #endregion 
}