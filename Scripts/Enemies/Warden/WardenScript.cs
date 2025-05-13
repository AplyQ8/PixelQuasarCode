using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;
using Pathfinding;

public class WardenScript : EnemyScript
{
    private enum BattleState
    {
        Pursuit,
        Shooting,
        Attack
    }
    
    private static readonly int AttackDirX = Animator.StringToHash("AttackDirX");
    private static readonly int AttackDirY = Animator.StringToHash("AttackDirY");
    protected static readonly int AttackEnd = Animator.StringToHash("AttackEnd");
    protected static readonly int ShootingPrep = Animator.StringToHash("ShootingPrep");
    protected static readonly int Shooting = Animator.StringToHash("Shooting");
    protected static readonly int ShootingAfter = Animator.StringToHash("ShootingAfter");

    private BattleState battleState;
    
    private CircleCollider2D attackRangeCollider;
    
    private LayerMask playerMask;

    [Header("WARDEN")] 
    [SerializeField] private float shootingCooldown;
    private float shootingCooldownTimer;
    private int shotsLeft;
    [SerializeField] private float shotMinDistance;
    [SerializeField] private float betweenShotsDelay;
    
    [SerializeField] private float attackCooldown;
    private float attackCooldownTimer;
    private bool readyToAttack;
    private Vector2 attackDirection;
    public float attackAngle;
    private Vector3 attackGoalPoint;
    
    
    private EnemyProjectileSpawner boltSpawner;
    private float centerBottomDistance;
    
    [Serializable] private class WardenBoltShift
    {
        public Vector2 top;
        public Vector2 topRight;
        public Vector2 right;
        public Vector2 bottomRight;
        public Vector2 bottom;
        public Vector2 bottomLeft;
        public Vector2 left;
        public Vector2 topLeft;
    } 
    [SerializeField] private WardenBoltShift boltShifts;

    private WardenAttack wardenAttack;
    private ICanAttack attackScript;

    protected override void EnemyStart()
    {
        boltSpawner = GetComponent<EnemyProjectileSpawner>();
        attackRangeCollider = transform.Find("AttackRange").gameObject.GetComponent<CircleCollider2D>();
        attackScript = gameObject.transform.GetComponent<ICanAttack>();
        wardenAttack = GetComponentInChildren<WardenAttack>();
        
        playerMask = LayerMask.GetMask("Player");
        
        centerBottomDistance = (transform.position - bodyBottom.position).magnitude;
        
        SetCanBeHooked(false);
        
        battleState = BattleState.Pursuit;

        shootingCooldownTimer = shootingCooldown;
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
                destinationPoint = playerBottom.position;
                MoveToDestination();

                shootingCooldownTimer -= Time.deltaTime;
                attackCooldownTimer -= Time.deltaTime;
                
                if (shootingCooldownTimer <= 0)
                {
                    float distanceToPlayer = (playerBottom.transform.position - transform.position).magnitude;
                    if (distanceToPlayer >= shotMinDistance)
                    {
                        battleState = BattleState.Shooting;
                        EnterShootingState();
                        return;
                    }
                }
                
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
                break;
            case BattleState.Shooting:
                rigidBody.velocity = Vector2.zero;
                Vector2 direction = (player.transform.position - transform.position).normalized;
                animationController.SetFloat(AttackDirX, direction.x, .1f, Time.deltaTime);
                animationController.SetFloat(AttackDirY, direction.y, .1f, Time.deltaTime);
                break;

        }
        
    }
    
    
    protected override void StunPreprocessing()
    {
        if (battleState == BattleState.Attack)
        {
            animationController.ResetTrigger(AttackEnd);
            attackCooldownTimer = attackCooldown*0.66f;
            battleState = BattleState.Pursuit;
        }
        else if (battleState == BattleState.Shooting)
        {
            animationController.ResetTrigger(ShootingPrep);
            animationController.ResetTrigger(Shooting);
            animationController.ResetTrigger(ShootingAfter);
            shootingCooldownTimer = shootingCooldown*0.5f;
            battleState = BattleState.Pursuit;
        }
    }
    
    public void EnterAttackState()
    {
        if(state != EnemyState.Battle)
            return;

        if (battleState == BattleState.Shooting)
        {
            animationController.ResetTrigger(Shooting);
            shootingCooldownTimer = shootingCooldown * 0.5f;
        }
        
        // attackDirection = GetAttackDirection();
        // Vector2 directionVector = (player.transform.position - transform.position).normalized;
        // animationController.SetFloat(AttackDirX, directionVector.x, 0f, Time.deltaTime);
        // animationController.SetFloat(AttackDirY, directionVector.y, 0f, Time.deltaTime);
        // PlayAttackAnim();
        // attackEnd = false;
        // attackInterrupted = false;
        // attackRangeCollider.enabled = false;
        
        // attackDirection = GetAttackDirection();
        attackGoalPoint = playerBottom.position;
        
        readyToAttack = false;
        attackRangeCollider.enabled = false;
        // StartCoroutine(AttackPrep());
        var directionVector = (attackGoalPoint - bodyBottom.position).normalized;
        animationController.SetFloat(AttackDirX, directionVector.x, 0f, Time.deltaTime);
        animationController.SetFloat(AttackDirY, directionVector.y, 0f, Time.deltaTime);
        PlayAttackAnim();
        battleState = BattleState.Attack;
    }

    public void Attack()
    {
        wardenAttack.AttackArea(attackGoalPoint);
    }

    
    public void EndAttack()
    {
        animationController.SetTrigger(AttackEnd);
        attackCooldownTimer = attackCooldown;
        battleState = BattleState.Pursuit;
    }


    private void EnterShootingState()
    {
        shotsLeft = 3;
        
        Vector2 direction = (player.transform.position - transform.position).normalized;
        animationController.SetFloat(AttackDirX, direction.x);
        animationController.SetFloat(AttackDirY, direction.y);
        
        animationController.SetTrigger(ShootingPrep);
        // StartCoroutine(ShootingPrep());
    }

    public void EndShootingPrep()
    {
        animationController.SetTrigger(Shooting);
    }

    public void Shoot()
    {
        var bolt = boltSpawner.pool.Get();
        float angle = EnemyUtility.AngleBetweenTwoPoints(player.transform.position, transform.position);
        Vector2 direction = (player.transform.position - transform.position).normalized;
        
        CorrectBoltStartPosition(bolt, angle);
        float correctedAngle = EnemyUtility.AngleBetweenTwoPoints(player.transform.position, bolt.transform.position);
        Vector2 correctedDirection = (player.transform.position - bolt.transform.position).normalized;
        bolt.GetComponent<WardenBolt>().SetDirection(correctedDirection, correctedAngle);
        bolt.SetShadowOffset(centerBottomDistance);
        bolt.CheckStartCollision(bodyBottom);

        shotsLeft -= 1;
        float distanceToPlayer = (playerBottom.transform.position - transform.position).magnitude;
            
        if (shotsLeft > 0 && distanceToPlayer >= shotMinDistance)
        {
            StartCoroutine(BetweenShotsPrep());
        }
        else
        {
            shootingCooldownTimer = shootingCooldown;
            EndShoot();
        }
    }

    private void CorrectBoltStartPosition(EnemyProjectile bolt, float angle)
    {
        var direction = EnemyUtility.AngleToDirection(angle);

        Vector2 shift = Vector2.zero;

        switch (direction)
        {
            case EnemyUtility.Direction.North:
                shift = boltShifts.top;
                break;
            case EnemyUtility.Direction.NorthEast:
                shift = boltShifts.topRight;
                break;
            case EnemyUtility.Direction.East:
                shift = boltShifts.right;
                break;
            case EnemyUtility.Direction.SouthEast:
                shift = boltShifts.bottomRight;
                break;
            case EnemyUtility.Direction.South:
                shift = boltShifts.bottom;
                break;
            case EnemyUtility.Direction.SouthWest:
                shift = boltShifts.bottomLeft;
                break;
            case EnemyUtility.Direction.West:
                shift = boltShifts.left;
                break;
            case EnemyUtility.Direction.NorthWest:
                shift = boltShifts.topLeft;
                break;
        }

        bolt.transform.position = bolt.transform.position + (Vector3)shift;
    }
    
    private void EndShoot()
    {
        animationController.SetTrigger(ShootingAfter);
    }

    public void EndShootingAfter()
    {
        battleState = BattleState.Pursuit;
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
        float distanceToSlow = 3f;
        float slowSpeed = 0;
        for(;;)
        {
            if (state == EnemyState.Battle && battleState == BattleState.Pursuit)
            {
                
                if ((playerBottom.transform.position - bodyBottom.position).magnitude < distanceToSlow && 
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


    IEnumerator AttackPrep()
    {
        yield return new WaitForSeconds(1f);
        wardenAttack.AttackArea(attackGoalPoint);
        StartCoroutine(EndAttackPrep());
    }
    IEnumerator EndAttackPrep()
    {
        yield return new WaitForSeconds(0.3f);
        EndAttack();
    }
    
    // IEnumerator ShootingPrep()
    // {
    //     yield return new WaitForSeconds(1);
    //     Shoot();
    // }
    
    IEnumerator BetweenShotsPrep()
    {
        yield return new WaitForSeconds(betweenShotsDelay);
        if(battleState == BattleState.Shooting)
            animationController.SetTrigger(Shooting);
    }
    
    
}
