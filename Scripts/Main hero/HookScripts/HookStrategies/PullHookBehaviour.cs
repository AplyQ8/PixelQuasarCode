using System;
using ObjectLogicInterfaces;
using UnityEngine;

namespace Main_hero.HookScripts.HookStrategies
{
    [CreateAssetMenu(fileName = "PullHook", menuName = "Hooks/PullHook")]
    public class PullHookBehaviour : HookBehaviour
    {
        public override void OnThrown(HookContext context, float timeInMovement)
        {
            // Vector2 direction = (context.TargetPosition - (Vector2)context.PlayerTransform.position).normalized;
            // context.HookTransform.position = (Vector2)context.PlayerTransform.position + direction * (context.HookThrowTimer * HookSpeedToTarget);
            // CorrectChainMovement(context, _hookThrowTimer);
            // _hookThrowTimer += timeInMovement;
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
            // Vector2 direction = ((Vector2)context.PlayerTransform.position - (Vector2)context.HookTransform.position).normalized;
            // context.HookTransform.position += (Vector3)(direction * (HookSpeedBack * Time.deltaTime));
            CorrectChainMovement(context, _hookThrowDuration - _hookThrowTimer);
            _hookThrowTimer += timeInMovement;
            base.OnReturning(context, timeInMovement);
        }

        public override void OnHit(HookContext context, Collider2D collision)
        {
            if (context.CurrentHookState != HookScript.HookState.Thrown)
                return;
            TryHook(context, collision);
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
        
        private void TryHook(HookContext context, Collider2D collision)
        {
            if (collision.TryGetComponent<IHookable>(out var hookableObject))
            {
                if (hookableObject.IsIntangible())
                    return;
                float catchDistance = (context.HookTransform.position - context.PlayerTransform.position).magnitude;
                float distancePercentage = catchDistance / HookThrowDistance;
                int dealtDamage = (int)Math.Floor(MaxDamage * distancePercentage);
                hookableObject.PulledByHook(
                    context.HookEndTransform, 
                    context.PlayerTransform.position, 
                    dealtDamage,
                    context.HookStrategyHandler);
                CallOnHitEvent();
                CallHookDistanceCheck(distancePercentage);
            }
        }
    }
}