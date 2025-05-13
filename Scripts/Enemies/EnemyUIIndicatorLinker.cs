using ObjectLogicInterfaces;
using UnityEngine;

namespace Enemies
{
    public class EnemyUIIndicatorLinker : MonoBehaviour
    {
        [SerializeField] private HealthBar healthBar;
        private IHealth _healthScript;
        private IDamageable _damageableScript;

        private void Awake()
        {
            if (TryGetComponent(out IHealth health))
            {
                _healthScript = health;
                health.OnHealthChange += OnHealthChange;
                health.OnHealthBoundariesChange += OnMaxHealthBoundariesChange;
            }

            if (TryGetComponent(out IDamageable damageable))
            {
                _damageableScript = damageable;
                damageable.OnDeathEvent += DeathEvent;
            }
            
        }

        private void OnHealthChange(float value)
        {
            healthBar.SetHealth(value);
        }

        private void OnMaxHealthBoundariesChange(float value)
        {
            healthBar.SetMaxHealth(value);
        }

        private void DeathEvent()
        {
            healthBar.gameObject.SetActive(false);
            _healthScript.OnHealthChange -= OnHealthChange;
            _healthScript.OnHealthBoundariesChange -= OnMaxHealthBoundariesChange;
            _damageableScript.OnDeathEvent -= DeathEvent;
        }
        
    }
}
