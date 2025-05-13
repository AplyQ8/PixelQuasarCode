using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class BattleDetector : MonoBehaviour
{
    public float radius;

    private LayerMask playerMask;
    private LayerMask enemyMask;
    
    private Transform bodyBottom;

    [SerializeField] private BattleZone battleZone;

    public bool canSeeBattle;
    
    void Start()
    {
        playerMask = LayerMask.GetMask("Player");
        enemyMask = LayerMask.GetMask("Enemy");
        
        bodyBottom = transform.Find("ObstacleCollider");

        StartCoroutine(BPRoutine());
    }

    private void OnEnable()
    {
        canSeeBattle = false;
        StartCoroutine(BPRoutine());
    }

    private IEnumerator BPRoutine()
    {
        // Проверяем детекцию битвы раз в 0.2 секунды
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            BattleDetectionCheck();
        }
    }

    private void BattleDetectionCheck()
    {
        // По умолчанию битва не обнаружена
        canSeeBattle = false;

        // Проверка попадания в зону патрулирования врага
        if (battleZone)
        {
            if (!battleZone.IsPlayerInside())
            {
                return;
            }
        }

        
        // Проверяем попадание игрока в область детекции
        Collider2D player = Physics2D.OverlapCircle(bodyBottom.position, radius, playerMask);
        
        if (player is not null)
        {
            
            var enemyColliders = Physics2D.OverlapCircleAll(bodyBottom.position, radius, enemyMask);

            foreach (var enemyCollider in enemyColliders)
            {
                EnemyScript enemyScript = enemyCollider.transform.GetComponent<EnemyScript>();
                if (enemyScript.GetEnemyState() == EnemyScript.EnemyState.Battle && enemyScript.GetCanSeePlayer())
                {
                    canSeeBattle = true;
                    break;
                }
            }
            
        }

    }

    public BattleZone GetBattleZone() => battleZone;

}
