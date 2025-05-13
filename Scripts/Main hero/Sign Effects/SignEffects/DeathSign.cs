using System.Collections.Generic;
using Status_Effect_System;
using UnityEngine;

namespace Main_hero.Sign_Effects.SignEffects
{
    [CreateAssetMenu(menuName = "Sign Effects/Effect Applier Sign")]
    public class DeathSign : SignEffect
    {
        [field: SerializeField] public bool IsTemporary { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        [SerializeField] private List<StatusEffectData> effectsToApply = new List<StatusEffectData>();
        private HeroStateHandler _stateHandler;
        public override void ApplyEffect(GameObject target)
        {
            _stateHandler = target.GetComponent<HeroStateHandler>();
            target.GetComponent<HeroStateHandler>().AttackingState.OnEnemyHit += OnEnemyHit;
        }
        public override void RemoveEffect(GameObject target)
        {
            _stateHandler.AttackingState.OnEnemyHit -= OnEnemyHit;
        }

        private void OnEnemyHit(Collider2D collider)
        {
            if (!collider.TryGetComponent(out IEffectable effectable)) return;
            foreach (var statusEffectData in effectsToApply)
            {
                effectable.ApplyEffect(statusEffectData);
            }
        }
    }
}
