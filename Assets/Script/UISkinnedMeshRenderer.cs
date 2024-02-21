using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkinnedMeshRenderer : MaskableGraphic
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Mesh m_SkinnedMesh;
    public Transform []bones;

    
    public List<Vector2> Uvs = null;
    public List<Vector3> Vertices = null;
    public List<Color> Colors = null;
    public List<int> Triangles = null;
    public List<Matrix4x4> bindposesMatrix4x4 = null;
    public List<BoneWeight> boneWeights = null;
    

    void OnEnable() {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        // m_SkinnedMesh = skinnedMeshRenderer.sharedMesh;
        SetVerticesDirty();
        SetMaterialDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (m_SkinnedMesh == null) return;

        m_SkinnedMesh.GetUVs(0, Uvs);


        m_SkinnedMesh.GetVertices(Vertices);

        m_SkinnedMesh.GetColors(Colors);

        m_SkinnedMesh.GetTriangles(Triangles, 0);

        m_SkinnedMesh.GetBindposes(bindposesMatrix4x4);

        m_SkinnedMesh.GetBoneWeights(boneWeights);

        bones = this.skinnedMeshRenderer.bones;


        //遍历顶点数，LBS蒙皮算法
        for (int i = 0; i < Vertices.Count; i++){

            BoneWeight boneWeight = boneWeights[i];

            Vector3 point = Vertices[i];

            Transform trans0 = bones[boneWeight.boneIndex0];

            Transform trans1 = bones[boneWeight.boneIndex1];

            Transform trans2 = bones[boneWeight.boneIndex2];

            Transform trans3 = bones[boneWeight.boneIndex3];

            Matrix4x4 tempMat0 = trans0.localToWorldMatrix * bindposesMatrix4x4[boneWeight.boneIndex0];

            Matrix4x4 tempMat1 = trans1.localToWorldMatrix * bindposesMatrix4x4[boneWeight.boneIndex1];

            Matrix4x4 tempMat2 = trans2.localToWorldMatrix * bindposesMatrix4x4[boneWeight.boneIndex2];

            Matrix4x4 tempMat3 = trans3.localToWorldMatrix * bindposesMatrix4x4[boneWeight.boneIndex3];

            Vector3 temp = tempMat0.MultiplyPoint(point) * boneWeight.weight0 +

                            tempMat1.MultiplyPoint(point) * boneWeight.weight1 +

                            tempMat2.MultiplyPoint(point) * boneWeight.weight2 +

                            tempMat3.MultiplyPoint(point) * boneWeight.weight3;

            Vertices[i] = this.skinnedMeshRenderer.worldToLocalMatrix.MultiplyPoint(temp);

            vh.AddVert(Vertices[i], Color.white, Uvs[i]);

        }

        //填充三角面
        for (int i = 0; i < Triangles.Count; i++)
        {

            try

            {

                vh.AddTriangle(Triangles[i], Triangles[i + 1], Triangles[i + 2]);

                i += 2;

            }

            catch (System.Exception ex)

            {

                Debug.LogError(ex.Message);

            }

        }

        void Update()
        {

    #if UNITY_EDITOR
            SetVerticesDirty();
    #endif

        }


    }

}
