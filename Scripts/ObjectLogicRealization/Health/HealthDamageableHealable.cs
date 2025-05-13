using System;
using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.Health
{
    public class HealthDamageableHealable : MonoBehaviour, IHealth, IDamageable, IHealable
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float deathHealthThreshold;
        [SerializeField] private float currentHealth;

        public event Action<float>
            OnHealthChange,
            OnHealthBoundariesChange;
        
        public event Action OnDeathEvent;

        private void Start()
        {
            currentHealth = maxHealth;
            OnHealthBoundariesChange?.Invoke(maxHealth);
            OnHealthChange?.Invoke(currentHealth);
        }
        
        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealthPoints() => maxHealth;
        
        public void IncreaseHealthPoints(float value)
        {
            maxHealth += value;
            OnHealthBoundariesChange?.Invoke(maxHealth);
        }

        public void DecreaseHealthPoints(float value)
        {
            maxHealth -= value;
            if (maxHealth < deathHealthThreshold)
            {
                OnDeathEvent?.Invoke();
                return;
            }
            OnHealthBoundariesChange?.Invoke(maxHealth);
        }

        public void TakeHeal(float value)
        {
            currentHealth += value;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            OnHealthChange?.Invoke(value);
        }
        
        public void TakeDamage(float value, DamageTypeManager.DamageType damageType)
        {
            currentHealth -= value;
            if (currentHealth < deathHealthThreshold)
            {
                OnHealthChange?.Invoke(deathHealthThreshold);
                OnDeathEvent?.Invoke();
                return;
            }
            OnHealthChange?.Invoke(currentHealth);
        }
        
        public void TakeDamage(float value, DamageTypeManager.DamageType damageType, Vector3 damageSourcePosition)
        {
            TakeDamage(value, damageType);
        }

        public void DeathFromFalling()
        {
            TakeDamage(currentHealth, DamageTypeManager.DamageType.Default);
        }
    }
}
