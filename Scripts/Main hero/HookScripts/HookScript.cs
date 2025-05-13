using System;
using System.Collections;
using Main_hero;
using ObjectLogicInterfaces;
using Pathfinding.Util;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

public class HookScript : MonoBehaviour
{
    //[SerializeField] private PlayerInput playerInput;
    //[SerializeField] private InputAction stopHook;
    #region Hook state

    public HookState CurrentHookState { get; private set; }
    private HookState _stateBeforeStop;
    public enum HookState
    {
        Inactive,
        Thrown,
        Returning,
        //Stopped
    }
    
    #endregion

    #region Hook characteristics
    
    [SerializeField] private SpriteRenderer hookSpriteRenderer;
    [SerializeField] private float hookSpeedToTarget = 15;
    [SerializeField] private float hookSpeedBack = 25;
    [SerializeField] private float hookThrowDistance = 7.5f;
    [SerializeField] private int maxDamage = 50;
    [SerializeField] private float stopDuration;
    [SerializeField] private Vector2 targetPosition;
    private float hookBaseSpeed;
    private Vector2 _hookSpeed;
    private float _hookThrowTimer;
    private float _hookThrowDuration;
    //private bool _canStopChain;
    //private bool _stopDone;
    //private Timer _stopTimer;
    private float _enemyStopPullDistance;

    #endregion

    #region Events

    public event Action<float> OnHookHit;
    public event Action OnHookMess, OnHookDisable;

    #endregion

    //[SerializeField] private bool canStopHook;
    [SerializeField] private Transform hookEnd;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject chain;
    [SerializeField] private HookShadowScript hookShadow;
    [SerializeField] private Transform heroObstacleCollider;
    //private IEnumerator _timerTickerCoroutine;
    private IEnumerator _hookUpdateCoroutine;
    
    private HeroSounds heroSounds;
    private bool playedChainEndSound;
    
    private LayerMask _obstacleMask;
    private LayerMask _playerBottomMask;
    private bool _objectCaught;
    

    private void Awake()
    {
        //_timerTickerCoroutine = TimerTickCoroutine();
        _hookUpdateCoroutine = HookPhysicsUpdate();
        CurrentHookState = HookState.Inactive;
        //_stopTimer = new Timer(stopDuration);
        //_stopTimer.OnTimerDone += ContinueHooking;
        gameObject.SetActive(false);
        hookShadow.OnTrigger += ObstacleCollisionEffect;
        _obstacleMask = LayerMask.GetMask("Obstacle");
        _playerBottomMask = LayerMask.GetMask("EnemyObstacleCollider");
        heroSounds = transform.parent.GetComponentInChildren<HeroSounds>();
        _objectCaught = false;

        //stopHook = playerInput.currentActionMap.FindAction("Hook");
    }

    public bool ActivateHook(Vector2 mousePosition)
    {
        heroSounds.PlayThrowChainSound();
        
        var position = player.position;
        Vector2 directionToTarget = (mousePosition - (Vector2)position).normalized;
        
        targetPosition = (Vector2)position + hookThrowDistance*directionToTarget;
        CorrectChainMovement(0);
        CalculateShadowOffset(directionToTarget);
        if (CheckStartCollision(heroObstacleCollider))
        {
            return false;
        }
            
        CurrentHookState = HookState.Thrown;
        hookBaseSpeed = hookSpeedToTarget;
        _hookThrowDuration = hookThrowDistance / hookBaseSpeed;
        _hookThrowTimer = 0;
        //_canStopChain = true;
        //_stopDone = false;
        _enemyStopPullDistance = 0;
        gameObject.SetActive(true);
        chain.SetActive(true);
        
        return true;
    }
    
    // private void StopHook(InputAction.CallbackContext context)
    // {
    //     switch (CurrentHookState)
    //     {
    //         case HookState.Thrown:
    //             if (_canStopChain && !_stopDone)
    //             {
    //                 _stateBeforeStop = CurrentHookState;
    //                 CurrentHookState = HookState.Stopped;
    //                 //_stopTimer = stopDuration;
    //                 _stopTimer.StartTimer();
    //                 _stopDone = true;
    //             }
    //             break;
    //         case HookState.Returning:
    //             if (_canStopChain && !_stopDone && (gameObject.transform.position - player.position).magnitude > _enemyStopPullDistance)
    //             {
    //                 _stateBeforeStop = CurrentHookState;
    //                 CurrentHookState = HookState.Stopped;
    //                 //_stopTimer = stopDuration;
    //                 _stopTimer.StartTimer();
    //                 _stopDone = true;
    //             }
    //             break;
    //         case HookState.Stopped:
    //             CurrentHookState = _stateBeforeStop;
    //             _stopTimer.StopTimer();
    //             break;
    //     }
    // }
    //For physics calculation
    private IEnumerator HookPhysicsUpdate()
    {
        //var delay = new WaitForEndOfFrame();
        while (true)
        {
            switch (CurrentHookState)
            {
                case HookState.Thrown:
                    if(_hookThrowTimer < _hookThrowDuration) {
                        CorrectChainMovement(_hookThrowTimer);
                        _hookThrowTimer += Time.deltaTime;
                        // Меняем цвет хука
                        float curDistance = (transform.position - player.position).magnitude;
                        hookSpriteRenderer.color = new Color(curDistance/hookThrowDistance, 0, 0, 1);
                    }
                    else {
                        heroSounds.PlayReturnChainSound();
                        playedChainEndSound = false;
                        CurrentHookState = HookState.Returning;
                        hookBaseSpeed = hookSpeedBack;
                        _hookThrowDuration = hookThrowDistance / hookBaseSpeed;
                        _hookThrowTimer = 0;
                    }
                    break;
                case HookState.Returning:
                    var distanceDifference = ((Vector2)player.position - (Vector2)transform.position).magnitude;
                    CorrectChainMovement(_hookThrowDuration - _hookThrowTimer);
                    _hookThrowTimer += Time.deltaTime;
                    float currentDistance = (transform.position - player.position).magnitude;
                    hookSpriteRenderer.color = new Color(currentDistance/hookThrowDistance, 0, 0, 1);
                    
                    if (!playedChainEndSound && distanceDifference < 5)
                    {
                        heroSounds.PlayChainEndSound();
                        playedChainEndSound = true;
                    }
                    break;
                // case HookState.Stopped:
                //     if(_stateBeforeStop == HookState.Thrown)
                //         CorrectChainMovement(_hookThrowTimer);
                //     else
                //         CorrectChainMovement(_hookThrowDuration - _hookThrowTimer);
                //     break;
            }

            yield return null;
        }
    }
    
    private void CorrectChainMovement(float timeInMovement)
    {
        // Рассчитываем угол между игроком и таргетом
        float angle = AngleBetweenTwoPoints(targetPosition, player.position);
                    
        // Корректируем поворот Хука
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,angle - 90));

        // Корректируем скорость таким образом, чтобы цепь летела по направлению к таргету
        Vector2 directionToTarget = (targetPosition - (Vector2)player.position).normalized;
        _hookSpeed = hookBaseSpeed*directionToTarget;
                    
        // Корректируем позицию цепи и маски
        transform.position = (Vector2)player.position + timeInMovement * _hookSpeed;
    }
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
    private void ContinueHooking() => CurrentHookState = _stateBeforeStop;
    private void OnEnable()
    {
        //StartCoroutine(_timerTickerCoroutine);
        StartCoroutine(_hookUpdateCoroutine);
        //SubscribeOnActionEvents();
    }
    private void OnDisable()
    {
        if (!_objectCaught)
            OnHookMess?.Invoke();
        else
        {
            _objectCaught = false;
        }
        //StopCoroutine(_timerTickerCoroutine);
        StopCoroutine(_hookUpdateCoroutine);
        CurrentHookState = HookState.Inactive;
        //_stopDone = false;
        _enemyStopPullDistance = 0;
        transform.position = player.position;
        hookSpriteRenderer.color = new Color(0, 0, 0, 1);
        chain.SetActive(false);
        //UnsubscribeFromActionEvents();
        OnHookDisable?.Invoke();
    }
    private IEnumerator TimerTickCoroutine()
    {
        while (true)
        {
            //_stopTimer.Tick();
            yield return new WaitForEndOfFrame();
        }
    }
    
    public void SetNewHookDistance(float dist)
    {
        hookThrowDistance += dist;
        hookThrowDistance = hookThrowDistance < 0 ? 0 : hookThrowDistance;
    }

    private void CalculateShadowOffset(Vector2 hookDirection)
    {
        Vector2 playerPivotPosition = heroObstacleCollider.position;
        float offset = ((Vector2)transform.position - playerPivotPosition).magnitude; 
        hookShadow.RecalculatePosition(hookEnd.position, offset, hookDirection);
    }
    private void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (collisionObj.gameObject.CompareTag("Player"))
        {
            if (CurrentHookState != HookState.Returning) return;
            heroSounds.StopChainSound();
            gameObject.SetActive(false);
            return;
        }
        if (HitThroughObstacle(collisionObj))
            return;
        if ((CurrentHookState == HookState.Thrown /*||
             CurrentHookState == HookState.Stopped*/ && _stateBeforeStop == HookState.Thrown))
        {
            IHookable hookableObject = collisionObj.gameObject.GetComponent<IHookable>();

            if (hookableObject.IsIntangible())
                return;
            
            // Наносим урон в зависимости от дистанции до игрока
            float catchDistance = (transform.position - player.position).magnitude;
            float distancePercentage = catchDistance / hookThrowDistance;
            int dealtDamage = (int)Math.Floor(maxDamage * distancePercentage);

            // Цепляем врага на хук и получаем дистанцию до героя, на которой враг слезает с него (нужно для того,
            // чтобы после этого нельзя было приостановить хук)
            //_enemyStopPullDistance = hookableObject.PulledByHook(gameObject.transform, player.position, dealtDamage);
            //За попадание даем герою адреналин
            
            OnHookHit?.Invoke(distancePercentage);
            _objectCaught = true;
            
        }

        ReturnAfterCollision();

    }

    private void ObstacleCollisionEffect(Collider2D collision)
    {
        ObstacleType obstacleType = ObstacleType.Default;
        
        if (collision.TryGetComponent(out ObstacleTypeHolder obstacleTypeHolder))
            obstacleType = obstacleTypeHolder.GetObstacleType();

        heroSounds.PlayHitObstacleSound(obstacleType);
        ReturnAfterCollision();
    }

    private void ReturnAfterCollision()
    {
        // Хук летит обратно
        heroSounds.PlayReturnChainSound();
        playedChainEndSound = false;
        CurrentHookState = HookState.Returning;
        float currentDistance = (transform.position - player.position).magnitude;
        hookBaseSpeed = hookSpeedBack;
        _hookThrowDuration = currentDistance / hookBaseSpeed;
        _hookThrowTimer = 0;
    }

    private bool CheckStartCollision(Transform sourceBottom)
    {
        Vector2 hookDirection = hookShadow.GetPosition - sourceBottom.position;
        var rayToObstacle = Physics2D.Raycast(sourceBottom.position, hookDirection.normalized,
            hookDirection.magnitude, _obstacleMask);
        if (!rayToObstacle) 
            return false;
        var rayToEnemyBottom = Physics2D.Raycast(sourceBottom.position, hookDirection.normalized,
            hookDirection.magnitude, _playerBottomMask);
        
        return !rayToEnemyBottom || rayToObstacle.distance < rayToEnemyBottom.distance;

    }
    private bool HitThroughObstacle(Collider2D objCollider)
    {
        if (!objCollider.transform.Find("ObstacleCollider"))
        {
            return true;
        }
        var shadowToPlayerBottom =  objCollider.transform.Find("ObstacleCollider").position - hookShadow.GetPosition;
        if (_hookSpeed.y <= 0)
        {
            return false;
        }
        return Physics2D.Raycast(hookShadow.GetPosition, shadowToPlayerBottom.normalized,
            shadowToPlayerBottom.magnitude, _obstacleMask);
        //return false;
    }

    // private void SubscribeOnActionEvents()
    // {
    //     if (!canStopHook) return;
    //     stopHook.performed += StopHook;
    // }

    // private void UnsubscribeFromActionEvents()
    // {
    //     stopHook.performed -= StopHook;
    // }

    
}
