using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class PhantomAttack : MonoBehaviour
{
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int AttackEnd = Animator.StringToHash("AttackEnd");
    
    private Vector2 directionVector;
    private float attackDuration;
    private float attackPrepDuration;
    private float attackTimer;
    private float attackDistance = 180;
    float attackBaseSpeed;

    private bool attacking;
    private bool preparation;
    private bool alreadyHitPlayer;
    
    private Collider2D attackCollider;
    private SpriteRenderer visualisationSpriteRenderer;

    
    private Transform phantomTransform;
    private PhantomScript phantomScript;

    private PhantomSound phantomSound;
    
    private Transform playerBottom;
    private Transform bodyBottom;
    private LayerMask obstacleMask;
    
    private Animator animationController;
    private ICanAttack _attackScript;
    
    // Start is called before the first frame update
    void Start()
    {
        phantomTransform = transform.parent;
        phantomScript = phantomTransform.gameObject.GetComponent<PhantomScript>();
        attackDuration = phantomScript.attackDuration;
        attackPrepDuration = phantomScript.attackPrepDuration;
        attackBaseSpeed = attackDistance / attackDuration;
        attackCollider = GetComponent<BoxCollider2D>();
        attackCollider.enabled = false;
        visualisationSpriteRenderer = GetComponent<SpriteRenderer>();
        visualisationSpriteRenderer.enabled = false;
        animationController = gameObject.GetComponent<Animator>();
        _attackScript = phantomTransform.GetComponent<ICanAttack>();
        phantomSound = transform.parent.GetComponentInChildren<PhantomSound>();
        
        playerBottom = GameObject.FindWithTag("Player").transform.Find("ObstacleCollider");
        bodyBottom = transform.parent.Find("ObstacleCollider");
        obstacleMask = LayerMask.GetMask("Obstacle");
    }

    // Update is called once per frame
    void Update()
    {
        if (!attacking)
            return;

        if (phantomScript.GetEnemyState() != EnemyScript.EnemyState.Battle)
        {
            EndAttack();
            phantomScript.AttackInterrupted();
        }

        if (preparation)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                preparation = false;
                phantomSound.PlaySwordAttackSound();
                attackTimer = attackDuration;
            }

            return;
        }
        
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + attackBaseSpeed * Time.deltaTime);
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            EndAttack();
        }
    }
    
    public void StartAttack(EnemyUtility.Direction attackDirection)
    {
        EnemyUtility.Direction attackStartDirection = (EnemyUtility.Direction)(((int)attackDirection + 2) % 8);
        float attackStartAngle = EnemyUtility.DirectionToAngle(attackStartDirection);
        transform.rotation = Quaternion.Euler(new Vector3(0f,0f,attackStartAngle));
        attackTimer = attackPrepDuration;
        attacking = true;
        preparation = true;
        alreadyHitPlayer = false;
        attackCollider.enabled = true;
        animationController.SetTrigger(Attack);
        visualisationSpriteRenderer.enabled = true;
    }
    
    public void EndAttack()
    {
        attacking = false;
        attackCollider.enabled = false;
        visualisationSpriteRenderer.enabled = false;
        if(attackTimer > 0)
            animationController.SetTrigger(AttackEnd);
    }

    public bool InterruptAttack()
    {
        EndAttack();
        return alreadyHitPlayer;
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {

        if (!collider.CompareTag("Player") || alreadyHitPlayer)
            return;

        if (NoObstaclesOnTheWay(obstacleMask))
        {
            phantomSound.PlayDealDmgSound();
            if (collider.TryGetComponent(out IDamageable damageableObject))
            {
                damageableObject.TakeDamage(_attackScript.GetCurrentAttack(), DamageTypeManager.DamageType.Default);
            }
        }
        
        alreadyHitPlayer = true;

    }
    
    private bool NoObstaclesOnTheWay(LayerMask layerMask)
    {
        Vector2 directionToTarget = (playerBottom.position - bodyBottom.position).normalized;
        float distanceToTarget = (playerBottom.position - bodyBottom.position).magnitude;
        
        return !Physics2D.Raycast(bodyBottom.position, directionToTarget,
            distanceToTarget, layerMask);
    }
    
    public bool GetAlreadyHit()
    {
        return alreadyHitPlayer;
    }
}
