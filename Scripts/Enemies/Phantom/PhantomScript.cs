using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using ObjectLogicInterfaces;
using ObjectLogicRealization.Health;
using UnityEngine;

public class PhantomScript : EnemyScript
{
    private enum BattleState
    {
        Pursuit,
        Vanished,
        DashAttackPreparation,
        DashAttack,
        Attack
    }

    
    private static readonly int AttackDirX = Animator.StringToHash("AttackDirX");
    private static readonly int AttackDirY = Animator.StringToHash("AttackDirY");
    private static readonly int Appearance = Animator.StringToHash("Appearance");
    protected static readonly int AttackEnd = Animator.StringToHash("AttackEnd");
    protected static readonly int DashAttack = Animator.StringToHash("DashAttack");
    protected static readonly int DashAttackEnd = Animator.StringToHash("DashAttackEnd");
    private static readonly int Disappearance = Animator.StringToHash("Disappearance");
    
    private BattleState battleState;
    
    private System.Random rnd = new System.Random();

    private BoxCollider2D boxCollider;
    private BoxCollider2D playerBoxCollider;
    private CircleCollider2D attackRangeCollider;

    private PhantomSound phantomSound;

    private GameObject enemyIndicator;
    
    [Header("PHANTOM")]
    [SerializeField] private float dashAttackCooldown;
    private float dashAttackCooldownTimer;
    [SerializeField] private float vanishedDuration;
    private float vanishedTimer;
    [SerializeField] private float dashAttackPrepDuration;
    private float dashAttackPrepTimer;
    private bool playerVanishedBeforeDash;
    [SerializeField] private float dashAttackDuration;
    private float dashAttackTimer;
    private Vector3 dashAttackStopPoint;

    private PhantomObstacleCollider dashObstacleCollider;

    private Transform dashAttackCollider;
    
    [SerializeField] private float dashAttackSpeed;
    private Vector2 dashAttackDirection;
    [SerializeField] private float dashAttackDistance;
    [SerializeField] private float dashAttackDamage;
    
    private bool readyToAttack;
    public float attackDuration;
    public float attackPrepDuration;
    private float attackTimer;
    [SerializeField] private float attackCooldown;
    private float attackCooldownTimer;
    private PhantomAttack phantomAttack;
    private bool attackInterrupted;

    [SerializeField] private int attackInterruptionProb;
    private float currentAttackInterruptionProb;

    private bool lowHpAttackComplete;
    

    protected override void EnemyStart()
    {
        battleState = BattleState.Pursuit;
        
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        playerBoxCollider = player.transform.Find("ObstacleCollider").GetComponent<BoxCollider2D>();
        dashObstacleCollider = GetComponentInChildren<PhantomObstacleCollider>();
        phantomSound = (PhantomSound)enemySound;

        dashAttackCollider = transform.Find("DashAttackCollider");

        enemyIndicator = transform.Find("EnemyIndicater").gameObject;

        dashAttackCooldownTimer = dashAttackCooldown;
        
        attackRangeCollider = transform.Find("AttackRange").gameObject.GetComponent<CircleCollider2D>();
        phantomAttack = GetComponentInChildren<PhantomAttack>();
        attackCooldownTimer = attackCooldown;
        readyToAttack = false;

        currentAttackInterruptionProb = attackInterruptionProb;

        lowHpAttackComplete = false;
        
        transform.GetComponent<EnemyHealth>().TakeHeal(transform.GetComponent<EnemyHealth>().GetMaxHealthPoints());
        
        StartCoroutine(CheckDistanceToPlayer());
        StartCoroutine(CheckHp());
    }

    protected override void BattleStart()
    {
        battleState = BattleState.Pursuit;
        attackCooldownTimer = attackCooldown;
        readyToAttack = false;
    }
    

    protected override void BattleUpdate()
    {
        
        lookingDirection = (player.transform.position - transform.position).normalized;
        
        switch (battleState)
        {
            case BattleState.Pursuit:
                MoveToPlayer();
                
                dashAttackCooldownTimer -= Time.deltaTime;
                if (dashAttackCooldownTimer <= 0)
                {
                    vanishedTimer = vanishedDuration;
                    rigidBody.velocity = Vector2.zero;
                    boxCollider.enabled = false;
                    // spriteRenderer.enabled = false;
                    animationController.SetTrigger(Disappearance);
                    phantomSound.PlayDisappearanceSound();
                    attackRangeCollider.enabled = false;
                    readyToAttack = false;
                    enemyIndicator.SetActive(false);
                    attackCooldownTimer = attackCooldown;
                    dashAttackCooldownTimer = dashAttackCooldown;
                    playerVanishedBeforeDash = false;
                    battleState = BattleState.Vanished;
                    break;
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
            
            case BattleState.Vanished:
                vanishedTimer -= Time.deltaTime;
                if (vanishedTimer <= 0)
                {
                    transform.position = GetDashAttackStartPoint();
                    Vector2 directionVector = (playerBottom.position - bodyBottom.position).normalized;
                    animationController.SetFloat(AttackDirX, directionVector.x, 0f, Time.deltaTime);
                    animationController.SetFloat(AttackDirY, directionVector.y, 0f, Time.deltaTime);
                    phantomSound.PlayAppearanceSound();
                    animationController.ResetTrigger(DashAttackEnd);
                    animationController.ResetTrigger(DashAttack);
                    animationController.SetTrigger(Appearance);
                    dashAttackPrepTimer = dashAttackPrepDuration;
                    enemyIndicator.SetActive(true);
                    spriteRenderer.enabled = true;
                    // spriteRenderer.color = Color.blue;
                    // dashAttackDirection = (playerBottom.position - bodyBottom.position).normalized;
                    battleState = BattleState.DashAttackPreparation;
                }
                break;
            
            case BattleState.DashAttackPreparation:
                dashAttackPrepTimer -= Time.deltaTime;
                if (dashAttackPrepTimer <= 0)
                {
                    boxCollider.enabled = false;
                    if (!playerVanishedBeforeDash)
                    {
                        dashAttackDirection = (playerBottom.position - bodyBottom.position).normalized;
                        dashAttackTimer = dashAttackDuration;
                        
                        Physics2D.IgnoreCollision(playerBoxCollider, boxCollider, true);
                        phantomSound.PlayDashSwingSound(player.transform.position);
                        animationController.SetTrigger(DashAttack);
                        Vector2 directionVector = (playerBottom.position - bodyBottom.position).normalized;
                        animationController.SetFloat(AttackDirX, directionVector.x, 0f, Time.deltaTime);
                        animationController.SetFloat(AttackDirY, directionVector.y, 0f, Time.deltaTime);
                        
                        dashAttackCollider.gameObject.SetActive(true);
                        battleState = BattleState.DashAttack;
                    }
                    else
                    {
                        battleState = BattleState.Pursuit;
                    }
                }
                break;
            
            case BattleState.DashAttack:

                if (dashAttackTimer <= 0)
                {
                    boxCollider.enabled = true;
                    Physics2D.IgnoreCollision(playerBoxCollider, boxCollider, false);
                    battleState = BattleState.Pursuit;
                    dashAttackCooldownTimer = dashAttackCooldown;
                    animationController.SetTrigger(DashAttackEnd);
                    dashAttackCollider.gameObject.SetActive(false);
                    break;
                }
                rigidBody.velocity = dashAttackDirection * dashAttackSpeed;
                dashAttackTimer -= Time.deltaTime;
                
                break;
            
            case BattleState.Attack:
                rigidBody.velocity = Vector2.zero;
                attackTimer -= Time.deltaTime;
                dashAttackCooldownTimer -= Time.deltaTime;
                if (attackTimer <= 0)
                {
                    readyToAttack = false;
                    if (!attackInterrupted)
                    {
                        attackCooldownTimer = attackCooldown;
                        // animationController.SetTrigger(AttackEnd);
                    }
                    animationController.SetTrigger(AttackEnd);

                    battleState = BattleState.Pursuit;
                }

                break;
            
        }
        
    }

    
    protected override void EnemyTimersTickInStun()
    {
        dashAttackCooldownTimer -= Time.deltaTime;
    }
    
    protected override void EnterInterruptedBattle(BattleInterruptionSource source = BattleInterruptionSource.Default)
    {
        base.EnterInterruptedBattle(source);
        if (source == BattleInterruptionSource.Pull || source == BattleInterruptionSource.Stun)
        {
            switch (battleState)
            {
                case BattleState.Attack:
                    attackCooldownTimer = attackCooldown/2;
                    break;
            }
        }

    }

    protected override void StunPreprocessing()
    {
        if (battleState == BattleState.DashAttackPreparation)
        {
            // dashAttackTimer = 0;
            battleState = BattleState.Pursuit;
            dashAttackCooldownTimer = dashAttackCooldown;
            animationController.SetTrigger(DashAttack);
            animationController.SetTrigger(DashAttackEnd);
        }

        if (battleState == BattleState.Attack)
        {
            AttackInterrupted();
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
                if (rnd.Next(1, 101) < currentAttackInterruptionProb)
                {
                    currentAttackInterruptionProb -= 5;
                    AttackInterrupted();   
                    GetHitBounce(hitDirection);
                }
                else
                {
                    currentAttackInterruptionProb = attackInterruptionProb;
                    StartPassiveBounce(hitDirection);
                }
                break;
            case BattleState.DashAttack:
                break;
            case BattleState.DashAttackPreparation:
                break;
            default:
                GetHitBounce(hitDirection);
                break;
        }
    }
    
    
    protected override void EnterAlertFromBattle()
    {
        if (battleState == BattleState.DashAttackPreparation || battleState == BattleState.Vanished)
        {
            playerVanishedBeforeDash = true;
        }
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
            yield return new WaitForSeconds(.01f);
        }
        EnterState(EnemyState.Alert);
    }
    
    
    
    private Vector3 GetDashAttackStartPoint()
    {
        EnemyUtility.Direction bestDirection = EnemyUtility.Direction.East;
        float maxDistance = 0;
        bool severalBest = false;
        List<EnemyUtility.Direction> severalBestDirections = new List<EnemyUtility.Direction>();
        foreach (var dir in (EnemyUtility.Direction[])Enum.GetValues(typeof(EnemyUtility.Direction)))
        {
            RaycastHit2D hit = Physics2D.Raycast(playerBottom.position, EnemyUtility.DirectionToVector(dir),
                dashAttackDistance, obstacleReliefMask);
            float distanceToObstacle = hit.distance;
            if (hit.transform == null)
            {
                distanceToObstacle = dashAttackDistance;
                severalBest = true;
                severalBestDirections.Add(dir);
            }
            if (!severalBest && distanceToObstacle > maxDistance)
            {
                maxDistance = hit.distance;
                bestDirection = dir;
            }
        }
        
        if (severalBest)
        {
            int randomIndex = rnd.Next(0, severalBestDirections.Count);
            bestDirection = severalBestDirections[randomIndex];
            maxDistance = dashAttackDistance;
        }
        
        var bottomStartPoint = playerBottom.position + (Vector3)EnemyUtility.DirectionToVector(bestDirection) * maxDistance;
        return bottomStartPoint + (transform.position - bodyBottom.position);
    }
    
    
    public void EnterAttackState()
    {
        if (state != EnemyState.Battle)
        {
            readyToAttack = false;
            attackRangeCollider.enabled = false;
            attackCooldownTimer = attackCooldown/4;
            return;
        }
        
        float angle = EnemyUtility.AngleBetweenTwoPoints(player.transform.position, transform.position);
        EnemyUtility.Direction attackDirection = EnemyUtility.AngleToDirection(angle);
        attackTimer = attackDuration + attackPrepDuration;
        attackInterrupted = false;
        attackRangeCollider.enabled = false;
        battleState = BattleState.Attack;
        Vector2 directionVector = (player.transform.position - transform.position).normalized;
        animationController.SetFloat(AttackDirX, directionVector.x, 0f, Time.deltaTime);
        animationController.SetFloat(AttackDirY, directionVector.y, 0f, Time.deltaTime);
        animationController.ResetTrigger(AttackEnd);
        animationController.SetTrigger(Attack);
        phantomAttack.StartAttack(attackDirection);
        phantomSound.PlaySwordAppearanceSound();
    }

    public void AttackInterrupted()
    {
        if (attackInterrupted)
            return;
        bool hitPlayer = phantomAttack.GetAlreadyHit();
        attackInterrupted = true;
        attackTimer = 0;
        if (!hitPlayer)
        {
            attackCooldownTimer = 0.1f;
        }
        else
        {
            attackCooldownTimer = attackCooldown;
        }
    }

    public void EnableBoxCollider()
    {
        boxCollider.enabled = true;
    }
    
    

    public void ObstacleColliderEnterTrigger(Collider2D collisionObj)
    {
        if (battleState == BattleState.DashAttack && 
            (collisionObj.gameObject.CompareTag("Obstacle") || collisionObj.gameObject.CompareTag("Relief")))
        {
            dashAttackTimer = 0;
        }
    }

    // private void OnTriggerEnter2D(Collider2D collisionObj)
    // {
    //     if (battleState == BattleState.DashAttack && dashAttackTimer > 0 && collisionObj.gameObject.CompareTag("Player"))
    //     {
    //         if (NoObstaclesOnTheWay(obstacleReliefMask))
    //         {
    //             if (collisionObj.TryGetComponent(out IDamageable damageableObject))
    //             {
    //                 damageableObject.TakeDamage(dashAttackDamage, DamageTypeManager.DamageType.Default);
    //             }
    //         }
    //     }
    // }

    public void DashAttackTriggerEnter(Collider2D collisionObj)
    {
        if (battleState == BattleState.DashAttack && dashAttackTimer > 0 && collisionObj.gameObject.CompareTag("Player"))
        {
            if (NoObstaclesOnTheWay(obstacleReliefMask))
            {
                phantomSound.PlayDashDealDamageSound(player.transform.position);
                if (collisionObj.TryGetComponent(out IDamageable damageableObject))
                {
                    damageableObject.TakeDamage(dashAttackDamage, DamageTypeManager.DamageType.Default);
                }
            }
        }
    }
    
    
    IEnumerator CheckDistanceToPlayer()
    {
        float distanceToSlow = 2.5f;
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

    IEnumerator CheckHp()
    {
        for(;;)
        {
            if (!lowHpAttackComplete)
            {
                float hpPercent = transform.GetComponent<EnemyHealth>().GetCurrentHealth() /
                                  transform.GetComponent<EnemyHealth>().GetMaxHealthPoints();
                if (hpPercent <= 0.33)
                {
                    lowHpAttackComplete = true;
                    dashAttackCooldownTimer = 0;
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    

}
