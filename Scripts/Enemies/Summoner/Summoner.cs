using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Summoner : KeepDistanceEnemy
{
    private enum BattleState
    {
        Movement,
        Summon
    }
    private BattleState battleState;
    
    protected static readonly int AttackDirX = Animator.StringToHash("AttackDirX");
    protected static readonly int AttackDirY = Animator.StringToHash("AttackDirY");
    protected static readonly int AttackEnd = Animator.StringToHash("AttackEnd");
    
    private ObjectPool summonPool;
    [Header("PRIEST")]
    [SerializeField] private GameObject summonPrefab;
    
    [SerializeField] private float spawnSummonCooldown;
    private float spawnSummonCooldownTimer;
    private bool readyToSpawnSummon;
    [SerializeField] private float distanceToSpawnSummon;
    
    private bool summonPrepEnd;
    private bool summonSucces;

    [SerializeField] private float spawnShift;
    
    private LayerMask abyssMask;

    protected override void EnemyStart()
    {
        battleState = BattleState.Movement;
        
        summonPool = gameObject.GetComponent<ObjectPool>();
        summonPool.InitializePool("Summon", 10, summonPrefab);
        
        abyssMask = LayerMask.GetMask("Abyss");
        
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
        moveDirTimer = 0;
        spawnSummonCooldownTimer = spawnSummonCooldown/10;
    }

    protected override void BattleUpdate()
    {
        lookingDirection = (player.transform.position - transform.position).normalized;
        
        switch (battleState)
        {
            case BattleState.Movement:
                
                MoveToComfortZone();
                
                if (!readyToSpawnSummon)
                {
                    spawnSummonCooldownTimer -= Time.deltaTime;
                    if (spawnSummonCooldownTimer <= 0)
                    {
                        readyToSpawnSummon = true;
                    }
                }

                if (readyToSpawnSummon && (playerBottom.position - bodyBottom.position).magnitude < distanceToSpawnSummon)
                {
                    EnterSummonState();
                }
                
                break;
            
            case BattleState.Summon:
                rigidBody.velocity = Vector2.zero;
                if (summonPrepEnd)
                {
                    summonPrepEnd = false;
                    CreateSummons();
                    summonSucces = true;
                }
                break;
        }
    }
    
    protected override void EnemyTimersTickInStun()
    {
        spawnSummonCooldownTimer -= Time.deltaTime;
    }

    protected override void StunPreprocessing()
    {
        if (battleState == BattleState.Summon)
        {
            SummonEnd();
            if (!summonSucces)
            {
                spawnSummonCooldownTimer = spawnSummonCooldown/2;
            }
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
            case BattleState.Summon:
                break;
            default:
                GetHitBounce(hitDirection);
                break;
        }
    }
    
    public void EnterMovementState()
    {
        moveDirTimer = 0;
        battleState = BattleState.Movement;
    }

    public void EnterSummonState()
    {
        Vector2 attackDir = (playerBottom.position - bodyBottom.position).normalized;
        animationController.SetFloat(AttackDirX, attackDir.x, 0f, Time.deltaTime);
        animationController.SetFloat(AttackDirY, attackDir.y, 0f, Time.deltaTime);
        summonPrepEnd = false;
        animationController.ResetTrigger(AttackEnd);
        animationController.SetTrigger(Attack);
        summonSucces = false;
        battleState = BattleState.Summon;
    }
    
    IEnumerator TryToEnterAlert()
    {
        for (;;)
        {
            if (battleState != BattleState.Summon)
            {
                break;
            }
            yield return new WaitForSeconds(.01f);
        }
        EnterState(EnemyState.Alert);
    }

    public void Summon()
    {
        summonPrepEnd = true;
    }
    public void SummonEnd()
    {
        spawnSummonCooldownTimer = spawnSummonCooldown;
        EnterMovementState();
        readyToSpawnSummon = false;
        animationController.SetTrigger(AttackEnd);
    }
    
    private void CreateSingleSummon(Vector2 spawnPoint)
    {
        GameObject newEnemy =
            summonPool.SpawnFromPool("Summon", spawnPoint, Quaternion.identity);
        newEnemy.GetComponent<EnemyScript>().SetSpawnVars(summonPool, "Summon");
        newEnemy.transform.SetParent(null);
        
        readyToSpawnSummon = false;
    }

    private void CreateSummons()
    {
        var spawnPoint = (Vector2)transform.position + Vector2.right*spawnShift;
        if (CheckObstaclesInPoint(spawnPoint))
        {
            CreateSingleSummon(spawnPoint);
        }
        
        spawnPoint = (Vector2)transform.position + Vector2.left*spawnShift;
        if (CheckObstaclesInPoint(spawnPoint))
        {
            CreateSingleSummon(spawnPoint);
        }
    }

    private bool CheckObstaclesInPoint(Vector2 point)
    {
        var obstacle = Physics2D.OverlapCircle(point, 0.3f, obstacleReliefMask | abyssMask);
        if (obstacle != null)
        {
            return false;
        }

        return true;
    }
}
