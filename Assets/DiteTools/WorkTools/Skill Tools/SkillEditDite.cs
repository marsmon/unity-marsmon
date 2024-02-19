using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YF.Art
{
    public enum SKILL_EDIT_MOD
    {
        LINE = 1,
        RANGE = 2,
        SECTOR = 3,
        CUSTOM = 0,
    }
    public enum SceneMode
    {
        DEFAULT,
        ZMXS,
    }
    public enum SKILL_MODE
    {
        SELF,
        TAG
    }



    [ExecuteInEditMode]
    // [SelectionBase]
    public class SkillEditDite : MonoBehaviour
    {
        // [HideInInspector]
        public bool IsDebug;
        [SerializeField]
        [Tooltip("说明:\n  LINE:线性技能\n  RANGE:范围型技能\n  SECTOR:扇形技能\n L:大 M:中 S:小\n C:暂时无效")]
        [HideInInspector] public SKILL_EDIT_MOD showMode = SKILL_EDIT_MOD.LINE;
        [SerializeField]
        [HideInInspector] public float distance1 = 600f;
        [HideInInspector] public float width = 200f;
        [HideInInspector] public float angle = 120f;
        [HideInInspector] public Color color = new Color(0.5f, 0, 0.115f, 0.3f);
        [HideInInspector] public SKILL_MODE AttackMode = SKILL_MODE.SELF;

        [HideInInspector]
        public Transform TagMode0;
        [HideInInspector]
        public Transform SelfMode0;
        [HideInInspector] public float TagFar = 200f;
        [HideInInspector]
        public GameObject Node;
        [HideInInspector]
        public GameObject SrcPre;
        [HideInInspector]
        public GameObject bg0;
        string basePath = "Assets";
        static Mesh mesh;

        readonly float[] dirData = new float[8] { 0, 45, 90, 135, 180, 225, 270, 315 };

        public RoleManage role;

        void roleInit()
        {
            role = GameObject.FindObjectOfType<RoleManage>();
        }
        protected virtual void setRole()
        {
            if (Application.isPlaying)
            {
                if (this.role != null)
                {
                    this.role.speed = 6;
                    this.role.RoleSize = 0.55f;
                    this.SelfMode0.position = this.role.transform.position;
                }
                else
                {
#if UNITY_EDITOR
                    roleInit();
#endif
                }
            }
            // this.transform.position = this.role.transform.position;

        }

        private void m_Reset()
        {

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
            getBasePath();
            Transform[] gs = this.GetComponentsInChildren<Transform>();
            foreach (Transform item in gs)
            {
                if (item == this.transform || item.GetComponent<Camera>() != null)
                {
                    continue;
                }
                item.hideFlags = HideFlags.None;// HideFlags.HideInHierarchy;
            }
            this.gameObject.name = "Skill Edit Root";
            this.CreatSelfRoot();
            _Update();
            BoxCollider b;
            if (!this.gameObject.TryGetComponent<BoxCollider>(out b))
            {
                b = this.gameObject.AddComponent<BoxCollider>();
            }
            b.center = new Vector3(0, 1, 0);
            b.size = new Vector3(1, 2, 1);
        }
        void OnDisable()
        {
            Transform[] gs = this.GetComponentsInChildren<Transform>();
            foreach (Transform item in gs)
            {
                item.hideFlags = HideFlags.None;
            }

        }
        private void OnHierarchyChange()
        {
            if (this.Node != null)
                this.Node.hideFlags = IsDebug ? HideFlags.None : HideFlags.HideInHierarchy;
            if (this.bg0 != null)
                this.bg0.hideFlags = IsDebug ? HideFlags.None : HideFlags.HideInHierarchy;
        }
        void OnValidate()
        {
            if (this.Node != null)
                this.Node.hideFlags = IsDebug ? HideFlags.None : HideFlags.HideInHierarchy;
            if (this.bg0 != null)
                this.bg0.hideFlags = IsDebug ? HideFlags.None : HideFlags.HideInHierarchy;
        }

        private void OnDestroy()
        {
            Transform[] gs = this.GetComponentsInChildren<Transform>();
            foreach (Transform item in gs)
            {
                item.hideFlags = HideFlags.None;
            }

            if (this.Node != null)
            {
                this.Node.hideFlags = HideFlags.None;
#if UNITY_EDITOR
                DestroyImmediate(this.Node.gameObject);
#else
                Destroy(this.Node.gameObject);
#endif
            }
        }

        bool CheckError()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget != UnityEditor.BuildTarget.Android) return false;
            if (UnityEditor.PlayerSettings.colorSpace != ColorSpace.Linear) return false;
            var tt = UnityEditor.Rendering.EditorGraphicsSettings.GetTierSettings(UnityEditor.BuildTargetGroup.Android, Graphics.activeTier);
            if (!tt.hdr) return false;

#endif
            return true;
        }
        private void Update()
        {
            this._Update();
        }

        private void _Update()
        {
            if (this.transform.localToWorldMatrix != Matrix4x4.identity)
            {
                this.transform.position = Vector3.zero;
                this.transform.rotation = Quaternion.identity;
                this.transform.localScale = Vector3.one;
            }
            // this.transform.localScale = new Vector3 (1f,1.331087f,1f); 
            this.setMode();
            if (this.SelfMode0 != null)
            {
                this.SelfMode0.position = this.transform.position;
                this.setRole();
            }

            this.CheckMeshNode();
            this.CreatSelfRoot();

        }

        private void OnGUI()
        {
            Vector2 pos = new Vector2(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.5f);
            GUIStyle gs = new GUIStyle();
            gs.fontSize = 30;
            gs.fontStyle = FontStyle.Bold;
            gs.alignment = TextAnchor.MiddleCenter;
            gs.normal.textColor = CheckError() ? Color.red * 0.5f : Color.red;

            GUI.Label(new Rect(pos.x, pos.y - 2, 0, 0), "●----●", gs); //●
        }

        public void CreatSelfRoot()
        {
            if (this.SelfMode0 == null)
            {
                GameObject g = new GameObject();
                g.name = "Self_Root";
                g.AddComponent<SkillToolsRoot>();
                g.transform.position = this.transform.position;
                g.transform.rotation = Quaternion.identity;
                this.SelfMode0 = g.transform;
            }
            if (this.TagMode0 == null)
            {
                GameObject g = new GameObject();
                g.name = "Tag_Root";
                g.AddComponent<SkillToolsRoot_Target>();
                g.transform.position = Vector3.zero;
                g.transform.rotation = Quaternion.identity;
                g.SetActive(false);
                g.SetActive(true);
                this.TagMode0 = g.transform;
                this.changeDir(2);
            }

        }
        public void setMode()
        {
            switch (this.AttackMode)
            {
                case SKILL_MODE.SELF:
                    if (this.Node != null)
                        this.Node.transform.position = this.transform.position;
                    break;
                case SKILL_MODE.TAG:
                    if (TagMode0 != null)
                    {
                        this.Node.transform.position = TagMode0.position;
                    }
                    else
                    {
                        this.Node.transform.position = this.transform.position;
                    }
                    break;
                    // case SKILL_MODE.FLY_I:

                    //     break;
            }
        }

        public void changeDir(int dir)
        {
            UnityEngine.Object[] lg = Resources.FindObjectsOfTypeAll(typeof(SkillToolsRoot));
            foreach (SkillToolsRoot g in lg)
            {
                switch (this.AttackMode)
                {
                    case SKILL_MODE.SELF:
                        g.setDirIndex(dir, distance1);
                        break;
                    case SKILL_MODE.TAG:
                        g.setDirIndex(dir, TagFar);
                        break;
                        // case SKILL_MODE.FLY_I:
                        //     g.setDirIndex(dir, distance1);
                        //     break;
                }
                g._Update();
            }
        }

        public void CheckMeshNode()
        {
            if (this.SrcPre != null)
            {
                if (this.bg0 == null)
                {
                    string armname = "Arm_ljlksajlfjhfkss0231aaa";
                    Transform n = this.transform.Find(armname);
                    if (n == null)
                    {
                        this.bg0 = GameObject.Instantiate(this.SrcPre);
                        this.bg0.transform.name = armname;
                        this.bg0.transform.parent = this.transform;
                        this.bg0.transform.position = Vector3.zero;
                        this.bg0.transform.localEulerAngles = Vector3.zero;
                        this.bg0.hideFlags = IsDebug ? HideFlags.None : HideFlags.HideInHierarchy;
                    }
                    else
                    {
                        this.bg0 = n.gameObject;
                    }
                }
            }

            if (this.Node == null)
            {
                string armname = "Arm_ljlksajlfjhfkss0231";
                Transform n = this.transform.Find(armname);
                if (n == null)
                {
                    GameObject g = new GameObject(armname);
                    g.hideFlags = HideFlags.HideInHierarchy;
                    g.transform.parent = this.transform;
                    g.transform.localEulerAngles = Vector3.zero;
                    g.tag = "EditorOnly";
                    this.Node = g;
                }
                else
                {
                    this.Node = n.gameObject;
                }
            }

            this.Node.hideFlags = IsDebug ? HideFlags.None : HideFlags.HideInHierarchy;
            this.Node.tag = IsDebug ? "Untagged" : "EditorOnly";
            if (this.bg0 != null)
            {
                this.bg0.hideFlags = IsDebug ? HideFlags.None : HideFlags.HideInHierarchy;
                this.bg0.tag = IsDebug ? "Untagged" : "EditorOnly";
                for (int i = 0; i < this.bg0.transform.childCount; i++)
                {
                    this.bg0.transform.GetChild(i).tag = IsDebug ? "Untagged" : "EditorOnly";
                }

            }



            MeshRenderer mr;
            if (!this.Node.gameObject.TryGetComponent<MeshRenderer>(out mr))
            {
                mr = this.Node.AddComponent<MeshRenderer>();
            }
            if (mr.sharedMaterial == null)
                mr.sharedMaterial = new Material(Shader.Find("Unlit/SkillEditDiteShader"));

            MeshFilter mf;
            if (!this.Node.gameObject.TryGetComponent<MeshFilter>(out mf))
            {
                mf = this.Node.AddComponent<MeshFilter>();
            }
            SkillToolsRoot srt;
            if (!this.Node.gameObject.TryGetComponent<SkillToolsRoot>(out srt))
            {
                this.Node.AddComponent<SkillToolsRoot>();
            }
            updateWorld();
            mf.sharedMesh = SkillEditDite.mesh;
            mr.sharedMaterial.SetColor("_Color", this.color);
        }


        void getBasePath()
        {
#if UNITY_EDITOR
            SkillEditDite o = UnityEditor.AssetDatabase.LoadAssetAtPath<SkillEditDite>(basePath + "\\SkillEditDite.cs") as SkillEditDite;
            if (o != null)
            {
                return;
            }
#endif
        }

        void OnDrawGizmos()
        {
            // Gizmos.color = Color.green * 0.25f;
            // float s = 0.25f;
            // Gizmos.DrawWireSphere(this.transform.position + Vector3.up * 0.5f,1f); DrawWireCube
            // Gizmos.DrawCube(this.transform.position - new Vector3(0,-0.5f,0),new Vector3(30,1,30));


        }

        public void updateWorld()
        {
            switch (showMode)
            {
                case SKILL_EDIT_MOD.CUSTOM:
                    SkillEditDite.mesh = CreateMesh((this.width * 0.01f) / 2.0f, this.distance1 * 0.01f);
                    break;
                case SKILL_EDIT_MOD.LINE:
                    SkillEditDite.mesh = CreateMesh((this.width * 0.01f) / 2.0f, this.distance1 * 0.01f);
                    break;
                case SKILL_EDIT_MOD.RANGE:
                    SkillEditDite.mesh = CreateMesh(this.distance1 * 0.01f, 360.0f, this.distance1 * 0.01f);
                    break;
                case SKILL_EDIT_MOD.SECTOR:
                    SkillEditDite.mesh = CreateMesh(this.distance1 * 0.01f, Mathf.Max(0.0001f, this.angle), 5f);
                    break;
                default:
                    break;
            }
        }


        public static Mesh CreateMesh(float w, float h)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Sector";
            Vector3[] vertices = new Vector3[4];

            vertices[0] = new Vector3(w, 0f, 0f);
            vertices[1] = new Vector3(-w, 0f, 0f);
            vertices[2] = new Vector3(w, 0f, h);
            vertices[3] = new Vector3(-w, 0f, h);

            int[] triangles = new int[6] { 0, 1, 2, 1, 3, 2 };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            // mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }

        public static Mesh CreateMesh(float radius, float angle, float cellAngle)
        {
            int segments = Mathf.FloorToInt(angle / cellAngle);
            // 完善分段
            float deltaAngle = angle / Mathf.Floor(angle / cellAngle);
            // UnityEngine.Debug.LogFormat("CreateMesh radius:{0} angle:{1} cellAngle:{2} segments:{3} deltaAngle:{4}", radius, angle, cellAngle, segments, deltaAngle);

            Mesh mesh = new Mesh();
            mesh.name = "Sector";
            Vector3[] vertices = new Vector3[3 + segments - 1];
            vertices[0] = new Vector3(0f, 0f, 0f);

            // Vector2[] uvs = new Vector2[vertices.Length];
            // uvs[0] = new Vector2(0f, 0f);

            float currentAngle = 90f + angle / 2f;
            for (int i = 1; i < vertices.Length; i++)
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * radius;
                float z = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * radius;
                vertices[i] = new Vector3(x, 0f, z);
                // uvs[i] = new Vector2(0f, 0f);
                currentAngle -= deltaAngle;
            }

            int[] triangles = new int[segments * 3];
            for (int i = 0, vi = 1; i < triangles.Length; i += 3, vi++)
            {
                triangles[i] = 0;
                triangles[i + 1] = vi;
                triangles[i + 2] = vi + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            // mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }


        protected virtual Quaternion setDir(int index)
        {
            return Quaternion.Euler(0, this.dirData[index], 0);
        }
    }
}


// switch (this.xiangmu)
// {
//     case YFXIANGMU.ZMXS:

//         bloomdata = (BloomData)Resources.Load(basePath + "/掌门下山Bloom") as BloomData;
//         if (bloomdata != null) bloomdata.setValue();
//         break;

//     default:
//         bloomdata = (BloomData)Resources.Load(basePath + "/BloomDataDef") as BloomData;
//         if (bloomdata != null) bloomdata.setValue();
//         break;
// }



// // case SKILL_EDIT_MOD.LINE_C: 
// //     SkillEdit.mesh = CreateMesh(this.width/ 2,  this.distance1);                
// //     break;
// case SKILL_EDIT_MOD.LINE_L:
//     this.distance1 = 8f;
//     // this.distance2 = 7.75f;
//     this.width = 2f;
//     SkillEditDite.mesh = CreateMesh(this.width / 2, this.distance1);
//     break;
// case SKILL_EDIT_MOD.LINE_M:
//     this.distance1 = 5f;
//     // this.distance2 = 4.75f;
//     this.width = 2f;
//     SkillEditDite.mesh = CreateMesh(this.width / 2, this.distance1);
//     break;
// case SKILL_EDIT_MOD.LINE_S:
//     this.distance1 = 1.2f;
//     // this.distance2 = 1.2f - 0.25f;
//     this.width = 1.5f;
//     SkillEditDite.mesh = CreateMesh(this.width / 2, this.distance1);
//     break;

// // case SKILL_EDIT_MOD.RANGE_C: 
// //     SkillEdit.mesh = CreateMesh(this.radius, 360, this.radius);
// //     break;
// case SKILL_EDIT_MOD.RANGE_L:
//     this.radius = 3f;
//     SkillEditDite.mesh = CreateMesh(this.radius, 360, this.radius);
//     break;
// case SKILL_EDIT_MOD.RANGE_M:
//     this.radius = 2f;
//     SkillEditDite.mesh = CreateMesh(this.radius, 360, this.radius);
//     break;
// case SKILL_EDIT_MOD.RANGE_S:
//     this.radius = 1.3f;
//     SkillEditDite.mesh = CreateMesh(this.radius, 360, this.radius);
//     break;
// case SKILL_EDIT_MOD.SECTOR_L:
//     this.radius = 3f;
//     this.angle = 135f;
//     SkillEditDite.mesh = CreateMesh(this.radius, this.angle, 5f);
//     break;
// case SKILL_EDIT_MOD.SECTOR_M:
//     this.radius = 2f;
//     this.angle = 120f;
//     SkillEditDite.mesh = CreateMesh(this.radius, this.angle, 5f);
//     break;
// case SKILL_EDIT_MOD.SECTOR_S:
//     this.radius = 1.3f;
//     this.angle = 90f;
//     SkillEditDite.mesh = CreateMesh(this.radius, this.angle, 5f);
//     break;