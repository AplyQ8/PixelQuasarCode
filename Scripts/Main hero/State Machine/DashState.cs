using System;
using ObjectLogicInterfaces;
using ObjectLogicRealization.Adrenaline;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Main_hero.State_Machine
{
    [Serializable]
    public class DashState: BaseState
    {
        private Rigidbody2D _rigidbody;
        
        [SerializeField] private HeroSounds heroSounds;
        
        [Header("Dash params")]
        [SerializeField] private float dashSpeed;
        [SerializeField] private float dashTime;
        [SerializeField] public float dashCooldown;

        private DashStates _currentDashState;
        private float _movementThreshold = 0.1f;
        private Vector2 _dashDirection;
        private HeroAdrenaline _heroAdrenaline;
        
        #region Timers

        public Timer DashCooldownTimer;
        private Timer _inDashTimer;

        #endregion

        public event Action OnCooldownClickEvent;
        public event Action OnDash;
        
        public override void InitializeState(GameObject hero, HeroStateHandler stateHandler, Transform heroTransform,
            Animator animator, PlayerInput playerInput)
        {
            base.InitializeState(hero, stateHandler, heroTransform, animator, playerInput);
            _currentDashState = DashStates.Ready;
            _rigidbody = Hero.GetComponent<Rigidbody2D>();
            _heroAdrenaline = Hero.GetComponent<HeroAdrenaline>();
            DashCooldownTimer = new Timer(dashCooldown);
            DashCooldownTimer.OnTimerDone += OnCooldownDoneEvent;

            _inDashTimer = new Timer(dashTime);
            _inDashTimer.OnTimerDone += OnInDashDoneEvent;
        }

        public override void EnterState()
        {
            if (_currentDashState == DashStates.Cooldown)
            {
                OnCooldownClickEvent?.Invoke();
                StateHandler.SwitchState(StateHandler.PreviousState);
                return;
            }

            if (!IsMoving())
            {
                StateHandler.SwitchState(StateHandler.PreviousState);
                return;
            }

            if (!_heroAdrenaline.IsAdrenalineEnough(AdrenalineModificatorManager.AdrenalineModificator.Dash))
            {
                StateHandler.SwitchState(StateHandler.PreviousState);
                return;
            }
            
            StartDash(_rigidbody.velocity.normalized);
            
        }

        public override void UpdateState(HeroStateHandler stateHandler)
        {
            if (_currentDashState is not DashStates.InDash)
                return;
            _rigidbody.velocity = _dashDirection * dashSpeed;
        }
        
        private void StartDash(Vector2 dashDirection)
        {
            OnDash?.Invoke();
            _dashDirection = dashDirection;
            _currentDashState = DashStates.InDash;
            _inDashTimer.StartTimer();
            heroSounds.PlayDashSound();
        }

        private void OnCooldownDoneEvent()
        {
            _currentDashState = DashStates.Ready;
        }

        private void OnInDashDoneEvent()
        {
            _currentDashState = DashStates.Cooldown;
            DashCooldownTimer.StartTimer();
            StateHandler.SwitchState(StateHandler.NormalState);
        }

        private bool IsMoving() => _rigidbody.velocity.magnitude > _movementThreshold;

        private void initializeTimers()
        {
            DashCooldownTimer = new Timer(dashCooldown);
            DashCooldownTimer.OnTimerDone += OnCooldownDoneEvent;

            _inDashTimer = new Timer(dashTime);
            _inDashTimer.OnTimerDone += OnInDashDoneEvent;
        }

        public void TickTimers()
        {
            if (DashCooldownTimer == null || _inDashTimer == null)
            {
                initializeTimers();
            }
            DashCooldownTimer.Tick();
            _inDashTimer.Tick();
        }

        public override void ExitState()
        {
            base.ExitState();
            _rigidbody.velocity = Vector2.zero;
        }
    }
    
    public enum DashStates
    {
        Cooldown,
        Ready,
        InDash
    }
    
}