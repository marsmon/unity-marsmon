
using UnityEngine;
using UnityEditor;

namespace YF.Art.PostProcessing.Editor {
    [CustomEditor(typeof(YFPostProcessing))]
    public class YFPostProcessingInspector : UnityEditor.Editor {
        private YFPostProcessing _target { get { return target as YFPostProcessing; } }
    
        public override void OnInspectorGUI() {
            EditorGUILayout.BeginVertical();

            _target.isBloomRun = EditorGUILayout.Toggle("是否开启Bloom", _target.isBloomRun);
            if (_target.isBloomRun) {
                // EditorGUILayout.BeginHorizontal();
                // GUILayout.Space(20);
                // _target.sepBlurSpread = EditorGUILayout.FloatField("SepBlurSpread", _target.sepBlurSpread);
                // EditorGUILayout.EndHorizontal();

                // EditorGUILayout.BeginHorizontal();
                // GUILayout.Space(20);
                // _target.bloomIntensity = EditorGUILayout.FloatField("BloomIntensity", _target.bloomIntensity);
                // EditorGUILayout.EndHorizontal();
            }

            _target.isShineRun = EditorGUILayout.Toggle("是否开启Shine", _target.isShineRun);
            if (_target.isShineRun) {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                _target.DayToNight = EditorGUILayout.Slider("DayToNight", _target.DayToNight, 0.0f, 1.0f);
                EditorGUILayout.EndHorizontal();

                // EditorGUILayout.BeginHorizontal();
                // _target.SunColor = EditorGUILayout.GradientField("SunColor", _target.SunColor);
                // EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }
}