using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main_hero.HookScripts.HookStrategies;
using ObjectLogicInterfaces;
using ObjectLogicRealization.Health;
using ObjectLogicRealization.Move;
using UnityEngine;
using Pathfinding;
using RouteScripts;
using Random = UnityEngine.Random;

public class EnemyScript : MonoBehaviour, IHookable, IObstructed
{
    #region Animation properties
    
    protected static readonly int Speed = Animator.StringToHash("Speed");
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
    protected static readonly int LastVertical = Animator.StringToHash("LastVertical");
    protected static readonly int Stunned = Animator.StringToHash("Stunned");
    protected static readonly int Chase = Animator.StringToHash("Chase");
    protected static readonly int Warning = Animator.StringToHash("Warning");
    protected static readonly int Normal = Animator.StringToHash("Normal");
    protected static readonly int Die = Animator.StringToHash("Die");
    protected static readonly int Falling = Animator.StringToHash("Falling");
    
    #endregion
    
    #region Animation Events

    public event Action
        OnNormalAnimation,
        OnAttackAnimation,
        OnChaseAnimation,
        OnDieEvent,
        OnWarningAnimation;

    public event Action<float> OnSetSpeed;
    public event Action<bool> OnStunAnimation;
    public event Action<Rigidbody2D, Vector2> OnSetVerticals;
        
    #endregion
    
    #region Variables
    
    protected EnemyState state;
    public enum EnemyState
    {
        Pulled,
        Feared,
        Patrolling,
        Battle,
        Alert,
        Bounced,
        Stunned,
        Death
    }
    
    public enum BattleInterruptionSource
    {
        Default,
        Pull,
        Stun,
        Hit
    }
    
    [SerializeField] protected Animator animationController;
    
    protected IEnumerator CurrentStun;
    public bool IsStunned { get; private set; }
    
    private EnemyState stateBeforePull;
    private EnemyState stateBeforeStun;
    public float stopPullDistance = 2;
    protected bool canBeHooked;
    protected bool canBeStunned;
    private Transform hookTransform;
    
    protected FieldOfView fieldOfView;
    protected Vector2 lookingDirection = Vector2.up;
    protected BattleDetector battleDetector;

    protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rigidBody;
    protected Collider2D collider2D;
    protected Transform bodyBottom;

    protected EnemySound enemySound;

    protected float obstacleColliderSize;

    protected GameObject player;
    protected Transform playerBottom;
    [NonSerialized] public bool canSeePlayer = false;
    
    protected List<PatrolPoint> patrolPoints;
    [SerializeField] protected GameObject patrolRoute;
    protected bool patrolStopover;
    protected float patrolStopoverTimer;
    protected int destinationPointIndex = 0;
    protected Vector3 destinationPoint;
    
    // [SerializeField] protected float baseSpeed;
    protected EnemyMove moveScript;
    [Header("Bounce")]
    [SerializeField] protected float getHitBackForce;
    [SerializeField] protected float getHitTotalTime;
    protected float knockBackForce;
    protected float knockBackTotalTime;
    protected Vector2 bounceDirection;
    protected Vector2 speed;
    private Vector2 velocityBeforeBounce;
    
    [Header("Duration")]
    public float aggressionDuration;
    protected float aggressionTimer;
    public float alertDuration;
    protected float alertTimer;
    private float alertChangeLookDuration = 1.5f;
    private float alertChangeLookTimer;
    private int alertLookDirIndex;
    protected bool tryingEnterAlert;

    private float stunTimer;
    
    private float nextWaypointDistance = 0.3f;
    protected Path path;
    protected int currentWaypoint = 0;
    protected Seeker seeker;
    protected bool directPathToDestination;
    
    private bool enemiesOnWay;
    private int workaroundDirection;
    private float workaroundDistance = 3;
    
    protected LayerMask obstacleMask; 
    protected LayerMask obstacleReliefMask;

    [field: NonSerialized] public float DeathDuration { get; private set; }
    
    private IFearable _fearScript;

    private EnemySpawner spawner;
    private string spawnId;
    private Utilities.ObjectPool spawnPool;
    private HookStrategyHandler _hookStrategyHandler;

    #endregion



    #region Start
    void Awake()
    {
        IsStunned = false;
        canSeePlayer = false;
        canBeHooked = true;
        canBeStunned = true;
        directPathToDestination = false;
        enemiesOnWay = false;
        workaroundDirection = 1;
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        collider2D = gameObject.GetComponent<Collider2D>();
        fieldOfView = gameObject.GetComponent<FieldOfView>();
        battleDetector = gameObject.GetComponent<BattleDetector>();
        moveScript = gameObject.GetComponent<EnemyMove>();
        enemySound = gameObject.GetComponentInChildren<EnemySound>();
        player = GameObject.FindWithTag("Player");

        bodyBottom = transform.Find("ObstacleCollider");
        playerBottom = player.transform.Find("ObstacleCollider");
        
        obstacleColliderSize = (bodyBottom.GetComponent<BoxCollider2D>().size*transform.localScale/2).magnitude;
        
        obstacleMask = LayerMask.GetMask("Obstacle");
        obstacleReliefMask = LayerMask.GetMask("Obstacle", "Relief");


        if (TryGetComponent(out IFearable fear))
        {
            _fearScript = fear;
        }

        if (TryGetComponent(out IDamageable damageable))
        {
            damageable.OnDeathEvent += TryEnterDeath;
        }
        
        if (TryGetComponent(out IStunable stunScript))
        {
            stunScript.OnGetStunnedEvent += GetStunned;
        }
        
        StartCoroutine(CheckEnemiesOnWay());
        StartCoroutine(ChangeWorkaroundMovementDirectionRoutine());
        
        // Стартовые действия, зависящие от вида врага
        EnemyStart();
    }

    private void OnEnable()
    {
        collider2D.enabled = true;
        canSeePlayer = false;
        
        SetPatrolRoute(patrolRoute);
        state = EnemyState.Patrolling;
        EnterState(EnemyState.Patrolling);
        
        EnemyOnEnable();
    }
    
    // Стартовые действия, зависящие от вида врага
    protected virtual void EnemyStart()
    {
        
    }
    
    protected virtual void EnemyOnEnable()
    {
        
    }
    
    #endregion

    #region Update
    // Update is called once per frame
    void Update()
    {
        // Проверка эффекта страха учитывая приоритет состояний
        if (_fearScript is not null)
        {
            if(_fearScript.IsFeared() && state > EnemyState.Feared)
                EnterState(EnemyState.Feared);
        }
        
        SetAnimationMoveSpeed();
        
        switch (state)
        {   
            case EnemyState.Patrolling:
                SetAnimationDirection();
                Patrolling();
                break;
            
            //Летит на хуке
            case EnemyState.Pulled:
                UpdatePulled();
                break;
            case EnemyState.Battle:
                SetAnimationDirection();
                CheckPlayerDetection();
                BattleUpdate();
                break;
            case EnemyState.Alert:
                SetAnimationDirection();
                AlertUpdate();
                break;
            case EnemyState.Feared:
                UpdateFeared();
                break;
            case EnemyState.Bounced:
                UpdateBounced();
                break;
            case EnemyState.Stunned:
                UpdateStunned();
                break;
            case EnemyState.Death:
                rigidBody.velocity = Vector2.zero;
                break;
        }
        
    }

    private void UpdatePulled()
    {
        // if ((transform.position - player.transform.position).magnitude > stopPullDistance)
        // {
        //     transform.position = hookTransform.position;
        // }
        // else {
        //     EndPulledByHook();
        // }
        
        transform.position = hookTransform.position;
    }

    private void UpdateFeared()
    {
        SetAnimationDirection();
        // Направление движения от объекта страха
        lookingDirection = -(_fearScript.GetObjectOfFear().position - transform.position).normalized;
        rigidBody.velocity = lookingDirection * moveScript.GetCurrentMoveSpeed();
        SetAnimationMoveSpeed();
        if (!_fearScript.IsFeared())
        {
            // Восстанавливаем поиск маршрута
            InvokeRepeating("UpdatePath", 0f, 0.5f);
                    
            if (canSeePlayer)
                EnterState(EnemyState.Battle);
            else
                EnterState(EnemyState.Alert);
        } 
    }

    private void UpdateBounced()
    {
        rigidBody.velocity = new Vector2(
            bounceDirection.x * knockBackForce,
            bounceDirection.y * knockBackForce);
        knockBackTotalTime -= Time.deltaTime;
        EnemyTimersTickInStun();
        if (knockBackTotalTime <= 0)
        {
            rigidBody.velocity = velocityBeforeBounce;
                    
            if (canSeePlayer)
                EnterInterruptedBattle(BattleInterruptionSource.Hit);
            else
                EnterState(EnemyState.Alert);
        }
    }

    private void UpdateStunned()
    {
        // spriteRenderer.color = new Color(1f, 0, 1f, 1);
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0)
        {
            EndStunnedState();
        }
    }
    
    
    protected virtual void AlertUpdate()
    {
        rigidBody.velocity = Vector2.zero;
        
        if (!fieldOfView.canSeePlayer && !battleDetector.canSeeBattle)
        {
            alertTimer -= Time.deltaTime;
            
            alertChangeLookTimer -= Time.deltaTime;
            if (alertChangeLookTimer <= 0)
            {
                alertChangeLookTimer = alertChangeLookDuration;
                
                alertLookDirIndex = (alertLookDirIndex + Random.Range(1, 4)) % 4;
                lookingDirection = EnemyUtility.base4Directions[alertLookDirIndex];
            }
        }
        else
        {
            canSeePlayer = true;
            EnterState(EnemyState.Battle);
        }
        

        if (alertTimer <= 0)
        {
            canSeePlayer = false;
            EnterState(EnemyState.Patrolling);
        }
    }
    
    
    // Действия в бою, зависящие от типа врага
    protected virtual void BattleUpdate()
    {
        
    }
    
    
    // Кулдауны, которые доолжны продолжать обновляться в станах/микростанах
    protected virtual void EnemyTimersTickInStun()
    {
        
    }
    
    #endregion
    
    
    
    #region Movements Logic
    public virtual void Patrolling()
    {
        if (!patrolStopover)
        {
            bool destinationReached = MoveToDestination();
            //Перемещение между точками
            if (destinationReached || (bodyBottom.position - destinationPoint).magnitude < 0.01*moveScript.GetCurrentMoveSpeed())
            {
                patrolStopover = true;
                rigidBody.velocity = Vector2.zero;
                patrolStopoverTimer = patrolPoints[destinationPointIndex].GetStopoverDuration();
                if (patrolStopoverTimer > 0)
                    lookingDirection = patrolPoints[destinationPointIndex].GetStopoverLookingDir();
            }
        }
        else
        {
            patrolStopoverTimer -= Time.deltaTime;
            if (patrolStopoverTimer <= 0)
            {
                patrolStopover = false;
                
                if (patrolPoints.Count != 0)
                {
                    destinationPointIndex += 1;
                    destinationPointIndex %= patrolPoints.Count;
                    destinationPoint = patrolPoints[destinationPointIndex].GetPointPosition();
                }
                else
                {
                    destinationPoint = bodyBottom.position;
                }
                UpdatePath();
            }
        }
        
        // Проверка детекции игрока
        if (fieldOfView.canSeePlayer && !canSeePlayer)
        {
            canSeePlayer = true;
            EnterState(EnemyState.Battle);
        } else if (battleDetector.canSeeBattle)
        {
            EnterState(EnemyState.Battle);
        }
    }
    
    public void UpdatePath()
    {
        if (seeker != null && seeker.IsDone())
        {
            if ((bodyBottom.position - destinationPoint).magnitude > 0.01 * moveScript.GetCurrentMoveSpeed())
            {
                seeker.StartPath(bodyBottom.position, destinationPoint, OnPathComplete);
            }

        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            if (IsPathNegligible(path))
                path = null;
            currentWaypoint = 0;
        }
    }

    private bool IsPathNegligible(Path p)
    {
        if (p.path.Count == 1)
        {
            return (p.vectorPath[0] - bodyBottom.position).magnitude < 0.1 * moveScript.GetCurrentMoveSpeed();
        }
        if (p.path.Count == 2)
        {
            return (p.vectorPath[0] - p.vectorPath[1]).magnitude < 0.1 * moveScript.GetCurrentMoveSpeed();
        }

        return false;
    }

    protected virtual bool MoveToDestination()
    {
        if (DirectPathToDestinationExist())
        {
            directPathToDestination = true;
            rigidBody.velocity = (destinationPoint - bodyBottom.position).normalized * 
                                 moveScript.GetCurrentMoveSpeed();
            
            if(rigidBody.velocity != Vector2.zero)
                lookingDirection = rigidBody.velocity.normalized;
            
            AvoidOtherEnemies();
            
            return (bodyBottom.position - destinationPoint).magnitude < 0.01 * moveScript.GetCurrentMoveSpeed();
        }

        if (directPathToDestination)
        {
            directPathToDestination = false;
            UpdatePath();
        }
        
        // Прерываем исполнение, если маршрут не существует или уже пройден
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            rigidBody.velocity = Vector2.zero;
            return true;
        }
        
        // Находим направление к следующей промежуточной точке пути
        Vector3 intermediateDestinationPoint = path.vectorPath[currentWaypoint];
        Vector2 direction = (intermediateDestinationPoint - bodyBottom.position).normalized;
        float distance = Vector2.Distance(bodyBottom.position, intermediateDestinationPoint);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
        
        speed = direction * moveScript.GetCurrentMoveSpeed();
        rigidBody.velocity = speed;
        
        // Направление взгляда в сторону движения (по направлению скорости)
        if(rigidBody.velocity != Vector2.zero)
            lookingDirection = rigidBody.velocity.normalized;
        
        AvoidOtherEnemies();
        return false;
    }

    protected void AvoidOtherEnemies()
    {
        Vector2 directionToDestination = destinationPoint-bodyBottom.position;
        float distanceToDestination = directionToDestination.magnitude;
        // var hitClose = Physics2D.RaycastAll(bodyBottom.position, directionToDestination, closeHitRadius,
        //     LayerMask.GetMask("EnemyObstacleCollider"));
        var hits = Physics2D.RaycastAll(bodyBottom.position, directionToDestination, 1,
            LayerMask.GetMask("EnemyObstacleCollider"));

        bool shouldStop = false;
        foreach (var hit in hits)
        {
            if (hit.transform.gameObject == gameObject)
            {
                continue;
            }

            Transform hitBodyBottom = hit.transform.gameObject.GetComponent<EnemyScript>().bodyBottom;
            float hitDistanceToDestination = (destinationPoint-hitBodyBottom.position).magnitude;
        
            if (hitDistanceToDestination < distanceToDestination)
            {
                shouldStop = true;
                break;
            }
        }
        
        if (shouldStop)
        {
            rigidBody.velocity = Vector2.zero;
        }

        // if (hitClose.Length <= 1 && hit.Length > 1)
        // {
        //     rigidBody.velocity = Vector2.zero;
        //     
        //     // Vector2 vectorToPlayer = (playerBottom.position - bodyBottom.position);
        //     //
        //     // Vector2 tangentVector = Vector2.Perpendicular(vectorToPlayer.normalized);
        //     // // tangentVector *= clockwise;
        //     //
        //     // rigidBody.velocity = moveScript.GetCurrentMoveSpeed() * tangentVector;
        // }
    }

    protected bool DirectPathToDestinationExist()
    {
        Vector2 directionToDestination = destinationPoint-bodyBottom.position;
        float distanceToDestination = directionToDestination.magnitude;
        bool result = !Physics2D.CircleCast(bodyBottom.position, obstacleColliderSize, directionToDestination,
            distanceToDestination, obstacleReliefMask);
        return result;
    }
    
    
    public void SetPatrolRoute(GameObject route)
    {
        patrolRoute = route;
        if (patrolRoute)
        {
            patrolPoints = patrolRoute.transform.Cast<Transform>()
                .Select(point => point.GetComponent<PatrolPointContainer>().GetPatrolPoint()).ToList();
        }
        else
        {
            patrolPoints = new List<PatrolPoint>();
        }

        if (patrolPoints.Count == 0)
        {
            patrolPoints.Add(new PatrolPoint(transform.position, Vector2.up, 10));
        }
        
        destinationPoint = patrolPoints[0].GetPointPosition();
        
        if (patrolPoints.Count != 0)
        {
            if ((bodyBottom.position - destinationPoint).magnitude < 0.1)
                lookingDirection = patrolPoints[destinationPointIndex].GetStopoverLookingDir();
            else
                lookingDirection = (destinationPoint - bodyBottom.position).normalized;
            transform.position = patrolPoints[0].GetPointPosition() + (transform.position - bodyBottom.position);
        }
        else
        {
            lookingDirection = Vector2.up;
        }
        // animationController.SetFloat(LastHorizontal, lookingDirection.normalized.x, 0f, 0);
        // animationController.SetFloat(LastVertical, lookingDirection.normalized.y, 0f, 0);

        patrolStopover = false;
        
        seeker = GetComponent<Seeker>();
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }
    
    protected void MoveToPlayer()
    {
        if (!enemiesOnWay)
        {
            destinationPoint = playerBottom.position;
            MoveToDestination();
        }
        else
        {
            WorkaroundMovement();
        }
    }
    
    private void WorkaroundMovement()
    {
        Vector2 vectorToPlayer = playerBottom.position - bodyBottom.position;
        
        Vector2 tangentVector = Vector2.Perpendicular(vectorToPlayer.normalized);
        tangentVector *= workaroundDirection;
        
        Vector2 directionVector = (vectorToPlayer.normalized + tangentVector * 0.75f).normalized;

        rigidBody.velocity = moveScript.GetCurrentMoveSpeed() * directionVector;
        lookingDirection = rigidBody.velocity.normalized;
    }
    
    private void ChangeWorkaroundMovementDirection()
    {
        workaroundDirection *= -1;
    }
    
    IEnumerator ChangeWorkaroundMovementDirectionRoutine()
    {
        for (;;)
        {
            float randomCooldown = Random.Range(0.5f * 3,
                1.5f * 3);
            yield return new WaitForSeconds(randomCooldown);
            ChangeWorkaroundMovementDirection();
        }
    }
    
    IEnumerator CheckEnemiesOnWay()
    {
        for (;;)
        {
            yield return new WaitForSeconds(0.35f);

            if (state != EnemyState.Battle)
            {
                enemiesOnWay = false;
                continue;
            }
            
            Vector2 directionToPlayer = player.transform.position - transform.position;
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionToPlayer.normalized,
                workaroundDistance, LayerMask.GetMask("Enemy"));

            enemiesOnWay = false;
            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.transform != transform &&
                    hit.transform.parent != transform)
                {
                    enemiesOnWay = true;
                    break;
                }
            }
            
        }
    }
    
    #endregion

    
    #region External influences

    public float PulledByHook(Transform Hook, Vector3 pullBeginning, int damage, HookStrategyHandler hookStrategyHandler)
    {   
        bool abortHook = !PulledPreprocessing();
        if (abortHook)
        {
            return stopPullDistance;
        }
        
        hookTransform = Hook;
        rigidBody.velocity = Vector2.zero;
        
        //Сместить в центр хука
        transform.position = hookTransform.position;
        
        //Поворот в направлении полета
        float angle = EnemyUtility.AngleBetweenTwoPoints(pullBeginning, transform.position);
        transform.rotation = Quaternion.Euler(new Vector3(0f,0f,angle+90));

        stateBeforePull = state;
        _hookStrategyHandler = hookStrategyHandler;
        _hookStrategyHandler.OnHookDisable += EndPulledByHook;
        EnterState(EnemyState.Pulled);
        if (TryGetComponent(out IDamageable damageable))
        {
            if (OneShotByHook())
            {
                damageable.TakeDamage(1000000000, DamageTypeManager.DamageType.Default);
            }
            else
            {
                damageable.TakeDamage(damage, DamageTypeManager.DamageType.Default);
            }
        }

        
        enemySound?.PlayGetHookedSound();
        return stopPullDistance;
    }
    
    public void EndPulledByHook()
    {
        _hookStrategyHandler.OnHookDisable -= EndPulledByHook;
        _hookStrategyHandler = null;
        if(stateBeforePull == EnemyState.Battle)
            EnterInterruptedBattle(BattleInterruptionSource.Pull);
        else if(stateBeforePull == EnemyState.Patrolling)
            EnterState(EnemyState.Alert);
        else
            EnterState(stateBeforePull);
                    
        transform.rotation = Quaternion.Euler(0, 0, 0);
        

    }
    
    public virtual bool IsIntangible() => false;
    
    

    protected virtual bool OneShotByHook()
    {
        return false;
    }
    
    protected void SetCanBeHooked(bool hookable)
    {
        this.canBeHooked = hookable;
    }
    
    
    protected virtual bool PulledPreprocessing()
    {
        // Возвращает false, если хук надо прервать
        return canBeHooked;
    }
    
    public virtual void GetHitPreprocessing(Vector2 hitDirection)
    {
        GetHitBounce(hitDirection);
    }

    protected void GetHitBounce(Vector2 hitDirection)
    {
        SetBounceCharacteristics(getHitBackForce, getHitTotalTime, hitDirection);
    }
    
    protected virtual void EnterInterruptedBattle(BattleInterruptionSource source = BattleInterruptionSource.Default)
    {
        state = EnemyState.Battle;
        PlayChaseAnim();
    }
    
    public void GetStunned(float duration)
    {
        if (!canBeStunned)
            return;
        StunPreprocessing();
        stunTimer = duration;
        if(!(state == EnemyState.Death || (state == EnemyState.Pulled && stateBeforePull == EnemyState.Death)))
            EnterState(EnemyState.Stunned);
    }
    
    protected virtual void StunPreprocessing()
    {
        
    }
    
    private IEnumerator StunTimer()
    {
        yield return new WaitForSeconds(stunTimer);
        //animationController.SetBool(Stunned, false);
        OnStunAnimation?.Invoke(false);
        IsStunned = false;
        EnterState(stateBeforeStun);
    }
    
    private void SetBounceCharacteristics(float kbForce, float kbTotalTime, Vector2 kbDir)
    {
        if (IsStunned)
            return;
        knockBackForce = kbForce;
        knockBackTotalTime = kbTotalTime;
        bounceDirection = kbDir;
        EnterState(EnemyState.Bounced);
    }

    protected void StartPassiveBounce(Vector2 hitDirection)
    {
        if (IsStunned)
            return;

        StartCoroutine(PassiveBounce(hitDirection));
    }
    
    private IEnumerator PassiveBounce(Vector2 hitDirection)
    {
        float passiveBounceTimer = getHitTotalTime;
        while (passiveBounceTimer > 0)
        {
            passiveBounceTimer -= Time.deltaTime;
            transform.position += new Vector3(hitDirection.x * getHitBackForce * Time.deltaTime,
                hitDirection.y * getHitBackForce * Time.deltaTime, 0);
            yield return new WaitForEndOfFrame();
        }
    }

    public void GetDamageReaction()
    {
        GetDamageColor();

        if (state == EnemyState.Patrolling || state == EnemyState.Alert)
        {
            canSeePlayer = true;
            EnterState(EnemyState.Battle);
        }
    }

    private void GetDamageColor()
    {
        spriteRenderer.color = new Color(235f/255, 0, 15f/255f, 1);
        StartCoroutine(ResetColor(0.25f));
    }

    private IEnumerator ResetColor(float delay)
    {
        yield return new WaitForSeconds(delay);
        spriteRenderer.color = Color.white;
    }

    private void EndStunnedState()
    {
        OnStunAnimation?.Invoke(false);
        IsStunned = false;

        if(stateBeforeStun == EnemyState.Battle)
            EnterInterruptedBattle(BattleInterruptionSource.Stun);
        else if(stateBeforeStun == EnemyState.Patrolling)
            EnterState(EnemyState.Alert);
        else
            EnterState(stateBeforeStun);
    }

    #endregion
    

    #region Enter States
    public void EnterState(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Patrolling:
                EnterPatrollingState();
                break;
            case EnemyState.Battle:
                EnterBattleState();
                break;
            case EnemyState.Alert:
                EnterAlertState();
                break;
            case EnemyState.Pulled:
                break;
            case EnemyState.Feared:
                EnterFearedState();
                break;
            case EnemyState.Bounced:
                EnterBouncedState();
                break;
            case EnemyState.Stunned:
                EnterStunnedState();
                break;
            case EnemyState.Death:
                EnterDeathState();
                break;
        }

        state = newState;
    }

    private void EnterPatrollingState()
    {
        destinationPointIndex = 0;
        if (patrolPoints.Count > 0)
        {
            destinationPoint = patrolPoints[0].GetPointPosition();
        }

        patrolStopover = false;
        
        moveScript.SetPatrolSpeed();
        
        // spriteRenderer.color = new Color(1, 1, 1, 1);
        OnNormalAnimation?.Invoke();
    }

    private void EnterBattleState()
    {
        if(canSeePlayer || battleDetector.canSeeBattle)
            aggressionTimer = aggressionDuration;
        
        if(Random.Range(0, 100) < 50)
            enemySound?.PlayRegularSound();
        
        moveScript.SetBaseSpeed();
        // spriteRenderer.color = new Color(1, 160f/255, 0, 1);
        BattleStart();

        animationController.ResetTrigger("Warning");
        PlayChaseAnim();
    }

    private void EnterAlertState()
    {
        alertTimer = alertDuration;
        if (!fieldOfView.IsInBattleZone())
        {
            alertTimer /= 3;
        }
        destinationPoint = transform.position;
        
        alertChangeLookTimer = alertChangeLookDuration;
        alertLookDirIndex = Random.Range(0, 4);
        lookingDirection = EnemyUtility.base4Directions[alertLookDirIndex];
        
        tryingEnterAlert = false;
        // spriteRenderer.color = new Color(1, 1, 0, 1);
        PlayWarningAnim();
    }

    private void EnterFearedState()
    {
        // Прерываем движение по маршруту. По окончании эффекта необходимо восстановить
        CancelInvoke();
        OnNormalAnimation?.Invoke();
        destinationPoint = bodyBottom.position;
        rigidBody.velocity = Vector2.zero;
        // spriteRenderer.color = new Color(120f/255, 0, 1, 1);
    }

    private void EnterBouncedState()
    {
        // CancelInvoke();
        velocityBeforeBounce = rigidBody.velocity;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        rigidBody.velocity = Vector2.zero;
    }

    private void EnterStunnedState()
    {
        if (IsStunned)
            return;
        IsStunned = true;
        OnStunAnimation?.Invoke(true);
        stateBeforeStun = state;
        destinationPoint = bodyBottom.position;
        rigidBody.velocity = Vector2.zero;
        // spriteRenderer.color = new Color(1f, 0, 1f, 1);
        if (stateBeforeStun == EnemyState.Pulled)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            stateBeforeStun = stateBeforePull;
        }
    }

    protected virtual void EnterDeathState()
    {
        OnStunAnimation?.Invoke(false);
        enemySound?.PlayDeathSound();
        OnDieEvent?.Invoke();
        collider2D.enabled = false;
    }
    
    
    
    protected virtual void EnterAlertFromBattle()
    {
        EnterState(EnemyState.Alert);
    }
    
    protected virtual void BattleStart()
    {
        
    }
    
    
    public void TryEnterDeath()
    {
        if (state == EnemyState.Pulled)
        {
            stateBeforePull = EnemyState.Death;
        }
        else
        {
            EnterState(EnemyState.Death);
        }
    }
    
    #endregion

    protected void CheckPlayerDetection()
    {
        if (fieldOfView.canSeePlayer)
        {
            canSeePlayer = true;
            aggressionTimer = aggressionDuration;
        }
        else
        {
            canSeePlayer = false;
            if(!battleDetector.canSeeBattle)
                aggressionTimer -= Time.deltaTime;
        }

        if ((aggressionTimer <= 0 || fieldOfView.playerDisappeared))
        {
            canSeePlayer = false;
            if (!tryingEnterAlert)
            {
                tryingEnterAlert = true;
                EnterAlertFromBattle();
            }
        }
    }

    public void DeathEnd()
    {
        transform.GetComponent<EnemyHealth>().Death();
    }
    
    protected void OnCollisionEnter2D(Collision2D collisionObj)
    {

        if (collisionObj.gameObject.CompareTag("Obstacle") || collisionObj.gameObject.CompareTag("Relief"))
        {
            ChangeWorkaroundMovementDirection();
        }
    }
    
    
    #region Getters
    public EnemyState GetEnemyState() => state;
    public bool GetCanSeePlayer() => canSeePlayer;

    public virtual bool CanFall()
    {
        return state != EnemyState.Pulled && state != EnemyState.Death;
    }
    
    public Vector2 GetLookingDirection() => lookingDirection;
    
    #endregion

    #region Setters

    public void SetLookingDirection(Vector2 newLookDir) => lookingDirection=newLookDir;

    #endregion

    
    #region Animation

    public void SetAnimationDirection()
    {
        OnSetVerticals?.Invoke(rigidBody, lookingDirection);
    }

    private void SetAnimationMoveSpeed()
    {
        float moveMagnitude = (float)Math.Sqrt(
            Math.Pow(rigidBody.velocity.x, 2) + Math.Pow(rigidBody.velocity.y, 2));
        OnSetSpeed?.Invoke(moveMagnitude);
    }

    public void StartFallingAnimation()
    {
        animationController.SetBool(Falling, true);
    }
    
    private protected void PlayAttackAnim()
    {
        OnAttackAnimation?.Invoke();
    }

    private protected void PlayChaseAnim()
    {
        OnChaseAnimation?.Invoke();
    }

    private protected void PlayWarningAnim()
    {
        OnWarningAnimation?.Invoke();
    }

    #endregion

    #region Spawn

    public void SetSpawnVars(EnemySpawner newSpawner, string id)
    {
        spawner = newSpawner;
        spawnId = id;
    }
    public void SetSpawnVars(Utilities.ObjectPool newPool, string id)
    {
        spawnPool = newPool;
        spawnId = id;
    }
    
    public EnemySpawner GetSpawner() => spawner;
    public Utilities.ObjectPool GetSpawnPool() => spawnPool;
    public string GetSpawnId() => spawnId;

    #endregion
    
    
    #region Helper Methods
    
    protected bool NoObstaclesOnTheWay(LayerMask layerMask)
    {
        Vector2 directionToTarget = (playerBottom.position - bodyBottom.position).normalized;
        float distanceToTarget = (playerBottom.position - bodyBottom.position).magnitude;
        
        return !Physics2D.Raycast(bodyBottom.position, directionToTarget,
            distanceToTarget, layerMask);
    }

    #endregion

    public Vector3 GetPivotPoint() => bodyBottom.position;
}
