using System;
using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.Health
{
    [RequireComponent(typeof(IEvasionable))]
    [RequireComponent(typeof(IResistible))]
    [RequireComponent(typeof(IColorChangeable))]
    public class HealthEvasionableResistible : MonoBehaviour, IDamageable, IHealable, IHealth
    {
        public event Action<float>
            OnHealthChange,
            OnHealthBoundariesChange;

        public event Action OnDeathEvent;
        [field: SerializeField] public float MaxHealth { get; private set; }
        [field: SerializeField] public float MinHealthPoints { get; private set; }
        [field: SerializeField] public float CurrentHealth { get; private set; }
        private IEvasionable _evasionScript;
        private IResistible _resistanceScript;
        private IColorChangeable _colorChangeable;

        private void Start()
        {
            CurrentHealth = MaxHealth;
            _evasionScript = GetComponent<IEvasionable>();
            _resistanceScript = GetComponent<IResistible>();
            _colorChangeable = GetComponent<IColorChangeable>();
            OnHealthBoundariesChange?.Invoke(MaxHealth);
            OnHealthChange?.Invoke(CurrentHealth);
        }

        public void TakeDamage(float damageValue, DamageTypeManager.DamageType damageType)
        {
            if (_evasionScript.AvoidedDamage())
                return;
            CurrentHealth -= damageValue * (1 - _resistanceScript.GetResistance(damageType));
            _colorChangeable.ChangeColorTemporarily(Color.red, 0.2f);
            OnHealthChange?.Invoke(CurrentHealth);
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                OnDeathEvent?.Invoke();
            }
        }

        public void TakeDamage(float value, DamageTypeManager.DamageType damageType, Vector3 damageSourcePosition)
        {
            TakeDamage(value, damageType);
        }

        public void TakeHeal(float healValue)
        {
            CurrentHealth += healValue;
            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;
            OnHealthChange?.Invoke(CurrentHealth);
        }

        public float GetCurrentHealth() => CurrentHealth;
        public float GetMaxHealthPoints() => MaxHealth;

        public void IncreaseHealthPoints(float value)
        {
            MaxHealth += value;
            OnHealthBoundariesChange?.Invoke(MaxHealth);
        }

        public void DecreaseHealthPoints(float value)
        {
            MaxHealth -= value;
            if (MaxHealth < MinHealthPoints)
                MaxHealth = MinHealthPoints;
            OnHealthBoundariesChange?.Invoke(MaxHealth);
        }
        
        public void DeathFromFalling()
        {
            TakeDamage(CurrentHealth, DamageTypeManager.DamageType.Default);
        }
    }
}
