using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class WardenAttack : MonoBehaviour
{
    [Serializable] private class WardenAttackRanges
    {
        public float top;
        public float topRight;
        public float right;
        public float bottomRight;
        public float bottom;
        public float bottomLeft;
        public float left;
        public float topLeft;
    }

    [SerializeField] private WardenAttackRanges attackRanges;
    
    private SpriteRenderer visualisationSpriteRenderer;

    private Transform wardenTransform;
    
    private Transform playerBottom;
    private Transform bodyBottom;
    
    private LayerMask obstacleMask;

    private ICanAttack attackScript;
    
    private LayerMask playerMask;
    protected GameObject player;
    
    // private float attackRadius;
    private float attackAngle;
    // Start is called before the first frame update
    void Start()
    {
        wardenTransform = transform.parent;
        // attackRadius = wardenTransform.gameObject.GetComponent<WardenScript>().attackRadius;
        attackAngle = wardenTransform.gameObject.GetComponent<WardenScript>().attackAngle;
        visualisationSpriteRenderer = GetComponent<SpriteRenderer>();
        visualisationSpriteRenderer.enabled = false;
        attackScript = gameObject.transform.parent.GetComponent<ICanAttack>();
        
        playerMask = LayerMask.GetMask("Player");
        obstacleMask = LayerMask.GetMask("Obstacle");
        
        player = GameObject.FindWithTag("Player");
        
        playerBottom = GameObject.FindWithTag("Player").transform.Find("ObstacleCollider");
        bodyBottom = transform.parent.Find("ObstacleCollider");
    }

    public void AttackArea(Vector3 attackGoalPoint)
    {
        Vector2 attackDirection = GetAttackDirection(attackGoalPoint);
        var attackRadius = GetAttackRange(attackGoalPoint);
        // VisualizeAttackZone(attackGoalPoint);
        
        // Collider2D playerCollider = Physics2D.OverlapCircle(bodyBottom.position, attackRadius, playerMask);
        float distanceToPlayer = (playerBottom.position - bodyBottom.position).magnitude;
        if (distanceToPlayer <= attackRadius)
        {
            
            Vector2 directionToPlayer = (playerBottom.position - bodyBottom.position).normalized;
            if (Vector2.Angle(attackDirection, directionToPlayer) < attackAngle / 2)
            {
                TryDamagePlayer(player.GetComponent<Collider2D>());
            }
        }
        
        // visualisationSpriteRenderer.enabled = true;
        // StartCoroutine(DisableRenderer());
    }

    private void VisualizeAttackZone(Vector3 attackGoalPoint)
    {
        float angle = EnemyUtility.AngleBetweenTwoPoints(attackGoalPoint, bodyBottom.position);
        float rotationAngle = EnemyUtility.DirectionToAngle(EnemyUtility.AngleToDirection(angle));
        transform.rotation = Quaternion.Euler(new Vector3(0f,0f,rotationAngle+225));
    }
    
    private Vector2 GetAttackDirection(Vector3 attackGoalPoint)
    {
        float angle = EnemyUtility.AngleBetweenTwoPoints(attackGoalPoint, bodyBottom.position);
        return EnemyUtility.DirectionToVector(EnemyUtility.AngleToDirection(angle));
    }

    private float GetAttackRange(Vector3 attackGoalPoint)
    {
        float angle = EnemyUtility.AngleBetweenTwoPoints(attackGoalPoint, bodyBottom.position);
        var direction = EnemyUtility.AngleToDirection(angle);
        switch (direction)
        {
            case EnemyUtility.Direction.North:
                return attackRanges.top;
            case EnemyUtility.Direction.NorthEast:
                return attackRanges.topRight;
            case EnemyUtility.Direction.East:
                return attackRanges.right;
            case EnemyUtility.Direction.SouthEast:
                return attackRanges.bottomRight;
            case EnemyUtility.Direction.South:
                return attackRanges.bottom;
            case EnemyUtility.Direction.SouthWest:
                return attackRanges.bottomLeft;
            case EnemyUtility.Direction.West:
                return attackRanges.left;
            case EnemyUtility.Direction.NorthWest:
                return attackRanges.topLeft;
        }

        return attackRanges.top;
    }
    
    private void TryDamagePlayer(Collider2D playerCollider)
    {
        if (NoObstaclesOnTheWay(obstacleMask))
        {
            if (playerCollider.TryGetComponent(out IDamageable damageableObject))
            {
                damageableObject.TakeDamage(attackScript.GetCurrentAttack(), DamageTypeManager.DamageType.Default);
            }
        }
    }
    
    protected bool NoObstaclesOnTheWay(LayerMask layerMask)
    {
        Vector2 directionToTarget = (playerBottom.position - bodyBottom.position).normalized;
        float distanceToTarget = (playerBottom.position - bodyBottom.position).magnitude;
        
        return !Physics2D.Raycast(bodyBottom.position, directionToTarget,
            distanceToTarget, layerMask);
    }
    
    IEnumerator DisableRenderer()
    {
        yield return new WaitForSeconds(0.3f);
        visualisationSpriteRenderer.enabled = false;
    }
}
