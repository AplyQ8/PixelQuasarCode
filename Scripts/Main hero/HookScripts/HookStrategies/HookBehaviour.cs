using System;
using ObjectLogicInterfaces;
using UnityEngine;

namespace Main_hero.HookScripts.HookStrategies
{
    public abstract class HookBehaviour : ScriptableObject
    {
        public float HookSpeedToTarget;
        public float HookSpeedBack;
        public float HookThrowDistance;
        public float maxThrowDistance;
        public float minHookThrowDistance;
        public int MaxDamage;
        public Vector2 HookSpeed { get; protected set; }
        protected float _hookBaseSpeed;
        
        protected float _hookThrowTimer;
        protected float _hookThrowDuration;

        public event Action OnHookAlmostReturned, OnHookHit;
        public event Action<float> OnHookHitDistanceCheck;

        public bool AdrenalineInfluencesDistance = false;

        public virtual void ActivateHook(HookContext context)
        {
            CorrectChainMovement(context, 0);
            _hookBaseSpeed = HookSpeedToTarget;
            HookThrowDistance = maxThrowDistance;
            if (AdrenalineInfluencesDistance)
            {
                HookThrowDistance = Math.Max(minHookThrowDistance,
                    maxThrowDistance * context.Adrenaline.GetAdrenalineRatio());
            }
            _hookThrowDuration = HookThrowDistance / _hookBaseSpeed;
            _hookThrowTimer = 0;
        }
        public abstract void OnThrown(HookContext context, float timeInMovement);

        public virtual void OnReturning(HookContext context, float timeInMovement)
        {
            var distanceDifference = 
                ((Vector2)context.PlayerTransform.position - (Vector2)context.HookTransform.position).magnitude;
            if(distanceDifference < 5)
                OnHookAlmostReturned?.Invoke();
        }
        public abstract void OnHit(HookContext context, Collider2D collision);
        public abstract void OnObstacleHit(HookContext context, Collider2D collision);

        //public abstract void HookPhysicsUpdate(HookContext context);
        public virtual void CorrectChainMovement(HookContext context, float timeInMovement)
        {
            // Рассчитываем угол между игроком и таргетом
            float angle = AngleBetweenTwoPoints(context.TargetPosition, context.PlayerTransform.position);
                    
            // Корректируем поворот Хука
            //gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f,0f,angle - 90));
            context.HookTransform.rotation = Quaternion.Euler(new Vector3(0f,0f,angle - 90));

            // Корректируем скорость таким образом, чтобы цепь летела по направлению к таргету
            Vector2 directionToTarget = (context.TargetPosition - (Vector2)context.PlayerTransform.position).normalized;
            HookSpeed = _hookBaseSpeed*directionToTarget;
                    
            // Корректируем позицию цепи и маски
            context.HookTransform.position = (Vector2)context.PlayerTransform.position + timeInMovement * HookSpeed;
        }
        protected float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }

        protected void CallOnHitEvent()
        {
            OnHookHit?.Invoke();
        }

        protected void CallHookDistanceCheck(float distancePercentage) =>
            OnHookHitDistanceCheck?.Invoke(distancePercentage);
        
    }
    
}