using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace YF
{
    class TmpSpritedata{
        public string name;
        public Sprite sprite;
    }
    class FrameCreatAnimClip : EditorWindow {

        const string m_modelRootDir = "Assets/Game/Demo/Demo Art/ModelFrame"; //Assets/Game/Demo/Demo Art/ModelFrame        
        const string m_modelRootDirBase = "Assets/Game/Art/ModelFrame/";
        int m_curModelID = 0;
        System.Type spriterenderer_type = typeof(SpriteRenderer);
        System.Type transform_type = typeof(Transform);
        


        DefaultAsset path;

        Dictionary<string, Sprite> spriteDict;
        Dictionary<int, Dictionary<int, List<KeyValuePair<int, string>>>> animationDict;



        [MenuItem("Tools/ModelFrame/FrameCreatAnimClip")]
        static void ShowWindow()
        {
            var window = GetWindow<FrameCreatAnimClip>();
            window.titleContent = new GUIContent("FrameCreatAnimClip");
            window.Show();
        }

        void OnGUI() {
            this.path = (DefaultAsset)EditorGUILayout.ObjectField((UnityEngine.Object)this.path,typeof(DefaultAsset),true);
            string[] s = new string[]{AssetDatabase.GetAssetPath(this.path)};
            EditorGUILayout.TextArea(s[0]);

            


            if(GUILayout.Button("执行")){
                // Debug.Log( Selection.activeObject);
                this.Run(s);
            }
        }


        public void Run(string[] path) {
            CONST.LoadActions();

            if (! Directory.Exists(m_modelRootDir)) {
                return;
            }
            DirectoryInfo directory = new DirectoryInfo(m_modelRootDir);  
            try {
                    m_curModelID = Convert.ToInt32(this.path.name);    
            } catch (System.Exception e) {
                UnityEngine.Debug.LogError("ModelFrame目录含有错误的模型目录命名" + e.ToString());
                // continue;
                return;
            }



            spriteDict = new Dictionary<string, Sprite>();
            animationDict = new Dictionary<int, Dictionary<int, List<KeyValuePair<int, string>>>>();
            // 参数初始化
            this.spriteDict.Clear();
            this.animationDict.Clear();

            // 采集Sprite
            CollectSprite(path);
            // return;
            // 生成Animations
            MakeAnimation();

            // 生成Controller
            MakeController();
        }

        public TmpSpritedata FromPathgetData(string path){
            TmpSpritedata data = new TmpSpritedata();
            string ff = AssetDatabase.GUIDToAssetPath(path);
            data.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(ff) as Sprite;
            if(data.sprite == null){
                return null;
            }
            string[] d = ff.Split('/');
            int len = d.Length - 1;
            string id = this.path.name;
            string frame = d[len].Split('.')[0];
            frame =  Convert.ToUInt32(frame).ToString("D2");
            string anim = d[len-1];
            anim =  Convert.ToUInt32(anim).ToString("D2");
            string arm = d[len-2];
            arm =  Convert.ToUInt32(arm).ToString("D1");
            // Debug.Log(id  + "_" + arm + "_" + anim + "_" + frame + ".png");
            data.name = id  + "_" + arm + "_" + anim + "_" + frame;
            return data;
        }


        void CollectSprite(string[] path) {
            string[] objPath = AssetDatabase.FindAssets("t:texture2d",path); //
            List<TmpSpritedata> objs = new List<TmpSpritedata>();
            foreach(string _path in objPath){
                TmpSpritedata dd = this.FromPathgetData(_path);                
                if (dd != null){
                    objs.Add(dd);
                }
            }
            
            // return;            
            foreach(var obj in objs) {
                if(obj == null){
                    Debug.Log(obj.name);
                }
                if (! (obj.sprite is Sprite)) {
                    continue;
                }

                try {
                    spriteDict.Add(obj.name, obj.sprite as Sprite);
                }
                catch (System.Exception e) {
                    UnityEngine.Debug.LogError("spriteDict add fail " + e.ToString());
                }

                try {
                    // 字符串裁剪
                    Regex reg = new Regex("(?<key1>.*?)_(?<key2>.*?)_(?<key3>.*?)_(?<key4>.*)");
                    Match match = reg.Match(obj.name);
                    int directionId = Convert.ToInt32(match.Groups["key2"].Value);
                    int actionId = Convert.ToInt32(match.Groups["key3"].Value);
                    int frameIdx = Convert.ToInt32(match.Groups["key4"].Value);

                    Dictionary<int, List<KeyValuePair<int, string>>> dict;
                    List<KeyValuePair<int, string>> list;

                    if (animationDict.ContainsKey(directionId)) {
                        dict = animationDict[directionId];

                    } else {
                        dict = new Dictionary<int, List<KeyValuePair<int, string>>>();
                        animationDict[directionId] = dict;
                    }

                    if (dict.ContainsKey(actionId)) {
                        list = dict[actionId];
                    } else {
                        list = new List<KeyValuePair<int, string>>();
                        dict[actionId] = list;
                    }

                    list.Add(new KeyValuePair<int, string>(frameIdx, obj.name));
                }
                catch (System.Exception e) {
                    UnityEngine.Debug.LogError("animationDict add fail " + e.ToString());
                }
                // }

                // 对List排序
                foreach (var dict in animationDict) {
                    foreach (var list in dict.Value.Values) {
                        list.Sort(new PairComparer());
                    }
                }
            }

            // 填充左朝向图片数据
            if (animationDict.ContainsKey((int)DIRECTION.RIGHT)) {
                animationDict[(int)DIRECTION.LEFT] = animationDict[(int)DIRECTION.RIGHT];
            }

            // 法宝只有下方朝向, 需要填充其他3朝向图片数据
            // if (m_curModelID >= 21000 && m_curModelID <= 21999) {
            //     animationDict[(int)DIRECTION.LEFT] = animationDict[(int)DIRECTION.DOWN];
            //     animationDict[(int)DIRECTION.RIGHT] = animationDict[(int)DIRECTION.DOWN];
            //     animationDict[(int)DIRECTION.UP] = animationDict[(int)DIRECTION.DOWN];
            // }
        }

        const float FRAME_INTERVAL = 1f / 60f;

        void MakeAnimation() {
            string animationsDirPath = Path.Combine(Path.Combine(m_modelRootDir, m_curModelID.ToString()), "Animations");
            YF.FileUtil.CheckDirAndCreateWhenNeeded(animationsDirPath);

            foreach (ACTION action in CONST.ACTIONS) {
                Dictionary<int, List<KeyValuePair<int, string>>> dict;
                if (!this.animationDict.TryGetValue((int)action.direction, out dict)) {
                    continue;
                }

                if (dict.Count <= 0) {
                    continue;
                }

                List<KeyValuePair<int, string>> list;

                if (!dict.TryGetValue(action.id, out list)) {
                    continue;
                }

                if (list.Count <= 0) {
                    continue;
                }

                MakeAnimationClip(action);
            }
        }

        void MakeAnimationClip(ACTION action) {
            string animationsDirPath = Path.Combine(Path.Combine(m_modelRootDir, m_curModelID.ToString()), "Animations");

            int direction = (int)action.direction;
            Dictionary<int, List<KeyValuePair<int, string>>> dict = this.animationDict[direction];
            List<KeyValuePair<int, string>> list = dict[action.id];

            string clipPath = Path.Combine(animationsDirPath, action.name + "_" + action.id + ".anim");

            AnimationClip clip = null;
            if (File.Exists(clipPath)) {
                clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
                clip.ClearCurves();
            } else {
                clip = new AnimationClip();
                AssetDatabase.CreateAsset(clip, clipPath);
            }
            clip.frameRate = 60f;

            // 设置循环
            AnimationClipSettings setting = AnimationUtility.GetAnimationClipSettings(clip);
            setting.loopTime = action.loop;
            AnimationUtility.SetAnimationClipSettings(clip, setting);

            // 设置动画
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.type = spriterenderer_type;
            curveBinding.path = "";
            curveBinding.propertyName = "m_Sprite";

            int count = list.Count;
            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[count + 1];

            // dict.Keys
            for (int k = 0; k < count; k++) {
                var pair = list[k];
                keyframes[k] = new ObjectReferenceKeyframe();
                keyframes[k].time = Convert.ToSingle((FRAME_INTERVAL * pair.Key * 2).ToString("f4"));
                if (this.spriteDict.ContainsKey(pair.Value)) {
                    keyframes[k].value = this.spriteDict[pair.Value];

                    if (k == count - 1) {
                        // 最后一帧
                        keyframes[count] = new ObjectReferenceKeyframe();
                        keyframes[count].time = Convert.ToSingle((FRAME_INTERVAL * (pair.Key * 2 + 1)).ToString("f4"));
                        keyframes[count].value = this.spriteDict[pair.Value];
                    }
                }
            }
            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyframes);

            bool singleDirection = false;
            // 单朝向的, 不添加翻转曲线
            if (m_curModelID >= 21000 && m_curModelID <= 21999) {
                // 法宝调整位置曲线
                EditorCurveBinding curveBindingPosition = new EditorCurveBinding();
                curveBindingPosition.type = transform_type;
                curveBindingPosition.path = "";
                
                Keyframe[] positionKeyframes = new Keyframe[1];
                if (direction == (int)DIRECTION.UP) {
                    positionKeyframes[0] = new Keyframe(0f, -0.5f);
                    curveBindingPosition.propertyName = "m_LocalPosition.y";
                } else if (direction == (int)DIRECTION.RIGHT) {
                    positionKeyframes[0] = new Keyframe(0f, -0.5f);
                    curveBindingPosition.propertyName = "m_LocalPosition.x";
                } else if (direction == (int)DIRECTION.DOWN) {
                    positionKeyframes[0] = new Keyframe(0f, 0.5f);
                    curveBindingPosition.propertyName = "m_LocalPosition.y";
                } else if (direction == (int)DIRECTION.LEFT) {
                    positionKeyframes[0] = new Keyframe(0f, 0.5f);
                    curveBindingPosition.propertyName = "m_LocalPosition.x";
                }

                AnimationCurve curvePosition = new AnimationCurve(positionKeyframes);
                AnimationUtility.SetEditorCurve(clip, curveBindingPosition, curvePosition);

            } else if (m_curModelID >= 23000 && m_curModelID <= 23999) {
                // npc 单朝向
                singleDirection = true;
            }

            if (!singleDirection) {
                // 翻转动画曲线
                EditorCurveBinding flipCurveBinding = new EditorCurveBinding();
                flipCurveBinding.type = spriterenderer_type;
                flipCurveBinding.path = "";
                flipCurveBinding.propertyName = "m_FlipX";

                // 翻转帧, 将角色进行翻转
                Keyframe[] flipKeyFrame = new Keyframe[1];
                if (direction != (int)DIRECTION.LEFT) {
                    flipKeyFrame[0] = new Keyframe(0f, 0f);
                } else {
                    flipKeyFrame[0] = new Keyframe(0f, 1f);
                }
                AnimationCurve curve = new AnimationCurve(flipKeyFrame);
                AnimationUtility.SetEditorCurve(clip, flipCurveBinding, curve);

                EditorUtility.SetDirty(clip);
            }
        }

        void MakeController() {
            string animationsDirPath = Path.Combine(Path.Combine(m_modelRootDir, m_curModelID.ToString()), "Animations");

            string controllerPath = Path.Combine(animationsDirPath, "RoleController.overrideController");
            AnimatorOverrideController controller = null;
            if (File.Exists(controllerPath)) {
                controller = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(controllerPath);
            } else {
                string baseControllerPath = Path.Combine(m_modelRootDirBase, "Common/Animations/RoleBaseController.controller");
                RuntimeAnimatorController baseController = AssetDatabase.LoadAssetAtPath<AnimatorController>(baseControllerPath);
                if (baseController == null) {
                    Debug.LogError("baseControllerPath : " + baseControllerPath + " 不存在");
                    return;
                }

                controller = new AnimatorOverrideController(baseController);
                AssetDatabase.CreateAsset(controller, controllerPath);
            }

            AnimationClip[] orignalClips = controller.runtimeAnimatorController.animationClips;
            List<KeyValuePair<AnimationClip, AnimationClip>> overrideClips = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (AnimationClip orignalClip in orignalClips)
            {
                // Debug.Log("Debug.Log : 原动画:" + orignalClip);
                
                string orignalName = orignalClip.name;
                string overrideClipName = orignalName.Replace("Zero", "");
                Debug.Log("Debug.Log : 新动画:" + overrideClipName);
                string overrideClipPath = Path.Combine(animationsDirPath, overrideClipName + ".anim");
                Debug.Log("Debug.Log : 新动画路径:" + overrideClipPath);
                if (! File.Exists(overrideClipPath)) {
                    continue;
                }

                AnimationClip overrideClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(overrideClipPath);
                if (overrideClip == null) {
                    continue;
                }

                overrideClips.Add(new KeyValuePair<AnimationClip, AnimationClip>(orignalClip, overrideClip));
            }
            controller.ApplyOverrides(overrideClips);
        }

         class PairComparer : IComparer<KeyValuePair<int, string>> {
            public int Compare(KeyValuePair<int, string> a, KeyValuePair<int, string> b) {
                if (a.Key == b.Key) {
                    return 0;
                } else if (a.Key < b.Key) {
                    return -1;
                } else {
                    return 1;
                }
            }
        }
    }
}