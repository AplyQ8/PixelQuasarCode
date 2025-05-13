using System.Net;
using ObjectLogicInterfaces;
using UnityEngine;

namespace Status_Effect_System.Signs_Effects.Death_Effects
{
    [CreateAssetMenu(menuName = "Status Effects/Signs/Death/Death Label")]
    public class DeathLabelEffect : StatusEffectData
    {
        [SerializeField] private float explosionDamage;
        [SerializeField] private float explosionRadius;
        [SerializeField] private LayerMask objectsToHitLayer;
        [SerializeField] private ParticleSystem explosionVFXEffect;

        public override void StartEffect(GameObject objectToApplyEffect)
        {
            if (!objectToApplyEffect.TryGetComponent(out IDamageable damageable)) return;
            damageable.OnDeathEvent += () => OnObjectDies(objectToApplyEffect, damageable);
        }

        public override void EndEffect(GameObject objectToApplyEffect)
        {
            if (!objectToApplyEffect.TryGetComponent(out IDamageable damageable)) return;
            damageable.OnDeathEvent -= () => OnObjectDies(objectToApplyEffect, damageable);
        }

        private void OnObjectDies(GameObject deadObject, IDamageable damageable)
        {
            //Instantiate(explosionVFXEffect, deadObject.transform.position, Quaternion.identity);
            damageable.OnDeathEvent -= () => OnObjectDies(deadObject, damageable);
            var hitObjects = Physics2D.OverlapCircleAll(deadObject.transform.position, explosionRadius, objectsToHitLayer);
            foreach (var hitObject in hitObjects)
            {
                if(hitObject.gameObject == deadObject) continue;
                if(!hitObject.TryGetComponent(out IDamageable damageableObject)) continue;
                damageableObject.TakeDamage(explosionDamage, damageType, deadObject.transform.position);
                
            }
        }
    }
}