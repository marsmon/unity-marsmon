using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Spine.Unity;

namespace YF.Art
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SkillToolsRoot_Target : SkillToolsRoot
    {
        [HideInInspector] public Sprite tagsprite;
        SpriteRenderer sr;
        Matrix4x4 TagMatrix = Matrix4x4.identity;
        [HideInInspector] [SerializeField] float Far = 0;


        // protected override void setRole(){

        // }
        public override void setDirIndex(int index, float Far = 0)
        {
            this.dirindex = index;
            if (sr != null)
            {
                sr.flipX = index > 3;
            }
            this.Far = Far * 0.01f;
            TagMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, this.dirData[this.dirindex], 0)) * Matrix4x4.Translate(Vector3.forward * Far * 0.01f);
        }

        protected override void setDir()
        {
            this.transform.position = TagMatrix.MultiplyPoint3x4(Vector3.zero);
            this.transform.rotation = Quaternion.identity;
        }
        // void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawCube(this.transform.position + Vector3.up * 0.5f,new Vector3(0.5f,1f,0.5f));
        //     // Gizmos.DrawLine(this.transform.position - new Vector3(0.5f,0,0),this.transform.position + new Vector3(0.5f,0,0));
        //     // Gizmos.DrawLine(this.transform.position - new Vector3(0,0,0.5f),this.transform.position + new Vector3(0,0,0.5f));

        // }
        private void Awake()
        {
            _init();
        }
        private void OnEnable()
        {
            _init();
        }
        private void _init()
        {
            if (tagsprite == null) return;

            if (!this.gameObject.TryGetComponent<SpriteRenderer>(out sr))
            {
                sr = this.gameObject.AddComponent<SpriteRenderer>();
            }
            sr.sprite = tagsprite;
            sr.hideFlags = HideFlags.HideInInspector;
            sr.sharedMaterial.hideFlags = HideFlags.HideInInspector;
        }

#if UNITY_EDITOR  
        private void LateUpdate()
        {
            if (!CheckError())
            {
                setRoleDir(Color.red);
            }
            else
            {
                setRoleDir(Color.white);
                return;
            }
        }
        bool CheckError()
        {
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget != UnityEditor.BuildTarget.Android) return false;
            if (UnityEditor.PlayerSettings.colorSpace != ColorSpace.Linear) return false;
            var tt = UnityEditor.Rendering.EditorGraphicsSettings.GetTierSettings(UnityEditor.BuildTargetGroup.Android, Graphics.activeTier);
            if (!tt.hdr) return false;
            return true;

        }
        void setRoleDir(Color c)
        {
            if (sr == null)
            {
                sr = this.gameObject.GetComponent<SpriteRenderer>();
            }
            sr.color = c;
        }

#endif

    }
}