using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DT.FrameWork
{
    public class DTPlayManage : DTRoleManage
    {
        public bool isworld = true;
        [SerializeField] Transform tagobj;
        [SerializeField] float rottime = 0.01f;
        Vector3 dir = Vector3.forward;
        public float Speed = 1f;
        bool ismove = false;
        string srcstate = "idle";
        string attackstate = "skill";
        Vector3 lockdir = Vector3.forward;
        protected override void OnStart()
        {
            base.OnStart();
            DTGameScene.Instance.play = this;
            animation["idle"].clip.AddEvent(new AnimationEvent
            {
                functionName = "OnInidle",
                time = 0f
            });
            animation[attackstate].clip.AddEvent(new AnimationEvent
            {
                functionName = "outSecondaryChange",
                time = animation[attackstate].clip.length
            });


        }
        protected void outSecondaryChange()
        {
            // IsfirstChange = false;
            // Debug.Log("outSecondaryChange");
            IsSecondaryChange = false;
            isattack = false;
            if (ismove)
            {
                animation.Play("run");
            }
            else
            {
                animation.Play("idle");
            }
        }
        protected override void unInitialize()
        {
            DTGameScene.Instance.play = null;
            base.unInitialize();
        }

        float changetime;

        // IEnumerator changdir()
        // { 

        //     this.transform.forward = Vector3.Slerp(transform.forward, Vector3.back, m_time);
        //     yield return new WaitForSeconds(1);
        // }
        public void OnInidle()
        {
            // " ".DTLog("OnInidle");
            // m_time = 0;
            // if (this.transform.forward == Vector3.back) return;
            // StartCoroutine("changdir");
            // Vector3 currentVel = Vector3.zero;
            // this.transform.forward = Vector3.Slerp(transform.forward, Vector3.back, 200 * Time.deltaTime);
        }
        public override void Idle()
        {
            this.ismove = false;
            if (IsSecondaryChange)
                return;
            base.Idle();
        }

        float m_time;
        Vector3 newpos;
        public override void Move(Vector2 direction)
        {
            this.ismove = direction.x != 0 || direction.y != 0;

            if (ismove)
            {
                if (!animation.IsPlaying("run") && !IsSecondaryChange)
                    animation.Play("run");
                dir.x = direction.x;
                dir.z = direction.y;//direction.y;
                dir.Normalize();
                if (isworld)
                {
                    Vector3 d = Mathf.Abs(dir.x) > Mathf.Abs(dir.z) ? new Vector3(dir.x, 0, 0) : new Vector3(0, 0, dir.z);
                    dir = d;
                }
            }
            else
            {
                Idle();
            }

        }

        public override void Attack(int index)
        {
            base.Attack(index);
            if (animation.IsPlaying(attackstate)) return;
            IsSecondaryChange = true;
            isattack = true;
            // lockdir = getlook();
            animation.Play(attackstate);
        }

        bool isattack;
        Vector3 currentVel = Vector3.zero;
        public override void OnUpdate()
        {
            lockdir = getlook();
            Vector3 newdir = animation.IsPlaying(attackstate) ? lockdir : dir;
            if (ismove)
            {
                //rot


                //move
                Vector3 p = this.transform.position;


                this.transform.forward = newdir;
                p += dir * this.Speed * Time.deltaTime;
                if (!isworld)
                {
                    Bounds box = DTGameScene.Instance.box;
                    p.z = Mathf.Min(box.max.z, Mathf.Max(p.z, box.min.z));
                    p.x = Mathf.Min(box.max.x, Mathf.Max(p.x, box.min.x));
                }



                this.transform.position = p;
            }
            else
            {
                if (isworld)
                {

                }
            }
            m_time += Time.deltaTime;
        }

        // IEnumerator buchangweizhi(){
        //     while(true){
        //         this.transform.position 
        //     }
        //     yield return null;
        // }

        Vector3 getlook()
        {
            Vector3 p1 = this.transform.position;
            p1.y = 0;
            Vector3 p0 = tagobj.position;
            p0.y = 0;
            Vector3 p = (p0 - p1);
            p.y = 0;
            return p.normalized;
        }
    }
}
