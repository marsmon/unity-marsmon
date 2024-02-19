using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DT.Art
{
    [RequireComponent(typeof(Animator))]
    public class NewBehaviourScript : MonoBehaviour
    {
        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            
        }
        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 1);
        }
    }

}