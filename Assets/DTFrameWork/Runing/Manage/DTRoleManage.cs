using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DT.FrameWork
{


    [RequireComponent(typeof(Animation))]
    public class DTRoleManage : DTBehaviour, IDTRoleState
    {
        protected new Animation animation;
        protected bool IsfirstChange;
        protected bool IsSecondaryChange;
        bool isCanrun = false; 
        
        protected override void Initialize()
        {
            StartCoroutine("Init");
        }


        IEnumerator Init()
        {
            yield return new WaitUntil(() => DTGameScene.Instance != null);
            OnStart();
        }
        protected virtual void OnStart()
        {
            isCanrun = true;
            DTGameScene.Instance.onUpdate += OnUpdate;
            if (animation == null)
                animation = GetComponent<Animation>();
        }
        protected override void unInitialize()
        {
            base.unInitialize();
            DTGameScene.Instance.onUpdate -= OnUpdate;
            animation = null;
            isCanrun = false;
        }

        public virtual void Idle()
        {
            if (!isCanrun) return;
            // IsfirstChange = false;
            // IsSecondaryChange = false;

            if (animation.IsPlaying("idle")) return;
            animation.Play("idle");
        }

        public virtual void Move(Vector2 direction)
        {
            if (!isCanrun) return;
            if (animation.IsPlaying("run")) return;
            animation.Play("run");
        }
        public virtual void Attack(int index)
        {  
            if (!isCanrun) return;

        }
        public virtual bool Die()
        {
            if (!isCanrun) return false;
            return false;
        }
        public virtual void Hit()
        {
            if (!isCanrun) return;
        }
    }
}
