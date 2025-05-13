using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class IncenserScript : KeepDistanceEnemy
{
    
    private static readonly int BuffPrepEnd = Animator.StringToHash("BuffPrepEnd");
    private enum BattleState
    {
        Movement,
        Cast
    }
    private BattleState battleState;
    
    // [Header("INCENSER")]
    private IncenserBuffZone incenseBuffZone;
    private bool incenseActive;

    protected override void EnemyStart()
    {
        battleState = BattleState.Cast;

        incenseBuffZone = transform.GetComponentInChildren<IncenserBuffZone>();

        incenseActive = false;
        
        moveDirTimer = 0;
        farAwayFromComfortZone = false;
        inComfortZone = false;
    }
    
    protected override void EnterAlertFromBattle()
    {
        StartCoroutine(TryToEnterAlert());
    }

    protected override void BattleStart()
    {
        if (!incenseActive)
        {
            animationController.ResetTrigger(BuffPrepEnd);
            animationController.SetTrigger(Attack);
            battleState = BattleState.Cast;
        }
        else
        {
            battleState = BattleState.Movement;
        }
    }

    protected override void BattleUpdate()
    {
        lookingDirection = (player.transform.position - transform.position).normalized;
        
        switch (battleState)
        {
            case BattleState.Movement:
                
                MoveToComfortZone();
                
                break;
            case BattleState.Cast:
                rigidBody.velocity = Vector2.zero;
                break;
        }
    }
    
    
    protected override void StunPreprocessing()
    {
        if (battleState == BattleState.Cast)
        {
            StopCast();
        }
    }
    
    protected override bool PulledPreprocessing()
    {

        StunPreprocessing();

        return true;
    }

    
    public void EnterMovementState()
    {
        moveDirTimer = 0;
        battleState = BattleState.Movement;
    }

    public void StopCast()
    {
        incenseActive = true;
        incenseBuffZone.StartIncense();
        animationController.SetTrigger(BuffPrepEnd);
        EnterMovementState();
    }

    IEnumerator StopIncenseDelay()
    {
        yield return new WaitForSeconds(alertDuration);
        if (state != EnemyState.Battle)
        {
            incenseActive = false;
            incenseBuffZone.StopIncense();
        }
    }
    
    IEnumerator TryToEnterAlert()
    {
        for (;;)
        {
            if (battleState == BattleState.Movement)
            {
                break;
            }
            yield return new WaitForSeconds(.01f);
        }

        StartCoroutine(StopIncenseDelay());
        EnterState(EnemyState.Alert);
    }
    
}
