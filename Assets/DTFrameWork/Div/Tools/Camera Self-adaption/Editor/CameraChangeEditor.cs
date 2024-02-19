using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraChange))]
[CanEditMultipleObjects]
class CameraChangeEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if(GUILayout.Button("Set")){
            var tag = target as CameraChange;
            tag.SetSize();
        }
    }
}