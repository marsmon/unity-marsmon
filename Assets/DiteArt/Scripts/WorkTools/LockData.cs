using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace YF.Art
{
    [SelectionBase]
    // [ExecuteInEditMode]
    public class LockData : MonoBehaviour
    {
        public bool IsView = true;

        
        void Awake()
        {
            this.transform.hideFlags = HideFlags.NotEditable;
        }


        public void OnValidate()
        {
            Transform[] dd = GetComponentsInChildren<Transform>();
            foreach(var g in dd){
                if( g == this.transform) continue;
                g.hideFlags = this.IsView ? HideFlags.None :  HideFlags.HideInHierarchy;
            }
        }
    }

}
