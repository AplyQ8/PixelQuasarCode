using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;
using Pathfinding;

public class GargoyleScript : EnemyScript
{
    
    private enum BattleState
    {
        Pursuit,
        Petrification,
        AfterPetrification,
        Attack,
        AfterAttack,
        Dash
    }
    
    private BattleState battleState;
    
    private static readonly int LookingDirX = Animator.StringToHash("LookingDirX");
    private static readonly int LookingDirY = Animator.StringToHash("LookingDirY");
    private static readonly int AttackDirX = Animator.StringToHash("AttackDirX");
    private static readonly int AttackDirY = Animator.StringToHash("AttackDirY");
    private static readonly int AttackPrep = Animator.StringToHash("AttackPrep");
    private static readonly int GroundHit = Animator.StringToHash("GroundHit");
    private static readonly int JumpOut = Animator.StringToHash("JumpOut");
    private static readonly int AttackEnd = Animator.StringToHash("AttackEnd");
    private static readonly int Heal = Animator.StringToHash("Heal");
    private static readonly int HealEnd = Animator.StringToHash("HealEnd");
    
    private BoxCollider2D obstacleCollider;
    private BoxCollider2D flyingObstacleCollider;
    private CircleCollider2D attackRangeCollider;
    private GargoyleSound gargoyleSound;
    
    [Header("GARGOYLE")]
    [SerializeField] private float innerPursuitRadius;
    [SerializeField] private float outerPursuitRadius;
    [SerializeField] private float approachSpeed;

    private int clockwise;
    
    [SerializeField] private float petrificationCooldown;
    private float petrificationCooldownTimer;
    [SerializeField] private float petrificationDuration;
    private float petrificationTimer;

    [SerializeField] private float verticalMoveSpeed;
    [SerializeField] private float verticalMoveDuration;
    
    [SerializeField] private float attackCooldown;
    private float attackCooldownTimer;
    private Vector3 attackFinalPoint;
    [SerializeField] private float attackSnap;
    private float attackJerk;
    private float attackAcceleration;
    private float attackSpeed;
    [SerializeField] private float attackStartVerticalSpeed;
    [SerializeField] private float attackVerticalAcceleration;
    private float attackVerticalSpeed;
    private List<BoxCollider2D> ignoreColliders;
    private List<GameObject> ignoreCollisionObjects;
    private GargoyleAttack gargoyleAttack;
    private bool attackInterrupted;
    private bool readyToAttack;

    [SerializeField] private float afterAttackDuration;
    private float afterAttackTimer;
    
    [SerializeField] private float dashDuration;
    private float dashTimer;
    private Vector2 dashDirection;
    [SerializeField] private float dashSpeed;
    
    [SerializeField] private float healPerSec;

    private LayerMask abyssMask;
    

    protected override void EnemyStart()
    {
        battleState = BattleState.Pursuit;
        clockwise = 1;
        
        obstacleCollider = transform.Find("ObstacleCollider").GetComponent<BoxCollider2D>();
        flyingObstacleCollider = transform.Find("FlyingObstacleCollider").GetComponent<BoxCollider2D>();
        attackRangeCollider = transform.Find("AttackRange").gameObject.GetComponent<CircleCollider2D>();
        gargoyleAttack = GetComponentInChildren<GargoyleAttack>();
        abyssMask = LayerMask.GetMask("Abyss");
        gargoyleSound = (GargoyleSound)enemySound;

        petrificationCooldownTimer = petrificationCooldown;
        attackCooldownTimer = attackCooldown;

        ignoreColliders = new List<BoxCollider2D>();
        ignoreCollisionObjects = new List<GameObject>();
        
        readyToAttack = false;
    }
    
    protected override void BattleStart()
    {
        battleState = BattleState.Pursuit;
        attackRangeCollider.enabled = false;
        attackCooldownTimer = attackCooldown/2;
        readyToAttack = false;
    }

    protected override void BattleUpdate()
    {
        lookingDirection = (player.transform.position - transform.position).normalized;
        
        switch (battleState)
        {
            case BattleState.Pursuit:
                
                animationController.SetFloat(LookingDirX, lookingDirection.normalized.x, 0f, Time.fixedDeltaTime);
                animationController.SetFloat(LookingDirY, lookingDirection.normalized.y, 0f, Time.fixedDeltaTime);
                
                SetMoveDirection();
                
                if (!readyToAttack)
                {

                    attackCooldownTimer -= Time.deltaTime;
                    if (attackCooldownTimer <= 0)
                    {
                        readyToAttack = true;
                        attackRangeCollider.enabled = true;
                    }
                }
                

                petrificationCooldownTimer -= Time.deltaTime;
                if (petrificationCooldownTimer <= 0)
                {
                    EnterPetrificationState();
                }

                break;
            
            case BattleState.Petrification:
                petrificationTimer -= Time.deltaTime;
                if (petrificationTimer <= 0)
                {
                    petrificationCooldownTimer = petrificationCooldown;
                    SetCanBeHooked(true);
                    canBeStunned = true;
                    animationController.SetTrigger(HealEnd);
                    // VerticalMove(Vector2.up);
                    battleState = BattleState.AfterPetrification;
                    // battleState = BattleState.Pursuit;
                }
                PetrificationHeal();
                break;
            
            case BattleState.Attack:
                
                if ((bodyBottom.position - attackFinalPoint).magnitude < 0.2 || attackInterrupted)
                {
                    rigidBody.velocity = Vector2.zero;
                    RestoreIgnoredColliders();
                    
                    attackCooldownTimer = attackCooldown;
                    if (!attackInterrupted)
                    {
                        gargoyleSound.PlayFallImpactSound();
                        gargoyleAttack.StartAttack();
                        EnterAfterAttackState();
                    }
                    else
                    {
                        battleState = BattleState.Pursuit;
                    }
                }
                else
                {
                    UpdateAttackSpeed(); 
                }
                break;
            
            case BattleState.AfterAttack:
                afterAttackTimer -= Time.deltaTime;
                if (afterAttackTimer <= 0)
                {
                    EnterDashState();
                }
                break;
            
            case BattleState.Dash:
                rigidBody.velocity -= rigidBody.velocity.normalized * (dashSpeed - moveScript.GetCurrentMoveSpeed()) / dashDuration *
                                      Time.deltaTime;
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    animationController.SetTrigger(AttackEnd);
                    battleState = BattleState.Pursuit;
                }
                break;

        }
        
    }


    protected override void StunPreprocessing()
    {
        switch (battleState)
        {
            case BattleState.Attack:
                AttackInterrupted();
                break;
            case BattleState.Dash:
                dashTimer = 0;
                break;
            case BattleState.AfterPetrification:
                EnterPursuit();
                break;
            
        }
    }
    
    protected override bool PulledPreprocessing()
    {
        switch (battleState)
        {
            case BattleState.Attack:
                AttackInterrupted();
                break;
            case BattleState.Dash:
                dashTimer = 0;
                break;
            case BattleState.Petrification:
                gargoyleSound.PlayHealingGetHookedSound();
                break;
            case BattleState.AfterPetrification:
                EnterPursuit();
                break;
                
        }
        
        return canBeHooked;
    }
    
    public override void GetHitPreprocessing(Vector2 hitDirection)
    {
        switch (battleState)
        {
            case BattleState.Pursuit:
                GetHitBounce(hitDirection);
                break;
            default:
                break;
        }
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
            yield return new WaitForSeconds(.01f);
        }
        
        EnterState(EnemyState.Alert);
    }

    public void EnterAttackState()
    {
        attackRangeCollider.enabled = false;
        if (state != EnemyState.Battle)
            return;
        
        battleState = BattleState.Attack;
        readyToAttack = false;
        gargoyleSound.PlayFallingSound();
        
        attackFinalPoint = playerBottom.position;
        Vector2 attackDir = attackFinalPoint - bodyBottom.position;
        if (attackDir.magnitude > outerPursuitRadius)
        {
            attackFinalPoint = bodyBottom.position + (Vector3)attackDir.normalized * outerPursuitRadius;
        }
        
        if (Physics2D.OverlapCircle(attackFinalPoint, 0.01f, abyssMask))
        {
            battleState = BattleState.Pursuit;
            attackCooldownTimer = 0.1f;
            return;
        }
        
        animationController.SetFloat(AttackDirX, lookingDirection.normalized.x, 0f, Time.fixedDeltaTime);
        animationController.SetFloat(AttackDirY, lookingDirection.normalized.y, 0f, Time.fixedDeltaTime);
        animationController.ResetTrigger(Attack);
        animationController.ResetTrigger(AttackEnd);
        animationController.ResetTrigger(GroundHit);
        animationController.ResetTrigger(JumpOut);
        animationController.SetTrigger(AttackPrep);
        
        attackSpeed = 0;
        attackAcceleration = 0;
        attackJerk = 0;
        attackVerticalSpeed = attackStartVerticalSpeed;

        ignoreColliders.Clear();
        ignoreCollisionObjects.Clear();
        obstacleCollider.enabled = false;
        flyingObstacleCollider.enabled = true;

        // obstacleCollider.enabled = false;
        // obstacleCollider.enabled = true;

        attackInterrupted = false;
        
        rigidBody.velocity = Vector2.zero;
    }


    private void AttackInterrupted()
    {
        if (attackInterrupted)
            return;
        attackInterrupted = true;
        animationController.SetTrigger(GroundHit);
        animationController.SetTrigger(JumpOut);
        animationController.SetTrigger(AttackEnd);
    }

    private void EnterPetrificationState()
    {
        battleState = BattleState.Petrification;
        gargoyleSound.PlayHealingSound();
        animationController.SetTrigger(Heal);
        petrificationTimer = petrificationDuration;
        rigidBody.velocity = Vector2.zero;
        VerticalMove(Vector2.down);
        SetCanBeHooked(false);
        canBeStunned = false;
    }

    private void EnterAfterAttackState()
    {
        battleState = BattleState.AfterAttack;
        animationController.SetTrigger(GroundHit);
        afterAttackTimer = afterAttackDuration;
        rigidBody.velocity = Vector2.zero;
    }

    private void EnterDashState()
    {
        battleState = BattleState.Dash;
        gargoyleSound.PlayDashSound();
        
        // dashDirection = Vector2.up;
        dashDirection = Random.insideUnitCircle.normalized;
        RaycastHit2D hit = Physics2D.Raycast(bodyBottom.position, dashDirection,
            innerPursuitRadius, obstacleReliefMask);
        if (hit.transform != null)
        {
            hit = Physics2D.Raycast(bodyBottom.position, -dashDirection,
                innerPursuitRadius, obstacleReliefMask);
            if (hit.transform == null)
            {
                dashDirection = -dashDirection;
            }
        }
        
        rigidBody.velocity = dashDirection * dashSpeed;
        dashTimer = dashDuration;
        
        animationController.SetTrigger(JumpOut);
    }

    public void EnterPursuit()
    {
        battleState = BattleState.Pursuit;
    }

    private void PetrificationHeal()
    {
        float healValue = healPerSec * Time.deltaTime;
        if (TryGetComponent(out IHealable healable))
        {
            healable.TakeHeal(healValue);
        }
        
    }

    private void SetMoveDirection()
    {
        Vector2 vectorToPlayer = (playerBottom.position - bodyBottom.position);

        Vector2 tangentVector = Vector2.Perpendicular(vectorToPlayer.normalized);
        tangentVector *= clockwise;

        rigidBody.velocity = moveScript.GetCurrentMoveSpeed() * tangentVector;
        
        float distanceToPlayer = vectorToPlayer.magnitude;
        if (distanceToPlayer > outerPursuitRadius)
        {
            rigidBody.velocity += vectorToPlayer.normalized * approachSpeed;
        }
        else if (distanceToPlayer < innerPursuitRadius)
        {
            rigidBody.velocity -= vectorToPlayer.normalized * approachSpeed;
        }
        
    }

    private void ChangeRotationDirection()
    {
        clockwise *= -1;
    }

    private void UpdateAttackSpeed()
    {
        attackJerk += attackSnap * Time.deltaTime;
        attackAcceleration += attackJerk * Time.deltaTime;
        attackSpeed += attackAcceleration * Time.deltaTime;
        Vector2 attackDirection = (attackFinalPoint - bodyBottom.position).normalized;
        rigidBody.velocity = attackDirection * attackSpeed;
        
        if (attackVerticalSpeed > 0)
        {
            attackVerticalSpeed -= attackVerticalAcceleration * Time.deltaTime;
            // transform.position += new Vector3(0, attackVerticalSpeed * Time.deltaTime, 0);
            rigidBody.velocity += attackVerticalSpeed*Vector2.up;
        }
    }

    public override bool CanFall()
    {
        return base.CanFall() && (state != EnemyState.Battle || battleState != BattleState.Attack);
    }

    public void VerticalMove(Vector2 direction)
    {
        rigidBody.velocity += direction * verticalMoveSpeed;
        StartCoroutine(VerticalMoveEnd(verticalMoveDuration));
    }

    IEnumerator VerticalMoveEnd(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (battleState == BattleState.Petrification || battleState == BattleState.AfterPetrification)
        {
            rigidBody.velocity = Vector2.zero;
        }
    }

    private void RestoreIgnoredColliders()
    {
        foreach (var ignoreCollider in ignoreColliders)
        {
            Physics2D.IgnoreCollision(ignoreCollider, flyingObstacleCollider, false);
        }
        
        ignoreColliders.Clear();
        ignoreCollisionObjects.Clear();
        
        obstacleCollider.enabled = true;
        flyingObstacleCollider.enabled = false;
        
    }
    
    private void OnCollisionEnter2D(Collision2D collisionObj)
    {
        
        if (battleState == BattleState.Pursuit && 
                (collisionObj.gameObject.CompareTag("Obstacle") || collisionObj.gameObject.CompareTag("Relief")))
        {
            ChangeRotationDirection();
        }

        if (battleState == BattleState.Attack && attackVerticalSpeed > 0)
        {
            var ignoreCollider = collisionObj.transform.GetComponent<BoxCollider2D>();
            if (ignoreCollider == null)
                return;
            
            ignoreColliders.Add(ignoreCollider);
            ignoreCollisionObjects.Add(collisionObj.gameObject);
            Physics2D.IgnoreCollision(ignoreCollider, flyingObstacleCollider, true);
        }
        
        if (battleState == BattleState.Attack && attackVerticalSpeed <= 0 && 
            !ignoreCollisionObjects.Contains(collisionObj.gameObject))
        {
            AttackInterrupted();
        }
        
    }
    
}
