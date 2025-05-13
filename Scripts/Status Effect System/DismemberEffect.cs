using System.Collections;
using System.Collections.Generic;
using Status_Effect_System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Dismember")]
public class DismemberEffect : StatusEffectData
{
    public override void StartEffect(GameObject objectToApplyEffect)
    {
        currentLifeTime = LifeTime;
        
    }
    public override void EndEffect(GameObject objectToApplyEffect)
    {
    }
}
