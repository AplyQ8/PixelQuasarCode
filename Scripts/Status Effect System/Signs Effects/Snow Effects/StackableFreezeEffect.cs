using ObjectLogicInterfaces;
using UnityEngine;

namespace Status_Effect_System.Signs_Effects.Snow_Effects
{
    [CreateAssetMenu(menuName = "Status Effects/Stackable Freeze")]
    public class StackableFreezeEffect : StackableStatusEffect
    {
        [Range(0, 1)] [SerializeField] private float slowDownPerStack; 
        [SerializeField] private float stunDuration; 
        [SerializeField] private Gradient freezeColorGradient;
    
        public override void StartEffect(GameObject objectToApplyEffect)
        {
            IncreaseStack(objectToApplyEffect, 1);
            CalculateLifeTimeWithResist(objectToApplyEffect);
        }
        public override void IncreaseStack(GameObject objectToApplyEffect, int currentStacks)
        {
            if (objectToApplyEffect.TryGetComponent(out IMovable movable))
            {
                movable.SlowDownByPercent(slowDownPerStack); // Замедляем объект
            }

            if (currentStacks >= MaxStacks && objectToApplyEffect.TryGetComponent(out IStunable stunable))
            {
                stunable.GetStunned(stunDuration);
                if (objectToApplyEffect.TryGetComponent(out IEffectable effectable))
                {
                    effectable.RemoveEffect(this);
                }
                if (objectToApplyEffect.TryGetComponent(out IMovable movableScript))
                {
                    for (int i = 0; i < MaxStacks; i++)
                    {
                        movableScript.SpeedUpByPercent(slowDownPerStack); 
                    } 
                }

                
            }
            UpdateColor(objectToApplyEffect, currentStacks);
        }

        public override void ReduceStack(GameObject objectToApplyEffect, int currentStacks)
        {
            if (objectToApplyEffect.TryGetComponent(out IMovable movable))
            {
                movable.SpeedUpByPercent(slowDownPerStack); // Восстанавливаем скорость
            }
            UpdateColor(objectToApplyEffect, currentStacks);
        }
        
        private void UpdateColor(GameObject objectToApplyEffect, int currentStacks)
        {
            if (objectToApplyEffect.TryGetComponent(out IColorChangeable colorChanger))
            {
                float t = (float)currentStacks / MaxStacks;
                Color newColor = freezeColorGradient.Evaluate(t);
                colorChanger.ChangeColor(newColor);
            }
        }
        
    }
}