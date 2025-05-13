using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class GoblinAttack : MonoBehaviour
{

    private Vector2 directionVector;
    [SerializeField] private float shift;
    private float attackDuration;
    private float attackTimer;
    [SerializeField] private float attackDistance;
    float attackBaseSpeed;

    private bool attacking;
    private bool onWayBack;
    private bool alreadyHitPlayer;
    
    private Collider2D attackCollider;
    private SpriteRenderer visualisationSpriteRenderer;
    private GoblinSound goblinSound;

    private Transform goblinTransform;
    
    private Transform playerBottom;
    private Transform bodyBottom;
    
    private LayerMask obstacleMask;

    private ICanAttack _attackScript;
    // Start is called before the first frame update
    void Start()
    {
        goblinTransform = transform.parent;
        attackDuration = goblinTransform.gameObject.GetComponent<GoblinScript>().attackDuration;
        attackBaseSpeed = attackDistance / (attackDuration/2);
        attackCollider = GetComponent<BoxCollider2D>();
        attackCollider.enabled = false;
        visualisationSpriteRenderer = GetComponent<SpriteRenderer>();
        visualisationSpriteRenderer.enabled = false;
        _attackScript = gameObject.transform.parent.GetComponent<ICanAttack>();
        goblinSound = transform.parent.GetComponentInChildren<GoblinSound>();
        
        obstacleMask = LayerMask.GetMask("Obstacle");
        
        playerBottom = GameObject.FindWithTag("Player").transform.Find("ObstacleCollider");
        bodyBottom = transform.parent.Find("ObstacleCollider");
    }

    // Update is called once per frame
    void Update()
    {
        if (!attacking)
            return;

        if (!onWayBack)
        {
            transform.position += (Vector3)(directionVector * (attackBaseSpeed * Time.deltaTime));
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                attackTimer = attackDuration / 2;
                onWayBack = true;
            }
        }
        else
        {
            transform.position -= (Vector3)(directionVector * (attackBaseSpeed * Time.deltaTime));
        }
    }

    public void StartAttack(EnemyUtility.Direction attackDirection)
    {

        float rotationAngle = EnemyUtility.DirectionToAngle(attackDirection);
        directionVector = EnemyUtility.DirectionToVector(attackDirection);
        
        // goblinSound?.PlayAttackSound();
        
        transform.rotation = Quaternion.Euler(new Vector3(0f,0f,rotationAngle));
        transform.position += (Vector3)(shift * directionVector);
        attackTimer = attackDuration/2;
        attacking = true;
        attackCollider.enabled = true;
        onWayBack = false;
        alreadyHitPlayer = false;
    }

    public void EndAttack()
    {
        transform.position = goblinTransform.position;
        attacking = false;
        attackCollider.enabled = false;
        alreadyHitPlayer = false;
    }

    public bool GetAlreadyHit()
    {
        return alreadyHitPlayer;
    }
    
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Obstacle"))
        {
            EndAttack();
            return;
        }
        if (!collider.CompareTag("Player") || onWayBack || alreadyHitPlayer)
            return;
        onWayBack = true;
        alreadyHitPlayer = true;
        attackTimer = attackDuration/2 - attackTimer;
        

        if (NoObstaclesOnTheWay(obstacleMask))
        {
            goblinSound?.PlayDealDmgSound();
            if (collider.TryGetComponent(out IDamageable damageableObject))
            {
                damageableObject.TakeDamage(_attackScript.GetCurrentAttack(), DamageTypeManager.DamageType.Default);
            }
        }
        
        EndAttack();
    }
    
    private bool NoObstaclesOnTheWay(LayerMask layerMask)
    {
        Vector2 directionToTarget = (playerBottom.position - bodyBottom.position).normalized;
        float distanceToTarget = (playerBottom.position - bodyBottom.position).magnitude;
        
        return !Physics2D.Raycast(bodyBottom.position, directionToTarget,
            distanceToTarget, layerMask);
    }
}
