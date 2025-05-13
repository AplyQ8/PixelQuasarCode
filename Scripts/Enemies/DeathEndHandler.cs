using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEndHandler : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemyScript = animator.GetComponentInParent<EnemyScript>();
        enemyScript.DeathEnd();
    }
}
