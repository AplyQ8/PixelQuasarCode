using ObjectLogicInterfaces;
using Status_Effect_System;
using UnityEngine;

namespace DefaultNamespace.Status_Effect_System
{
    [CreateAssetMenu(menuName = "Status Effects/SlowDown")]
    public class SlowDownEffect : StatusEffectData
    {
        [SerializeField] private float slowDown;
        public override void SetValues(float slowDownValue) => slowDown = slowDownValue;
        
        public override void StartEffect(GameObject objectToApplyEffect)
        {
            //Apply visual effect
            CalculateLifeTimeWithResist(objectToApplyEffect);
            if (objectToApplyEffect.TryGetComponent(out IMovable movable))
            {
                movable.SlowDown(slowDown);
            }
        }
        public override void RestartEffect(GameObject objectToApplyEffect)
        {
            CalculateLifeTimeWithResist(objectToApplyEffect);
        }
        
        public override void EndEffect(GameObject objectToApplyEffect)
        {
            if (objectToApplyEffect.TryGetComponent(out IMovable movable))
            {
                movable.SpeedUp(slowDown);
            }
        }
    }
}