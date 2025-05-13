using System;
using UnityEngine;

namespace Tests
{
    public class GoblinAnimationEvent : MonoBehaviour
    {
        [SerializeField] private GoblinScript goblinScript;
        private GoblinSound goblinSound;

        private void Start()
        {
            goblinSound = transform.parent.GetComponentInChildren<GoblinSound>();
        }

        public void AttackPrep()
        {
            goblinSound.PlayAttackPrepSound();
        }

        public void StartAttack()
        {
            goblinScript.StartAttack();
            goblinSound.PlayAttackSound();
        }
        
        public void EndAttack()
        {
            goblinScript.EndAttack();
        }
        
        public void PlayStepSound()
        {
            goblinSound.PlayStepsSound();
        }
    }
}