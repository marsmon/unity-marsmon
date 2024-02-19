using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DT.FrameWork
{
    public interface IRoleAnimState
    {
        Transform root { get; }
        Animator animator { get; }
        bool atkbegin { get; set; }


        void InAtk();
        void OnAtk();
        void IdleorRun(bool isrun);
        void SetAttack();
        // void RestBaseState();
        // void ResetAnimator();
    }
}