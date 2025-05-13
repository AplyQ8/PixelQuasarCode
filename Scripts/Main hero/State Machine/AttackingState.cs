using System;
using System.Collections.Generic;
using System.Linq;
using ObjectLogicInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Main_hero.State_Machine
{
    [Serializable]
    public class AttackingState: BaseState
    {
        #region Animation triggers
        private static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
        private static readonly int LastVertical = Animator.StringToHash("LastVertical");
        
        private static readonly int AttackNumber = Animator.StringToHash("AttackNumber");
        private static readonly int Attack = Animator.StringToHash("Attack");

        #endregion

        [SerializeField] private HeroSounds heroSounds;
        
        [SerializeField] private InputAction attack;
        
        private Rigidbody2D _rigidbody;
        private Transform _heroBottom;
        
        public event Action OnAttack;
        public event Action<Collider2D> OnEnemyHit;
        [SerializeField] private int _numberOfAvailableAttacks;

        [SerializeField] private Timer _attackCooldownTimer;
        [SerializeField] private Timer _attackSeriesCooldownTimer;
        [SerializeField] private Timer _attackSeriesAbortionTimer;
        [SerializeField]private Timer _attackSeriesFinalCooldownTimer;

        [Header("Attack cooldown properties")] 
        [SerializeField] private float attackCooldownTime;
        [SerializeField] private float attackSeriesCooldownTime;
        [SerializeField] private float attackSeriesAbortionTime;
        
        [Header("Attack characteristics")]
        [SerializeField] private float AttackOffset = 2f;
        [SerializeField] private float _attackRadius = 1f;
        [SerializeField] private float attackMovementSpeed = 1f;
        [SerializeField] private LayerMask objectsToHitLayer;
        [SerializeField] private LayerMask throughCollisionMask;

        private bool _canAttack;
        private enum AttackCondition
        {
            CurrentAnimationEnd,
            CooldownEnd
        }
        
        private Dictionary<AttackCondition, bool> _attackConditionsDictionary;

        private ICanAttack _attackScript;

        private Vector3 _currentAttackPosition;
        private Vector3 _currentAttackDirection;
        private bool _attackIsRegistered;
        
        public int CurrentAttackCounter
        {
            get => _currentAttackCounter;
            private set => _currentAttackCounter = value >= _numberOfAvailableAttacks ? 0 : value;
        }
        private int _currentAttackCounter;

        public override void InitializeState(GameObject hero, HeroStateHandler stateHandler, Transform heroTransform,
            Animator animator, PlayerInput playerInput)
        {
            base.InitializeState(hero, stateHandler, heroTransform, animator, playerInput);
            _heroBottom = Hero.transform.Find("ObstacleCollider");
            _attackScript = Hero.GetComponent<ICanAttack>();
            _rigidbody = Hero.GetComponent<Rigidbody2D>();
            
            _currentAttackCounter = 0;
            
            _attackCooldownTimer = new Timer(attackCooldownTime);
            _attackCooldownTimer.OnTimerDone += AttackCooldownTimer;
            _attackSeriesCooldownTimer = new Timer(attackSeriesCooldownTime);
            _attackSeriesCooldownTimer.OnTimerDone += AttackSeriesCooldownTimer;
            _attackSeriesAbortionTimer = new Timer(attackSeriesAbortionTime);
            _attackSeriesAbortionTimer.OnTimerDone += ResetAttackCounter;
            _attackSeriesFinalCooldownTimer = new Timer(attackSeriesCooldownTime - attackSeriesAbortionTime);
            _attackSeriesFinalCooldownTimer.OnTimerDone += AttackSeriesCooldownTimer;
            _canAttack = true;
            
            _attackConditionsDictionary = new Dictionary<AttackCondition, bool>
            {
                { AttackCondition.CurrentAnimationEnd, true },
                { AttackCondition.CooldownEnd, true }
            };
            
            attack = playerInput.currentActionMap.FindAction("Attack");
        }

        public override void EnterState()
        {
            base.EnterState();
            if (!_canAttack)
            {
                StateHandler.SwitchState(StateHandler.NormalState);
                return;
            }
            SubscribeOnActionEvents();
            StartAttack();
            
        }

        public override void ExitState()
        {
            base.ExitState();
            UnsubscribeFromActionEvents();
        }
        public void ProceedAttack()
        {
            // Handle hitting enemies
            HandleHitEnemies(_currentAttackPosition);
            
            
            if (CurrentAttackCounterEqualsMax())
            {
                _attackSeriesCooldownTimer.StartTimer();
            }
            else
            {
                _attackCooldownTimer.StartTimer();
                _attackSeriesAbortionTimer.StartTimer();
            }
            
            _canAttack = false;
            CurrentAttackCounter++;
        }

        private void StartAttack(Vector3? attackDirection = null, Vector3? attackPosition = null)
        {
            _attackSeriesAbortionTimer.StopTimer();
            ResetAttackConditions();

            // Stop player's movement
            _rigidbody.velocity = Vector2.zero;

            if (attackDirection.HasValue && attackPosition.HasValue)
            {
                // Если были переданы attackDirection и attackPosition, используем их
                _currentAttackDirection = attackDirection.Value;
                _currentAttackPosition = attackPosition.Value;
            }
            else
            {
                // Если параметры не переданы, вычисляем их
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _currentAttackDirection = CalculateAttackDirection(mousePosition);
                _currentAttackPosition = Hero.transform.position + _currentAttackDirection * AttackOffset;
            }
            
            // Give player a movement in attack direction
            _rigidbody.velocity = _currentAttackDirection.normalized * attackMovementSpeed;

            // Start attack animations
            StartAttackAnimations(_currentAttackDirection);
            heroSounds.PlayAttackSound();
        }

        public void StopAttackMovement()
        {
            _rigidbody.velocity = Vector2.zero;
        }
        
        private Vector3 CalculateAttackDirection(Vector3 mousePosition)
        {
            Vector3 direction = (mousePosition - Hero.transform.position).normalized;
            direction.z = 0; // Ensure direction is in 2D plane
            return direction;
        }
        private void StartAttackAnimations(Vector3 attackDirection)
        {
            Animator.SetFloat(LastHorizontal, attackDirection.x);
            Animator.SetFloat(LastVertical, attackDirection.y);
            Animator.SetTrigger(Attack);
            Animator.SetInteger(AttackNumber, CurrentAttackCounter);
        }
        private void HandleHitEnemies(Vector3 attackPosition)
        {
            var hitObjects = Physics2D.OverlapCircleAll(attackPosition, _attackRadius, objectsToHitLayer);

            float damage = _attackScript.GetCurrentAttack();

            bool OnAttackIsInvoked = false;
        
            foreach (var hitObject in hitObjects)
            {
                if (IsEnemyObstructed(hitObject))
                    continue;

                if (!hitObject.TryGetComponent(out IDamageable damageable)) continue;
                damageable.TakeDamage(damage, DamageTypeManager.DamageType.MeleeAttack, _heroBottom.position);
                OnEnemyHit?.Invoke(hitObject);
                if (OnAttackIsInvoked) continue;
                OnAttack?.Invoke();
                OnAttackIsInvoked = true;

            }
        }
        private bool IsEnemyObstructed(Collider2D hitObject)
        {
            if (hitObject.gameObject.TryGetComponent(out IObstructed obstructedScript))
            {
                Vector3 enemyPosition = obstructedScript.GetPivotPoint();
                RaycastHit2D hit = 
                    Physics2D.Linecast(
                        Hero.GetComponent<IMovable>().GetPivotPosition(), 
                        enemyPosition, 
                        throughCollisionMask);
                return hit.collider != null;
            }
        
            return true;
        }
        public void AttackAnimationEnd()
        {
            CalculateAttackAvailability(AttackCondition.CurrentAnimationEnd);
            if (_attackIsRegistered && _canAttack)
            {
                _attackIsRegistered = false;
                attack.performed -= HandleAttacking;
                StartAttack(_currentAttackDirection, _currentAttackPosition);
                return;
            }
            StateHandler.SwitchState(StateHandler.NormalState);
            
        }
        private void AttackCooldownTimer()
        {
            CalculateAttackAvailability(AttackCondition.CooldownEnd);
        }
        private void CalculateAttackAvailability(AttackCondition condition)
        {
            _attackConditionsDictionary[condition] = true;
            if (_attackConditionsDictionary.Values.All(value => value) /*&& !CurrentAttackCounterEqualsMax()*/)
                _canAttack = true;
        }
        private void ResetAttackConditions()
        {
            foreach (var condition in _attackConditionsDictionary.Keys.ToList())
            {
                _attackConditionsDictionary[condition] = false;
            }
        }
        private void AttackSeriesCooldownTimer()
        {
            _canAttack = true;
        }
        private void ResetAttackCounter()
        {
            CurrentAttackCounter = 0;
            _canAttack = false;
            _attackSeriesFinalCooldownTimer.StartTimer();
            
        }

        private void initializeTimers()
        {
            _attackCooldownTimer = new Timer(attackCooldownTime);
            _attackCooldownTimer.OnTimerDone += AttackCooldownTimer;
            _attackSeriesCooldownTimer = new Timer(attackSeriesCooldownTime);
            _attackSeriesCooldownTimer.OnTimerDone += AttackSeriesCooldownTimer;
            _attackSeriesAbortionTimer = new Timer(attackSeriesAbortionTime);
            _attackSeriesAbortionTimer.OnTimerDone += ResetAttackCounter;
            _attackSeriesFinalCooldownTimer = new Timer(attackSeriesCooldownTime - attackSeriesAbortionTime);
            _attackSeriesFinalCooldownTimer.OnTimerDone += AttackSeriesCooldownTimer;
        }
        private bool CurrentAttackCounterEqualsMax() => CurrentAttackCounter == _numberOfAvailableAttacks - 1;
        public void TickTimers()
        {
            if (_attackCooldownTimer == null || _attackSeriesCooldownTimer == null ||
                _attackSeriesAbortionTimer == null || _attackSeriesFinalCooldownTimer == null)
            {
                initializeTimers();
            }
            _attackCooldownTimer.Tick();
            _attackSeriesCooldownTimer.Tick();
            _attackSeriesAbortionTimer.Tick();
            _attackSeriesFinalCooldownTimer.Tick();
        }

        public override void UnsubscribeFromActionEvents()
        {
            base.UnsubscribeFromActionEvents();
            attack.performed -= HandleAttacking;
        }

        public void RegisterAttackClick()
        {
            attack.performed += HandleAttacking;
        }

        private void HandleAttacking(InputAction.CallbackContext obj)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _currentAttackDirection = CalculateAttackDirection(mousePosition);
            _currentAttackPosition = Hero.transform.position + _currentAttackDirection * AttackOffset;
            _attackIsRegistered = true;
        }
    }
}