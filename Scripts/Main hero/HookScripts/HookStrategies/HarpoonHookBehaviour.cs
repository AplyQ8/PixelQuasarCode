using System;
using Main_hero.HookScripts.HookStrategies;
using ObjectLogicInterfaces;
using UnityEngine;

namespace Tests
{
    [CreateAssetMenu(fileName = "HarpoonHook", menuName = "Hooks/HarpoonHook")]
    public class HarpoonHookBehaviour : HookBehaviour
    {
        [SerializeField] private float pullForce;
        [SerializeField] private HarpoonStates currentState;

        private enum HarpoonStates
        {
            Ordinary,
            Caught
        }

        public override void ActivateHook(HookContext context)
        {
            currentState = HarpoonStates.Ordinary;
            base.ActivateHook(context);
        }
        
        public override void OnThrown(HookContext context, float timeInMovement)
        {
            if(_hookThrowTimer < _hookThrowDuration) {
                CorrectChainMovement(context, _hookThrowTimer);
                _hookThrowTimer += timeInMovement;
            }
            else {
                context.SetState(HookScript.HookState.Returning);
                _hookBaseSpeed = HookSpeedBack;
                _hookThrowDuration = HookThrowDistance / _hookBaseSpeed;
                _hookThrowTimer = 0;
            }
        }

        public override void OnReturning(HookContext context, float timeInMovement)
        {
            CorrectChainMovement(context, _hookThrowDuration - _hookThrowTimer);
            _hookThrowTimer += timeInMovement;
            base.OnReturning(context, timeInMovement);
        }

        public override void OnHit(HookContext context, Collider2D collision)
        {
            if (context.CurrentHookState != HookScript.HookState.Thrown)
                return;
            if (collision.TryGetComponent<IHookable>(out var hookableObject))
            {
                if (hookableObject.IsIntangible())
                    return;
                context.HookTransform.position = collision.transform.position;
                context.TargetPosition = collision.transform.position;
                context.HookTransform.SetParent(null);
                currentState = HarpoonStates.Caught;
                context.SetState(HookScript.HookState.Returning);
                float currentDistance = (context.HookTransform.position - context.PlayerTransform.position).magnitude;
                _hookBaseSpeed = pullForce;
                _hookThrowDuration = currentDistance / _hookBaseSpeed;
                _hookThrowTimer = 0;
                TryStun(collision, _hookThrowDuration);
                TryHook(collision, context);
                CallOnHitEvent();
                return;
            }
            OnObstacleHit(context, collision);
        }

        public override void OnObstacleHit(HookContext context, Collider2D collision)
        {
            context.SetState(HookScript.HookState.Returning);
            float currentDistance = (context.HookTransform.position - context.PlayerTransform.position).magnitude;
            _hookBaseSpeed = HookSpeedBack;
            _hookThrowDuration = currentDistance / _hookBaseSpeed;
            _hookThrowTimer = 0;
        }

        public override void CorrectChainMovement(HookContext context, float timeInMovement)
        {
            switch (currentState)
            {
                case HarpoonStates.Ordinary:
                    base.CorrectChainMovement(context, timeInMovement);
                    break;
                case HarpoonStates.Caught:
                    float angle = AngleBetweenTwoPoints(context.TargetPosition, context.PlayerTransform.position);
                    context.HookTransform.rotation = Quaternion.Euler(new Vector3(0f,0f,angle - 90));
                    
                    Vector2 directionToTarget = (context.TargetPosition - (Vector2)context.PlayerTransform.position).normalized;
                    HookSpeed = _hookBaseSpeed*directionToTarget;

                    context.PlayerTransform.position =
                        (Vector2)context.HookTransform.position - timeInMovement * HookSpeed;
                    break;
            }
        }

        private void TryStun(Collider2D collision, float stunDuration)
        {
            if (collision.TryGetComponent(out IStunable stunable))
            {
                stunable.GetStunned(stunDuration);
            }
        }
        
        private void TryHook(Collider2D collision, HookContext context)
        {
            float catchDistance = (context.HookTransform.position - context.PlayerTransform.position).magnitude;
            float distancePercentage = catchDistance / HookThrowDistance;
            int dealtDamage = (int)Math.Floor(MaxDamage * distancePercentage);
            if (collision.TryGetComponent(out EnemyScript enemyScript))
            {
                if (!collision.TryGetComponent(out IDamageable damageable)) return;
                damageable.TakeDamage(dealtDamage, DamageTypeManager.DamageType.Default);
                CallHookDistanceCheck(distancePercentage);
                return;
            }
            if (!collision.TryGetComponent(out IHookable hookableObject)) return;
            hookableObject.PulledByHook(
                context.HookEndTransform, 
                context.PlayerTransform.position, 
                dealtDamage, 
                context.HookStrategyHandler);
            CallHookDistanceCheck(distancePercentage);

        }
    }
    
}