using ObjectLogicRealization.Attack;
using UnityEngine;

namespace Status_Effect_System.InstabilityEffects
{
    [CreateAssetMenu(fileName = "HeroAttackEmpower", menuName = "Status Effects/Instability/AttackEmpower")]
    public class InstabilityAttackEmpower : StatusEffectData
    {
        [Range(0, 1)][SerializeField] private float attackEmpowerPercent;
        
        public override void StartEffect(GameObject objectToApplyEffect)
        {
            if (objectToApplyEffect.TryGetComponent(out HeroAttack attackScript))
            {
                attackScript.EmpowerAttackByPercent(attackEmpowerPercent);
            }
        }
        
        public override void EndEffect(GameObject objectToApplyEffect)
        {
            if (objectToApplyEffect.TryGetComponent(out HeroAttack attackScript))
            {
                attackScript.WeakenAttackByPercent(attackEmpowerPercent);
            }
        }
    }
}