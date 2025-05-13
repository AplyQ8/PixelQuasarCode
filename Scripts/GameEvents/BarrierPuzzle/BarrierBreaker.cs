using System;
using System.Collections;
using System.Collections.Generic;
using Main_hero.HookScripts.HookStrategies;
using ObjectLogicInterfaces;
using UnityEngine;

public class BarrierBreaker : MonoBehaviour, IHookable
{
    [SerializeField][Range(0, 3)] private int initialCharge;
    [SerializeField][Range(0, 3)] private int charge;

    public float speed;
    private Vector3 CurrentCellCenter;
    
    private Collider2D _collider2D;
    private SpriteRenderer _spriteRenderer;
    private Vector3 initialPos;

    private bool isPulled;
    private Transform _hookTransform;
    private HookStrategyHandler _hookStrategyHandler;
    private Rigidbody2D _rigidbody2D;
    private float stopPullDistance = 3f;

    private List<BarrierWall> triggeredWalls;

    private bool gameActive = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _collider2D = transform.GetComponent<Collider2D>();
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();
        _rigidbody2D = transform.GetComponent<Rigidbody2D>();
        initialPos = transform.position;

        gameActive = true;
        
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPulled)
        {
            _rigidbody2D.velocity = Vector2.zero;
            transform.position = _hookTransform.position;
        }
        else
        {
            _rigidbody2D.velocity = (CurrentCellCenter - transform.position).normalized * speed;
        }
    }

    private void SetCharge(int newCharge)
    {
        charge = newCharge;
        
        if (charge == 0)
            BreakDown();
        
        SetColor();
    }

    public void DecreaseCharge()
    {
        SetCharge(charge - 1);
    }

    public void BreakDown()
    {
        charge = 0;
        _collider2D.enabled = false;
        _spriteRenderer.enabled = false;
    }

    public void Reset()
    {
        transform.position = initialPos;
        RecalculateNearestCellCenter();
        SetCharge(initialCharge);
        isPulled = false;
        triggeredWalls = new List<BarrierWall>();
        _collider2D.enabled = true;
        _spriteRenderer.enabled = true;
    }

    private void SetColor()
    {
        switch (charge)
        {
            case 1:
                _spriteRenderer.color = Color.cyan;
                break;
            case 2:
                _spriteRenderer.color = Color.yellow;
                break;
            case 3:
                _spriteRenderer.color = Color.red;
                break;
            case 4:
                _spriteRenderer.color = Color.magenta;
                break;
        }
    }
    
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.TryGetComponent<BarrierWall>(out var barrierWall) &&
            !triggeredWalls.Contains(barrierWall))
        {
            triggeredWalls.Add(barrierWall);
            OnHitBarrierWall(barrierWall);
        }
    }

    private void OnHitBarrierWall(BarrierWall barrierWall)
    {
        if (barrierWall.charge > 0)
        {
            DecreaseCharge();
            barrierWall.DecreaseCharge();
        }
        else if (barrierWall.charge == -2)
        {
            SetCharge(0);
        }
    }
    
    public void RecalculateNearestCellCenter()
    {
        // Ensure the parent has a Grid component
        Grid grid = transform.parent.GetComponent<Grid>();

        // Convert the object's world position to cell position
        Vector3Int cellPosition = grid.WorldToCell(transform.position);

        // Get the world position of the cell's center
        Vector3 cellCenter = grid.GetCellCenterWorld(cellPosition);

        CurrentCellCenter = cellCenter;
    }

    IEnumerator AfterEndPulled()
    {
        yield return new WaitForSeconds(0.3f);
        triggeredWalls = new List<BarrierWall>();
        if(charge != 0)
            _collider2D.enabled = true;
    }
    
    
    
    public bool IsIntangible() => false;
    public void EndPulledByHook()
    {
        isPulled = false;
        RecalculateNearestCellCenter();
        _collider2D.enabled = false;
        if(gameObject.activeSelf)
            StartCoroutine(AfterEndPulled());
        _hookStrategyHandler.OnHookDisable -= EndPulledByHook;
    }
    
    public float PulledByHook(Transform Hook, Vector3 pullBeginning, int damage, HookStrategyHandler hookStrategyHandler)
    {
        isPulled = true;
        _hookTransform = Hook;
        _hookStrategyHandler = hookStrategyHandler;
        hookStrategyHandler.OnHookDisable += EndPulledByHook;
        return stopPullDistance;
    }
    
    // OnValidate is called when a value in the Inspector is changed
    private void OnValidate()
    {
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Ensure initialCharge is clamped to a valid range
        initialCharge = Mathf.Clamp(initialCharge, 0, 4);

        // If initialCharge changes, set charge to the same value
        if (charge != initialCharge)
        {
            if (charge <= 0 && initialCharge > 0)
            {
                _collider2D.enabled = true;
                _spriteRenderer.enabled = true;
            }
            charge = initialCharge;
        }

        // Call SetCharge to ensure color updates in real-time
        SetCharge(charge);
    }

    public void CorrectPosition()
    {
        if (gameActive)
            return;
        
        RecalculateNearestCellCenter();
        transform.position = CurrentCellCenter;
    }
    
}
