using System;
using ObjectLogicInterfaces;
using UnityEngine;

namespace Main_hero.SuperAttackScripts
{
    [RequireComponent(typeof(Collider2D))]
    public class SuperAttackCollsionDetector : MonoBehaviour
    {
        private Collider2D _attackCollider;

        #region Damage Info

        private float _damage;
        private DamageTypeManager.DamageType _damageType;

        #endregion

        private void Start()
        {
            _attackCollider = GetComponent<Collider2D>();
            Deactivate();
        }

        public void Activate(float damage, DamageTypeManager.DamageType damageType)
        {
            _damage = damage;
            _damageType = damageType;
            _attackCollider.enabled = true;
        }

        public void Deactivate()
        {
            _attackCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(_damage, _damageType, transform.position);
            }
        }
    }
}
