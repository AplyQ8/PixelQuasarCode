using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

[CreateAssetMenu(menuName = "Bomb effects/DamageBomb")]
public class DamageBomb : BaseBombEffect
{
    public override void StartEffect(GameObject objectToApplyEffect)
    {
        currentLifeTime = LifeTime;
        if (objectToApplyEffect.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage, DamageTypeManager.DamageType.Default);
        }
        
    }
}
