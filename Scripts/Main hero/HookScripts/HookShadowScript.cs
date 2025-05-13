using System;
using UnityEngine;

namespace Main_hero
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class HookShadowScript : MonoBehaviour
    {
        //[SerializeField] private Transform pivotPosition;
        public delegate void CollisionAction(Collider2D collision);
        public event CollisionAction OnTrigger;
        public event Action OnPlayerCollision;
        public void RecalculatePosition(Vector2 hookPosition, float offset, Vector2 direction)
        {
            direction = direction.normalized;
            if (direction.y < 0)
                offset *= 1 + direction.y;

            offset *= 0.8f;
            gameObject.transform.position = hookPosition + offset * Vector2.down;
        }
        
        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
                OnPlayerCollision?.Invoke();
            if (!collision.CompareTag("Obstacle"))
                return;
            OnTrigger?.Invoke(collision);
        }

        public Vector3 GetPosition => gameObject
            .transform.position;
    }
}