using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DT.FrameWork
{
    [ExecuteAlways]
    public class DTManageBase<T> where T : new()
    {
        public event Action OnInit;
        public event Action InitEnd;
        private static T instance;
        private static readonly object locker = new object();
        private DTManageBase() { }

        public static T Instance
        {
            get
            {
                lock (locker)
                {
                    if (instance == null)
                        instance = new T();
                    return instance;
                }
            }
        }
    }
}
