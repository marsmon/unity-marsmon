using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DT.FrameWork
{
    interface IDTRoleState
    { 
        // bool IsfirstChange { get; set; }
        // bool IsSecondaryChange { get; set; }
        void Idle();
        void Move(Vector2 direction); 
        void Attack(int index); 
        void Hit();
        bool Die(); 
    }
}
