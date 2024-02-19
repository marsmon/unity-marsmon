using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YF.Dev{
    [ExecuteInEditMode]
    public class LockTransform : MonoBehaviour
    {
        public bool isCanRot;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            this.lockTransform(this.isCanRot);
        }

        private void lockTransform(bool b){
            this.transform.localPosition = Vector3.zero;
            if(b){
                Vector3 rot = this.transform.localEulerAngles;
                rot.x = 0;
                rot.z = 0;
                this.transform.localEulerAngles = rot;
            }
            else{
                this.transform.localEulerAngles = Vector3.zero;
            }
            
            this.transform.localScale = Vector3.one;
            // pos.y = 0.001f;
            // this.transform.position = pos;
        }
    }
}