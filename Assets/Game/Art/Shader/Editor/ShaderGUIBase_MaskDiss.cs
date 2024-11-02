// #define DT_BUTTON
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System;


namespace DiteScripts.YF.EditoShaderGUI.Div
{
   //============================================================================================
    //ShaderGUIBase_Increment
    //============================================================================================
    #region ShaderGUIBase_Increment
    internal partial class ShaderGUIBase_MaskDiss : ShaderGUIBase
    {
         protected override void HelpOnGUI(string HelpBox){
            EditorGUILayout.HelpBox(@"Custom1.xyzw(T0.zw|xy);   Custom2.xyzw(T1.zw|xy)
Custom1.xy = main.tex.uv;
Custom1.zw = eff.tex.uv;
Custom2.xy = mask.tex.uv;
Custom2.z = 溶解值;
",MessageType.Info);
        }
    }
    #endregion
}
