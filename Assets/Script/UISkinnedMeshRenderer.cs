
#if UNITY_2018_3 || UNITY_2019 || UNITY_2018_3_OR_NEWER
#define NEW_PREFAB_SYSTEM
#endif

#if UNITY_2018_2_OR_NEWER
#define HAS_CULL_TRANSPARENT_MESH
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UISkinnedMeshRenderer : MaskableGraphic
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Mesh skinnedMesh;
    private Mesh _bakeMesh;
    
    #region Overrides
    private Texture baseTexture = null;
    // This is used by the UI system to determine what to put in the MaterialPropertyBlock.
    Texture overrideTexture;
    public Texture OverrideTexture {
        get { return overrideTexture; }
        set {
            overrideTexture = value;
            canvasRenderer.SetTexture(this.mainTexture); // Refresh canvasRenderer's texture. Make sure it handles null.
        }
    }
    #endregion
    
    #region Internals
    public override Texture mainTexture {
        get {
            if (overrideTexture != null) return overrideTexture;
            return baseTexture;
        }
    }
    #endregion

    void OnEnable() {
        _bakeMesh = new Mesh();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = skinnedMeshRenderer.sharedMesh;
        this.material = skinnedMeshRenderer.sharedMaterial;
        baseTexture = skinnedMeshRenderer.sharedMaterial.mainTexture;
        SetVerticesDirty();
        SetMaterialDirty();
        Rebuild(CanvasUpdate.PreRender);
    }
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (skinnedMesh == null) return;
        skinnedMeshRenderer.BakeMesh(_bakeMesh);

        // Get data from mesh
        Vector3[] verts = _bakeMesh.vertices;
        Vector2[] uvs = _bakeMesh.uv;
        if (uvs.Length < verts.Length)
            uvs = new Vector2[verts.Length];

        // Get mesh bounds parameters
        Vector2 meshMin = _bakeMesh.bounds.min;
        Vector2 meshSize = _bakeMesh.bounds.size;
        var size = rectTransform.rect.size;
        var pivot = rectTransform.pivot;

        // Add scaled vertices
        for (int ii = 0; ii < verts.Length; ii++)
        {
            Vector3 v = verts[ii];
            v.x = ((v.x - meshMin.x) / meshSize.x - pivot.x) * size.x;
            v.y = ((v.y - meshMin.y) / meshSize.y - pivot.y)*size.y;
            v.z *= size.x;
            vh.AddVert(v, color, uvs[ii]);
        }
        
        // Add triangles
        int[] tris = _bakeMesh.triangles;
        for (int ii = 0; ii < tris.Length; ii += 3)
            vh.AddTriangle(tris[ii], tris[ii + 1], tris[ii + 2]);
    }

    void LateUpdate()
    {
        SetVerticesDirty();
    }

}

