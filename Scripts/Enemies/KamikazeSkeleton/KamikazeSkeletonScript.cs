using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;
using Pathfinding;

public class KamikazeSkeletonScript : EnemyScript
{
    private static readonly int ExplosionPrep = Animator.StringToHash("ExplosionPrep");
    private static readonly int Explosion = Animator.StringToHash("Explosion");
    private static readonly int Dash = Animator.StringToHash("Dash");
    private enum BattleState
    {
        Pursuit,
        DashPrep,
        Dash,
        ExplosionPrep,
        Explosion,
        Awake
    }

    private BattleState battleState;
    private CircleCollider2D dashRangeCollider;
    private CircleCollider2D explosionRangeCollider;

    [Header("KAMIKAZE SKELETON")]
    [SerializeField] private float dashPrepDuration;
    [SerializeField] private float dashMaxDuration;
    [SerializeField] private float explosionPrepDuration;
    [SerializeField] private float dashCooldown;
    private float dashPrepTimer;
    private float dashTimer;
    private float explosionPrepTimer;
    private float dashCooldownTimer;
    private bool readyToDash;
    
    [SerializeField] private float dashSpeed;

    [SerializeField] private KamikazeExplosion explosionPref;
    private bool explosionUnfinished;
    private bool alreadyDead;

    protected override void EnemyStart()
    {
        battleState = BattleState.Awake;
        dashRangeCollider = transform.Find("DashRange").gameObject.GetComponent<CircleCollider2D>();
        explosionRangeCollider = transform.Find("ExplosionStartRange").gameObject.GetComponent<CircleCollider2D>();
        
        readyToDash = true;
        explosionUnfinished = false;
        alreadyDead = false;
        dashCooldownTimer = 0;
    }
    
    protected override void EnemyOnEnable()
    {
        StartAwake();
    }

    protected override void BattleStart()
    {
        readyToDash = false;
        dashCooldownTimer = 0;
    }

    protected override void BattleUpdate()
    {
        lookingDirection = (player.transform.position - transform.position).normalized;
        
        switch (battleState)
        {
            case BattleState.Pursuit:
                destinationPoint = playerBottom.position;
                MoveToDestination();

                if (!readyToDash)
                {
                    dashCooldownTimer -= Time.deltaTime;
                    if (dashCooldownTimer <= 0)
                    {
                        readyToDash = true;
                        dashRangeCollider.enabled = true;
                    }
                }

                break;
            
            case BattleState.DashPrep:
                rigidBody.velocity = Vector2.zero;
                
                dashPrepTimer -= Time.deltaTime;
                if (dashPrepTimer <= 0)
                {
                    explosionRangeCollider.enabled = true;
                    dashTimer = dashMaxDuration;
                    moveScript.SetSpeed(dashSpeed);
                    // spriteRenderer.color = new Color(1f, 0, 0.5f, 1);
                    animationController.SetBool(Dash, true);
                    battleState = BattleState.Dash;
                }
                break;
            
            case BattleState.Dash:
                destinationPoint = playerBottom.position;
                MoveToDestination();

                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    readyToDash = false;
                    dashCooldownTimer = dashCooldown;
                    dashRangeCollider.enabled = false;
                    moveScript.SetBaseSpeed();
                    // spriteRenderer.color = new Color(1f, 1, 1f, 1);
                    animationController.SetBool(Dash, false);
                    battleState = BattleState.Pursuit;
                }
                break;
            
            case BattleState.ExplosionPrep:
                destinationPoint = playerBottom.position;
                MoveToDestination();
                
                explosionPrepTimer -= Time.deltaTime;
                if (explosionPrepTimer <= 0)
                {
                    Explode();
                    battleState = BattleState.Explosion;
                }
                
                break;
            
            case BattleState.Explosion:
                break;
            case BattleState.Awake:
                break;
        }
        
    }
    
    public void StartAwake()
    {
        battleState = BattleState.Awake;
    }
    
    public void EndAwake()
    {
        dashRangeCollider.enabled = true;
        battleState = BattleState.Pursuit;
    }

    public void StartDashPrep()
    {
        dashRangeCollider.enabled = false;
        explosionRangeCollider.enabled = false;
        dashPrepTimer = dashPrepDuration;
        battleState = BattleState.DashPrep;
        
    }
    
    public void StartExplosionPrep()
    {
        if (state != EnemyState.Battle)
        {
            explosionRangeCollider.enabled = false;
            explosionRangeCollider.enabled = true;
            return;
        }

        rigidBody.velocity = (player.transform.position - transform.position).normalized * dashSpeed;
        explosionRangeCollider.enabled = false;
        explosionPrepTimer = explosionPrepDuration;
        explosionUnfinished = true;
        // spriteRenderer.color = new Color(0.5f, 0, 0.5f, 1);
        animationController.SetTrigger(ExplosionPrep);
        battleState = BattleState.ExplosionPrep;
        
    }

    private void Explode()
    {
        animationController.SetBool(Explosion, true);
        rigidBody.velocity = Vector2.zero;
        Instantiate(explosionPref, transform.position, Quaternion.identity);
        explosionUnfinished = false;
        if (!alreadyDead && TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(1000000000, DamageTypeManager.DamageType.Default);
        }
    }

    protected override void EnterDeathState()
    {
        alreadyDead = true;
        if (explosionUnfinished)
        {
            Explode();
        }
        base.EnterDeathState();
    }

    protected override bool OneShotByHook()
    {
        return true;
    }
    
    public override void GetHitPreprocessing(Vector2 hitDirection)
    {
        switch (battleState)
        {
            default:
                GetHitBounce(hitDirection);
                break;
        }
    }
    
    protected override void EnterAlertFromBattle()
    {
        StartCoroutine(TryToEnterAlert());
        explosionRangeCollider.enabled = false;
    }

    IEnumerator TryToEnterAlert()
    {
        for (;;)
        {
            if (battleState == BattleState.Pursuit || battleState == BattleState.Dash)
            {
                break;
            }
            yield return new WaitForSeconds(.1f);
        }
        EnterState(EnemyState.Alert);
    }
    
    
}
