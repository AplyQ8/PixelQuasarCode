using System;
using System.Collections;
using System.Collections.Generic;
using Main_hero.HookScripts.HookStrategies;
using ObjectLogicInterfaces;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    public float rearDetectionRadius;
    [Range(0, 360)] public float angle;

    public LayerMask playerMask;
    private LayerMask obstacleMask;
    private LayerMask projectileMask;
    private LayerMask enemyMask;
    
    private Transform playerBottom;
    private Transform bodyBottom;
    private Transform hook;

    public bool canSeePlayer;
    public bool playerDisappeared = false;
    
    public bool canSeeHook;
    public bool seenHookRecently;
    
    private GameObject _hero;
    private float heroColliderSize;
    private IInvisible _heroInvisibility;
    
    private BattleZone battleZone;
    private bool inBattleZone;

    private EnemyScript enemyScript;
    
    void Start()
    {
        enemyScript = gameObject.GetComponent<EnemyScript>();
        _hero = GameObject.FindWithTag("Player");
        heroColliderSize = (_hero.GetComponent<BoxCollider2D>().size*_hero.transform.localScale/2).magnitude;
        _heroInvisibility = _hero.GetComponent<IInvisible>();
        
        battleZone = gameObject.GetComponent<BattleDetector>().GetBattleZone();
        inBattleZone = true;

        obstacleMask = LayerMask.GetMask("Obstacle");
        projectileMask = LayerMask.GetMask("Projectile");
        enemyMask = LayerMask.GetMask("Enemy");
        
        playerBottom = _hero.transform.Find("ObstacleCollider");
        bodyBottom = transform.Find("ObstacleCollider");
        //hook = GameObject.FindWithTag("Hook").transform;
        try
        {
            hook = FindObjectOfType<HookStrategyHandler>().transform;
        }
        catch (NullReferenceException)
        {
            hook = _hero.transform.Find("Hook").transform;
        }
        

        seenHookRecently = false;
        
        StartCoroutine(FOVRoutine());
        StartCoroutine(CheckIfInBattleZone());
    }

    private void OnEnable()
    {
        canSeePlayer = false;
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        // Проверяем детекцию игрока раз в 0.2 секунды
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        PlayerCheck();
        HookCheck();
    }

    private void PlayerCheck()
    {
        // По умолчанию игрок не обнаружен
        canSeePlayer = false;

        float checkRadius = radius;
        if (!inBattleZone && enemyScript.GetEnemyState() != EnemyScript.EnemyState.Patrolling)
        {
            checkRadius = radius / 2;
        }
        
        // Проверяем попадание игрока в внешнюю область видимости
        Collider2D player = Physics2D.OverlapCircle(bodyBottom.position, checkRadius, playerMask);

        if (player is null)
            return;
        
        
        // Проверка видимости игрока
        if (!_heroInvisibility.IsVisible())
        {
            playerDisappeared = true;
            return;
        }
        playerDisappeared = false;
        
        
        Vector2 directionToPlayer = (playerBottom.position - bodyBottom.position).normalized;
        
        // Направление взгляда
        Vector2 lookingDirection = enemyScript.GetLookingDirection();
        // Проверка попадания в угол обзора
        if (Vector2.Angle(lookingDirection, directionToPlayer) < angle / 2)
        {
            directionToPlayer = (playerBottom.position - bodyBottom.position).normalized;
            float distanceToPlayer = Vector2.Distance(bodyBottom.position, playerBottom.position);

            if (!Physics2D.Raycast(bodyBottom.position, directionToPlayer,
                    distanceToPlayer, obstacleMask))
            {
                canSeePlayer = true;
            }

        }
        else
        {
            // Если игрок обнаружен во внешнешней области ввидимости, но не попал в угол обзора, 
            // то проверяем его попадание в ближнюю область видимости (обнаружение за спиной)
            player = Physics2D.OverlapCircle(bodyBottom.position, rearDetectionRadius, playerMask);

            if (player is not null)
            {
                directionToPlayer = (playerBottom.position - bodyBottom.position).normalized;
                float distanceToPlayer = Vector2.Distance(bodyBottom.position, playerBottom.position);

                if (!Physics2D.Raycast(bodyBottom.position, directionToPlayer,
                        distanceToPlayer, obstacleMask))
                {
                    canSeePlayer = true;
                }

            }
        }
    }

    private void HookCheck()
    {
        canSeeHook = false;

        float checkRadius = radius;
        if (!inBattleZone && enemyScript.GetEnemyState() != EnemyScript.EnemyState.Patrolling)
        {
            checkRadius = radius / 2;
        }
        
        Vector2 directionToHook = (hook.position - bodyBottom.position).normalized;
        float distanceToHook = Vector2.Distance(bodyBottom.position, hook.position);

        if (distanceToHook > checkRadius || !hook.gameObject.activeSelf)
            return;

        if (Physics2D.Raycast(bodyBottom.position, directionToHook,
                distanceToHook, obstacleMask))
            return;
        
        // Направление взгляда
        Vector2 lookingDirection = enemyScript.GetLookingDirection();
        // Проверка попадания в угол обзора
        if (Vector2.Angle(lookingDirection, directionToHook) < angle / 2 ||
            distanceToHook <= rearDetectionRadius)
        {
            canSeeHook = true;

            if (IsPulledEnemyNearby())
            {
                canSeePlayer = true;
                return;
            }

            if (enemyScript.GetEnemyState() == EnemyScript.EnemyState.Patrolling)
            {
                enemyScript.EnterState(EnemyScript.EnemyState.Alert);
                enemyScript.SetLookingDirection(directionToHook);
                seenHookRecently = true;
                StartCoroutine(ResetSeenHookRecently());
            }
            else if (enemyScript.GetEnemyState() == EnemyScript.EnemyState.Alert && !seenHookRecently)
            {
                canSeePlayer = true;
            }
        }
    }

    private IEnumerator ResetSeenHookRecently()
    {
        yield return new WaitForSeconds(0.5f);
        seenHookRecently = false;
    }

    private bool IsPulledEnemyNearby()
    {
        var enemyColliders = Physics2D.OverlapCircleAll(bodyBottom.position, radius,
            enemyMask);

        foreach (var enemyCollider in enemyColliders)
        {
            EnemyScript enemyScript = enemyCollider.transform.GetComponent<EnemyScript>();
            if (enemyScript.GetEnemyState() == EnemyScript.EnemyState.Pulled)
            {
                return true;
            }
        }

        return false;
    }
    
    
    private IEnumerator CheckIfInBattleZone()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            inBattleZone = true;
            if (battleZone)
            {
                if (!battleZone.IsEnemyInside(transform))
                {
                    inBattleZone = false;
                }
            }
        }
    }

    public bool IsInBattleZone() => inBattleZone;


}
