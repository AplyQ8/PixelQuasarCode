using UnityEngine;

namespace Tests
{
    public class PhantomAnimationEvent : MonoBehaviour
    {
        [SerializeField] private PhantomScript phantomScript;

        private PhantomSound phantomSound;

        private void Start()
        {
            phantomSound = transform.parent.GetComponentInChildren<PhantomSound>();
        }
        public void EnableBoxCollider()
        {
            phantomScript.EnableBoxCollider();
        }


    }
}