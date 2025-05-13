using ObjectLogicInterfaces;
using UnityEngine;

namespace Status_Effect_System
{
    public abstract class StatusEffectData : ScriptableObject
    {
        public int ID => GetInstanceID();
        [SerializeField] protected DamageTypeManager.DamageType damageType;
        public string effectName;
        public Sprite icon;
        [field: SerializeField] public float LifeTime { get; private set; }
        public EffectType effectType;
        public GameObject effectTarget;
        public float currentLifeTime;
        public ParticleSystem particles;

        [field: SerializeField] public EffectDurability DurabilityType { get; private set; }

        public bool consumesBlood;
    
        public enum EffectType
        {
            Positive,
            Negative
        }

        public enum EffectDurability
        {
            Permanent,
            Durable,
            Stackable
        }

        public virtual void SetValues(int value)
        { }
        public virtual void SetValues(float value)
        { }

        public virtual void StartEffect(GameObject objectToApplyEffect) { }
        public virtual void RestartEffect(GameObject objectToApplyEffect) { }

        public virtual bool HandleEffect(GameObject objectToApplyEffect) => false;

        public virtual void EndEffect(GameObject objectToApplyEffect) { }
        public virtual void SetParticleEffect(ParticleSystem particleEffect) => particles = particleEffect;

        public virtual void SetLifeTime(float effectLifeTime)
        {
            if (effectLifeTime <= 0)
                return;
            LifeTime = effectLifeTime;
        }

        protected void CalculateLifeTimeWithResist(GameObject objectToApplyEffect)
        {
            if (objectToApplyEffect.TryGetComponent(out IResistible resistible))
            {
                currentLifeTime = LifeTime * (1 - resistible.GetResistance(DamageTypeManager.DamageType.Effect));
                return;
            }
            currentLifeTime = LifeTime;
        }

        public void CalculateLifeTimeWithoutResist()
        {
            currentLifeTime = LifeTime;
        }
    }
}
