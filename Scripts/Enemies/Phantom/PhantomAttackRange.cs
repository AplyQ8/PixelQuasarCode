using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomAttackRange : EnemyAttackRange
{
    protected override void StartAttack()
    {
        ((PhantomScript)enemyScript).EnterAttackState();
    }

    protected override LayerMask GetObstacleMask()
    {
        return LayerMask.GetMask("Obstacle", "Relief");
    }
}
