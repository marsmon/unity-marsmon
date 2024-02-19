using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



namespace DT.FrameWork
{
#if UNITY_EDITOR
    // using UnityEditor;
    [ExecuteAlways]
#endif
    public partial class DTGameScene : MonoBehaviour
    {
        public SpriteRenderer bgimag;
        public Sprite[] bgtex;
        int bgindex = 0;

        public void setbg()
        {
            if (bgimag != null && bgtex.Length > 0)
            {
                bgimag.sprite = bgtex[bgindex % bgtex.Length];
                bgindex++;
            }
        }
        public event Action onUpdate;
        public static DTGameScene Instance { get; private set; }

        public DTRockingBar RockingBar;
        public DTPlayManage play;

        public DTBtn atkbtn;

        [SerializeField] BoxCollider scenesize;
        public Bounds box;
        private void Awake()
        {
            // CreateInstance(); 
            InitAndCheck();
        }
        private void InitAndCheck()
        {
            // Destroy();
            CreateInstance();
            Playisinit = this.play != null;
            RockingBarisinit = this.RockingBar != null;
        }

        private void Destroy()
        {
            Instance = null;
            // RockingBar = null;
        }

        void Update()
        {
            CreateInstance();
            RockingBarManage();
            OnUpdate();
            inputfun();
            box = new Bounds();
            if (this.scenesize != null)
            {
                box = this.scenesize.bounds;
            }
        }



        
        float ffff(float v){
            if(v > 0) return 1f;
            else if(v < 0) return -1f;
            else return 0f;
        }
        bool iskey = true;
        void inputfun()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                play.Attack(1);
            }
            if (iskey)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                if (play)
                    play.Move(new Vector2(ffff(h), ffff(v)));
            }
        }

        private void CreateInstance()
        {
            if (DTGameScene.Instance == null)
            {
                DTGameScene.Instance = this;
            }
        }
        bool Playisinit = false;
        bool RockingBarisinit = false;

        void pppp()
        {
            iskey = false;
        }
        void pppp2()
        {
            iskey = true;
        }
        private void RockingBarManage()
        {
            if (RockingBar == null) return;
            RockingBar.onDragBegin += this.pppp;
            RockingBar.onDragEnd += this.pppp2;

            if (play == null) return;
            if (!Playisinit)
            {
                RockingBar.onDrag += play.Move;
                RockingBar.onDragEnd += play.Idle;
                atkbtn.onChick += play.Attack;
            }
            Playisinit = play != null;
            RockingBarisinit = RockingBar != null;

        }

        public void OnUpdate()
        {
            if (onUpdate != null)
                onUpdate();


        }

    }

}
