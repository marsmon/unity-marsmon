using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DT.FrameWork
{

    public partial class DTRockingBar : DTBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Transform target;
        public float radius = 50f;
        public Vector2 direction;
        //跟踪当前拖动状态
        private bool isDragging = false;
        //拇指被拖动的参考
        private RectTransform thumb;

        //initialize variables
        protected override void Initialize()
        {
            thumb = target.GetComponent<RectTransform>(); 
        }

        /// Event fired by UI Eventsystem on drag start.
        public void OnBeginDrag(PointerEventData data)
        {
            isDragging = true;
            if (onDragBegin != null)
                onDragBegin();
        }
        /// Event fired by UI Eventsystem on drag. 
        public void OnDrag(PointerEventData data)
        {
            //get RectTransforms of involved components
            RectTransform draggingPlane = transform as RectTransform;
            Vector3 mousePos;
            //check whether the dragged position is inside the dragging rect,
            //then set global mouse position and assign it to the joystick thumb
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, data.position, data.pressEventCamera, out mousePos))
            {
                thumb.position = mousePos;
            }
            //length of the touch vector (magnitude)
            //calculated from the relative position of the joystick thumb
            float length = target.localPosition.magnitude;
            //if the thumb leaves the joystick's boundaries,
            //clamp it to the max radius
            if (length > radius)
            {
                target.localPosition = Vector3.ClampMagnitude(target.localPosition, radius);
            }
            //set the Vector2 thumb position based on the actual sprite position
            direction = target.localPosition;
            //smoothly lerps the Vector2 thumb position based on the old positions
            direction = direction / radius * Mathf.InverseLerp(radius, 2, 1);
        }
        /// 
        /// Event fired by UI Eventsystem on drag end.
        /// 
        public void OnEndDrag(PointerEventData data)
        {
            //we aren't dragging anymore, reset to default position
            direction = Vector2.zero;
            target.position = transform.position;

            //set dragging to false and fire callback
            isDragging = false;
            if (onDragEnd != null)
                onDragEnd();
        }




    }


    public partial class DTRockingBar
    {
        /// <summary>
        /// 当用户输入操纵杆开始移动时触发回调
        /// </summary>
        public event Action onDragBegin;
        /// <summary>
        /// 操纵杆移动或按住时触发回调
        /// </summary>
        public event Action<Vector2> onDrag;
        /// <summary>
        /// 释放操纵手柄输入时触发回调
        /// </summary>
        public event Action onDragEnd;

        // set joystick thumb position to drag position each frame
        void Update()
        {
#if UNITY_EDITOR
            target.localPosition = direction * radius;
            target.localPosition = Vector3.ClampMagnitude(target.localPosition, radius);
#endif 
            if (isDragging && onDrag != null)
                onDrag(direction);
        }

    }
}
