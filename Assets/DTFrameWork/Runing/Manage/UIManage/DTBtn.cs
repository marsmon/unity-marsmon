using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DT.FrameWork
{
    public partial class DTBtn : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] int btnid = 0;
        public event Action<int> onChick;
        public void OnPointerClick(PointerEventData eventData)
        { 
            if (onChick != null)
                onChick(btnid);
        } 
        void Start()
        {
            DTGameScene.Instance.atkbtn = this;
        }
    }


}
