using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using ObjectLogicRealization.Health;
using UnityEngine;
using Pathfinding;
using Utilities;

public class DeathGod : EnemyScript
{
    private enum BattleState
    {
        Movement,
        Dash,
        HalberdThrow,
        JumpAttack,
        SpawnDaggers,
        MeleeAttackSeries,
        ShootingSpheres
    }

    private enum Phase
    {
        Phase1,
        Phase2,
        Phase3
    }
    
    private enum HalberdThrowState
    {
        Prep,
        Spawning,
        Catch,
        After
    }
    
    private enum JumpAttackState
    {
        Prep,
        Jump,
        After
    }
    
    private enum SpawnDaggersState
    {
        Prep,
        After
    }
    
    private enum MeleeAttackState
    {
        Prep,
        Attack,
        After
    }
    
    private enum ShootSphereState
    {
        Prep,
        Shooting,
        After
    }

    
    private static readonly int AttackDirX = Animator.StringToHash("AttackDirX");
    private static readonly int AttackDirY = Animator.StringToHash("AttackDirY");
    protected static readonly int AttackEnd = Animator.StringToHash("AttackEnd");

    private BattleState battleState;
    private Phase phase;
    private HalberdThrowState halberdThrowState;
    private JumpAttackState jumpAttackState;
    private SpawnDaggersState spawnDaggersState;
    private MeleeAttackState meleeAttackState;
    private ShootSphereState shootSphereState;

    private ObjectPool objectPools;
    
    private float centerBottomDistance;

    [SerializeField] private DeathGodArena arena;
    [SerializeField] private ArenaBarrelsManager arenaBarrelsManager;
    
    
    [Header("HALBERD THROW")] 
    [SerializeField] private HalberdThrowParameters halberdThrowSingleParameters;
    [SerializeField] private HalberdThrowParameters halberdThrowMultyParameters;
    private HalberdThrowParameters halberdThrowParameters;
    
    [Header("JUMP ATTACK")] 
    [SerializeField] private JumpAttackParameters jumpAttackParameters;
    
    [Header("SPAWN DAGGERS")] 
    [SerializeField] private SpawnDaggersParameters spawnDaggersParameters;

    [Header("MELEE ATTACK SERIES")]
    [SerializeField] private List<MeleeAttackSeriesParameters> allMeleeAttackSeries;
    private MeleeAttackSeriesParameters currentMeleeAttackSeries;
    
    [Header("SHOOT SPHERE")] 
    [SerializeField] private ShootSphereParameters shootSphereParameters;
    
    

    [Header("Movement")] 
    [SerializeField] private CircularMovementParameters circularMovement;
    private enum MovementMode
    {
        Pursuit,
        Circular
    }
    private MovementMode movementMode;
    [SerializeField] private float changeMovementStrategyCooldown;
    
    [Header("Dash")] 
    [SerializeField] private float dashMaxDistance;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCooldownMin;
    [SerializeField] private float dashCooldownMax;
    private float dashCooldownTimer;
    private Vector2 dashDirection;
    private float dashTimer;
    


    [Header("Phases")] 
    [SerializeField] private PhaseParameters phase1Parameters;
    [SerializeField] private PhaseParameters phase2Parameters;
    [SerializeField] private PhaseParameters phase3Parameters;

    private float attackCooldownMin;
    private float attackCooldownMax;
    private ActionSelector attackSelector;
    private ActionSelector movementSelector;
    private float attackCooldownTimer;

    
    private EnemyHealth healthScript;
    
    
    
    protected override void EnemyStart()
    {
        centerBottomDistance = (transform.position - bodyBottom.position).magnitude;

        healthScript = GetComponent<EnemyHealth>();
        
        battleState = BattleState.Movement;
        movementMode = MovementMode.Pursuit;
        
        StartPhase(Phase.Phase1);
        StartCoroutine(CheckPhaseChangeCondition());
        
        StartCoroutine(ChooseMovementModeRoutine());
        circularMovement.ClockwiseDirection = 1;
        StartCoroutine(ChangeCircularMovementDirectionRoutine());
        StartDashCooldown();
        
        objectPools = GetComponent<ObjectPool>();
        halberdThrowParameters = halberdThrowSingleParameters;
        objectPools.InitializePool("HalberdPortal", 5, halberdThrowParameters.portalPrefab);
        objectPools.InitializePool("ThrownHalberd", 5, halberdThrowParameters.halberdPrefab);
        
        jumpAttackParameters.AreaScript = GetComponentInChildren<DeathGodJumpAttackArea>();
        
        objectPools.InitializePool("DeadManDagger", 15, spawnDaggersParameters.deadManDaggerPrefab);
        StartAttackCooldown();
        
        foreach (var meleeAttackSeries in allMeleeAttackSeries) 
            meleeAttackSeries.InitAttacks();
        
        objectPools.InitializePool("DeathGodSphere", 15, shootSphereParameters.spherePrefab);
        
        StartCoroutine(CheckDistanceToPlayer());
    }


    protected override void BattleUpdate()
    {
        lookingDirection = (player.transform.position - transform.position).normalized;
        
        switch (battleState)
        {
            case BattleState.Movement:
                PerformMovement();
                
                attackCooldownTimer -= Time.deltaTime;
                if (attackCooldownTimer <= 0)
                {
                    StartAttack();
                }

                break;
            
            case BattleState.Dash:
                DashUpdate();
                break;
            
            case BattleState.HalberdThrow:
                HalberdThrowUpdate();
                break;
            case BattleState.JumpAttack:
                JumpAttackUpdate();
                break;
            case BattleState.SpawnDaggers:
                SpawnDaggersUpdate();
                break;
            case BattleState.MeleeAttackSeries:
                MeleeAttackSeriesUpdate();
                break;
            case BattleState.ShootingSpheres:
                ShootSphereUpdate();
                break;
            
        }
        
    }


    #region Phase Management
    
    [System.Serializable]
    private class PhaseParameters
    {
        public float attackCooldownMin;
        public float attackCooldownMax;
        public ActionSelector attackSelector;
        
        public ActionSelector movementSelector;
    }

    private void StartPhase(Phase newPhase)
    {
        phase = newPhase;

        switch (newPhase)
        {
            case Phase.Phase1:
                ActivatePhaseParameters(phase1Parameters);
                arenaBarrelsManager.StartSpawningBarrels();
                break;
            case Phase.Phase2:
                ActivatePhaseParameters(phase2Parameters);
                arenaBarrelsManager.StopSpawningBarrels();
                StartSpawningEnemies();
                break;
            case Phase.Phase3:
                ActivatePhaseParameters(phase3Parameters);
                arenaBarrelsManager.StartSpawningBarrels();
                StopSpawningEnemies();
                break;
        }
    }

    private void ActivatePhaseParameters(PhaseParameters newParameters)
    {
        attackCooldownMin = newParameters.attackCooldownMin;
        attackCooldownMax = newParameters.attackCooldownMax;
        attackSelector = newParameters.attackSelector;
        movementSelector = newParameters.movementSelector;
    }

    IEnumerator CheckPhaseChangeCondition()
    {
        for (;;)
        {
            yield return new WaitForSeconds(0.1f);
            float hpPercent = healthScript.GetCurrentHealth() / healthScript.GetMaxHealthPoints();

            switch (phase)
            {
                case Phase.Phase1:
                    if(hpPercent <= 0.6666)
                        StartPhase(Phase.Phase2);
                    break;
                case Phase.Phase2:
                    if(hpPercent <= 0.3333)
                        StartPhase(Phase.Phase3);
                    break;
                case Phase.Phase3:
                    break;
            }
        }
    }

    #endregion
    
    
    #region Halberd Throw
    
    [System.Serializable]
    private class HalberdThrowParameters
    {
        public float cooldown;
        private float cooldownTimer;
        
        public float prepDuration;
        public float spawnDuration;
        public float catchDuration;
        public float afterDuration;
        private float timer;

        public int intermediateThrowsNumber;
        private int throwsLeft;
        public float intermediateThrowDelay;

        public float spawnIntoPlayerProb;
        
        public GameObject portalPrefab;
        public GameObject halberdPrefab;

        private Vector2[] portalsPositions;
        
        public float CooldownTimer
        {
            get => cooldownTimer;
            set => cooldownTimer = value;
        }
        
        public float Timer
        {
            get => timer;
            set => timer = value;
        }
        
        public int ThrowsLeft
        {
            get => throwsLeft;
            set => throwsLeft = value;
        }

        public Vector2[] PortalsPositions
        {
            get => portalsPositions;
            set => portalsPositions = value;
        }
        
        public void StartCooldown()
        {
            cooldownTimer = cooldown;
        }
    }
    
    
    private void HalberdThrowUpdate()
    {
        rigidBody.velocity = Vector2.zero;

        halberdThrowParameters.Timer -= Time.deltaTime;
        if (halberdThrowParameters.Timer <= 0)
        {
            switch (halberdThrowState)
            {
                case HalberdThrowState.Prep:
                    StartHalberdThrowAttack();
                    break;
                case HalberdThrowState.Spawning:
                    if (halberdThrowParameters.ThrowsLeft > 0)
                    {
                        IntermedHalberdThrow();
                        halberdThrowParameters.ThrowsLeft -= 1;
                    }
                    else
                        FinHalberdThrow();
                    break;
                case HalberdThrowState.Catch:
                    EndHalberdCatch();
                    break;
                case HalberdThrowState.After:
                    EndHalberdThrowAfter();
                    break;
            }
        }
    }

    private void EnterHalberdThrowState(HalberdThrowParameters parameters)
    {
        halberdThrowParameters = parameters;
        battleState = BattleState.HalberdThrow;
        StartHalberdThrowPrep();
    }

    private void StartHalberdThrowPrep()
    {
        halberdThrowState = HalberdThrowState.Prep;
        halberdThrowParameters.Timer = halberdThrowParameters.prepDuration;
        halberdThrowParameters.ThrowsLeft = halberdThrowParameters.intermediateThrowsNumber;

        spriteRenderer.color = Color.magenta;
    }

    private void StartHalberdThrowAttack()
    {
        halberdThrowState = HalberdThrowState.Spawning;
        halberdThrowParameters.Timer = halberdThrowParameters.spawnDuration;
        
        halberdThrowParameters.PortalsPositions = GetPortalsSpawnPoints();
        CreateHalberdPortal(halberdThrowParameters.PortalsPositions[0]);
        if(halberdThrowParameters.intermediateThrowsNumber == 0)
            CreateHalberdPortal(halberdThrowParameters.PortalsPositions[1]);
        
        ThrowHalberd(transform.position, halberdThrowParameters.PortalsPositions[0]);
        
    }

    private void ThrowHalberd(Vector3 startPoint, Vector3 goalPoint)
    {
        GameObject thrownHalberdObject =
            objectPools.SpawnFromPool("ThrownHalberd", startPoint, Quaternion.identity);
        DeathGodThrownHalberd thrownHalberd = thrownHalberdObject.GetComponent<DeathGodThrownHalberd>();
        float angle = EnemyUtility.AngleBetweenTwoPoints(goalPoint, startPoint);
        Vector2 direction = (goalPoint - startPoint).normalized;
        thrownHalberd.GetComponent<DeathGodThrownHalberd>().SetDirection(
            direction, 
            angle);
        thrownHalberd.GetComponent<DeathGodThrownHalberd>().SetGoalPoint(goalPoint);
            
        thrownHalberd.SetShadowOffset(centerBottomDistance);
        // thrownHalberd.CheckStartCollision(bodyBottom);
    }
    
    Vector2[] GetPortalsSpawnPoints(bool randomGeneration = false)
    {
        Vector2 center = arena.transform.position;
        var a = arena.semiMajorAxis;
        var b = arena.semiMinorAxis;
        var point = transform.position;
        var direction = (player.transform.position - transform.position).normalized;

        if (randomGeneration)
        {
            if (Random.Range(0f, 1f) <= halberdThrowParameters.spawnIntoPlayerProb)
                point = player.transform.position;
            else
                point = GetRandomPointOnArena();
            
            direction = Random.onUnitSphere;
        }
        
        float h = center.x, k = center.y;
        float x0 = point.x, y0 = point.y;
        float dx = direction.x, dy = direction.y;

        // Quadratic coefficients
        float A = (dx * dx) / (a * a) + (dy * dy) / (b * b);
        float B = 2 * ((dx * (x0 - h)) / (a * a) + (dy * (y0 - k)) / (b * b));
        float C = ((x0 - h) * (x0 - h)) / (a * a) + ((y0 - k) * (y0 - k)) / (b * b) - 1;

        // Discriminant
        float discriminant = B * B - 4 * A * C;

        if (discriminant < 0)
        {
            // No intersection
            return null;
        }

        // Calculate t values
        float sqrtDiscriminant = Mathf.Sqrt(discriminant);
        float t1 = (-B + sqrtDiscriminant) / (2 * A);
        float t2 = (-B - sqrtDiscriminant) / (2 * A);

        // Calculate intersection points
        Vector2 intersection1 = new Vector2(x0 + t1 * dx, y0 + t1 * dy);
        Vector2 intersection2 = new Vector2(x0 + t2 * dx, y0 + t2 * dy);

        return new Vector2[] { intersection1, intersection2 };
    }
    
    private void IntermedHalberdThrow()
    {
        halberdThrowState = HalberdThrowState.Spawning;
        halberdThrowParameters.Timer = halberdThrowParameters.spawnDuration;

        var intermediatePortalsPositions = GetPortalsSpawnPoints(true);
        CreateHalberdPortal(intermediatePortalsPositions[0]);
        CreateHalberdPortal(intermediatePortalsPositions[1]);

        StartCoroutine(DelayedThrow(intermediatePortalsPositions));
    }

    IEnumerator DelayedThrow(Vector2[] intermediatePortalsPositions=null)
    {
        yield return new WaitForSeconds(halberdThrowParameters.intermediateThrowDelay);
        
        if(intermediatePortalsPositions == null)
            ThrowHalberd( halberdThrowParameters.PortalsPositions[1], 
                transform.position);
        else
            ThrowHalberd(intermediatePortalsPositions[0],
                intermediatePortalsPositions[1]);
    }

    private void FinHalberdThrow()
    {
        halberdThrowState = HalberdThrowState.Catch;
        halberdThrowParameters.Timer = halberdThrowParameters.catchDuration;
        if (halberdThrowParameters.intermediateThrowsNumber > 0)
            halberdThrowParameters.Timer += halberdThrowParameters.intermediateThrowDelay;

        if (halberdThrowParameters.intermediateThrowsNumber > 0)
        {
            CreateHalberdPortal(halberdThrowParameters.PortalsPositions[1]);
            StartCoroutine(DelayedThrow());
        }
        else
            ThrowHalberd( halberdThrowParameters.PortalsPositions[1],
                transform.position);
    }
    
    private void EndHalberdCatch()
    {
        halberdThrowState = HalberdThrowState.After;
        halberdThrowParameters.Timer = halberdThrowParameters.afterDuration;
    }
    
    private void EndHalberdThrowAfter()
    {
        StartAttackCooldown();
        spriteRenderer.color = Color.white;
        EnterMovementState();
    }
    
    

    private void CreateHalberdPortal(Vector2 spawnPoint)
    {
        GameObject newPortal =
            objectPools.SpawnFromPool("HalberdPortal", spawnPoint, Quaternion.identity);
        newPortal.GetComponent<DeathGodPortal>().SetLifeTime(3);
        newPortal.transform.SetParent(null);
    }
    
    
    
    #endregion
    
    
    #region Jump Attack
    
    [System.Serializable]
    private class JumpAttackParameters
    {
        public float cooldown;
        private float cooldownTimer;
        
        public float prepDuration;
        public float duration;
        public float afterDuration;
        private float timer;

        public float jumpHeight;
        
        public float damage;

        private Vector3 jumpStartPoint;
        private Vector3 jumpEndPoint;
        
        private DeathGodJumpAttackArea areaScript;
        
        public float CooldownTimer
        {
            get => cooldownTimer;
            set => cooldownTimer = value;
        }
        
        public float Timer
        {
            get => timer;
            set => timer = value;
        }
        
        public Vector3 JumpStartPoint
        {
            get => jumpStartPoint;
            set => jumpStartPoint = value;
        }

        public Vector3 JumpEndPoint
        {
            get => jumpEndPoint;
            set => jumpEndPoint = value;
        }

        public DeathGodJumpAttackArea AreaScript
        {
            get => areaScript;
            set => areaScript = value;
        }

        public void StartCooldown()
        {
            cooldownTimer = cooldown;
        }
    }
    
    public float GetJumpAttackDamage() => jumpAttackParameters.damage;
    
    
    private void JumpAttackUpdate()
    {

        switch (jumpAttackState)
        {
            case JumpAttackState.Prep:
                rigidBody.velocity = Vector2.zero;
                break;
            case JumpAttackState.Jump:
                UpdatePositionInJump();
                break;
            case JumpAttackState.After:
                rigidBody.velocity = Vector2.zero;
                break;
        }

        jumpAttackParameters.Timer -= Time.deltaTime;
        if (jumpAttackParameters.Timer <= 0)
        {
            switch (jumpAttackState)
            {
                case JumpAttackState.Prep:
                    StartJumpAttack();
                    break;
                case JumpAttackState.Jump:
                    EndJumpAttack();
                    break;
                case JumpAttackState.After:
                    EndJumpAttackAfter();
                    break;
            }
        }
    }
    
    private void EnterJumpAttackState()
    {
        battleState = BattleState.JumpAttack;
        StartJumpAttackPrep();
    }
    
    private void StartJumpAttackPrep()
    {
        jumpAttackState = JumpAttackState.Prep;
        jumpAttackParameters.Timer = jumpAttackParameters.prepDuration;

        spriteRenderer.color = Color.magenta;
    }
    
    private void StartJumpAttack()
    {
        jumpAttackParameters.JumpEndPoint = playerBottom.position + Vector3.up*centerBottomDistance;
        jumpAttackParameters.JumpStartPoint = transform.position;
        
        jumpAttackState = JumpAttackState.Jump;
        jumpAttackParameters.Timer = jumpAttackParameters.duration;
    }
    
    private void EndJumpAttack()
    {
        transform.position = jumpAttackParameters.JumpEndPoint;
        jumpAttackParameters.AreaScript.StartAttack();
        
        jumpAttackState = JumpAttackState.After;
        jumpAttackParameters.Timer = jumpAttackParameters.afterDuration;
    }
    
    private void EndJumpAttackAfter()
    {
        StartAttackCooldown();
        spriteRenderer.color = Color.white;
        EnterMovementState();
    }

    private void UpdatePositionInJump()
    {
        float elapsedTime = jumpAttackParameters.duration - jumpAttackParameters.Timer;
        float jumpDuration = jumpAttackParameters.duration;
        var startPosition = jumpAttackParameters.JumpStartPoint;
        var targetPosition = jumpAttackParameters.JumpEndPoint;
        var jumpHeight = jumpAttackParameters.jumpHeight;
        
        float t = elapsedTime / jumpDuration;
        t = JumpTimeScale(t);

        // Modify time with a sine function to create the gravity-like effect
        float gravityEffect = Mathf.Sin(t * Mathf.PI); // Creates slow down at peak

        // Interpolate X and Y positions
        Vector2 currentPosition = Vector2.Lerp(startPosition, targetPosition, t);

        // Add parabolic height based on gravity-like easing
        float height = jumpHeight * gravityEffect;
        transform.position = new Vector3(currentPosition.x, currentPosition.y + height, transform.position.z);

    }

    private float JumpTimeScale(float x)
    {
        // Скейл времени прыжка по кубической параболе
        // Время x берется по относительной шкале - от 0 до 1
        // Время после скейла также должно быть в интервале от 0 до 1
        // f = ax^3 + bx^2 + c
        // c = 1 - a - b, чтобы f(1) = 1
        // Настраивать можно по параметрам k и a
        // k - значение параболы в x=0.5
        
        float k = 0.35f;
        float a = 1.2f;
        float b = -1.5f*a - 4*k + 2;
        float c = 1 - a - b;
        return a*x*x*x + b*x*x + c*x;
    }
    
    #endregion


    #region Spawn Daggers

    [System.Serializable]
    private class SpawnDaggersParameters
    {
        public float cooldown;
        private float cooldownTimer;
        
        public float prepDuration;
        public float duration;
        public float afterDuration;
        private float timer;

        public int numberOfSpawns;
        
        [Range(0,1)] public float underPlayerSpawnChance;
        
        public GameObject deadManDaggerPrefab;
        
        public float CooldownTimer
        {
            get => cooldownTimer;
            set => cooldownTimer = value;
        }
        
        public float Timer
        {
            get => timer;
            set => timer = value;
        }

        public void StartCooldown()
        {
            cooldownTimer = cooldown;
        }
        
    }
    
    private void SpawnDaggersUpdate()
    {
        rigidBody.velocity = Vector2.zero;

        spawnDaggersParameters.Timer -= Time.deltaTime;
        if (spawnDaggersParameters.Timer <= 0)
        {
            switch (spawnDaggersState)
            {
                case SpawnDaggersState.Prep:
                    StartSpawnDaggers();
                    break;
                case SpawnDaggersState.After:
                    EndSpawnDaggersAfter();
                    break;
            }
        }
    }
    
    private void EnterSpawnDaggersState()
    {
        battleState = BattleState.SpawnDaggers;
        StartSpawnDaggersPrep();
    }

    private void StartSpawnDaggersPrep()
    {
        spawnDaggersState = SpawnDaggersState.Prep;
        spawnDaggersParameters.Timer = spawnDaggersParameters.prepDuration;

        spriteRenderer.color = Color.magenta;
    }

    private void StartSpawnDaggers()
    {
        spawnDaggersState = SpawnDaggersState.After;
        spawnDaggersParameters.Timer = spawnDaggersParameters.afterDuration;

        StartCoroutine(SpawnDaggers());
    }

    IEnumerator SpawnDaggers()
    {
        float spawnInterval = spawnDaggersParameters.duration / spawnDaggersParameters.numberOfSpawns;
        for (int i = 0; i < spawnDaggersParameters.numberOfSpawns; i++)
        {
            SpawnDagger();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnDagger()
    {
        var spawnPoint = GetDaggerSpawnPoint();
        
        GameObject newDagger =
            objectPools.SpawnFromPool("DeadManDagger", spawnPoint, Quaternion.identity);
        newDagger.transform.SetParent(null);
        newDagger.GetComponent<DeadManDagger>().StartAttack();
    }

    private Vector3 GetDaggerSpawnPoint()
    {

        if (Random.Range(0f, 1f) <= spawnDaggersParameters.underPlayerSpawnChance)
        {
            return playerBottom.position;
        }

        return GetRandomPointOnArena();
    }

    private void EndSpawnDaggersAfter()
    {
        StartAttackCooldown();
        spriteRenderer.color = Color.white;
        EnterMovementState();
    }

    #endregion


    #region Melee Attack

    [System.Serializable]
    private class MeleeAttackParameters
    {
        
        public float prepDuration;
        public float duration;
        public float afterDuration;
        private float timer;

        public float movementSpeed;
        
        public float damage;

        public GameObject attackObject;
        private DeathGodMeleeAttack script;
        
        public float Timer
        {
            get => timer;
            set => timer = value;
        }

        public DeathGodMeleeAttack Script
        {
            get => script;
        }

        public void InitScript()
        {
            script = attackObject.GetComponent<DeathGodMeleeAttack>();
            script.SetDamage(damage);
        }

    }
    
    [System.Serializable]
    private class MeleeAttackSeriesParameters
    {
        
        public float cooldown;
        private float cooldownTimer;

        public MeleeAttackParameters[] meleeAttacks;
        private int currentMeleeAttackIndex;

        private Vector3 currentAttackDirection;
        
        public float CooldownTimer
        {
            get => cooldownTimer;
            set => cooldownTimer = value;
        }
        
        public MeleeAttackParameters CurrentAttack
        {
            get => meleeAttacks[currentMeleeAttackIndex];
        }
        
        public int CurrentAttackIndex
        {
            get => currentMeleeAttackIndex;
            set => currentMeleeAttackIndex = value;
        }
        
        public Vector3 AttackDirection
        {
            get => currentAttackDirection;
            set => currentAttackDirection = value;
        }
        
        public void StartCooldown()
        {
            cooldownTimer = cooldown;
        }

        public void InitAttacks()
        {
            foreach (var meleeAttack in meleeAttacks)
            {
                meleeAttack.InitScript();
            }
        }

    }
    
    private void MeleeAttackSeriesUpdate()
    {
        // rigidBody.velocity = Vector2.zero;

        currentMeleeAttackSeries.CurrentAttack.Timer -= Time.deltaTime;
        if (currentMeleeAttackSeries.CurrentAttack.Timer <= 0)
        {
            switch (meleeAttackState)
            {
                case MeleeAttackState.Prep:
                    StartMeleeAttack();
                    break;
                case MeleeAttackState.Attack:
                    EndMeleeAttack();
                    break;
                case MeleeAttackState.After:
                    currentMeleeAttackSeries.CurrentAttackIndex += 1;
                    if (currentMeleeAttackSeries.CurrentAttackIndex <
                        currentMeleeAttackSeries.meleeAttacks.Length)
                        StartMeleeAttackPrep();
                    else
                        EndMeleeAttackSeries();
                    
                    break;
            }
        }
    }
    
    private void EnterMeleeAttackSeriesState(MeleeAttackSeriesParameters seriesParameters)
    {
        currentMeleeAttackSeries = seriesParameters;
        currentMeleeAttackSeries.CurrentAttackIndex = 0;
        
        battleState = BattleState.MeleeAttackSeries;
        StartMeleeAttackPrep();
    }
    
    private void StartMeleeAttackPrep()
    {
        meleeAttackState = MeleeAttackState.Prep;
        currentMeleeAttackSeries.CurrentAttack.Timer = 
            currentMeleeAttackSeries.CurrentAttack.prepDuration;
        
        rigidBody.velocity = Vector2.zero;
        // currentMeleeAttackSeries.AttackDirection = (player.transform.position - transform.position).normalized;
        currentMeleeAttackSeries.AttackDirection = GetAttackDirection();

        spriteRenderer.color = Color.magenta;
    }
    
    private Vector2 GetAttackDirection()
    {
        float angle = EnemyUtility.AngleBetweenTwoPoints(player.transform.position, transform.position);
        Vector2 res = EnemyUtility.DirectionToVector(EnemyUtility.AngleToDirection(angle));
        return res;
    }

    private void StartMeleeAttack()
    {
        meleeAttackState = MeleeAttackState.Attack;
        currentMeleeAttackSeries.CurrentAttack.Timer = currentMeleeAttackSeries.CurrentAttack.duration;
        currentMeleeAttackSeries.CurrentAttack.Script.StartAttack(currentMeleeAttackSeries.AttackDirection);
        MoveInMeleeAttack();
    }

    private void MoveInMeleeAttack()
    {
        rigidBody.velocity = currentMeleeAttackSeries.AttackDirection * 
                             currentMeleeAttackSeries.CurrentAttack.movementSpeed;
        StartCoroutine(StopMeleeAttackMovement());
    }

    IEnumerator StopMeleeAttackMovement()
    {
        yield return new WaitForSeconds(0.1f);
        rigidBody.velocity = Vector2.zero;
    }

    private void EndMeleeAttack()
    {
        meleeAttackState = MeleeAttackState.After;
        currentMeleeAttackSeries.CurrentAttack.Timer = currentMeleeAttackSeries.CurrentAttack.afterDuration;
        currentMeleeAttackSeries.CurrentAttack.Script.EndAttack();
    }

    private void EndMeleeAttackSeries()
    {
        StartAttackCooldown();
        spriteRenderer.color = Color.white;
        EnterMovementState();
    }

    #endregion


    #region Shoot Sphere

    [System.Serializable]
    private class ShootSphereParameters
    {
        public float cooldown;
        private float cooldownTimer;
        
        public float prepDuration;
        public float betweenDuration;
        public float afterDuration;
        private float timer;
        
        public int spheresNumber;
        private int spheresLeft;

        [Range(0,1)] public float underPlayerSpawnChance;
        private bool spawnedOnPlayer;
        
        public GameObject spherePrefab;
        
        public int SpheresLeft
        {
            get => spheresLeft;
            set => spheresLeft = value;
        }
        
        public bool SpawnedOnPlayer
        {
            get => spawnedOnPlayer;
            set => spawnedOnPlayer = value;
        }
        
        public float CooldownTimer
        {
            get => cooldownTimer;
            set => cooldownTimer = value;
        }
        
        public float Timer
        {
            get => timer;
            set => timer = value;
        }
        
        public void StartCooldown()
        {
            cooldownTimer = cooldown;
        }
    }
    
    private void ShootSphereUpdate()
    {
        rigidBody.velocity = Vector2.zero;

        shootSphereParameters.Timer -= Time.deltaTime;
        if (shootSphereParameters.Timer <= 0)
        {
            switch (shootSphereState)
            {
                case ShootSphereState.Prep:
                    ShootSphere();
                    break;
                case ShootSphereState.Shooting:
                    if (shootSphereParameters.SpheresLeft > 0)
                        ShootSphere();
                    else
                        StopShooting();
                    break;
                case ShootSphereState.After:
                    EndShootSphereAfter();
                    break;
            }
        }
        
    }
    
    private void EnterShootSphereState()
    {
        battleState = BattleState.ShootingSpheres;
        StartShootSpherePrep();
    }
    
    private void StartShootSpherePrep()
    {
        shootSphereState = ShootSphereState.Prep;
        shootSphereParameters.Timer = shootSphereParameters.prepDuration;
        shootSphereParameters.SpheresLeft = shootSphereParameters.spheresNumber;
        shootSphereParameters.SpawnedOnPlayer = false;

        spriteRenderer.color = Color.magenta;
    }

    private void ShootSphere()
    {
        shootSphereState = ShootSphereState.Shooting;
        shootSphereParameters.Timer = shootSphereParameters.betweenDuration;
        shootSphereParameters.SpheresLeft -= 1;
        
        var goalPoint = GetSphereGoalPoint();
        
        GameObject newSphere =
            objectPools.SpawnFromPool("DeathGodSphere", transform.position, Quaternion.identity);
        newSphere.transform.SetParent(null);
        newSphere.GetComponent<DeathGodSphere>().SetGoalPoint(goalPoint);
    }

    private Vector3 GetSphereGoalPoint()
    {
        if (Random.Range(0f, 1f) <= shootSphereParameters.underPlayerSpawnChance &&
            !shootSphereParameters.SpawnedOnPlayer)
        {
            shootSphereParameters.SpawnedOnPlayer = true;
            return playerBottom.position;
        }

        return GetRandomPointOnArena();
    }
    
    private void StopShooting()
    {
        shootSphereState = ShootSphereState.After;
        shootSphereParameters.Timer = shootSphereParameters.afterDuration;
    }
    
    private void EndShootSphereAfter()
    {
        StartAttackCooldown();
        spriteRenderer.color = Color.white;
        EnterMovementState();
    }

    #endregion
    

    #region Choose Attack

    private void StartAttack()
    {
        float distanceToPlayer = (playerBottom.position - bodyBottom.position).magnitude;
        string attackName = attackSelector.GetAction(distanceToPlayer);

        switch (attackName)
        {
            case "Halberd Throw":
                EnterHalberdThrowState(halberdThrowSingleParameters);
                break;
            case "Jump Attack":
                EnterJumpAttackState();
                break;
            case "Spawn Daggers":
                EnterSpawnDaggersState();
                break;
            case "Melee Attack Series":
                var meleeAttackSeries = ChooseMeleeAttackSeries();
                EnterMeleeAttackSeriesState(meleeAttackSeries);
                break;
            case "Shoot Spheres":
                EnterShootSphereState();
                break;
            case "Halberd Throw Multy":
                EnterHalberdThrowState(halberdThrowMultyParameters);
                break;
        }
    }

    private MeleeAttackSeriesParameters ChooseMeleeAttackSeries()
    {
        int randomIndex = Random.Range(0, allMeleeAttackSeries.Count);
        return allMeleeAttackSeries[randomIndex];
    }

    private void StartAttackCooldown()
    {
        attackCooldownTimer = Random.Range(attackCooldownMin, attackCooldownMax);
    }

    #endregion



    #region Movement
    
    private void EnterMovementState()
    {
        battleState = BattleState.Movement;
    }

    private void PursuitMovement()
    {
        destinationPoint = playerBottom.position;
        MoveToDestination();
    }
    
    [System.Serializable]
    private class CircularMovementParameters
    {
        
        public float circularSpeed;
        public float approachSpeed;
        private int clockwiseDirection;
        public float outerRadius;
        public float innerRadius;
        public float stopRadius;
        public float changeDirectionCooldown;
        
        public int ClockwiseDirection
        {
            get => clockwiseDirection;
            set => clockwiseDirection = value;
        }
    }
    
    private void CircularMovement()
    {
        Vector2 vectorToPlayer = (playerBottom.position - bodyBottom.position);
        
        float distanceToPlayer = vectorToPlayer.magnitude;
        if (distanceToPlayer < circularMovement.stopRadius)
        {
            rigidBody.velocity = -vectorToPlayer.normalized * circularMovement.approachSpeed;
            return;
        }
        
        
        Vector2 tangentVector = Vector2.Perpendicular(vectorToPlayer.normalized);
        tangentVector *= circularMovement.ClockwiseDirection;

        rigidBody.velocity = circularMovement.circularSpeed * tangentVector;
        
        if (distanceToPlayer > circularMovement.outerRadius)
        {
            rigidBody.velocity += vectorToPlayer.normalized * circularMovement.approachSpeed;
        }
        else if (distanceToPlayer < circularMovement.innerRadius)
        {
            rigidBody.velocity -= vectorToPlayer.normalized * (0.75f * circularMovement.approachSpeed);
        }
        
    }
    
    private void ChangeCircularMovementDirection()
    {
        circularMovement.ClockwiseDirection *= -1;
    }

    IEnumerator ChangeCircularMovementDirectionRoutine()
    {
        for (;;)
        {
            float randomCooldown = Random.Range(0.5f * circularMovement.changeDirectionCooldown,
                1.5f * circularMovement.changeDirectionCooldown);
            yield return new WaitForSeconds(randomCooldown);
            ChangeCircularMovementDirection();
        }
    }

    private void PerformMovement()
    {
        dashCooldownTimer -= Time.deltaTime;
        switch (movementMode)
        {
            case MovementMode.Pursuit:
                PursuitMovement();
                if (dashCooldownTimer <= 0)
                    PerformDash();
                break;
            case MovementMode.Circular:
                CircularMovement();
                break;
        }
    }

    private void ChooseMovementMode()
    {
        float distanceToPlayer = (playerBottom.position - bodyBottom.position).magnitude;
        string movementModeString = movementSelector.GetAction(distanceToPlayer);
        switch (movementModeString)
        {
            case "Pursuit":
                movementMode = MovementMode.Pursuit;
                break;
            case "Circular":
                movementMode = MovementMode.Circular;
                break;
        }
        
    }
    
    IEnumerator ChooseMovementModeRoutine()
    {
        for (;;)
        {
            ChooseMovementMode();
            float randomCooldown = Random.Range(0.5f * changeMovementStrategyCooldown,
                1.5f * changeMovementStrategyCooldown);
            yield return new WaitForSeconds(randomCooldown);
        }
    }

    #endregion

    
    #region Dash

    private void PerformDash()
    {
        Vector2 vectorToPlayer = playerBottom.position - bodyBottom.position;
        dashDirection = vectorToPlayer.normalized;
        
        float distanceToPlayer = vectorToPlayer.magnitude;
        float dashDistance = distanceToPlayer - 1 < dashMaxDistance ? distanceToPlayer - 1 : dashMaxDistance;
        dashTimer = dashDistance / dashSpeed;

        battleState = BattleState.Dash;
    }

    private void DashUpdate()
    {
        rigidBody.velocity = dashDirection * dashSpeed;
        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0)
        {
            EnterMovementState();
            StartDashCooldown();
        }
    }
    
    private void StartDashCooldown()
    {
        dashCooldownTimer = Random.Range(dashCooldownMin, dashCooldownMax);
    }

    #endregion


    #region Spawn Enemies

    private void StartSpawningEnemies()
    {
        arena.StartSpawningEnemies();
    }
    
    private void StopSpawningEnemies()
    {
        arena.StopSpawningEnemies();
    }

    #endregion


    private Vector3 GetRandomPointOnArena()
    {
        return arena.GetRandomPointOnArena();
    }
    
    protected override void EnterAlertFromBattle()
    {
        StartCoroutine(TryToEnterAlert());
    }

    IEnumerator TryToEnterAlert()
    {
        for (;;)
        {
            if (battleState == BattleState.Movement)
            {
                break;
            }
            yield return new WaitForSeconds(.1f);
        }
        EnterState(EnemyState.Alert);
    }
    
    IEnumerator CheckDistanceToPlayer()
    {
        float distanceToSlow = 3f;
        float slowSpeed = 0;
        for(;;)
        {
            if (state == EnemyState.Battle && battleState == BattleState.Movement)
            {
                
                if ((player.transform.position - transform.position).magnitude < distanceToSlow && 
                        NoObstaclesOnTheWay(obstacleReliefMask))
                {
                    moveScript.SetSpeed(slowSpeed);
                } else
                {
                    moveScript.SetBaseSpeed();
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collisionObj)
    {

        if (battleState == BattleState.Movement &&
            (collisionObj.gameObject.CompareTag("Obstacle") || collisionObj.gameObject.CompareTag("Relief")))
        {
            ChangeCircularMovementDirection();
        }
    }

    public override bool IsIntangible() => true;


}
