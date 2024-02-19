using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DT.FrameWork
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(Animator))]
    public partial class StateBase : MonoBehaviour, IRoleAnimState
    {
        Transform m_root;
        public Transform root
        {
            get
            {
                if (m_root == null)
                {
                    m_root = this.transform.parent;
                }
                this.isCanActive = this.isCanActive ? m_root != null : false;
                return m_root;
            }
            private set
            {
                m_root = value;
                this.isCanActive = this.isCanActive ? m_root != null : false;
            }
        }
        public bool atkbegin { get; set; }

        Animator m_animator;
        public Animator animator
        {
            get
            {
                if (m_animator == null)
                {
                    m_animator = this.GetComponent<Animator>();
                }
                this.isCanActive = this.isCanActive ? m_animator != null : false;
                return m_animator;
            }
            private set
            {
                m_animator = value;
                this.isCanActive = this.isCanActive ? m_animator != null : false;
            }
        }



        bool isCanActive = false;

        private void Start()
        {
            init();
        }

        private void OnEnable()
        {
            init();
        }

        private void OnDisable()
        {
            this.ResetAnimator();
        }

        public virtual bool GetAtkbegin()
        {
            return this.atkbegin;
        }


        public virtual void IdleorRun(bool isrun)
        {
            if (!isCanActive) return;
            animator.SetBool("IsRun", isrun);
        }

        public virtual void SetAttack()
        {
            if (!isCanActive) return;
            animator.SetTrigger("IsSkill");
        }

        public virtual void InAtk()
        {
            if (!isCanActive) return;
            int v = Random.Range(0, 2);
            animator.SetInteger("SkillMode", v);
            // InSuondState();
            // animator.SetBool("_SoundState", true);
        }

        public virtual void InDie()
        {
            if (!isCanActive) return;
            animator.SetBool("Dieing", true);
        }
        // public virtual void InHighestState()
        // {
        //     if (!isCanActive) return;
        //     animator.SetBool("_HighestState", true);
        // }

        // public virtual void RestSuondState()
        // {
        //     if (!isCanActive) return;
        //     animator.SetBool("_SoundState", false);
        // }
        // public virtual void RestHighestState()
        // {
        //     if (!isCanActive) return;
        //     animator.SetBool("_HighestState", false);
        // }

        // public virtual void RestBaseState()
        // {
        //     if (!isCanActive) return;
        //     animator.SetBool("_SoundState", false);
        //     animator.SetBool("_HighestState", true);
        // }

        public virtual void OnAtk()
        {
            if (!isCanActive) return;
            StartCoroutine("restAtk");
        }

        protected virtual void ResetAnimator()
        {
            animator.Play("idle", 0);
            // animator.Play("New State", 1);
            // animator.Play("New State", 2);
            for (int i = 0; i < animator.parameterCount; i++)
            {
                var t = animator.parameters[i];
                switch (t.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(t.name, t.defaultBool);
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        animator.ResetTrigger(t.name);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(t.name, t.defaultInt);
                        break;
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(t.name, t.defaultFloat);
                        break;
                }
            }

            // animator.SetBool("IsRun", false);
            // animator.SetBool("Dieing",false);

            // animator.parameters[0].name
            // animator.ResetTrigger("")
            // animator.SetBool("_SoundState", true);
            // animator.SetBool("_HighestState", false);
        }
        protected virtual void init()
        {
            isCanActive = false;
            this.root = this.transform.parent;
            this.animator = this.GetComponent<Animator>() as Animator;
            isCanActive = this.root && this.animator;
        }


        IEnumerator restAtk()
        {
            atkbegin = true;
            yield return new WaitForFixedUpdate();
            atkbegin = false;
        }
    }
}