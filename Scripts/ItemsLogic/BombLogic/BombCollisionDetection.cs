using System;
using UnityEngine;
using UnityEngine.Events;

namespace ItemsLogic.BombLogic
{
    [RequireComponent(typeof(Collider2D))]
    public class BombCollisionDetection : MonoBehaviour
    {
        public UnityEvent<bool> onCollisionDetectEvent;
        public UnityEvent<bool> onCollisionExitEvent;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Obstacle"))
                return;
            onCollisionDetectEvent?.Invoke(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Obstacle"))
                return;
            onCollisionExitEvent?.Invoke(false);
        }
    }
}
