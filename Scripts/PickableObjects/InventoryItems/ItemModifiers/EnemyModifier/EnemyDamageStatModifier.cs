using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

[CreateAssetMenu(menuName = "Stat Modifiers/General Modifiers/Damage modifier")]
public class DamageStatModifier : StatModifierSO
{
    [SerializeField] private DamageTypeManager.DamageType damageType;
    public override void AffectObject(GameObject objectToDealDamage, float value)
    {
        if (objectToDealDamage.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(value, damageType);
        }
    }

    public override void RemoveEffect(GameObject objectToRemoveEffect, float value)
    {
        
    }
}
