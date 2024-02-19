using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
namespace YF.Art
{
    [CustomEditor(typeof(ParticleSystempExtend)), CanEditMultipleObjects]
    public class ParticleSystempExtendEditor : Editor
    {
        private BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();
        public override void OnInspectorGUI()
        {
            var tag = target as ParticleSystempExtend;
            if (tag != null)
            {
                if (tag.m_System.main.maxParticles > 50)
                {
                    EditorGUILayout.HelpBox("请设定合理的粒子上限.\n当前上限设置已经超过50", MessageType.Error);
                }
            }
            base.OnInspectorGUI();

        }
        private void OnSceneGUI()
        {
            ParticleSystempExtend tag = (ParticleSystempExtend)target;

            Handles.matrix = tag.isWorld ? Matrix4x4.identity : tag.transform.localToWorldMatrix;
            // if (tag != null)
            // {
                // foreach (var m in tag.m_Particles)
                // {
                //     Handles.Label(m.position, m.velocity.ToString());
                // }
            // }
            m_BoundsHandle.center = tag.fanwei.center;
            m_BoundsHandle.size = tag.fanwei.size;
            m_BoundsHandle.SetColor(Color.green);
            EditorGUI.BeginChangeCheck();
            m_BoundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                // record the target object before setting new values so changes can be undone/redone
                Undo.RecordObject(tag, "Change Bounds");

                // copy the handle's updated data back to the target object
                Bounds newBounds = new Bounds();
                newBounds.center = m_BoundsHandle.center;
                newBounds.size = m_BoundsHandle.size;
                tag.fanwei = newBounds;
            }
        }
    }
}