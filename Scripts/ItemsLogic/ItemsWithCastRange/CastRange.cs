using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ItemsLogic.ItemsWithCastRange
{
    [RequireComponent(typeof(Collider2D))]
    public class CastRange : MonoBehaviour
    {
        [SerializeField] private LayerMask triggerMask;

        public event Action<GameObject> OnEnter;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if ((triggerMask & (1 << col.gameObject.layer)) == 0)
                return;
            OnEnter?.Invoke(col.gameObject);
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            if ((triggerMask & (1 << other.gameObject.layer)) == 0)
                return;
            other.GetComponent<Rigidbody2D>().WakeUp();
            OnEnter?.Invoke(other.gameObject);
        }
    }
}