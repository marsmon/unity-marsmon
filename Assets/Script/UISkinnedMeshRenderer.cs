
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

    public Vector2 _oneOfMeshSize;
    public Vector2 _pivotSizeScale;
    
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
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = skinnedMeshRenderer.sharedMesh;
        material = skinnedMeshRenderer.sharedMaterial;
        baseTexture = material.mainTexture;
        
        SetVerticesDirty();
        SetMaterialDirty();
        Rebuild(CanvasUpdate.PreRender);
    }

    void Initialize() {
        
        Vector2 meshSize = _bakeMesh.bounds.size;
        var size = rectTransform.rect.size;
        var pivot = rectTransform.pivot;

        _oneOfMeshSize.Set(1 / meshSize.x * size.x, 1 / meshSize.y * size.y);
        _pivotSizeScale.Set(pivot.x * size.x , pivot.y * size.y);
    }

    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        if (skinnedMesh == null) return;
        bool needInitialize = false;
        if (_bakeMesh == null)
        {   
            _bakeMesh = new Mesh();
            needInitialize = true;
        }
        skinnedMeshRenderer.BakeMesh(_bakeMesh);

        if (needInitialize)
            Initialize();
        
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
            v.x = (v.x - meshMin.x) * _oneOfMeshSize.x - _pivotSizeScale.x;
            v.y = (v.y - meshMin.y) * _oneOfMeshSize.y - _pivotSizeScale.y;
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

