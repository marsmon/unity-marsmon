using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DT.FrameWork
{
    public class DTBehaviour : MonoBehaviour
    {
        bool Isinit = false;
        protected virtual void Awake() { Initialize(); }
        protected virtual void OnEnable() { Initialize(); }
        protected virtual void Start() { }
        protected virtual void OnDisable() { unInitialize(); }
        protected virtual void OnDestroy() { unInitialize(); }
        protected virtual void Update() { }
        protected virtual void LateUpdate() { }

#if UNITY_EDITOR
        protected virtual void OnValidate() { }

        protected virtual void Reset() { }
#endif

        public virtual bool IsActive()
        {
            return isActiveAndEnabled ? !Isinit : false;
        }
        protected virtual void Initialize()
        {
            if (!IsActive()) return;
            Isinit = true;
            // 。。。
        }
        protected virtual void unInitialize()
        {
            Isinit = false;
        }
        public virtual void OnUpdate() { }
    }
}
