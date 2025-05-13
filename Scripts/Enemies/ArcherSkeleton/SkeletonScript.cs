using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Utilities;

public class SkeletonScript : KeepDistanceEnemy
{
    private static readonly int Dash = Animator.StringToHash("Dash");
    private static readonly int DashEnd = Animator.StringToHash("DashEnd");
    private static readonly int ShootDirX = Animator.StringToHash("ShootDirX");
    private static readonly int ShootDirY = Animator.StringToHash("ShootDirY");
    private static readonly int ShootEnd = Animator.StringToHash("ShootEnd");
    private enum BattleState
    {
        Movement,
        Dash,
        Attack
    }

    private ArrowCharacteristics _arrowToShoot;
    private struct ArrowCharacteristics
    {
        public Vector2 ShootDirection;
        public float Angle;

        public ArrowCharacteristics(Vector2 dir, float angle)
        {
            ShootDirection = dir;
            Angle = angle;
        }
    }
    
    
    private EnemyProjectileSpawner arrowSpawner;
    private float centerBottomDistance;

    private BattleState battleState;

    [Header("SKELETON")]
    [SerializeField] private float attackCooldown;
    private bool readyToAttack;
    
    
    [SerializeField] private float dashCooldown;
    private float dashCooldownTimer;
    [SerializeField] private float dashCloseSpeedupCoef;
    [SerializeField] private float dashDuration;
    private float dashTimer;
    private Vector2 dashDirection;
    [SerializeField] private float dashSpeed;

    private SkeletonSound skeletonSound;
    
    private System.Random rnd = new System.Random();
    private Timer _attackCooldownTimer;

    protected override void EnemyStart()
    {
        arrowSpawner = GetComponent<EnemyProjectileSpawner>();
        
        battleState = BattleState.Movement;

        skeletonSound = (SkeletonSound)enemySound;

        centerBottomDistance = (transform.position - bodyBottom.position).magnitude;
        
        dashCooldownTimer = dashCooldown;
        readyToAttack = false;

        moveDirTimer = 0;
        farAwayFromComfortZone = false;
        inComfortZone = false;
    }

    protected override void BattleStart()
    {
        moveDirTimer = 0;
        _attackCooldownTimer = new Timer(attackCooldown);
        _attackCooldownTimer.OnTimerDone += ReadyToAttack;
        _attackCooldownTimer.StartTimer();
        _attackCooldownTimer.SetLeftTime(attackCooldown/3);
        
        StartCoroutine(TimerUpdater());
        StartCoroutine(SpeedUpDashCooldown());
    }
    
    protected override void EnterAlertFromBattle()
    {
        StartCoroutine(TryToEnterAlert());
    }

    protected override void BattleUpdate()
    {
        lookingDirection = (player.transform.position - transform.position).normalized;
        
        switch (battleState)
        {
            case BattleState.Movement:
                
                MoveToComfortZone();
                
                
                // dashCooldownTimer -= Time.deltaTime;
                // if (dashCooldownTimer <= 0)
                // {
                //     dashDirection = rigidBody.velocity.normalized;
                //     dashTimer = dashDuration;
                //     spriteRenderer.color = new Color(1, 0.9f, 0.6f, 1);
                //     battleState = BattleState.Dash;
                //     animationController.SetTrigger(Dash);
                //     return;
                // }
                if (readyToAttack)
                {
                    battleState = BattleState.Attack;
                    EnterAttackState();
                }

                break;
            
            case BattleState.Dash:
                rigidBody.velocity = dashDirection * dashSpeed;
                dashTimer -= Time.deltaTime;
                moveDirTimer -= Time.deltaTime;
                //attackCooldownTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    animationController.SetTrigger(DashEnd);
                    dashCooldownTimer = dashCooldown;
                    dashCooldownTimer += dashCooldown * 0.4f * rnd.Next(-50, 50) / 100;
                    EnterMovementState();
                }
                break;
            
            case BattleState.Attack:
                Vector2 direction = (player.transform.position - transform.position).normalized;
                dashCooldownTimer -= Time.deltaTime;
                animationController.SetFloat(ShootDirX, direction.x, .1f, Time.deltaTime);
                animationController.SetFloat(ShootDirY, direction.y, .1f, Time.deltaTime);
                rigidBody.velocity = Vector2.zero;
                break;
        }
        
    }
    
    protected override bool PulledPreprocessing()
    {

        StunPreprocessing();

        return true;
    }

    protected override void StunPreprocessing()
    {
        if (battleState == BattleState.Attack)
        {
            skeletonSound?.StopBowLoadSound();
            _attackCooldownTimer.StartTimer();
            _attackCooldownTimer.SetLeftTime(attackCooldown/2);
            EndShoot();
        }
    }
    
    public override void GetHitPreprocessing(Vector2 hitDirection)
    {
        switch (battleState)
        {
            case BattleState.Attack:
                skeletonSound?.StopBowLoadSound();
                EndShoot();
                readyToAttack = true;
                GetHitBounce(hitDirection);
                break;
            case BattleState.Dash:
                break;
            default:
                GetHitBounce(hitDirection);
                break;
        }
    }

    public void EnterAttackState()
    {
        readyToAttack = false;
        Vector2 direction = (player.transform.position - transform.position).normalized;
        animationController.SetFloat(ShootDirX, direction.x);
        animationController.SetFloat(ShootDirY, direction.y);
        animationController.SetBool(ShootEnd, false);
        PlayAttackAnim();
        battleState = BattleState.Attack;
    }

    public void Shoot()
    {
        try
        {
            // var arrow = Instantiate(arrowPref, transform.position, Quaternion.identity);
            var arrow = arrowSpawner.pool.Get();
            float angle = EnemyUtility.AngleBetweenTwoPoints(player.transform.position, transform.position);
            Vector2 direction = (player.transform.position - transform.position).normalized;
            _arrowToShoot = new ArrowCharacteristics(direction, angle);
            arrow.GetComponent<SkeletonArrow>().SetDirection(
                _arrowToShoot.ShootDirection, 
                _arrowToShoot.Angle);
            
            arrow.SetShadowOffset(centerBottomDistance);
            arrow.CheckStartCollision(bodyBottom);
            
            _attackCooldownTimer.StartTimer();
            EndShoot();
        }
        catch (Exception e)
        {
            Debug.Log("Something went wrong while shooting" +
                      $"in {gameObject.name}"+
                      $"type of error {e}");
        }
    }

    private void EndShoot()
    {
        readyToAttack = false;
        animationController.SetBool(ShootEnd, true);
        EnterMovementState();
    }
    

    public void EnterMovementState()
    {
        moveDirTimer = 0;
        //animationController.SetTrigger(Chase);
        PlayChaseAnim();
        battleState = BattleState.Movement;
    }
    
    IEnumerator TryToEnterAlert()
    {
        for (;;)
        {
            if (battleState != BattleState.Dash)
            {
                break;
            }
            yield return new WaitForSeconds(.01f);
        }
        EnterState(EnemyState.Alert);
    }
    

    private void OnCollisionEnter2D(Collision2D collisionObj)
    {
        if (collisionObj.gameObject.CompareTag("Obstacle") || collisionObj.gameObject.CompareTag("Relief")
                                                           || collisionObj.gameObject.CompareTag("Player"))
        {
            moveDirTimer = 0;
            dashTimer = 0;
        }
    }

    private IEnumerator TimerUpdater()
    {
        while (true)
        {
            _attackCooldownTimer.Tick();
            yield return new WaitForEndOfFrame();
        }
    }
    
    private IEnumerator SpeedUpDashCooldown()
    {
        while (true)
        {
            float distanceToPlayer = (player.transform.position - transform.position).magnitude;
            if (distanceToPlayer < innerComfortZoneRadius)
            {
                dashCooldownTimer *= dashCloseSpeedupCoef;
            }
            yield return new WaitForSeconds(1);
        }
    }
    
    private void ReadyToAttack() => readyToAttack = true;

    private void OnDestroy()
    {
        StopCoroutine(TimerUpdater());
        StopCoroutine(SpeedUpDashCooldown());
        if (arrowSpawner != null && arrowSpawner.pool != null)
        {
            arrowSpawner.pool.Dispose();
        }
    }
}
