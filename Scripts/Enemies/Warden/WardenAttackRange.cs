using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardenAttackRange : EnemyAttackRange
{

    protected override void StartAttack()
    {
        ((WardenScript)enemyScript).EnterAttackState();
    }

    protected override LayerMask GetObstacleMask()
    {
        return LayerMask.GetMask("Obstacle", "Relief");
    }
}
