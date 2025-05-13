using System.Collections.Generic;
using ObjectLogicInterfaces;
using UI.Particles;
using UnityEngine;
using Utilities;

namespace HitEffects
{
    public class EnemyHitEffects : MonoBehaviour
    {
        [SerializeField] private GameObject damageableObject;
        private IDamageable _damageable;
        [SerializeField] private List<ParticleTrigger> hitParticles;
        [SerializeField] private List<ParticleTrigger> deathParticles;

        private void Start()
        {
            
            if (damageableObject.TryGetComponent(out IDamageable damageable))
            {
                _damageable = damageable;
                _damageable.OnHealthChange += PlayHitEffect;
                _damageable.OnDeathEvent += PlayDeathEffect;
            }
            else
            {
                Debug.LogWarning("Assigned object does not have IDamageable!");
            }
        }

        private void PlayHitEffect(float val)
        {
            if (hitParticles.Count == 0) return;
            int randomEffectIndex = RandomGenerator.Instance.RandomValueInRange(0, hitParticles.Count);
            var particles = hitParticles[randomEffectIndex];
            var instantiatedParticles = Instantiate(particles, transform.position, Quaternion.identity, transform);
            instantiatedParticles.TriggerParticles();
        }

        private void PlayDeathEffect()
        {
            if (deathParticles.Count == 0) return;
            int randomEffectIndex = RandomGenerator.Instance.RandomValueInRange(0, deathParticles.Count);
            var particles = deathParticles[randomEffectIndex];
            var instantiatedParticles = Instantiate(particles, transform.position, Quaternion.identity, transform);
            instantiatedParticles.TriggerParticles();
        }

        private void OnDisable()
        {
            if (_damageable is null) return;
            _damageable.OnHealthChange -= PlayHitEffect;
            _damageable.OnDeathEvent -= PlayDeathEffect;
        }
    }
}
