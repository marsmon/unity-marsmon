using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YF.Art
{
    public enum AnimDir
    {
        up = 0,
        right = 2,
        down = 4,
        left = 6
    }
    public enum AnimKey
    {
        idle,
        run
    }


    public class RoleManage : MonoBehaviour
    {
        public float RoleSize = 1f;
        public AnimKey key = AnimKey.idle;
        public AnimDir dir = AnimDir.right;
        public float speed = 5;

        static readonly Vector3 RoleScale = new Vector3(1f, 1.331087f, 1f);
        Animator animator;
        SpriteRenderer spriteRenderer;
        string m_animtorKey;
        // Start is called before the first frame update
        void Start()
        {
            this.animator = this.GetComponent<Animator>();
            this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        }
        /// <summary>
        /// Called when the script is loaded or a value is changed in the
        /// inspector (Called in the editor only).
        /// </summary>
        void OnValidate()
        {
            this.transform.localScale = RoleScale * this.RoleSize;
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (this.animator == null || this.spriteRenderer == null)
            {
                this.animator = this.GetComponent<Animator>();
                this.spriteRenderer = this.GetComponent<SpriteRenderer>();
            }
#endif
            this.transform.localScale = RoleScale * this.RoleSize;
            if (this.animator == null || this.spriteRenderer == null)
            {
                Debug.LogError("缺少组件");
                return;
            }
            inputKey();
            PlayAnimUpdate();
        }
        int mysign(float v)
        { 
            if (v > 0)
            {
                return 1;
            }
             if (v < 0)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        static Vector3 m_dir = Vector3.zero;
        void inputKey()
        {
            bool ish = false;
            bool isv = false;
            if(Input.GetKey(KeyCode.D)){
                this.key =  AnimKey.run;
                this.dir =  AnimDir.right;
                m_dir.x = 1;
                ish = true;
            }
            else if(Input.GetKey(KeyCode.A)){                
                this.key =  AnimKey.run;
                this.dir =  AnimDir.left;                
                m_dir.x = -1;                
                ish = true;
            }            
            if(Input.GetKey(KeyCode.W)){                
                this.key =  AnimKey.run;                
                this.dir = ish ? this.dir : AnimDir.up;                                
                m_dir.z = 1;
                isv = true;
            }
            else if(Input.GetKey(KeyCode.S)){                
                this.key =  AnimKey.run;
                this.dir = ish ? this.dir : AnimDir.down;
                m_dir.z = -1;
                isv = true;
            }
            if(!ish && !isv){
                this.key =  AnimKey.idle;
                m_dir = Vector3.zero;
                
            }
            m_dir = m_dir.normalized;
            this.transform.position += m_dir * speed * Time.deltaTime;

        }

        void PlayAnimUpdate()
        {
            string _animtorKey = this.key.ToString() + "_" + ((int)this.dir).ToString();
            if (this.dir == AnimDir.left)
            {
                _animtorKey = this.key.ToString() + "_2";
                this.spriteRenderer.flipX = true;
            }
            else
            {
                this.spriteRenderer.flipX = false;
            }
            if (m_animtorKey != _animtorKey){
                m_animtorKey = _animtorKey;
                this.animator.Play(m_animtorKey);
            }
        }
    }
}
