using UnityEngine;

namespace Tests
{
    public class SummonSkeletonShoot : MonoBehaviour
    {
        [SerializeField] private UndeadHand skeletonScript;
        private UndeadHandSound undeadHandSound;

        private void Start()
        {
            undeadHandSound = transform.parent.GetComponentInChildren<UndeadHandSound>();
        }

        public void Shoot()
        {
            undeadHandSound.PlayThrowSound();
            skeletonScript.Shoot();
        }

        public void EndAwake()
        {
            skeletonScript.EndAwake();
        }
    }
}