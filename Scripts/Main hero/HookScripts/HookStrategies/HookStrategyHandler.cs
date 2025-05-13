using System;
using System.Collections;
using ObjectLogicInterfaces;
using ObjectLogicRealization.Adrenaline;
using UnityEngine;

namespace Main_hero.HookScripts.HookStrategies
{
    public class HookStrategyHandler : MonoBehaviour
    {
        
        [SerializeField] private HookShadowScript hookShadowScript;
        [SerializeField] private HookBehaviour hookBehavior;
        [SerializeField] private Transform player;
        [SerializeField] private Transform hookEnd;
        //[SerializeField] private SpriteRenderer hookSpriteRenderer;
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private LayerMask playerBottomMask;
        [SerializeField] private GameObject chain;
        [SerializeField] private Transform heroObstacleCollider;

        [SerializeField] private HookContext context;
        [SerializeField] private HeroAdrenaline adrenaline;

        public bool IsActive { get; private set; }
        public HookScript.HookState CurrentHookState => context.CurrentHookState;
        private IEnumerator _hookUpdateCoroutine;

        public bool BehaviourWasForciblyChanged { get; private set; }

        #region Actions

        public event Action OnHookActivate, OnHookReturning, OnHookAlmostReturned, OnHookDisable;
        public event Action<ObstacleType> OnObstacleCollision;
        public event Action<HookBehaviour> OnHookHit;
        public event Action<float> OnHookHitDistanceCheck;
        

        #endregion

        private void Awake()
        {
            IsActive = false;
            hookShadowScript.OnTrigger += ObstacleCollisionEffect;
            _hookUpdateCoroutine = HookPhysicsUpdate();
            context = new HookContext
            {
                HookTransform = transform,
                PlayerTransform = player,
                ObstacleMask = obstacleMask,
                hookRigidBody = GetComponent<Rigidbody2D>(),
                HookEndTransform = hookEnd,
                HookStrategyHandler = this,
                Adrenaline = adrenaline
            };
            context.SetState(HookScript.HookState.Inactive);
            gameObject.SetActive(false);
            chain.SetActive(false);
        }

        private void ObstacleCollisionEffect(Collider2D collision)
        {
            ObstacleType obstacleType = ObstacleType.Default;
            if (collision.TryGetComponent(out ObstacleTypeHolder obstacleTypeHolder))
                obstacleType = obstacleTypeHolder.GetObstacleType();
            
            OnObstacleCollision?.Invoke(obstacleType);
            hookBehavior.OnObstacleHit(context, collision);
        }
        
        private IEnumerator HookPhysicsUpdate()
        {
            while (true)
            {
                switch (context.CurrentHookState)
                {
                    case HookScript.HookState.Thrown:
                        hookBehavior.OnThrown(context, Time.deltaTime);
                        break;
                    case HookScript.HookState.Returning:
                        hookBehavior.OnReturning(context, Time.deltaTime);
                        OnHookReturning?.Invoke();
                        break;
                }
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (context.CurrentHookState != HookScript.HookState.Returning) return;
                context.SetState(HookScript.HookState.Inactive);
                context.HookTransform.SetParent(context.PlayerTransform);
                gameObject.SetActive(false);
                return;
            }
            if (HitThroughObstacle(collision))
                return;
            TryDetectForceBehaviourChangerCollision(collision);
            hookBehavior.OnHit(context, collision);
        }

        private void TryDetectForceBehaviourChangerCollision(Collider2D collision)
        {
            if (context.CurrentHookState == HookScript.HookState.Returning) return;
            if (!collision.TryGetComponent(out IForceHookBehaviourChanger behaviourChanger)) return;
            SetHookStrategy(behaviourChanger.HookBehaviour);
            BehaviourWasForciblyChanged = true;
        }
        
        public bool ActivateHook(Vector2 mousePosition)
        {
            
            var position = player.position;
            Vector2 directionToTarget = (mousePosition - (Vector2)position).normalized;
        
            context.TargetPosition = (Vector2)position + hookBehavior.HookThrowDistance*directionToTarget;
            
            hookBehavior.CorrectChainMovement(context, 0);
            CalculateShadowOffset(directionToTarget);
            if (CheckStartCollision(heroObstacleCollider))
            {
                return false;
            }
            
            chain.SetActive(true);
            BehaviourWasForciblyChanged = false;
            context.SetState(HookScript.HookState.Thrown);
            hookBehavior.ActivateHook(context);
            SubscribeOnHookEvents(hookBehavior);
            
            gameObject.SetActive(true);
            OnHookActivate?.Invoke();
            StartCoroutine(_hookUpdateCoroutine);
            //CalculateShadowOffset();
            IsActive = true;
            return true;
        }
        
        private void OnDisable()
        {
            IsActive = false;
            context.SetState(HookScript.HookState.Inactive);
            OnHookDisable?.Invoke();
            StopCoroutine(_hookUpdateCoroutine);
            chain.SetActive(false);
            UnsubscribeFromHookEvents(hookBehavior);
        }

        private void OnDestroy()
        {
            hookShadowScript.OnTrigger -= ObstacleCollisionEffect;
        }
        private void CalculateShadowOffset(Vector2 hookDirection)
        {
            Vector2 playerPivotPosition = heroObstacleCollider.position;
            Vector2 hookStartPosition = player.position; // Предположим, начальная позиция хука совпадает с игроком
            float offset = ((Vector2)hookStartPosition - playerPivotPosition).magnitude;
            hookShadowScript.RecalculatePosition(hookEnd.position, offset, hookDirection);
        }
        private bool CheckStartCollision(Transform sourceBottom)
        {
            Vector2 hookDirection = hookShadowScript.GetPosition - sourceBottom.position;
            var rayToObstacle = Physics2D.Raycast(sourceBottom.position, hookDirection.normalized,
                hookDirection.magnitude, obstacleMask);
            if (!rayToObstacle) 
                return false;
            var rayToEnemyBottom = Physics2D.Raycast(sourceBottom.position, hookDirection.normalized,
                hookDirection.magnitude, playerBottomMask);
        
            return !rayToEnemyBottom || rayToObstacle.distance < rayToEnemyBottom.distance;

        }
        private bool HitThroughObstacle(Collider2D objCollider)
        {
            if (!objCollider.transform.Find("ObstacleCollider"))
            {
                return true;
            }
            var shadowToPlayerBottom =  objCollider.transform.Find("ObstacleCollider").position - hookShadowScript.GetPosition;
            // if (hookBehavior.HookSpeed.y <= 0)
            // {
            //     return false;
            // }
            return Physics2D.Raycast(hookShadowScript.GetPosition, shadowToPlayerBottom.normalized,
                shadowToPlayerBottom.magnitude, context.ObstacleMask);
        }

        public void SetHookStrategy(HookBehaviour behaviour)
        {
            UnsubscribeFromHookEvents(hookBehavior);
            hookBehavior = behaviour;
            SubscribeOnHookEvents(hookBehavior);
        }

        private void HookAlmostReturnedEvent()
        {
            OnHookAlmostReturned?.Invoke();
        }

        private void SubscribeOnHookEvents(HookBehaviour behaviour)
        {
            behaviour.OnHookAlmostReturned += HookAlmostReturnedEvent;
            behaviour.OnHookHit += HookHitEvent;
            behaviour.OnHookHitDistanceCheck += HookDistanceChecker;
        }

        private void UnsubscribeFromHookEvents(HookBehaviour behaviour)
        {
            behaviour.OnHookHit -= HookHitEvent;
            behaviour.OnHookAlmostReturned -= HookAlmostReturnedEvent;
            behaviour.OnHookHitDistanceCheck -= HookDistanceChecker;
        }
        private void HookHitEvent() => OnHookHit?.Invoke(hookBehavior);

        private void HookDistanceChecker(float distancePercentage) =>
            OnHookHitDistanceCheck?.Invoke(distancePercentage);
    }
}
