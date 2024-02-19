
using UnityEngine;

namespace YF.Dev {
    


    [ExecuteAlways]
    public class SkillEnv : MonoBehaviour {

        
        private void Start() {
            
            Shader.SetGlobalFloat("CAMERA_ROATION_X", 1f / Mathf.Cos(Mathf.Asin(0.66f)));
        }
    }
}