using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Spine.Unity;

namespace YF.Art
{
    [ExecuteInEditMode]
    public class SkillToolsRoot : MonoBehaviour
    {
        protected int dirindex = 2;
        protected readonly float[] dirData = new float[8] { 0, 45, 90, 135, 180 + 0, 225, 270, 315 };


        
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            MeshRenderer mr;
            if (this.gameObject.TryGetComponent<MeshRenderer>(out mr))
            {
                mr.sortingOrder = -1;
                mr.sortingLayerName = "Default";
            }

            // this.hideFlags = HideFlags.HideInInspector | HideFlags.DontSave;
        }
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            // roleInit();
        }
        private void OnEnable()
        {
            MeshRenderer mr;
            if (this.gameObject.TryGetComponent<MeshRenderer>(out mr))
            {
                mr.sortingOrder = -1;
                mr.sortingLayerName = "Default";
            }
        }
        void Update()
        {
            _Update();

        }

        public virtual void _Update()
        {
            this.setDir();
        }

        public virtual void setDirIndex(int index, float Far = 0)
        {
            this.dirindex = index;
        }

        protected virtual void setDir()
        {
            this.setDir(new Vector3(0, this.dirData[this.dirindex], 0));
        }
        protected virtual void setDir(Vector3 _dir)
        {
            this.transform.localEulerAngles = _dir;
        }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            float s = 0.25f;
            // Gizmos.DrawWireSphere(this.transform.position + Vector3.up * 0.5f,1f);
            Gizmos.DrawLine(this.transform.position - new Vector3(s, 0, 0), this.transform.position + new Vector3(s, 0, 0));
            Gizmos.DrawLine(this.transform.position - new Vector3(0, 0, s), this.transform.position + new Vector3(0, 0, s));

        }
    }
}