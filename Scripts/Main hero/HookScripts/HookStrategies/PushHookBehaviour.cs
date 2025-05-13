using System;
using ObjectLogicInterfaces;
using UnityEngine;

namespace Main_hero.HookScripts.HookStrategies
{
    [CreateAssetMenu(fileName = "PushHook", menuName = "Hooks/PushHook")]
    public class PushHookBehaviour : HookBehaviour
    {
        [SerializeField] private float stunOnObstacleCollisionDuration;
        private Collider2D _hookedObject;

        public override void ActivateHook(HookContext context)
        {
            base.ActivateHook(context);
            _hookedObject = null;
        }
        
        public override void OnThrown(HookContext context, float timeInMovement)
        {
            if(_hookThrowTimer < _hookThrowDuration) {
                CorrectChainMovement(context, _hookThrowTimer);
                _hookThrowTimer += timeInMovement;
            }
            else {
                if (_hookedObject != null)
                {
                    if (_hookedObject.TryGetComponent(out IHookable hookable))
                    {
                        hookable.EndPulledByHook();
                    }
                }
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
            if (context.CurrentHookState == HookScript.HookState.Returning)
                return;
            if(_hookedObject != null) return;
            if (collision.TryGetComponent<IHookable>(out var hookableObject))
            {
                if (hookableObject.IsIntangible())
                    return;
                hookableObject.PulledByHook(
                    context.HookTransform, 
                    context.PlayerTransform.position, 
                    MaxDamage,
                    context.HookStrategyHandler);
                _hookedObject = collision;
                float catchDistance = (context.HookTransform.position - context.PlayerTransform.position).magnitude;
                float distancePercentage = catchDistance / HookThrowDistance;
                CallHookDistanceCheck(distancePercentage);
                CallOnHitEvent();
                return;
            }
            OnObstacleHit(context, collision);
        }

        public override void OnObstacleHit(HookContext context, Collider2D collision)
        {
            
            context.SetState(HookScript.HookState.Returning);
            float currentDistance = (context.HookTransform.position - context.PlayerTransform.position).magnitude;
            if (_hookedObject != null)
            {
                if (_hookedObject.TryGetComponent(out IStunable stunable))
                {
                    float catchDistance = (context.HookTransform.position - context.PlayerTransform.position).magnitude;
                    float distancePercentage = catchDistance / HookThrowDistance;
                    float estimatedStunTime = (float)Math.Floor(stunOnObstacleCollisionDuration * distancePercentage);
                    stunable.GetStunned(estimatedStunTime);
                }
            }
            _hookBaseSpeed = HookSpeedBack;
            _hookThrowDuration = currentDistance / _hookBaseSpeed;
            _hookThrowTimer = 0;
        }
    }
}