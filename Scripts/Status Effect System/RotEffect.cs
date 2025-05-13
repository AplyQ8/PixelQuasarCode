using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using ObjectLogicInterfaces;
using Status_Effect_System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/RotEffect")]
public class RotEffect : StatusEffectData
{
    [SerializeField] private GameObject heroToApply;
    [SerializeField] private float distance;
    [SerializeField] private float bloodConsumption;
    [SerializeField] private float damage;
    [SerializeField] private float tickRate;
    [SerializeField] private Collider2D[] objectsToDealDamage;
    [SerializeField] private LayerMask enemyLayer;
    private float _elapsed = 0f;
    private Action _abilityDeactivation;

    private IBloodContent _bloodContent;

    public override void StartEffect(GameObject objectToApplyEffect)
    {
        if (objectToApplyEffect.TryGetComponent(out IBloodContent bloodContent))
            _bloodContent = bloodContent;
    }
    
    public override bool HandleEffect(GameObject objectToApplyEffect)
    {
        if (_bloodContent is null)
            return true;
        //check is there is enough blood value
        if (_bloodContent.GetCurrentBloodValue() < bloodConsumption)
        {
            objectToApplyEffect.GetComponent<IEffectable>().RemoveEffect(this);
        }
        _elapsed += Time.deltaTime;
        //Deal damage by tick rate
        if (_elapsed >= tickRate)
        {
            _elapsed %= tickRate;
            _bloodContent.SubtractBlood(bloodConsumption);
        }
        return false;
    }
    public void SetDistance(float dist) => distance = dist;
    public void SetBloodConsumption(float blood) => bloodConsumption = blood;
    public void SetTarget(GameObject target) => heroToApply = target;
    public void SetAction(Action abilityDeactivation) => _abilityDeactivation = abilityDeactivation;
}
