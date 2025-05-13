using System;
using System.Collections;
using System.Collections.Generic;
using In_Game_Menu_Scripts;
using ItemDrop;
using Main_hero;
using Main_hero.State_Machine;
using ObjectLogicInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Менеджр состояний объекта
public class HeroStateHandler : MonoBehaviour
{
    [SerializeField] public BaseState PreviousState { get; private set; }
    private BaseState _currentState;
    [Header("Hero animator")]
    [SerializeField] private Animator animator;
    [Header("GameMenu")]
    //[SerializeField] private GameObject inGameMenu;

    [SerializeField] private HeroStatUIReferences uiReferences;
    [SerializeField] private AbilityHandler abilityHandler;
    
    [SerializeField] private NormalState normalState;
    public NormalState NormalState => normalState;

    [SerializeField] private AttackingState attackingState;
    public AttackingState AttackingState => attackingState;

    [SerializeField] private DashState dashState;
    public DashState DashState => dashState;
    [SerializeField] private HookingState hookingState;
    public HookingState HookingState => hookingState;
    
    [SerializeField] private AbilityState abilityState;
    public AbilityState AbilityState => abilityState;
    
    [SerializeField] private InventoryState inventoryState;
    public InventoryState InventoryState => inventoryState;

    [SerializeField] private DialogueState dialogueState;
    public DialogueState DialogueState => dialogueState;

    [SerializeField] private SuperAttackState superAttackState;
    public SuperAttackState SuperAttackState => superAttackState;

    [SerializeField] private PlayerInput playerInput;

    void Awake()
    {
        try
        {
            gameObject.GetComponent<IStunable>().OnGetStunnedEvent += GetStun;
        }
        catch (Exception)
        {
            //empty
        }

        SceneManager.sceneLoaded += OnSceneLoad;
        SceneManager.sceneUnloaded += OnSceneUnload;
        playerInput = FindObjectOfType<PlayerInput>();
    }

    private void OnSceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        PauseController.Instance.OnPause += PauseEvent;
    }

    private void PauseEvent(bool isPaused)
    {
        _currentState.ProceedPause(isPaused);
        if(isPaused)
            _currentState.UnsubscribeFromActionEvents();
        else
        {
            _currentState.SubscribeOnActionEvents();
        }
    }
    

    private void OnSceneUnload(Scene arg0)
    {
        //PauseController.Instance.OnPause -= PauseEvent;
    }

    private void Start()
    {
        normalState.InitializeState(gameObject, this, gameObject.transform, animator, playerInput);
        attackingState.InitializeState(gameObject, this, gameObject.transform, animator, playerInput);
        dashState.InitializeState(gameObject, this, gameObject.transform, animator, playerInput);
        hookingState.InitializeState(gameObject, this, gameObject.transform, animator, playerInput);
        abilityState.InitializeState(gameObject, this, gameObject.transform, animator, playerInput);
        inventoryState.InitializeState(gameObject, this, gameObject.transform, animator, playerInput);
        dialogueState.InitializeState(gameObject, this, gameObject.transform, animator, playerInput);
        superAttackState.InitializeState(gameObject, this, gameObject.transform, animator, playerInput);

        _currentState = NormalState;
        PreviousState = NormalState;
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState.UpdateState(this);
        //hookingState.hookCooldownTimer.Tick();
        hookingState.RecalculateTimer();
        AttackingState.TickTimers();
        DashState.TickTimers();
        SuperAttackState.TickTimers();
    }

    public void SwitchState(BaseState state)
    {
        PreviousState = _currentState;
        _currentState.ExitState();
        _currentState = state;
        state.EnterState();
    }

    public bool CanUseAbility()
    {
        if (_currentState == NormalState || _currentState == abilityState)
        {
            return true;
        }

        return false;
    }

    public bool CanUseQuickSlot()
    {
        return _currentState is not StanState;
    }

    private void GetStun(float duration)
    {
        //_currentState.ExitState();
        SwitchState(new StanState(duration, gameObject, animator, this));
    }

    public BaseState GetCurrentState => _currentState;

    public bool EnterBagInventoryState(LootBagScript bag)
    {
        if (_currentState is global::AbilityState or global::HookingState)
            return false;
        PreviousState = _currentState;
        if (_currentState != inventoryState)
        {
            _currentState.ExitState();
            _currentState = inventoryState;
        }
        inventoryState.OpenBag(bag);

        return true;
        
    }

    public void ExitBagInventoryState()
    {
        if (_currentState != inventoryState)
            return;
        inventoryState.CloseBag();
    }

    public void TryExitAbilityState()
    {
        if (_currentState != abilityState)
            return;
        _currentState.ExitState();
        SwitchState(PreviousState);
    }

    public bool TryEnterDialogueState()
    {
        if (_currentState is global::HookingState)
            return false;
        SwitchState(dialogueState);
        return true;
    }

    public bool TryEnterAbilityState()
    {
        if (_currentState == abilityState)
        {
            return true;
        }
        if (CanUseAbility())
        {
            _currentState.ExitState();
            SwitchState(abilityState);
            return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        normalState.UnsubscribeFromActionEvents();
        attackingState.UnsubscribeFromActionEvents();
        dashState.UnsubscribeFromActionEvents();
        hookingState.UnsubscribeFromActionEvents();
        abilityState.UnsubscribeFromActionEvents();
        inventoryState.UnsubscribeFromActionEvents();
        dialogueState.UnsubscribeFromActionEvents();
        superAttackState.UnsubscribeFromActionEvents();
    }
}

