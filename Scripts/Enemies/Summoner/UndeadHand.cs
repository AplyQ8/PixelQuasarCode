using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using Tests;
using UnityEngine;

public class UndeadHand : EnemyScript
{
    private enum BattleState
    {
        Calm,
        Attack, 
        Awake
    }
    
    protected static readonly int Awake = Animator.StringToHash("Awake");
    protected static readonly int ShootEnd = Animator.StringToHash("ShootEnd");
    
    
    private struct ProjCharacteristics
    {
        public Vector2 ShootDirection;
        public float Angle;

        public ProjCharacteristics(Vector2 dir, float angle)
        {
            ShootDirection = dir;
            Angle = angle;
        }
    }
    private ProjCharacteristics _projToShoot;
    
    private EnemyProjectileSpawner projSpawner;
    private float centerBottomDistance;
    
    private UndeadHandSound undeadHandSound;
    
    [SerializeField] private BattleState battleState;

    [Header("UNDEAD HAND")]
    [SerializeField] private float lifeTime;
    private float lifeTimer;
    
    [SerializeField] private float attackCooldown;
    private float attackCooldownTimer;
    private bool readyToAttack;
    [SerializeField] private float distanceToAttack;
    
    private bool attackMade;
    
    protected override void EnemyStart()
    {
        projSpawner = GetComponent<EnemyProjectileSpawner>();
        centerBottomDistance = (transform.position - bodyBottom.position).magnitude;
        undeadHandSound = transform.GetComponentInChildren<UndeadHandSound>();
        
        battleState = BattleState.Calm;
    }

    protected override void BattleStart()
    {
        attackCooldownTimer = attackCooldown/10;
    }

    protected override void EnemyOnEnable()
    {
        StartAwake();
        lifeTimer = lifeTime;
        StartCoroutine(UpdateLifeTimer());
    }

    protected override void BattleUpdate()
    {
        lookingDirection = player.transform.position - transform.position;
        SetLookAnimDir();
        
        switch (battleState)
        {
            case BattleState.Calm:

                if (!readyToAttack)
                {
                    attackCooldownTimer -= Time.deltaTime;
                    if (attackCooldownTimer <= 0)
                    {
                        readyToAttack = true;
                    }
                }

                if (readyToAttack && (player.transform.position - transform.position).magnitude < distanceToAttack)
                {
                    EnterAttackState();
                }

                break;
            
            case BattleState.Attack:
                break;
            
            case BattleState.Awake:
                break;
        }
    }
    
    protected override bool PulledPreprocessing()
    {
        if (TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(1000000000, DamageTypeManager.DamageType.Default);
        }

        return false;
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
        }
    }
    
    public void EnterAttackState()
    {
        animationController.SetTrigger(Attack);
        animationController.SetBool(ShootEnd, false);
        attackMade = false;
        battleState = BattleState.Attack;
    }

    public void Shoot()
    {
        var projectile = projSpawner.pool.Get();
        float angle = EnemyUtility.AngleBetweenTwoPoints(player.transform.position, transform.position);
        Vector2 direction = (player.transform.position - transform.position).normalized;
        _projToShoot = new ProjCharacteristics(direction, angle);
        projectile.GetComponent<EnemyProjectile>().SetDirection(
            _projToShoot.ShootDirection, 
            _projToShoot.Angle);
        
        projectile.SetShadowOffset(centerBottomDistance);
        projectile.CheckStartCollision(bodyBottom);
        
        EndShoot();
    }
    
    private void EndShoot()
    {
        readyToAttack = false;
        attackMade = true;
        animationController.SetBool(ShootEnd, true);
        attackCooldownTimer = attackCooldown;
        battleState = BattleState.Calm;
    }

    public void EndAwake()
    {
        battleState = BattleState.Calm;
        attackCooldownTimer = attackCooldown/10;
    }
    
    private IEnumerator UpdateLifeTimer()
    {
        while (true)
        {
            lifeTimer -= 2;
            if (lifeTimer <= 0)
            {
                if (TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(1000000000, DamageTypeManager.DamageType.Default);
                }
            }
            yield return new WaitForSeconds(2);
        }
    }

    public void StartAwake()
    {
        animationController.SetTrigger(Awake);
        undeadHandSound.PlayAppearanceSound();
        battleState = BattleState.Awake;
    }

    private void SetLookAnimDir()
    {
        animationController.SetFloat(LastHorizontal, lookingDirection.normalized.x, 0f, Time.fixedDeltaTime);
        animationController.SetFloat(LastVertical, lookingDirection.normalized.y, 0f, Time.fixedDeltaTime);
    }
    
    private void OnDestroy()
    {
        StopCoroutine(UpdateLifeTimer());
        if (projSpawner != null && projSpawner.pool != null)
        {
            projSpawner.pool.Dispose();
        }
    }
}
