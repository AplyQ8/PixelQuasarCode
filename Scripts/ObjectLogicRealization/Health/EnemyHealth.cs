using System;
using System.Collections;
using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.Health
{
    public class EnemyHealth : PoolDeath, IHealth, IDamageable, IHealable, ICanFall
    {

        [SerializeField] private float maxHealth;
        [SerializeField] private float deathHealthThreshold;
        [SerializeField] private float currentHealth;
        
        protected EnemySound enemySound;

        private Transform bodyBottom;
    
        public event Action<float>
            OnHealthChange,
            OnHealthBoundariesChange;
            
        public event Action OnDeathEvent;
    
        private void Start()
        {
            currentHealth = maxHealth;
            enemyScript = transform.GetComponent<EnemyScript>();
            UniqueId = transform.GetComponent<UniqueId>();
            // enemyScript.OnDieEvent += Death;
            OnHealthBoundariesChange?.Invoke(maxHealth);
            OnHealthChange?.Invoke(currentHealth);
            
            enemySound = gameObject.GetComponentInChildren<EnemySound>();
            
            bodyBottom = transform.Find("ObstacleCollider");
        }

        void OnEnable()
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
            if (maxHealth <= deathHealthThreshold)
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
            OnHealthChange?.Invoke(currentHealth);
        }
            
        public void TakeDamage(float value, DamageTypeManager.DamageType damageType)
        {
            enemyScript.GetDamageReaction();
            
            currentHealth -= value;
            if (currentHealth <= deathHealthThreshold)
            {
                OnHealthChange?.Invoke(deathHealthThreshold);
                OnDeathEvent?.Invoke();
                return;
            }
            OnHealthChange?.Invoke(currentHealth);
        }

        public void TakeDamage(float value, DamageTypeManager.DamageType damageType, Vector3 damageSourcePosition)
        {
            enemyScript.GetDamageReaction();
            
            if (damageType == DamageTypeManager.DamageType.MeleeAttack)
            {
                enemySound?.PlayGetDmgSound();
            }
            
            currentHealth -= value;
            if (currentHealth <= deathHealthThreshold)
            {
                OnHealthChange?.Invoke(deathHealthThreshold);
                OnDeathEvent?.Invoke();
                return;
            }
            OnHealthChange?.Invoke(currentHealth);

            if (damageType == DamageTypeManager.DamageType.MeleeAttack)
            {
                enemyScript.GetHitPreprocessing((bodyBottom.position - damageSourcePosition).normalized);
            }
        }

        public void DeathFromFalling()
        {
            enemyScript.StartFallingAnimation();
            currentHealth = 0;
            OnHealthChange?.Invoke(currentHealth);
            OnDeathEvent?.Invoke();
        }


        public bool CanFall()
        {
            return enemyScript.CanFall();
        }
    }
}
