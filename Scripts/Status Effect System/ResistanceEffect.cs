using ObjectLogicInterfaces;
using UnityEngine;

namespace Status_Effect_System
{
    [CreateAssetMenu(menuName = "Status Effects/Resistance")]
    public class ResistanceEffect : StatusEffectData
    {
        [SerializeField] private float resistance;
        [SerializeField] private DamageTypeManager.DamageType damageTypeResistance;
        public override void StartEffect(GameObject objectToApplyEffect)
        {
            currentLifeTime = LifeTime;

            if (objectToApplyEffect.TryGetComponent(out IResistible resistible))
            {
                resistible.ApplyResistance(resistance, damageTypeResistance);
            }
        }
        public override void RestartEffect(GameObject objectToApplyEffect)
        {
            currentLifeTime = LifeTime;
        }
        public override void EndEffect(GameObject objectToApplyEffect)
        {
            if (objectToApplyEffect.TryGetComponent(out IResistible resistible))
            {
                resistible.ApplyResistance(-resistance, damageTypeResistance);
            }
        }
        
        public override void SetValues(float resistanceValue) => resistance = resistanceValue;
    }
}