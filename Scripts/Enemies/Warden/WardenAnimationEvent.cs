using System;
using UnityEngine;

namespace Tests
{
    public class WardenAnimationEvent : MonoBehaviour
    {
        [SerializeField] private WardenScript wardenScript;
        // private GoblinSound goblinSound;

        private void Start()
        {
            // goblinSound = transform.parent.GetComponentInChildren<GoblinSound>();
        }

        public void StartAttack()
        {
            wardenScript.Attack();
        }
        
        public void EndAttack()
        {
            wardenScript.EndAttack();
        }

        public void ShootingPrepEnd()
        {
            wardenScript.EndShootingPrep();
        }

        public void Shoot()
        {
            wardenScript.Shoot();
        }

        public void EndShootingAfter()
        {
            wardenScript.EndShootingAfter();
        }
        
        // public void PlayStepSound()
        // {
        //     goblinSound.PlayStepsSound();
        // }
    }
}