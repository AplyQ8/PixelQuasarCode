using System;
using System.Collections;
using System.Collections.Generic;
using Status_Effect_System;
using UnityEngine;

[CreateAssetMenu(menuName = "Stat Modifiers/Effect modifiers/Effect modifier")]
public class EffectModifier : StatModifierSO
{
    [SerializeField] private StatusEffectData effect;
    public override void AffectObject(GameObject objectToApplyEffect, float value)
    {
        if (objectToApplyEffect.TryGetComponent(out IEffectable effectable))
        {
            effectable.ApplyEffect(effect, value);
        }
    }

    public override void RemoveEffect(GameObject objectToRemoveEffect, float value)
    {
        if (objectToRemoveEffect.TryGetComponent(out IEffectable effectable))
        {
            effectable.RemoveEffect(effect);
        }
    }
}
