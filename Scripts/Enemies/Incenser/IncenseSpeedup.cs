using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using ObjectLogicRealization.Move;
using Status_Effect_System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/IncenseSpeedup")]
public class IncenseSpeedup : StatusEffectData
{
    [SerializeField] private float speedUpValue;
    [SerializeField] private float tickRate;
    private float _elapsed = 0f;
    
    public override void StartEffect(GameObject objectToApplyEffect)
    {
        currentLifeTime = LifeTime;
        if (objectToApplyEffect.TryGetComponent(out EnemyMove enemyMove))
        {
            enemyMove.SpeedUpBaseSpeed(speedUpValue);
        }
    }
    public override void RestartEffect(GameObject objectToApplyEffect)
    {
        currentLifeTime = LifeTime;
    }
    
    public override void EndEffect(GameObject objectToApplyEffect)
    {
        if (objectToApplyEffect.TryGetComponent(out EnemyMove enemyMove))
        {
            enemyMove.SlowDownBaseSpeed(speedUpValue);
        }
    }
}
