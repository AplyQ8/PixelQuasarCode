using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleZone : MonoBehaviour
{
    void Start()
    {
    }

    public bool IsPlayerInside()
    {
        BattleZonePart[] battleZoneParts = GetComponentsInChildren<BattleZonePart>();
        
        foreach (var zonePart in battleZoneParts)
        {
            if (zonePart.IsPlayerInside())
            {
                return true;
            }
        }

        return false;
    }

    public bool IsEnemyInside(Transform enemyTransform)
    {
        BattleZonePart[] battleZoneParts = GetComponentsInChildren<BattleZonePart>();
        
        foreach (var zonePart in battleZoneParts)
        {
            if (zonePart.IsEnemyInside(enemyTransform))
            {
                return true;
            }
        }

        return false;
    }
    
}
