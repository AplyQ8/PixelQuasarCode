using System;
using UnityEngine;

namespace NPC.Ghost
{
    [RequireComponent(typeof(Animator))]
    public class GhostNPC : MonoBehaviour
    {
        [SerializeField] private Animator _animationController;
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Appear = Animator.StringToHash("Appear");
        private static readonly int Disappear = Animator.StringToHash("Disappear");
        private Transform _observableObject;

        public event Action OnAppeared;
        public event Action OnDisappeared;
        
        
        void Update()
        {
            if (_observableObject is null) return;
            Vector2 direction = (_observableObject.position - transform.position).normalized;
            _animationController.SetFloat(Horizontal, direction.x);
        }

        public void Emerge(Transform observableObject)
        {
            _observableObject = observableObject;
            _animationController.SetTrigger(Appear);
        }

        public void Vanish() => _animationController.SetTrigger(Disappear);

        public void AppearedEvent() => OnAppeared?.Invoke();
        public void DisappearedEvent() => OnDisappeared?.Invoke();
    }
}
