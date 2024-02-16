using UnityEngine;

public class FBXLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FBXCombine instance = FBXCombineManager.Instance.Generate(
            "temp",
            "Demo/BoneBase",
            "Demo/Prefab/100@skin",
            "Demo/Prefab/200@skin",
            "Demo/Prefab/300@skin",
            "Demo/Tex/100",
            "Demo/Tex/200",
            "Demo/Tex/300",
            "Demo/AnimData/100@idle",
            "Demo/AnimData/200@idle",
            "Demo/AnimData/300@idle"
            );

        // AnimationClip clip = Resources.Load<AnimationClip>("Demo/Anim/300@idle");

        // Animation animation = instance.Instance.AddComponent<Animation>();
        // animation.AddClip(clip, "idle");
        // animation.Play("idle", PlayMode.StopSameLayer);
    }
}
