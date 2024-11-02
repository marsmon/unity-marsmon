using UnityEngine;

namespace YF {
    
public class PostScreenDissolve : MonoBehaviour
{
    public float DissolveTo = 4f;
    public float DissolveSpeed = 0.4f;
    public float DissolveT = 0.0f;
    public float DissolveFrom = 0.0f;
    public Material DissolveMaterial;

    public bool NeedLoop = false;
    public bool OriginToGray = true;
    
    private void Update(){
        if (DissolveMaterial == null) return;

#if UNITY_EDITOR
        if (OriginToGray) {
            DissolveMaterial.SetInt("GrayToOrigin", 1);
        } else {
            DissolveMaterial.SetInt("GrayToOrigin", 2);
        }
#endif

        DissolveT += Time.deltaTime * DissolveSpeed;
        if (NeedLoop && DissolveT > 1.0f)
        {
            DissolveT = 0.0f;
        }
        
        DissolveMaterial.SetFloat("_Clip", Mathf.Lerp(DissolveFrom, DissolveTo, DissolveT));
    }

#if UNITY_EDITOR
    void OnDestroy() {
        if (DissolveMaterial != null) {
            DissolveMaterial.SetInt("GrayToOrigin", 1);
            DissolveMaterial.SetFloat("_Clip", 0);   
        }
    }
#endif

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (DissolveMaterial != null) {
            Graphics.Blit(source, destination, DissolveMaterial);
        } else {
            Graphics.Blit(source, destination);
        }
    }
}

}