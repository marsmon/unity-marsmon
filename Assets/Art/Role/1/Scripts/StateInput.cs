using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DT.FrameWork
{
    public partial class StateBase
    {
        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D))
            {
                animator.SetBool("IsRun", true);
            }
            else
            {
                animator.SetBool("IsRun", false);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                animator.SetTrigger("IsSkill");
            }
#endif
        }
    }
}
