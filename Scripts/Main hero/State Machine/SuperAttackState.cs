using System;
using JetBrains.Annotations;
using Main_hero.SuperAttackScripts;
using ObjectLogicRealization.Adrenaline;
using ObjectLogicRealization.Move;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Main_hero.State_Machine
{
    [Serializable]
    public class SuperAttackState: BaseState
    {
        private static readonly int SuperAttackTrigger = Animator.StringToHash("SuperAttack");
        [field: SerializeField] public SuperAttack CurrentSuperAttack { get; private set; }
        [SerializeField] private PlayerMove _playerMoveScript;

        private Rigidbody2D _rigidbody2D;
        private float _holdTime;
        private bool _isHolding;
        private bool _isDecay;
        private InputAction _attack;
        private SuperAttackContext _context;
        public Timer CooldownTimer { get; private set; }
        private SuperAttacksStates _currentState;
        private HeroAdrenaline _heroAdrenaline;

        private float _currentFillMultiplier;
        [SerializeField] private float attackStateTimeThreshold;
        private float _attackThresholdHoldTime;
        [SerializeField] private float normalFillMultiplier = 1f;
        [SerializeField] private float instabilityFillMultiplier;
        
        [SerializeField] private InputAction move;

        [CanBeNull] public event Action<float> OnHoldTimeChange;
        [CanBeNull] public event Action OnSuperAttackChange;
        [CanBeNull] public event Action OnCooldownClickEvent, OnAttackExecuted;


        private bool _canUseAttack;
        private enum SuperAttacksStates
        {
            Ready,
            Holding,
            Cooldown
        }

        public override void InitializeState(GameObject hero, HeroStateHandler stateHandler, Transform heroTransform,
            Animator animator, PlayerInput playerInput)
        {
            base.InitializeState(hero, stateHandler, heroTransform, animator, playerInput);
            hero.GetComponentInChildren<SuperAttackFillIndicator>().Initialize(this);
            _rigidbody2D = hero.GetComponent<Rigidbody2D>();
            _heroAdrenaline = hero.GetComponent<HeroAdrenaline>();
            _playerMoveScript = Hero.GetComponent<PlayerMove>();
            _heroAdrenaline.OnInstabilityEnter += InstabilityEnterEvent;
            _heroAdrenaline.OnInstabilityExit += InstabilityExitEvent;
            InstabilityExitEvent();
            _attack = playerInput.currentActionMap.FindAction("Attack");
            _context = new SuperAttackContext(hero, animator, SuperAttackTrigger);
            _currentState = SuperAttacksStates.Ready;
            _isDecay = false;
            _holdTime = 0f;
            _canUseAttack = false;
            move = playerInput.currentActionMap.FindAction("Movement");
        }
        public override void EnterState()
        {
            base.EnterState();
            if (_currentState is SuperAttacksStates.Cooldown)
            {
                OnCooldownClickEvent?.Invoke();
                StateHandler.SwitchState(StateHandler.NormalState);
            }
            if (CurrentSuperAttack == null)
            {
                Debug.LogWarning("Super Attack is not set!");
                StateHandler.SwitchState(StateHandler.NormalState);
                return;
            }

            _rigidbody2D.velocity = Vector2.zero;
            _isDecay = false;
            _isHolding = true;
            _attackThresholdHoldTime = 0;
            _currentState = SuperAttacksStates.Holding;
            
            SubscribeOnActionEvents();
        }
        
        public override void UpdateState(HeroStateHandler stateHandler)
        {
            if (isPaused) return;
            
            if (_attackThresholdHoldTime < attackStateTimeThreshold)
            {
                _attackThresholdHoldTime += _currentFillMultiplier * Time.deltaTime;
                return;
            }

            if (!_isHolding || _holdTime >= CurrentSuperAttack.MaxHoldTime)
            {
                stateHandler.SwitchState(stateHandler.NormalState);
                return;
            }
            
            _holdTime += _currentFillMultiplier * Time.deltaTime;
            OnHoldTimeChange?.Invoke(_holdTime);
            CurrentSuperAttack.OnHold(_context, _holdTime);
            HandleMovement();
        }
        private void HandleMovement()
        {
            var inputVector = move.ReadValue<Vector2>();
            
            Vector2 moveDirection = inputVector.normalized;
        
            _rigidbody2D.velocity = moveDirection * _playerMoveScript.GetSpeedWhileSuperAttack();
        
        }
        
        public override void ExitState()
        {
            base.ExitState();
            if (ExecuteSuperAttack())
            {
                OnAttackExecuted?.Invoke();
                _currentState = SuperAttacksStates.Cooldown;
                CooldownTimer = new Timer(CurrentSuperAttack.Cooldown);
                CooldownTimer.StartTimer();
                CooldownTimer.OnTimerDone += SwitchToNormalState;
                _holdTime = 0f;
                OnHoldTimeChange?.Invoke(_holdTime);
            }
            else
            {
                _currentState = SuperAttacksStates.Ready;
                _isDecay = true;
            }
            _isHolding = false;
            UnsubscribeFromActionEvents();
        }

        public void TickTimers()
        {
            CooldownTimer?.Tick();
            if (!_isDecay) return;
            HandleDecaying();
        }
        
        private bool ExecuteSuperAttack()
        {
            return CurrentSuperAttack.Executed(_context, _holdTime);
        }
        
        public void CancelAttack()
        {
            _isHolding = false;
            StateHandler.SwitchState(StateHandler.NormalState);
        }

        private void HandleDecaying()
        {
            _holdTime -= Time.deltaTime * CurrentSuperAttack.DecayRate;
            _holdTime = Mathf.Max(0f, _holdTime);
            
            OnHoldTimeChange?.Invoke(_holdTime);
        }
        private void HandleAttacking(InputAction.CallbackContext obj)
        {
            if (_attackThresholdHoldTime < attackStateTimeThreshold)
            {
                _attackThresholdHoldTime = 0;
                StateHandler.SwitchState(StateHandler.AttackingState);
            }
            _isHolding = false;
        }

        private void SwitchToNormalState()
        {
            _currentState = SuperAttacksStates.Ready;
            CooldownTimer.OnTimerDone -= SwitchToNormalState;
            CooldownTimer = null;
        }

        public void ChangeSuperAttack(SuperAttack newSuperAttack)
        {
            CurrentSuperAttack = newSuperAttack;
            OnSuperAttackChange?.Invoke();
        }

        private void InstabilityEnterEvent()
        {
            _currentFillMultiplier = instabilityFillMultiplier;
            _canUseAttack = true;
        }

        private void InstabilityExitEvent()
        {
            _currentFillMultiplier = normalFillMultiplier;
            _canUseAttack = false;
        }
        public override void SubscribeOnActionEvents()
        {
            base.SubscribeOnActionEvents();
            _attack.canceled += HandleAttacking;
        }
        public override void UnsubscribeFromActionEvents()
        {
            base.UnsubscribeFromActionEvents();
            _attack.canceled -= HandleAttacking;
        }

        ~SuperAttackState()
        {
            try
            {
                _heroAdrenaline.OnInstabilityEnter -= InstabilityEnterEvent;
                _heroAdrenaline.OnInstabilityExit -= InstabilityExitEvent;
            }
            catch (NullReferenceException) { }
        }
    }

    public class SuperAttackContext
    {
        public GameObject Hero { get; private set; }
        public Animator Animator { get; private set; }
        public int AnimationTrigger { get; private set; }

        public SuperAttackContext(GameObject hero, Animator animator, int animationTrigger)
        {
            Hero = hero;
            Animator = animator;
            AnimationTrigger = animationTrigger;
        }
    }
}