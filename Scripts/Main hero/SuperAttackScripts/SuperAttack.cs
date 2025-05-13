using Main_hero.State_Machine;
using UnityEngine;

namespace Main_hero.SuperAttackScripts
{
    public abstract class SuperAttack : ScriptableObject
    {
        [field: SerializeField] public string AttackName { get; private set; }
        [field: SerializeField] public float MaxHoldTime { get; private set; } = 2f;
        [field: SerializeField] public float DecayRate { get; private set; } = 0.3f;
        [field: SerializeField] public float Cooldown { get; private set; } = 1f;
        [field: SerializeField] public float BaseDamage { get; private set; }
        [field: SerializeField] public DamageTypeManager.DamageType DamageType { get; private set; }
        private protected float CurrentHoldTime;

        public abstract bool Executed(SuperAttackContext context, float holdTime);

        public virtual void OnHold(SuperAttackContext context, float holdTime)
        {
            CurrentHoldTime = holdTime;
            //Debug.Log(CurrentHoldTime);
        }
    }
}