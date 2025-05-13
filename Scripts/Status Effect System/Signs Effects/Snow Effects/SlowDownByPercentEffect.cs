using ObjectLogicInterfaces;
using UnityEngine;

namespace Status_Effect_System.Signs_Effects.Snow_Effects
{
    [CreateAssetMenu(menuName = "Status Effects/Slow Down By Percent")]
    public class SlowDownByPercentEffect : StatusEffectData
    {
        [Range(0, 1)] [SerializeField] private float slowDownPercent;
        public override void StartEffect(GameObject objectToApplyEffect)
        {
            //Apply visual effect
            CalculateLifeTimeWithResist(objectToApplyEffect);
            if (objectToApplyEffect.TryGetComponent(out IMovable movable))
            {
                movable.SlowDownByPercent(slowDownPercent);
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
                movable.SpeedUpByPercent(slowDownPercent);
            }
        }
    }
}