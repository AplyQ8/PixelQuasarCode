using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GoblinScript : EnemyScript
{
    private enum BattleState
    {
        Pursuit,
        Attack
    }
    
    private static readonly int AttackDirX = Animator.StringToHash("AttackDirX");
    private static readonly int AttackDirY = Animator.StringToHash("AttackDirY");
    protected static readonly int AttackEnd = Animator.StringToHash("AttackEnd");

    private BattleState battleState;
    private CircleCollider2D attackRangeCollider;
    private GoblinSound goblinSound;

    private GoblinAttack goblinAttack;

    [Header("MARAUDER")]
    public float attackDuration;
    private bool attackEnd;
    [SerializeField] private float attackCooldown;
    private float attackCooldownTimer;
    private bool readyToAttack;
    private EnemyUtility.Direction attackDirection;
    private bool attackInterrupted;

    protected override void EnemyStart()
    {
        battleState = BattleState.Pursuit;
        attackRangeCollider = transform.Find("AttackRange").gameObject.GetComponent<CircleCollider2D>();
        goblinAttack = GetComponentInChildren<GoblinAttack>();
        goblinSound = (GoblinSound)enemySound;

        attackCooldownTimer = attackCooldown;
        readyToAttack = false;
        
        StartCoroutine(CheckDistanceToPlayer());
        
    }

    protected override void BattleUpdate()
    {
        lookingDirection = (player.transform.position - transform.position).normalized;
        
        switch (battleState)
        {
            case BattleState.Pursuit:
                MoveToPlayer();

                if (!readyToAttack)
                {
                    attackCooldownTimer -= Time.deltaTime;
                    if (attackCooldownTimer <= 0)
                    {
                        readyToAttack = true;
                        attackRangeCollider.enabled = true;
                    }
                }

                break;
            
            case BattleState.Attack:
                rigidBody.velocity = Vector2.zero;
                if (attackEnd)
                {
                    readyToAttack = false;
                    if (!attackInterrupted)
                    {
                        attackCooldownTimer = attackCooldown;
                        animationController.SetTrigger(AttackEnd);
                    }
                    battleState = BattleState.Pursuit;
                    // PlayChaseAnim();
                }

                break;
        }
        
    }
    
    protected override void EnterInterruptedBattle(BattleInterruptionSource source = BattleInterruptionSource.Default)
    {
        base.EnterInterruptedBattle(source);
        if (source == BattleInterruptionSource.Pull || source == BattleInterruptionSource.Stun)
        {
            switch (battleState)
            {
                case BattleState.Attack:
                    attackCooldownTimer = attackCooldown;
                    break;
            }
        }

    }

    protected override void StunPreprocessing()
    {
        if (battleState == BattleState.Attack)
        {
            InterruptAttack();
        }
    }
    
    protected override bool PulledPreprocessing()
    {
        StunPreprocessing();
        return true;
    }
    
    public override void GetHitPreprocessing(Vector2 hitDirection)
    {
        switch (battleState)
        {
            case BattleState.Attack:
                InterruptAttack();
                GetHitBounce(hitDirection);
                break;
            default:
                GetHitBounce(hitDirection);
                break;
        }
    }
    
    private void InterruptAttack()
    {
        if (attackInterrupted)
            return;
        bool hitPlayer = goblinAttack.GetAlreadyHit();
        attackInterrupted = true;
        EndAttack();
        animationController.SetTrigger(AttackEnd);
        if (!hitPlayer)
        {
            attackCooldownTimer = 0.1f;
        }
        else
        {
            attackCooldownTimer = attackCooldown;
        }
    }

    public void EnterAttackState()
    {
        if(state != EnemyState.Battle)
            return;
        
        // goblinSound?.PlayAttackPrepSound();
        
        attackDirection = GetAttackDirection();
        Vector2 directionVector = (player.transform.position - transform.position).normalized;
        animationController.SetFloat(AttackDirX, directionVector.x, 0f, Time.deltaTime);
        animationController.SetFloat(AttackDirY, directionVector.y, 0f, Time.deltaTime);
        animationController.ResetTrigger(AttackEnd);
        PlayAttackAnim();
        attackEnd = false;
        attackInterrupted = false;
        attackRangeCollider.enabled = false;
        battleState = BattleState.Attack;
    }

    public void StartAttack()
    {
        goblinAttack.StartAttack(attackDirection);
    }

    public void EndAttack()
    {
        goblinAttack.EndAttack();
        attackEnd = true;
    }

    private EnemyUtility.Direction GetAttackDirection()
    {
        float angle = EnemyUtility.AngleBetweenTwoPoints(player.transform.position, transform.position);
        return EnemyUtility.AngleToDirection(angle);
    }
    
    protected override void EnterAlertFromBattle()
    {
        StartCoroutine(TryToEnterAlert());
    }

    IEnumerator TryToEnterAlert()
    {
        for (;;)
        {
            if (battleState == BattleState.Pursuit)
            {
                break;
            }
            yield return new WaitForSeconds(.1f);
        }
        EnterState(EnemyState.Alert);
    }
    
    IEnumerator CheckDistanceToPlayer()
    {
        float distanceToSlow = 1.8f;
        float slowSpeed = 0;
        for(;;)
        {
            if (state == EnemyState.Battle && battleState == BattleState.Pursuit)
            {
                
                if ((player.transform.position - transform.position).magnitude < distanceToSlow && 
                        NoObstaclesOnTheWay(obstacleReliefMask))
                {
                    moveScript.SetSpeed(slowSpeed);
                } else
                {
                    moveScript.SetBaseSpeed();
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    
    
}
