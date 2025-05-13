using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

[CreateAssetMenu(menuName = "Stat Modifiers/Hero modifiers/Health Modifier")]
public class HeroStatHealthModifierSO : StatModifierSO
{
    public override void AffectObject(GameObject objectToApplyModifier, float value)
    {
        if (objectToApplyModifier.TryGetComponent(out IHealable healable))
        {
            healable.TakeHeal(value);
        }
    }

    public override void RemoveEffect(GameObject objectToRemoveEffect, float value)
    {
        
    }
}
