
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FBXCombineManager
{
    private const int COMBINE_TEXTURE_MAX = 2048;

    public static FBXCombineManager Instance = new FBXCombineManager();

    public FBXCombine Generate(string name, string bonebase, string hair, string head, string body, string hairTex, string headTex, string bodyTex, string hairAnim, string headAnim, string bodyAnim) {
         FBXCombine instance = new FBXCombine(name, bonebase, hair, head, body, hairTex, headTex, bodyTex, hairAnim, headAnim, bodyAnim);
         return instance;
    }

    private Type intToCurveType(int tp) {
        switch (tp) {
            case 1:
                return typeof(UnityEngine.Transform);

            default:
                UnityEngine.Debug.LogError("intToCurveType 缺少处理 tp:" + tp);
                return typeof(UnityEngine.GameObject);
        }
    }

    public void CombineObject(GameObject skeleton, SkinnedMeshRenderer[] meshes, Texture2D[] texture2Ds, TextAsset[] textAssets) {
        // Fetch all bones of the skeleton
        List<Transform> transforms = new List<Transform>();
		transforms.AddRange(skeleton.GetComponentsInChildren<Transform>(true));

		List<CombineInstance> combineInstances = new List<CombineInstance>();//the list of meshes
		List<Transform> bones = new List<Transform>();//the list of bones

		// Below informations only are used for merge materilas(bool combine = true)
        List<Vector2[]> oldUV = null;
        Material newMaterial = null;
		Texture2D newTexture2D = null;

        // Collect information from meshes
        for (int i = 0; i < meshes.Length; i ++)
		{
			SkinnedMeshRenderer smr = meshes[i];

            for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
			{
				CombineInstance ci = new CombineInstance();
				ci.mesh = smr.sharedMesh;
				ci.subMeshIndex = sub;
				combineInstances.Add(ci);
			}

            // Collect bones
            for (int j = 0 ; j < smr.bones.Length; j ++)
            {
                bool find = false;
                int tBase = 0;
                for (tBase = 0; tBase < transforms.Count; tBase ++)
                {
                    if (smr.bones[j].name.Equals(transforms[tBase].name))
                    {
                        bones.Add(transforms[tBase]);
                        find = true;
                        break;
                    }
                }

                if (!find) {
                    bones.Add(null);
                }
            }

        }

        newMaterial = new Material (Shader.Find ("YoFi/GGS_Char_SimpleBase"));
        oldUV = new List<Vector2[]>();
        
        // merge the texture
        List<Texture2D> Textures = new List<Texture2D>();
        foreach (var texture2D in texture2Ds)
        {
            Textures.Add(texture2D);
        }

        newTexture2D = new Texture2D(COMBINE_TEXTURE_MAX, COMBINE_TEXTURE_MAX, TextureFormat.RGBA32, true);
        Rect[] uvs = newTexture2D.PackTextures(Textures.ToArray(), 0);
        // newMaterial.mainTexture = newTexture2D
        newMaterial.SetTexture("_MainTex", newTexture2D);
        newMaterial.SetTexture("_SssTex", newTexture2D);
        newMaterial.SetFloat("_viewAngle", 20);
        newMaterial.SetFloat("_Outline", 0.15f);

        // reset uv
        Vector2[] uva, uvb;
        for (int j = 0; j < combineInstances.Count; j++)
        {
            uva = (Vector2[])(combineInstances[j].mesh.uv);
            uvb = new Vector2[uva.Length];
            for (int k = 0; k < uva.Length; k++)
            {
                uvb[k] = new Vector2((uva[k].x * uvs[j].width) + uvs[j].x, (uva[k].y * uvs[j].height) + uvs[j].y);
            }
            oldUV.Add(combineInstances[j].mesh.uv);
            combineInstances[j].mesh.uv = uvb;
        }

        // Create a new SkinnedMeshRenderer
        SkinnedMeshRenderer oldSKinned = skeleton.GetComponent<SkinnedMeshRenderer>();
		if (oldSKinned != null) {

			GameObject.DestroyImmediate(oldSKinned);
		}
		SkinnedMeshRenderer r = skeleton.AddComponent<SkinnedMeshRenderer>();
        r.rootBone = skeleton.transform;
        r.sharedMesh = new Mesh();
		r.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, false);// Combine meshes
		r.bones = bones.ToArray();// Use new bones
        r.material = newMaterial;
        for (int i = 0 ; i < combineInstances.Count ; i ++)
        {
            combineInstances[i].mesh.uv = oldUV[i];
        }

        AnimationClip clip = new AnimationClip();
        clip.legacy = true;
        clip.frameRate = 30;
        clip.wrapMode = WrapMode.Loop;

        // 解析动画
        int intValue;
        foreach (var textAsset in textAssets)
        {
            byte[] bytes = textAsset.bytes;
            int index = 0;
            int version = BitConverter.ToInt32(bytes, index);
            if (version == 1) {
                int length = BitConverter.ToInt32(bytes, index + 4);
                index = index + 8;
                for (int i = 0; i < length; i++)
                {
                    intValue = BitConverter.ToInt32(bytes, index);
                    string relativePath = Encoding.UTF8.GetString(bytes, index + 4, intValue);
                    index += 4 + intValue;

                    Type tp = intToCurveType(BitConverter.ToInt32(bytes, index));
                    index += 4;

                    intValue = BitConverter.ToInt32(bytes, index);
                    string propertyName = Encoding.UTF8.GetString(bytes, index + 4, intValue);
                    index += 4 + intValue;

                    intValue = BitConverter.ToInt32(bytes, index);
                    index += 4;

                    Keyframe[] keyframes = new Keyframe[intValue];

                    for (int j = 0; j < intValue; j++)
                    {
                        float time = BitConverter.ToSingle(bytes, index);
                        float value = BitConverter.ToSingle(bytes, index + 4);
                        float inTangent = BitConverter.ToSingle(bytes, index + 8);
                        float outTangent = BitConverter.ToSingle(bytes, index + 12);
                        float inWeight = BitConverter.ToSingle(bytes, index + 16);
                        float outWeight = BitConverter.ToSingle(bytes, index + 20);
                        int weightedMode = BitConverter.ToInt32(bytes, index + 24);
                        int tangentMode = BitConverter.ToInt32(bytes, index + 28);

                        Keyframe keyframe = new Keyframe(time, value, inTangent, outTangent, inWeight, outWeight);
                        keyframe.weightedMode = (WeightedMode)weightedMode;
                        keyframe.tangentMode = tangentMode;

                        keyframes[j] = keyframe;

                        index += 32;
                    }

                    AnimationCurve curve = new AnimationCurve(keyframes);
                    
                    // UnityEngine.Debug.Log("relativePath:" + relativePath + " ")
                    clip.SetCurve(relativePath, tp, propertyName, curve);
                }

            } else {
                UnityEngine.Debug.LogError("动画数据解析数据版本不支持");
                return;
            }
        }

        // Create a new Animation
        Animation oldAnimation = skeleton.GetComponent<Animation>();
        if (oldAnimation != null) {
            GameObject.DestroyImmediate(oldAnimation);
        }
        Animation a = skeleton.AddComponent<Animation>();
        a.AddClip(clip, "idle");
        a.Play("idle", PlayMode.StopAll);
    }

    public AnimationClip CombineAnimationClip(AnimationClip[] animationClips) {
        AnimationClip animationClip = new AnimationClip();

        return animationClip;
    }
}

public class FBXCombine
{
    public GameObject Instance = null;

    public string bonebase;
    public string hair;
    public string head;
    public string body;
    public string hairTex;
    public string headTex;
    public string bodyTex;
    public string hairAnim;
    public string headAnim;
    public string bodyAnim;

    public FBXCombine(string name, string bonebase, string hair, string head, string body, string hairTex, string headTex, string bodyTex, string hairAnim, string headAnim, string bodyAnim) {
        UnityEngine.Object res = Resources.Load(bonebase);

        this.Instance = GameObject.Instantiate(res) as GameObject;
        this.Instance.transform.localPosition = new Vector3(1.5f, 0.0f, 0.0f);
        this.Instance.transform.localEulerAngles = new Vector3(0.0f, 135.0f, 0.0f);

        this.hair = hair;
        this.head = head;
        this.body = body;

        this.hairTex = hairTex;
        this.headTex = headTex;
        this.bodyTex = bodyTex;

        this.hairAnim = hairAnim;
        this.headAnim = headAnim;
        this.bodyAnim = bodyAnim;

        string[] parts = new string[3];
        parts[0] = hair;
        parts[1] = head;
        parts[2] = body;

        string[] partTexs = new string[3];
        partTexs[0] = hairTex;
        partTexs[1] = headTex;
        partTexs[2] = bodyTex;

        string[] partAnims = new string[3];
        partAnims[0] = hairAnim;
        partAnims[1] = headAnim;
        partAnims[2] = bodyAnim;

        // 加载Mesh
        GameObject[] objects = new GameObject[parts.Length];
        SkinnedMeshRenderer[] meshes = new SkinnedMeshRenderer[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            res = Resources.Load(parts [i]);
            objects[i] = GameObject.Instantiate (res) as GameObject;
            meshes[i] = objects[i].GetComponentInChildren<SkinnedMeshRenderer> ();
        }

        Texture2D[] texture2Ds = new Texture2D[partTexs.Length];
        for (int i = 0; i < partTexs.Length; i++)
        {
            texture2Ds[i] = Resources.Load<Texture2D>(partTexs[i]);
        }

        TextAsset[] textAssets = new TextAsset[partAnims.Length];
        for (int i = 0; i < partAnims.Length; i++)
        {
            textAssets[i] = Resources.Load<TextAsset>(partAnims[i]);
        }

        // Combine meshes
        FBXCombineManager.Instance.CombineObject(Instance, meshes, texture2Ds, textAssets);

        // Delete temporal resources
        for (int i = 0; i < objects.Length; i++) {
			
			GameObject.DestroyImmediate (objects[i].gameObject);
		}
    }
}
