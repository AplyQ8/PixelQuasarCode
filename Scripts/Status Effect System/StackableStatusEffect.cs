using UnityEngine;

namespace Status_Effect_System
{
    public class StackableStatusEffect : StatusEffectData
    {
        [field: SerializeField] public int MaxStacks { get; private set; } = 5; 
        //[field: SerializeField] public float StackDuration { get; private set; } = 3f;

        public override void StartEffect(GameObject objectToApplyEffect)
        {
            CalculateLifeTimeWithResist(objectToApplyEffect);
        }

        public override void RestartEffect(GameObject objectToApplyEffect)
        {
            CalculateLifeTimeWithResist(objectToApplyEffect);
        }

        public override bool HandleEffect(GameObject objectToApplyEffect)
        {
            return false;
        }
        
        /// <summary>
        /// Метод вызывается при добавлении стаков. 
        /// Его переопределяют конкретные эффекты.
        /// </summary>
        public virtual void IncreaseStack(GameObject objectToApplyEffect, int currentStacks) { }

        /// <summary>
        /// Метод вызывается при снятии стаков. 
        /// Его переопределяют конкретные эффекты.
        /// </summary>
        public virtual void ReduceStack(GameObject objectToApplyEffect, int currentStacks) { }
    }
}