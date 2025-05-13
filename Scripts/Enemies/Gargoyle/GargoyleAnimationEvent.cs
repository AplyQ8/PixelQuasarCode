using System.Collections;
using UnityEngine;

namespace Tests
{
    public class GargoyleAnimationEvent : MonoBehaviour
    {
        [SerializeField] private GargoyleScript gargoyleScript;
        private GargoyleSound gargoyleSound;

        private bool wingsEventDone;
        
        private void Start()
        {
            gargoyleSound = transform.parent.GetComponentInChildren<GargoyleSound>();
            wingsEventDone = false;
        }
        public void EnterPursuit()
        {
            gargoyleScript.EnterPursuit();
        }

        public void VerticalMoveUp()
        {
            gargoyleScript.VerticalMove(Vector2.up);
        }

        public void WingsSwing()
        {
            if (wingsEventDone)
                return;
            wingsEventDone = true;
            StartCoroutine(WingsEventCooldown());
            gargoyleSound.PlayWingsSound();
        }
        
        public void SmallDash()
        {
            gargoyleSound.PlaySmallDashSound();
        }
        
        public void ToAgro()
        {
            gargoyleSound.PlayAgroSound();
        }

        IEnumerator WingsEventCooldown()
        {
            yield return new WaitForSeconds(0.4f);
            wingsEventDone = false;
        }
    }
}