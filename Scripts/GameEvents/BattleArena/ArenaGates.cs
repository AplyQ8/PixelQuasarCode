using UnityEngine;

namespace GameEvents
{
    public class ArenaGates : MonoBehaviour
    {
        [SerializeField] private Animator frontGate;
        [SerializeField] private Animator backGate;
        private static readonly int OpenTrigger = Animator.StringToHash("Open");
        private static readonly int CloseTrigger = Animator.StringToHash("Close");

        [Header("Colliders")] 
        [SerializeField] private Collider2D frontGateCollider;
        [SerializeField] private Collider2D backGateCollider;
        [SerializeField] private Collider2D closedCollider;
        

        public void Open()
        {
            frontGate.SetTrigger(OpenTrigger);
            backGate.SetTrigger(OpenTrigger);
            
            closedCollider.enabled = false;

            frontGateCollider.enabled = true;
            backGateCollider.enabled = true;
        }

        public void Close()
        {
            frontGate.SetTrigger(CloseTrigger);
            backGate.SetTrigger(CloseTrigger);
            
            closedCollider.enabled = true;

            frontGateCollider.enabled = false;
            backGateCollider.enabled = false;
        }
    }
}