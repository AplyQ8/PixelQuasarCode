using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using Status_Effect_System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Invisibility")]
public class InvisibilityEffect : StatusEffectData
{
    public override void StartEffect(GameObject objectToApplyEffect)
    {
        currentLifeTime = LifeTime;
        //characteristics.MakeTransparent(true);
        if (objectToApplyEffect.TryGetComponent(out IInvisible invisible))
        {
            invisible.MakeInvisible();
        }
    }

    public override void RestartEffect(GameObject objectToApplyEffect)
    {
        currentLifeTime = LifeTime;
    }
    public override void EndEffect(GameObject objectToApplyEffect)
    {
        if (objectToApplyEffect.TryGetComponent(out IInvisible invisible))
        {
            invisible.MakeVisible();
        }
    }
    
}
