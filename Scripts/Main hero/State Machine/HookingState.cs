using System;
using Main_hero.HookScripts.HookStrategies;
using ObjectLogicInterfaces;
using ObjectLogicRealization.Adrenaline;
using ObjectLogicRealization.Move;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

[Serializable]
//Состояние, которое характеризует процесс использования хука
public class HookingState : BaseState
{
    [SerializeField] private PlayerMove _playerMoveScript;
    public PlayerMove PlayerMoveScript => _playerMoveScript;
    private Rigidbody2D _rigidbody;
    //[SerializeField] private HookScript _hookScript;
    [SerializeField] private HookStrategyHandler _hookScript;
    [field: SerializeField] public float HookCooldown { get; private set; }
    public Timer HookCooldownTimer { get; private set; }
    private bool _readyToHook;
    private bool _successfulHookHit;
    [SerializeField] private HeroAdrenaline _heroAdrenaline;
    [field: SerializeField] public bool AdrenalineInfluencesCooldown { get; set; } = false;

    #region Input actions

    [SerializeField] private InputAction openInventory;
    [SerializeField] private InputAction move;

    #endregion
    
    public event Action OnCooldownClickEvent;
    
    public override void InitializeState(GameObject hero, HeroStateHandler stateHandler, Transform heroTransform,
        Animator animator, PlayerInput playerInput)
    {
        base.InitializeState(hero, stateHandler, heroTransform, animator, playerInput);
        _playerMoveScript = Hero.GetComponent<PlayerMove>();
        _rigidbody = Hero.GetComponent<Rigidbody2D>();
        _readyToHook = true;
        HookCooldownTimer = new Timer(HookCooldown);
        HookCooldownTimer.OnTimerDone += SetReadyToHook;
        openInventory = playerInput.currentActionMap.FindAction("OpenInventory");
        move = playerInput.currentActionMap.FindAction("Movement");
    }
    public override void EnterState()
    {
        if (!ReadyToHook())
        {
            if(HookCooldownTimer.IsActive) OnCooldownClickEvent?.Invoke();
            StateHandler.SwitchState(StateHandler.PreviousState);
            return;
        }
        base.EnterState();
        _rigidbody.velocity = new Vector2(0f, 0f);
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _hookScript.OnHookDisable += HookDisableEvent;
        _hookScript.OnHookHit += OnHookHit;
        if (!_hookScript.ActivateHook(mousePosition))
        {
            StateHandler.SwitchState(StateHandler.PreviousState);
            return;
        }

        _successfulHookHit = false;
        _readyToHook = false;
        //HookCooldownTimer.StartTimer();
        SubscribeOnInputActions();
    }

    public override void UpdateState(HeroStateHandler stateHandler)
    {
        HandleMovement();

    }

    public override void ExitState()
    {
        UnsubscribeFromInputActions();
        _hookScript.OnHookHit -= OnHookHit;
    }
    private void HandleOpenInGameMenu(InputAction.CallbackContext context)
    {
        StateHandler.SwitchState(StateHandler.InventoryState);

    }

    private void HandleMovement()
    {
        var inputVector = move.ReadValue<Vector2>();
            
        Vector2 moveDirection = inputVector.normalized;
        
        _rigidbody.velocity = moveDirection * _playerMoveScript.GetHookingMoveSpeed();
        
    }

    private void SubscribeOnInputActions()
    { }

    private void UnsubscribeFromInputActions()
    { }

    private void HookDisableEvent()
    {
        if (!_successfulHookHit)
        {
            HookCooldownTimer.StartTimer();
            StateHandler.SwitchState(StateHandler.NormalState);
            _hookScript.OnHookDisable -= HookDisableEvent;
        }
        else
        {
            _readyToHook = true;
            StateHandler.SwitchState(StateHandler.NormalState);
            _hookScript.OnHookDisable -= HookDisableEvent;
        }
    }

    private void OnHookHit(HookBehaviour behaviour)
    {
        _successfulHookHit = true;
    }
    
    public HookScript.HookState GetStateOfTheHook() => _hookScript.CurrentHookState;
    public void SetReadyToHook() => _readyToHook = true;
    public bool ReadyToHook() => _readyToHook;

    public void RecalculateTimer()
    {
        HookCooldownTimer.Tick();
    }
}