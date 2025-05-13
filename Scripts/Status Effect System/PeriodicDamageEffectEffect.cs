using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using Status_Effect_System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Periodic Damage Effect")]
public class PeriodicDamageEffect : StatusEffectData
{
    [SerializeField] private float damage;
    [SerializeField] private float tickRate;
    private float _elapsed = 0f;

    public override void SetValues(float damageValue) => damage = damageValue;
    public override void StartEffect(GameObject objectToApplyEffect)
    {
        if (objectToApplyEffect.TryGetComponent(out IResistible resistible))
        {
            currentLifeTime = LifeTime * (1 - resistible.GetResistance(DamageTypeManager.DamageType.Effect));
            return;
        }
        currentLifeTime = LifeTime;
    }
    public override void RestartEffect(GameObject objectToApplyEffect)
    {
        if (objectToApplyEffect.TryGetComponent(out IResistible resistible))
        {
            currentLifeTime = LifeTime * (1 - resistible.GetResistance(DamageTypeManager.DamageType.Effect));
            return;
        }
        currentLifeTime = LifeTime;
    }
    public override bool HandleEffect(GameObject objectToApplyEffect)
    {
        _elapsed += Time.deltaTime;
        if (!(_elapsed >= tickRate)) return false;
        _elapsed %= tickRate;
        if (objectToApplyEffect.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage, damageType);
        }

        return false;
    }
    public override void EndEffect(GameObject objectToApplyEffect)
    {
        //Remove visual effect
    }
}
