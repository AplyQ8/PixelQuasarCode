using System.Collections;
using UnityEngine;

namespace Tests
{
    public class ArcherShootEvent : MonoBehaviour
    {
        [SerializeField] private SkeletonScript skeletonScript;
        private SkeletonSound skeletonSound;

        private bool blockStepSound;
        
        private void Start()
        {
            skeletonSound = transform.parent.GetComponentInChildren<SkeletonSound>();
            blockStepSound = false;
        }
        
        public void Shoot()
        {
            skeletonScript.Shoot();
            skeletonSound.PlayBowSwingSound();
        }

        public void BowLoad()
        {
            skeletonSound.PlayBowLoadSound();
        }
        
        public void PlayStepSound()
        {
            if (!blockStepSound)
            {
                blockStepSound = true;
                StartCoroutine(StepSoundCooldown());
                skeletonSound.PlayStepsSound();
            }
        }

        IEnumerator StepSoundCooldown()
        {
            yield return new WaitForSeconds(0.2f);
            blockStepSound = false;
        }
    }
}