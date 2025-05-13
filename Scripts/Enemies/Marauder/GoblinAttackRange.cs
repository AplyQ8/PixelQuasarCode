using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAttackRange : EnemyAttackRange
{

    protected override void StartAttack()
    {
        ((GoblinScript)enemyScript).EnterAttackState();
    }

    protected override LayerMask GetObstacleMask()
    {
        return LayerMask.GetMask("Obstacle", "Relief");
    }
}
