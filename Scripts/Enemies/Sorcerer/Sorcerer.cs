using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Sorcerer : KeepDistanceEnemy
{
    
    private static readonly int AfterAttack = Animator.StringToHash("AfterAttack");
    private static readonly int AttackDirX = Animator.StringToHash("AttackDirX");
    private static readonly int AttackDirY = Animator.StringToHash("AttackDirY");
    private enum BattleState
    {
        Movement,
        Attack,
        AfterAttack
    }
    private BattleState battleState;
    
    private ObjectPool energySpherePool;
    
    [Header("SORCERER")]
    [SerializeField] private GameObject energySpherePrefab;
    
    [SerializeField] private float attackCooldown;
    private float attackCooldownTimer;
    [SerializeField] private float attackPrepDuration;
    private float attackPrepTimer;
    [SerializeField] private float attackAfterDuration;
    private float attackAfterTimer;
    
    private bool readyToAttack;

    protected override void EnemyStart()
    {
        battleState = BattleState.Movement;
        
        energySpherePool = gameObject.GetComponent<ObjectPool>();
        energySpherePool.InitializePool("SorcererSphere", 10, energySpherePrefab);
        
        readyToAttack = false;
        
        moveDirTimer = 0;
        farAwayFromComfortZone = false;
        inComfortZone = false;
    }
    
    protected override void BattleStart()
    {
        moveDirTimer = 0;
        attackCooldownTimer = attackCooldown;
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
                
                if (readyToAttack)
                {
                    battleState = BattleState.Attack;
                    EnterAttackState();
                }
                else
                {
                    attackCooldownTimer -= Time.deltaTime;
                    if (attackCooldownTimer <= 0)
                    {
                        readyToAttack = true;
                    }
                }

                break;
            
            case BattleState.Attack:
                rigidBody.velocity = Vector2.zero;

                attackPrepTimer -= Time.deltaTime;
                if (attackPrepTimer <= 0)
                {
                    Shoot();
                    attackAfterTimer = attackAfterDuration;
                    battleState = BattleState.AfterAttack;
                    animationController.SetTrigger(AfterAttack);
                }
                break;
            case BattleState.AfterAttack:
                rigidBody.velocity = Vector2.zero;

                // attackAfterTimer -= Time.deltaTime;
                // if (attackAfterTimer <= 0)
                // {
                //     EnterMovementState();
                // }
                break;
        }
        
    }
    
    protected override void StunPreprocessing()
    {
        
        if (battleState == BattleState.Attack)
        {
            attackCooldownTimer = attackCooldown / 2;
            EndShoot();
        }
        else if (battleState == BattleState.AfterAttack)
        {
            EndAfterAttack();
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
                EndShoot();
                readyToAttack = true;
                GetHitBounce(hitDirection);
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
        animationController.SetFloat(AttackDirX, direction.x);
        animationController.SetFloat(AttackDirY, direction.y);
        animationController.SetTrigger(Attack);
        // animationController.SetBool(ShootEnd, false);
        // PlayAttackAnim();
        attackPrepTimer = attackPrepDuration;
        battleState = BattleState.Attack;
    }

    public void Shoot()
    {
        Vector3 spawnPoint = player.transform.position;
        GameObject energySphere =
            energySpherePool.SpawnFromPool("SorcererSphere", spawnPoint, Quaternion.identity);
        energySphere.transform.SetParent(null);
        
        energySphere.GetComponent<SorcererSphere>().StartExplosion();
        energySphere.GetComponent<SorcererSphere>().SetPool(energySpherePool, "SorcererSphere");
            
        attackCooldownTimer = attackCooldown;
        readyToAttack = false;
    }

    private void EndShoot()
    {
        readyToAttack = false;
        // animationController.SetBool(ShootEnd, true);
        EnterMovementState();
    }

    public void EndAfterAttack()
    {
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
            if (battleState == BattleState.Movement || battleState == BattleState.Attack)
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
        }
    }
    
}
