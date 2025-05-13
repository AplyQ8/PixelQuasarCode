using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRange : MonoBehaviour
{
    // Start is called before the first frame update
    protected EnemyScript enemyScript;
    protected CircleCollider2D rangeCollider;
    
    protected GameObject player;
    protected Transform playerBottom;
    
    protected Transform bodyBottom;
    
    protected LayerMask obstacleMask;
    void Start()
    {
        enemyScript = transform.parent.gameObject.GetComponent<EnemyScript>();
        rangeCollider = GetComponent<CircleCollider2D>();
        rangeCollider.enabled = false;
        
        obstacleMask = GetObstacleMask();
        
        player = GameObject.FindWithTag("Player");
        playerBottom = player.transform.Find("ObstacleCollider");
        bodyBottom = transform.parent.Find("ObstacleCollider");
    }

    protected virtual LayerMask GetObstacleMask()
    {
        return LayerMask.GetMask("Obstacle");
    }

    protected virtual void StartAttack()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (collisionObj.gameObject.CompareTag("Player"))
        {
            if (NoObstaclesOnTheWay(obstacleMask))
            {
                StartAttack();
            }
            else
            {
                rangeCollider.enabled = false;
                StartCoroutine(UpdateLifeTimer());
            }
        }
    }
    
    private IEnumerator UpdateLifeTimer()
    {
        yield return new WaitForSeconds(0.1f);
        rangeCollider.enabled = true;
    }

    private bool NoObstaclesOnTheWay(LayerMask layerMask)
    {
        Vector2 directionToTarget = (playerBottom.position - bodyBottom.position).normalized;
        float distanceToTarget = (playerBottom.position - bodyBottom.position).magnitude;
        
        return !Physics2D.Raycast(bodyBottom.position, directionToTarget,
            distanceToTarget, layerMask);
    }
    
}
