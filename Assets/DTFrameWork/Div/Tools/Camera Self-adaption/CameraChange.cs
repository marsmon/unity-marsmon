
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    public float baseWidth = 1920f;
    public float baseHight = 1080f;
    public float xishu = 5f;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<Camera>().orthographicSize = baseWidth / Screen.height * xishu;
        Vector2 v = new Vector2(Screen.width, Screen.height);
        float wbl = 1f;
        if (v.x != baseHight) wbl = baseWidth / v.x;
        transform.GetComponent<Camera>().orthographicSize = (v.y >= baseHight ? v.y / baseHight : 1) * xishu * wbl;


        // DT.FrameWork.DTGameScene.Instance.RockingBar.onDrag += Move;
        // DT.FrameWork.DTGameScene.Instance.RockingBar.onDragEnd += Move0;
    }
    bool ismove = false;
    Vector3 dir;
    public void Move(Vector2 direction)
    {
        dir.x = direction.x;
        dir.z = direction.y;//direction.y;
        dir.Normalize();
    }
    public void Move0()
    {
        dir = Vector3.zero;
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (DT.FrameWork.DTGameScene.Instance != null)
            if (DT.FrameWork.DTGameScene.Instance.play != null)
                this.transform.parent.position = DT.FrameWork.DTGameScene.Instance.play.transform.position;
    }

#if UNITY_EDITOR

    public void SetSize()
    {
        Vector2 v = GameViewSize();
        Debug.Log(v);
        float wbl = 1f;
        if (v.x != baseHight) wbl = baseWidth / v.x;
        transform.GetComponent<Camera>().orthographicSize = (v.y >= baseHight ? v.y / baseHight : 1) * xishu * wbl;

    }
    private Vector2 GameViewSize()
    {
        var mouseOverWindow = UnityEditor.EditorWindow.mouseOverWindow;
        System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
        System.Type type = assembly.GetType("UnityEditor.PlayModeView");

        Vector2 size = (Vector2)type.GetMethod(
            "GetMainPlayModeViewTargetSize",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Static
        ).Invoke(mouseOverWindow, null);

        return size;
    }
#endif

}
