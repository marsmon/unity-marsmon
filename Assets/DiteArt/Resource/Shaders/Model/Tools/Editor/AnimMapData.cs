// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// using UnityEngine.Animations;
// using DiteScripts.YF.Editor;
using UnityEngine.Rendering;

namespace DiteScripts.YF.AnimMap.Editor
{

    //==============================================================================================================
    // public
    //==============================================================================================================

    public partial class AnimMapData : Object
    {
        string outdata = "";
        public int FrameRate = 30;
        public Transform RootBone;
        public bool Ismax = true;

        // /// <summary>
        // /// 结构：[BoneIndex,[clipname, [frame, animkey_value]]]
        // /// </summary>
        // /// <returns></returns>
        public Dictionary<int, Dictionary<AnimationClip, Matrix4x4[]>> BonesData = new Dictionary<int, Dictionary<AnimationClip, Matrix4x4[]>>();

        public List<BoneWeight> MeshWeightData = new List<BoneWeight>();

        // bool isInit = false;
        GameObject baseObj;
        List<AnimationClip> clips = new List<AnimationClip>();
        Matrix4x4[] BaseMatrix4X4s;
        Transform[] Bones;
        Transform srrTran;
        public Texture2D ExportTexture;

        public Mesh SrcMesh
        {
            get; private set;
        }
        public Mesh exportMesh
        {
            get; private set;
        }

        public AnimMapData() { }

        public void LoadData(GameObject go, int framerate = 0)
        {
            if (go == null) { return; }
            this.baseObj = go;
            this.RootBone = this.baseObj.transform;
            Animator ator = this.baseObj.GetComponentInChildren<Animator>() as Animator;
            SkinnedMeshRenderer smr = this.baseObj.GetComponentInChildren<SkinnedMeshRenderer>() as SkinnedMeshRenderer;
            if (ator == null || smr == null) { return; }
            this.GetAnimClip(ator);
            if (clips.Count < 1) { return; }
            // Debug.Log(clips);

            SrcMesh = smr.sharedMesh;
            srrTran = smr.transform;
            this.MeshWeightData = new List<BoneWeight>(smr.sharedMesh.boneWeights);
            // this.GetBoneWeight(smr.sharedMesh);
            this.BaseMatrix4X4s = SrcMesh.bindposes;
            this.Bones = smr.bones;

            this.SetFrameRate(framerate);
            this.getAnimData();

            CteatMesh();
            CreatMap();
        }


        public void CreatMap()
        {
            int off = 0;
            int h = this.Bones.Length * 4;
            int w = 0;
            foreach (var dic in this.clips)
            {
                w = w + BonesData[0][dic].Length + off;
            }

            this.ExportTexture = new Texture2D(w, h, TextureFormat.RGBAFloat, false, false);
            this.ExportTexture.wrapMode = TextureWrapMode.Clamp;
            this.ExportTexture.filterMode = FilterMode.Point;


            // this.BaseMatrix4X4s
            for (int i = 0; i < this.BaseMatrix4X4s.Length; i++)
            {
                for (int ww = 0; ww < w; ww++)
                {
                    Matrix4x4 matrix = this.BaseMatrix4X4s[i];

                    this.ExportTexture.SetPixel(ww, i * 4 + 0, new Color(matrix.m00, matrix.m01, matrix.m02, matrix.m03));
                    this.ExportTexture.SetPixel(ww, i * 4 + 1, new Color(matrix.m10, matrix.m11, matrix.m12, matrix.m13));
                    this.ExportTexture.SetPixel(ww, i * 4 + 2, new Color(matrix.m20, matrix.m21, matrix.m22, matrix.m23));
                    this.ExportTexture.SetPixel(ww, i * 4 + 3, new Color(matrix.m30, matrix.m31, matrix.m32, matrix.m33));
                }
            }

            // BonesData


            for (int boneindex = 0; boneindex < this.Bones.Length; boneindex++)
            {
                int _frame = 0;
                outdata = "";
                foreach (AnimationClip c in this.clips)
                {
                    outdata = outdata + c.name + ":  s:" + (_frame == 0 ? _frame.ToString() : (_frame + off).ToString());
                    Matrix4x4[] d = this.BonesData[boneindex][c];
                    int ff = 0;
                    for (int f = 0; f < d.Length; f++)
                    {
                        Matrix4x4 matrix = d[f];
                        ff = _frame + f;
                        this.ExportTexture.SetPixel(ff, boneindex * 4 + 0, new Color(matrix.m00, matrix.m01, matrix.m02, matrix.m03));
                        this.ExportTexture.SetPixel(ff, boneindex * 4 + 1, new Color(matrix.m10, matrix.m11, matrix.m12, matrix.m13));
                        this.ExportTexture.SetPixel(ff, boneindex * 4 + 2, new Color(matrix.m20, matrix.m21, matrix.m22, matrix.m23));
                        this.ExportTexture.SetPixel(ff, boneindex * 4 + 3, new Color(matrix.m30, matrix.m31, matrix.m32, matrix.m33));
                        // this.Exporttexture.SetPixel(ff, boneindex * 4 + 3, Color.black);
                    }
                    outdata = outdata + "  e:" + ff.ToString() + "\n";
                    _frame += d.Length + off;
                }
            }
            Debug.Log(outdata);
            this.ExportTexture.Apply();
        }
        public void CteatMesh()
        {
            exportMesh = SrcMesh.AnimMapCopyto(this.MeshWeightData, "Tmp AM Mesh", 0);
        }






        public void SetFrameRate(int framerate)
        {
            if (framerate != FrameRate && framerate > 0)
            {
                this.FrameRate = framerate;
            }
        }


        private void getAnimData()
        {
            this.BonesData = new Dictionary<int, Dictionary<AnimationClip, Matrix4x4[]>>();
            this.BonesData.Clear();
            for (int i = 0; i < this.Bones.Length; i++)
            {
                Dictionary<AnimationClip, Matrix4x4[]> clipdata = new Dictionary<AnimationClip, Matrix4x4[]>();
                foreach (AnimationClip key in this.clips)
                {
                    int frameCount = Mathf.FloorToInt((int)(key.length * Mathf.Max(this.FrameRate, 1)));
                    float perFrameTime = key.length / (frameCount - 1);
                    // float sampleTime = 0f;
                    Matrix4x4[] animdata = new Matrix4x4[frameCount];
                    for (int frame = 0; frame < frameCount; frame++)
                    {
                        key.SampleAnimation(this.baseObj, frame * perFrameTime);
                        Matrix4x4 matrix0 = this.BaseMatrix4X4s[i];//  *  Matrix4x4.Rotate(Quaternion.Euler(-90,0,0)) Matrix4x4.TRS(srrTran.localPosition,srrTran.localRotation,srrTran.localScale) ;

                        Matrix4x4 matrix = this.getNowFrameMatrix(this.Bones[i], matrix0);// this.BaseMatrix4X4s[i]);
                        animdata[frame] = matrix;
                    }
                    clipdata.Add(key, animdata);
                }
                this.BonesData.Add(i, clipdata);
            }
        }

        public Matrix4x4 getNowFrameMatrix(Transform currentBone, Matrix4x4 lastmat)
        {
            Matrix4x4 mat = Matrix4x4.TRS(currentBone.transform.localPosition, currentBone.transform.localRotation, currentBone.transform.localScale);
            Transform parent = currentBone.parent;

            if (parent.Equals(this.RootBone))
            {

                if (this.Ismax)
                {
                    mat = Matrix4x4.Rotate(Quaternion.Euler(90.0f, 0.0f, 0.0f)) * mat;
                }
                else
                {
                    mat = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, this.RootBone.transform.localScale) * mat;
                }
                lastmat = mat * lastmat;
                return lastmat;
            }
            lastmat = mat * lastmat;
            lastmat = this.getNowFrameMatrix(parent, lastmat);
            return lastmat;
        }

        private void GetAnimClip(Animator ator)
        {
            RuntimeAnimatorController ac = ator.runtimeAnimatorController;
            // Debug.Log(ac);
            this.clips = new List<AnimationClip>();
            if (ac == null)
            {
                Debug.LogError("丢失状态机！");
                clips.Clear();
                return;
            }
            if (ac.GetType() == typeof(AnimatorOverrideController))
            {
                clips.Clear();
                AnimatorOverrideController aoc = (AnimatorOverrideController)(ac);
                List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(aoc.overridesCount);
                aoc.GetOverrides(overrides);
                for (int i = 0; i < overrides.Count; ++i)
                {
                    if (overrides[i].Value != null)
                        clips.Add(overrides[i].Value);
                }
            }
            else
            {
                clips = new List<AnimationClip>(ac.animationClips);
            }
        }

        int selectBone = 0;
        int selectClip = 0;
        Vector2 srcpos;

        public void viewGUI()
        {

            if (this.baseObj == null) return;

            srcpos = EditorGUILayout.BeginScrollView(srcpos,GUILayout.Height(150));
            EditorGUIUtility.labelWidth = 100;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.ObjectField("Root:", this.RootBone, typeof(GameObject), false);
            EditorGUILayout.ObjectField("export mesh:", exportMesh, typeof(Mesh), false);
            EditorGUILayout.ObjectField("export tex:", this.ExportTexture, typeof(UnityEngine.Object), false);
            EditorGUILayout.FloatField("Frame Rate:", this.FrameRate);
            List<string> clipname = new List<string>();
            List<string> bonename = new List<string>();
            // Dictionary<string, Dictionary<int, Matrix4x4[]>> BonesData
            int allFrame = 0;
            foreach (var bn in this.Bones)
            {
                bonename.Add(bn.name);
            }
            foreach (var dic in this.clips)
            {
                clipname.Add(dic.name);
                allFrame = allFrame + BonesData[0][dic].Length;
            }


            selectBone = EditorGUILayout.Popup("Bone:", selectBone, bonename.ToArray());

            EditorGUILayout.BeginHorizontal();
            selectClip = EditorGUILayout.Popup("clip:", selectClip, clipname.ToArray());
            EditorGUILayout.IntField(BonesData[0][clips[selectClip]].Length, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Anim Clip Time Data:");
            EditorGUILayout.TextArea(this.outdata);
            
            // EditorGUILayout.BeginHorizontal();
            // int bc = this.Bones.Length;
            // EditorGUILayout.IntField("总骨骼:", bc);
            // EditorGUILayout.LabelField("X3=" + bc * 3, GUILayout.Width(50), GUILayout.ExpandWidth(false));
            // EditorGUILayout.IntField(Mathf.NextPowerOfTwo(Mathf.Max(bc * 3, 1)), GUILayout.Width(50));
            // EditorGUILayout.EndHorizontal();

            // EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.IntField("总帧数:", allFrame);
            // EditorGUILayout.IntField(Mathf.NextPowerOfTwo(Mathf.Max(allFrame, 1)), GUILayout.Width(50));
            // EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }


    public static class AnimMapDiteUtilExtend
    {
        public static Mesh AnimMapCopyto(this Mesh src, List<BoneWeight> bw, string name = "", float Rotx = 0)
        {
            if (src == null) { return null; }
            Mesh m_mesh = new Mesh();
            bool istowTex = false;

            m_mesh.name = name;
            VertexAttributeDescriptor[] vertexAttributeDescriptorList = new[]{
            new VertexAttributeDescriptor(VertexAttribute.Position,VertexAttributeFormat.Float32,3),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0,VertexAttributeFormat.Float32,2)
            };
            List<float> verticesAttributeBuffer = new List<float>();
            int vertexCount = 0;
            vertexCount += src.vertexCount;
            List<int> indexstart = new List<int>();
            float uvoffset = 1;
            if (src.subMeshCount > 1)
            {
                istowTex = true;
                uvoffset = 1f / src.subMeshCount;
                for (int i = 0; i < src.subMeshCount; i++)
                {
                    indexstart.Add(src.GetSubMesh(i).firstVertex);
                }
                indexstart.Add((src.vertexCount + 1));
            }

            int jishu = 1;
            for (int i = 0; i < src.vertexCount; i++)
            {

                Vector3 pos = Matrix4x4.Rotate(Quaternion.Euler(Rotx, 0.0f, 0.0f)).MultiplyPoint3x4(src.vertices[i]);
                // Vector3 nor = src.normals[i];
                Vector2 uv = src.uv[i];
                if (istowTex)
                {
                    if (i >= indexstart[jishu])
                    {
                        jishu += 1;
                    }
                    uv.x = uv.x * uvoffset + (float)(jishu - 1f) * uvoffset;
                }

                verticesAttributeBuffer.Add(pos.x);
                verticesAttributeBuffer.Add(pos.y);
                verticesAttributeBuffer.Add(pos.z);

                verticesAttributeBuffer.Add(uv.x);
                verticesAttributeBuffer.Add(uv.y);
            }

            int[] triangles = src.triangles;
            int verticesAttributeBufferLength = vertexCount * (3 + 2);
            int indexCount = triangles.Length;


            m_mesh.SetVertexBufferParams(vertexCount, vertexAttributeDescriptorList);
            m_mesh.SetVertexBufferData(verticesAttributeBuffer.ToArray(), 0, 0, verticesAttributeBufferLength, 0);

            //顶点索引文件
            m_mesh.SetIndexBufferParams(indexCount, IndexFormat.UInt32);
            m_mesh.SetIndexBufferData(triangles, 0, 0, indexCount);

            //子Mesh描述
            m_mesh.subMeshCount = 1;
            SubMeshDescriptor subMeshDescriptor = new SubMeshDescriptor(0, indexCount);
            m_mesh.SetSubMesh(0, subMeshDescriptor);
            m_mesh.RecalculateNormals();
            m_mesh.RecalculateBounds();


            List<Vector4> vid = new List<Vector4>();
            List<Vector4> vwe = new List<Vector4>();

            foreach (var dd in bw)
            {
                vid.Add(new Vector4(dd.boneIndex0 + dd.weight0 * 0.9f,
                                    dd.boneIndex1 + dd.weight1 * 0.9f,
                                    dd.boneIndex2 + dd.weight2 * 0.9f,
                                    dd.boneIndex3 + dd.weight3 * 0.9f
                                    ));
                vwe.Add(new Vector4(dd.weight0, dd.weight1, dd.weight2, dd.weight3));
            }
            m_mesh.SetUVs(1, vid);
            // m_mesh.SetUVs(2, vwe);
            // m_mesh.RecalculateTangents();
            m_mesh.UploadMeshData(true);//关闭可读写
            return m_mesh;
        }
    }
}