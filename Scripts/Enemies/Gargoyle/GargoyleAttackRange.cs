using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargoyleAttackRange : EnemyAttackRange
{
    protected override void StartAttack()
    {
        ((GargoyleScript)enemyScript).EnterAttackState();
    }
    
}
