using Main_hero.State_Machine;
using ObjectLogicInterfaces;
using UnityEngine;

namespace Main_hero.SuperAttackScripts
{
    [CreateAssetMenu(fileName = "FixedRadiusAttack", menuName = "SuperAttacks/Fixed Radius Attack", order = 0)]
    public class FixedRadiusAttack : SuperAttack
    {
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public LayerMask TargetLayer { get; private set; }
        [SerializeField] private LayerMask throughCollisionMask;

        private IMovable _heroMovable;
        
        public override bool Executed(SuperAttackContext context, float holdTime)
        {
            _heroMovable = context.Hero.GetComponent<IMovable>();
            if (holdTime < MaxHoldTime) return false;
            Collider2D[] hitTargets = Physics2D.OverlapCircleAll(context.Hero.transform.position, Radius, TargetLayer);
            context.Animator.SetTrigger(context.AnimationTrigger);
            foreach (var target in hitTargets)
            {
                if (target.TryGetComponent(out IDamageable damageable))
                {
                    if (IsEnemyObstructed(target)) continue;
                    damageable.TakeDamage(BaseDamage, DamageType, context.Hero.transform.position);
                }
                
            }

            return true;
        }
        private bool IsEnemyObstructed(Collider2D hitObject)
        {
            if (!hitObject.gameObject.TryGetComponent(out IObstructed obstructedScript)) return true;
            if (_heroMovable is null) return true;
            Vector3 enemyPosition = obstructedScript.GetPivotPoint();
            RaycastHit2D hit = 
                Physics2D.Linecast(
                    _heroMovable.GetPivotPosition(), 
                    enemyPosition, 
                    throughCollisionMask);
            return hit.collider is not null;

        }
        
    }
}