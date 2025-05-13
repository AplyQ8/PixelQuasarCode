using System;
using System.Collections;
using Main_hero.HookScripts.HookStrategies;
using ObjectLogicInterfaces;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnergySpirit : MonoBehaviour, IHookable
{

    public enum EnergySpiritColor
    {
        Yellow,
        Green,
        Blue,
        Purple
    }

    public enum SpiritState
    {
        Appearing,
        Moving,
        Pulled,
        Stabilizing
    }

    private SpiritState state;
    public EnergySpiritColor color;
    
    [Header("Movement Settings")]
    public float minSpeed = 1f; // Minimum speed
    public float maxSpeed = 3f; // Maximum speed

    public float minDirChangeInterval;
    public float maxDirChangeInterval;

    public float minTurningDuration;
    public float maxTurningDuration;

    public float stabilizingDuration;
    private float stabilizingTimer;

    public float appearingDuration;

    [Header("Area Settings")]
    public Collider2D movementArea; // Area collider
    
    protected Vector2 currentDirection;
    private Vector2 prevDirection;
    private Vector2 nextDirection;
    protected float currentSpeed;
    
    private float directionChangeTimer;
    
    private float turningTimer;
    private float currentTurningDuration;
    private bool turning;
    
    protected Rigidbody2D rb;
    private Collider2D _collider2D;
    
    private Transform _hookTransform;
    private HookStrategyHandler _hookStrategyHandler;
    private float stopPullDistance = 1f;

    private SpiritKernel spiritKernel;
    private bool CanReflectFromKernelOuter;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();

        CanReflectFromKernelOuter = true;

        // Initialize random direction and speed
        currentDirection = Random.insideUnitCircle.normalized;
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        turning = false;
        directionChangeTimer = Random.Range(minDirChangeInterval, maxDirChangeInterval);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        state = SpiritState.Appearing;
    }

    private void Update()
    {
        
        switch(state)
        {
            case SpiritState.Pulled:
                transform.position = _hookTransform.position;
                break;
            case SpiritState.Moving:
                MovingUpdate();
                break;
            case SpiritState.Appearing:
                MovingUpdate();
                break;
            case SpiritState.Stabilizing:
                StabilizingUpdate();
                break;
        }
        
    }

    protected virtual void MovingUpdate()
    {
        directionChangeTimer -= Time.deltaTime;

        // Change direction at intervals
        if (directionChangeTimer <= 0)
            StartTurning();

        if (turning)
            TurningUpdate();

        // Clamp position within the movement area
        KeepInsideArea();
        rb.velocity = currentDirection * currentSpeed;
    }

    private void StabilizingUpdate()
    {
        stabilizingTimer -= Time.deltaTime;
        if (stabilizingTimer <= 0)
        {
            EndStabilizing();
            return;
        }
        
        float stabilizingMovementSpeed = (spiritKernel.transform.position - transform.position).magnitude / 
                                         stabilizingTimer;
        rb.velocity = (spiritKernel.transform.position - transform.position).normalized *
                                        stabilizingMovementSpeed;
    }

    public void StartAppearing()
    {
        state = SpiritState.Appearing;
        
        currentDirection = Random.insideUnitCircle.normalized;
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        turning = false;
        directionChangeTimer = Random.Range(minDirChangeInterval, maxDirChangeInterval);
        
        StartCoroutine(EndAppearing());
    }

    IEnumerator EndAppearing()
    {
        yield return new WaitForSeconds(appearingDuration);
        if(state == SpiritState.Appearing)
            state = SpiritState.Moving;
    }

    private void StartTurning()
    {
        // Randomly adjust direction and speed
        // Vector2 randomOffset = Random.insideUnitCircle * 0.5f; // Random offset for curve-like motion
        nextDirection = Random.insideUnitCircle;
        prevDirection = currentDirection;
        currentSpeed = Random.Range(minSpeed, maxSpeed);

        turning = true;
        currentTurningDuration = Random.Range(minTurningDuration, maxTurningDuration);
        turningTimer = currentTurningDuration;
        
        directionChangeTimer = Random.Range(minDirChangeInterval, maxDirChangeInterval);
    }

    private void TurningUpdate()
    {
        turningTimer -= Time.deltaTime;
        if (turningTimer <= 0)
        {
            turning = false;
            return;
        }

        float k = turningTimer / currentTurningDuration;
        currentDirection = (k * prevDirection + (1 - k) * nextDirection).normalized;
    }

    private void KeepInsideArea()
    {
        if (movementArea == null)
            return;

        // Check if the object is outside the area
        if (!movementArea.OverlapPoint(transform.position))
        {
            // Calculate a new direction pointing back inside the area
            Vector2 center = movementArea.bounds.center;
            currentDirection = (center - (Vector2)transform.position).normalized;
            turning = false;
            directionChangeTimer = Random.Range(minDirChangeInterval, maxDirChangeInterval);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Relief"))
            Reflect(collision);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        
        if (collider.TryGetComponent<SpiritKernel>(out var spiritKernel) && 
            state != SpiritState.Appearing && spiritKernel.color == color)
        {
            StartStabilizing(spiritKernel);
            return;
        }

        if (collider.name == "SpiritKernelOuter" && CanReflectFromKernelOuter &&
            (collider.transform.parent.GetComponent<SpiritKernel>().color == color))
        {
            Reflect(collider.transform.position);
            CanReflectFromKernelOuter = false;
            StartCoroutine(KernelOuterCooldown());
        }
    }

    IEnumerator KernelOuterCooldown()
    {
        yield return new WaitForSeconds(0.3f);
        CanReflectFromKernelOuter = true;
    }

    private void Reflect(Collision2D collision)
    {
        // Change direction upon collision
        turning = false;
        Vector2 collisionNormal = (collision.GetContact(0).point-collision.GetContact(1).point).normalized;
        if (Mathf.Abs(collisionNormal.x) > Mathf.Abs(collisionNormal.y))
            currentDirection.y *= -1;
        else
            currentDirection.x *= -1;
    }

    private void Reflect(Vector3 collisionCenter)
    {
        turning = false;
        Vector2 collisionNormal = (transform.position - collisionCenter).normalized;
        currentDirection = Vector2.Reflect(currentDirection, collisionNormal).normalized;
    }
    private void StartStabilizing(SpiritKernel kernel)
    {
        if (state == SpiritState.Pulled)
            EndPulledByHook();
        
        state = SpiritState.Stabilizing;
        stabilizingTimer = stabilizingDuration;
        spiritKernel = kernel;
        _collider2D.enabled = false;
    }

    private void EndStabilizing()
    {
        spiritKernel.GainSpirit();
        Destroy(gameObject);
    }



    public bool IsIntangible() => false;
    public void EndPulledByHook()
    {
        state = SpiritState.Moving;
        _hookStrategyHandler.OnHookDisable -= EndPulledByHook;
    }
    
    public float PulledByHook(Transform Hook, Vector3 pullBeginning, int damage, HookStrategyHandler hookStrategyHandler)
    {
        state = SpiritState.Pulled;
        
        _hookTransform = Hook;
        //var hookStrategyHandler = Hook.GetComponent<HookStrategyHandler>();
        _hookStrategyHandler = hookStrategyHandler;
        hookStrategyHandler.OnHookDisable += EndPulledByHook;
        //SetHookBehaviour(hookStrategyHandler);
        return stopPullDistance;
    }
}
