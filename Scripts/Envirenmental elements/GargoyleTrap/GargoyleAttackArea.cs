using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GargoyleAttackArea : MonoBehaviour
{
    [SerializeField] private List<Transform> spottedTargets;
    
    public UnityEvent OnNoTarget;
    public UnityEvent<List<Transform>> OnTargetSpotted;

    private void Awake()
    {
        spottedTargets = new List<Transform>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        spottedTargets.Add(col.transform);
        OnTargetSpotted?.Invoke(spottedTargets);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        spottedTargets.Remove(other.transform);
        if(spottedTargets.Count is 0)
            OnNoTarget?.Invoke();
    }
    
    
}
