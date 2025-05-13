using UnityEngine;
using System;
using ItemDrop;
using ObjectLogicInterfaces;
using UnityEngine.InputSystem;
using Utilities;

[Serializable]
public class InventoryState : BaseState
{
    #region Animation triggers

    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
    private static readonly int LastVertical = Animator.StringToHash("LastVertical");
    private static readonly int Speed = Animator.StringToHash("Speed");

    #endregion
    

    #region Input Actions
    
    [SerializeField] private InputAction openInventory;
    [SerializeField] private InputAction closeInventory;
    //[SerializeField] private InputAction closeLootBag;
    [SerializeField] private InputAction move;
    [SerializeField] private InputAction exitState;

    #endregion
    
    private IMovable _moveScript;
    private Rigidbody2D _heroRigidbody;
    private HeroInventory _heroInventory;
    private LootBagScript _bag;
    
    private int _numberOfInventoriesOpened;
    private ISState _currentState;

    private Timer _delayBeforeSubscribingTimer;
    [SerializeField] private float delayBeforeListeningFOrActionsTime;
    public event Action OnEnteredState, OnExitState;

    private enum ISState
    {
        Inactive,
        HeroInventoryOpened,
        LootInventoriesOpened,
        BothInventoriesOpened
    }
    
    public override void InitializeState(GameObject hero, HeroStateHandler stateHandler, Transform heroTransform,
        Animator animator, PlayerInput playerInput)
    {
        
        base.InitializeState(hero, stateHandler, heroTransform, animator, playerInput);
        _currentState = ISState.Inactive;
        _moveScript = hero.GetComponent<IMovable>();
        _heroInventory = hero.GetComponentInChildren<HeroInventory>();
        
        if (!hero.TryGetComponent(out _heroRigidbody))
        {
            Debug.LogError("Hero does not have Rigidbody2D component");
        }
        _numberOfInventoriesOpened = 0;
        
        openInventory = playerInput.currentActionMap.FindAction("OpenInventory");
        closeInventory = playerInput.currentActionMap.FindAction("CloseInventory");
        move = playerInput.currentActionMap.FindAction("Movement");
        exitState = playerInput.currentActionMap.FindAction("ExitInventory");

        _delayBeforeSubscribingTimer = new Timer(delayBeforeListeningFOrActionsTime);
        _delayBeforeSubscribingTimer.OnTimerDone += SubscribeOnActionEvents;
    }

    public override void EnterState()
    {
        base.EnterState();
        OnEnteredState?.Invoke();
        OpenHeroInventory();
        _delayBeforeSubscribingTimer.StartTimer();
    }
    
    public override void UpdateState(HeroStateHandler stateHandler)
    {
        _delayBeforeSubscribingTimer.Tick();
        HandleMovement();
    }

    private void HandleCloseHeroInventory(InputAction.CallbackContext context)
    {
        if (_currentState is not (ISState.BothInventoriesOpened or ISState.HeroInventoryOpened)) return;
        InventoryUIFacade.GetInstance.CloseInventoryMenu();
        _numberOfInventoriesOpened--;
        
        if (_currentState is ISState.BothInventoriesOpened)
        {
            _currentState = ISState.LootInventoriesOpened;
        }
        CheckNumberOfOpenedInventories();
    }

    private void HandleOpenHeroInventory(InputAction.CallbackContext context)
    {
        if (_currentState is not ISState.LootInventoriesOpened) return;
        OpenHeroInventory();
    }

    private void OpenHeroInventory()
    {
        UnsubscribeFromActionEvents();
        InventoryUIFacade.GetInstance.OpenInventoryMenu();
        _heroInventory.GetCurrentInventoryState();
        _numberOfInventoriesOpened++;
        _currentState = _currentState switch
        {
            ISState.LootInventoriesOpened => ISState.BothInventoriesOpened,
            _ => ISState.HeroInventoryOpened
        };
        _delayBeforeSubscribingTimer.StartTimer();
    }
    
    private void HandleExitBagInventoryState(InputAction.CallbackContext context)
    {
        if (_currentState is ISState.BothInventoriesOpened or ISState.LootInventoriesOpened)
        {
            CloseBag();
        }
    }

    private void HandleMovement()
    {
        var inputVector = move.ReadValue<Vector2>();
            
        Vector2 moveDirection = inputVector.normalized;
            
        Animator.SetFloat(Horizontal, inputVector.x);
        Animator.SetFloat(Vertical, inputVector.y);
            
        if (inputVector.x != 0 || inputVector.y != 0)
        {
            Animator.SetFloat(LastHorizontal, inputVector.x);
            Animator.SetFloat(LastVertical, inputVector.y);
        }
            
        //_heroRigidbody.velocity = moveDirection * _moveScript.GetCurrentMoveSpeed();
        
        if(StateHandler.HookingState.GetStateOfTheHook() == HookScript.HookState.Inactive)
        { 
            _heroRigidbody.velocity =
                new Vector2(
                    moveDirection.x * _moveScript.GetCurrentMoveSpeed(),
                    moveDirection.y * _moveScript.GetCurrentMoveSpeed());
        }
        else
        {
            _heroRigidbody.velocity =
                new Vector2(
                    moveDirection.x * StateHandler.HookingState.PlayerMoveScript.GetHookingMoveSpeed(),
                    moveDirection.y * StateHandler.HookingState.PlayerMoveScript.GetHookingMoveSpeed());
        }

        float moveMagnitude = (float)Math.Sqrt(Math.Pow( inputVector.x, 2) + Math.Pow(inputVector.y, 2));
        Animator.SetFloat(Speed, moveMagnitude);
    }

    public override void ExitState()
    {
        base.ExitState();
        InventoryUIFacade.GetInstance.CloseInventoryMenu();
        if(_bag != null)
            CloseBagWithoutSwitchingState();
        _currentState = ISState.Inactive;
        UnsubscribeFromActionEvents();
        OnExitState?.Invoke();
    }
    
    public void CloseBag()
    {
        _numberOfInventoriesOpened--;
        
        _bag.UnsubscribeFromUIInventory();
        _bag = null;
        BagInventoryUI.Instance.CloseBagInventoryUI();
        if (_currentState is ISState.BothInventoriesOpened)
            _currentState = ISState.HeroInventoryOpened;
        CheckNumberOfOpenedInventories();
    }

    public void OpenBag(LootBagScript dropBag)
    {
        OnEnteredState?.Invoke();
        if(_bag == null)
        {
            _bag = dropBag;
            base.EnterState();
            _delayBeforeSubscribingTimer.StartTimer();
            _numberOfInventoriesOpened++;
            _currentState = _currentState switch
            {
                ISState.HeroInventoryOpened => ISState.BothInventoriesOpened,
                _ => ISState.LootInventoriesOpened
            };
        }
        else
        {
            _bag.CloseLootBag();
            _bag = dropBag;
            base.EnterState();
            _delayBeforeSubscribingTimer.StartTimer();
        }
        
        
    }

    private void CloseBagWithoutSwitchingState()
    {
        _bag.UnsubscribeFromUIInventory();
        _bag = null;
        BagInventoryUI.Instance.CloseBagInventoryUI();
    }

    private void CheckNumberOfOpenedInventories()
    {
        if (_numberOfInventoriesOpened != 0) return;
        StateHandler.SwitchState(StateHandler.NormalState);
        OnExitState?.Invoke();
    }

    private void ExitInventoryState(InputAction.CallbackContext context)
    {
        _numberOfInventoriesOpened = 0;
        StateHandler.SwitchState(StateHandler.NormalState);
    }

    public override void SubscribeOnActionEvents()
    {
        base.SubscribeOnActionEvents();
        openInventory.performed += HandleOpenHeroInventory;
        closeInventory.performed += HandleCloseHeroInventory;
        exitState.performed += ExitInventoryState;
        //closeLootBag.performed += HandleExitBagInventoryState;
    }

    public override void UnsubscribeFromActionEvents()
    {
        base.UnsubscribeFromActionEvents();
        openInventory.performed -= HandleOpenHeroInventory;
        closeInventory.performed -= HandleCloseHeroInventory;
        exitState.performed -= ExitInventoryState;
    }
}
