using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if false

[CustomEditor(typeof(AnimCheckTools))]
public class AnimCheckToolsGUI : Editor
{
    /// <summary>
    /// 当前运行时间
    /// </summary>
    private float m_RunningTime;
    /// <summary>
    /// 上一次系统时间
    /// </summary>
    private double m_PreviousTime;
    float Duration = 2;
    bool isPlaying = false;
    bool isloop = false;


    void OnDestroy()
    {
        EditorApplication.update -= inspectorUpdate;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var tag = target as AnimCheckTools;
        if (tag == null) return;
        if (!this.isPlaying)
        {
            tag.PlayAnim((int)(tag._yofitime));
        }
        if (Application.isPlaying)
        {
            EditorApplication.update -= inspectorUpdate;
        }
        if (GUILayout.Button("刷新")){
            tag._d = (AnimState) ((int)tag.d + 1 % 4);
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(isPlaying ? "Stop" : "Play"))
        {
            isPlaying = !isPlaying;
            if (isPlaying)
            {
                m_PreviousTime = EditorApplication.timeSinceStartup;
                m_RunningTime = 0f;
                EditorApplication.update += inspectorUpdate;
            }
            else
            {
                EditorApplication.update -= inspectorUpdate;
                m_PreviousTime = 0;
            }
        }
        Color bdef = GUI.backgroundColor;
        GUI.backgroundColor = isloop ? Color.green : Color.red;
        if (GUILayout.Button("loop", GUILayout.Width(50), GUILayout.ExpandWidth(false)))
        {
            isloop = !isloop;
        }
        GUI.backgroundColor = bdef;
        EditorGUILayout.EndHorizontal();
    }

    private void update()
    {
        if (Application.isPlaying)
        {
            return;
        }

        var tag = target as AnimCheckTools;
        if (tag == null) return;

        tag._yofitime = (int)(m_RunningTime * 30f);
        tag.PlayAnim((int)(m_RunningTime * 30f));
        SceneView.RepaintAll();
        Repaint();
    }
    private void inspectorUpdate()
    {
        this.inspectorUpdate(this.isPlaying, this.isloop, update, 2);
    }
    private void inspectorUpdate(bool isplaying, bool isloop, Action UpdateFun, float kDuration = 60)
    {
        var delta = EditorApplication.timeSinceStartup - m_PreviousTime;
        m_PreviousTime = EditorApplication.timeSinceStartup;

        if (!Application.isPlaying && isplaying)
        {
            if (isloop)
            {
                m_RunningTime = (m_RunningTime + (float)delta) % kDuration;
            }
            else
            {
                m_RunningTime = Mathf.Clamp(m_RunningTime + (float)delta, 0f, kDuration);
            }

            UpdateFun();
        }
    }

}
#endif