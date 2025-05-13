using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleZonePart : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    private LayerMask playerObstacleColliderMask;
    private LayerMask enemyObstacleColliderMask;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        playerObstacleColliderMask = LayerMask.GetMask("PlayerObstacleCollider");
        enemyObstacleColliderMask = LayerMask.GetMask("EnemyObstacleCollider");
    }

    public bool IsPlayerInside()
    {
        Vector2 boxSize = new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        boxSize = boxCollider.size * boxSize;
        
        if (Physics2D.OverlapBox(boxCollider.bounds.center, boxSize, 0,
                playerObstacleColliderMask))
        {
            return true;
        }

        return false;
    }

    public bool IsEnemyInside(Transform enemyTransform)
    {
        Vector2 boxSize = new Vector2(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y));
        boxSize = boxCollider.size * boxSize;

        var enemiesColliders = Physics2D.OverlapBoxAll(boxCollider.bounds.center, 
            boxSize, 0, enemyObstacleColliderMask);

        foreach (var enemyCollider in enemiesColliders)
        {
            if (enemyCollider.transform.parent == enemyTransform)
            {
                return true;
            }
        }

        return false;
    }
}
