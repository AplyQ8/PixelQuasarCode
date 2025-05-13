using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using Status_Effect_System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Fear Effect")]
public class FearEffect : StatusEffectData
{
    [SerializeField] private GameObject objectToRunFrom;
    
    public override void StartEffect(GameObject objectToApplyEffect)
    {
        if (objectToApplyEffect.TryGetComponent(out IFearable fearable))
        {
            fearable.ChangeFeared(true, objectToRunFrom.transform);
        }

        CalculateLifeTimeWithResist(objectToApplyEffect);
    }
    public override void RestartEffect(GameObject objectToApplyEffect)
    {
        CalculateLifeTimeWithResist(objectToApplyEffect);
    }
    public override void EndEffect(GameObject objectToApplyEffect)
    {
        if (objectToApplyEffect.TryGetComponent(out IFearable fearable))
        {
            fearable.ChangeFeared(false, objectToRunFrom.transform);
        }
    }

    public void SetObjectToRunFrom(GameObject obj) => objectToRunFrom = obj;
    public GameObject GetObjectToRunFrom() => objectToRunFrom;
}
