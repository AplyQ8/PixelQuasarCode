// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
// using ObjectLogicInterfaces;
// using UnityEngine;
//
// public class GunpowderBarrel : MonoBehaviour, IDamageable, IHookable, IObstructed
// {
//     [SerializeField] private Animator animator;
//     [SerializeField] private ParticleSystem[] particleSystems;
//     [SerializeField] private float explosionRadius;
//     [SerializeField] private LayerMask layerMask;
//     [SerializeField] private List<ModifierData> dataModifiers;
//     [SerializeField] private float stopPullDistance = 2f;
//     private bool _allParticlesStopped = false;
//     private GunpowderBarrelStates _currentState;
//     private static readonly int TriggerBarrel = Animator.StringToHash("TriggerBarrel");
//     private static readonly int Explode = Animator.StringToHash("Explode");
//
//     private Transform _hookTransform;
//     private Transform _playerTransform;
//
//     public event Action<float> OnHealthChange;
//     public event Action<float> OnHealthBoundariesChange;
//     public event Action OnDeathEvent;
//
//     [SerializeField] private enum GunpowderBarrelStates
//     {
//         Idle,
//         Tick,
//         Pulled,
//         Explosion
//     }
//
//     private void Start()
//     {
//         _currentState = GunpowderBarrelStates.Idle;
//     }
//
//     private void Update()
//     {
//         switch(_currentState)
//         {
//             case GunpowderBarrelStates.Pulled:
//                 transform.position = _hookTransform.position;
//                 break;
//         }
//     }
//
//     private void OnTriggerEnter2D(Collider2D col)
//     {
//         if (_currentState != GunpowderBarrelStates.Tick)
//             return;
//         if(col.CompareTag("Enemy"))
//             animator.SetTrigger(Explode);
//     }
//
//     private void SwitchState(GunpowderBarrelStates state)
//     {
//         switch (state)
//         {
//             case GunpowderBarrelStates.Tick:
//                 _currentState = GunpowderBarrelStates.Tick;
//                 animator.SetTrigger(TriggerBarrel);
//                 break;
//             case GunpowderBarrelStates.Explosion:
//                 _currentState = GunpowderBarrelStates.Explosion;
//                 ExplodeBarrel();
//                 break;
//         }
//     }
//     
//     
//     
//     public void TakeDamage(float value, DamageTypeManager.DamageType damageType)
//     {
//         SwitchState(GunpowderBarrelStates.Tick);
//     }
//
//     public void TakeDamage(float value, DamageTypeManager.DamageType damageType, Vector3 damageSourcePosition)
//     {
//         TakeDamage(value, damageType);
//     }
//
//     public float PulledByHook(Transform Hook, Vector3 pullBeginning, int damage)
//     {
//         _currentState = GunpowderBarrelStates.Pulled;
//         animator.SetTrigger(TriggerBarrel);
//         _hookTransform = Hook;
//         Hook.GetComponent<HookScript>().OnHookDisable += HookDisableEvent;
//         return stopPullDistance;
//     }
//
//     private void HookDisableEvent()
//     {
//         SwitchState(GunpowderBarrelStates.Tick);
//         _hookTransform.GetComponent<HookScript>().OnHookDisable -= HookDisableEvent;
//     }
//
//     private void TriggerParticles()
//     {
//         foreach (ParticleSystem ps in particleSystems)
//         {
//             ps.Play();
//         }
//     }
//
//     private void ExplodeBarrel()
//     {
//         TriggerParticles();
//         Collider2D[] objectsToDealDamage = Physics2D.OverlapCircleAll(transform.position, 
//             explosionRadius, 
//             layerMask);
//         foreach (var victim in objectsToDealDamage)
//         {
//             foreach (var modifier in dataModifiers)
//             {
//                 modifier.statModifier.AffectObject(victim.gameObject, modifier.value);
//             }
//         }
//     }
//
//     public void DestroyBarrel() => Destroy(gameObject);
//
//     public Vector3 GetPivotPoint() => transform.position;
// }


using System;
using System.Collections.Generic;
using Main_hero.HookScripts.HookStrategies;
using ObjectLogicInterfaces;
using UnityEngine;

namespace InteractableObjects.GunpowderBarrel
{
    public class GunpowderBarrel : MonoBehaviour, IDamageable, IHookable, IObstructed, IForceHookBehaviourChanger, IForceCameraShake
    {
        [field: SerializeField] public HookBehaviour HookBehaviour { get; set; }

        [field: SerializeField] public float ShakeMagnitude { get; private set; }
        [field: SerializeField] public float ShakeDuration { get; private set; }
        
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem[] particleSystems;
        [SerializeField] private float explosionRadius;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private List<ModifierData> dataModifiers;
        [SerializeField] private float stopPullDistance = 2f;
        private bool _allParticlesStopped = false;
        private GunpowderBarrelStates _currentState;
        private static readonly int TriggerBarrel = Animator.StringToHash("TriggerBarrel");
        private static readonly int Explode = Animator.StringToHash("Explode");

        private Transform _hookTransform;
        private Transform _playerTransform;

        public event Action<float> OnHealthChange;
        public event Action<float> OnHealthBoundariesChange;
        public event Action OnDeathEvent;

        private BoxCollider2D boxCollider;
        private CircleCollider2D circleCollider;
        private LayerMask enemyMask;

        private float circleColliderRadius;
        private HookStrategyHandler _hookStrategyHandler;

        [SerializeField] private enum GunpowderBarrelStates
        {
            Idle,
            Tick,
            Pulled,
            Explosion
        }

        private void Start()
        {
            _currentState = GunpowderBarrelStates.Idle;

            boxCollider = transform.GetComponent<BoxCollider2D>();
            circleCollider = transform.GetComponent<CircleCollider2D>();
            circleColliderRadius = circleCollider.radius*transform.localScale.x;
        
            enemyMask = LayerMask.GetMask("Enemy");
        }

        private void Update()
        {
            switch(_currentState)
            {
                case GunpowderBarrelStates.Pulled:
                    transform.position = _hookTransform.position;
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_currentState != GunpowderBarrelStates.Pulled)
                return;
            if (col.CompareTag("Enemy"))
            {
                animator.SetTrigger(TriggerBarrel);
                animator.SetTrigger(Explode);
            }
        }

        private void SwitchState(GunpowderBarrelStates state)
        {
            switch (state)
            {
                case GunpowderBarrelStates.Tick:
                    _currentState = GunpowderBarrelStates.Tick;
                    animator.SetTrigger(TriggerBarrel);
                    break;
                case GunpowderBarrelStates.Explosion:
                    _currentState = GunpowderBarrelStates.Explosion;
                    ExplodeBarrel();
                    break;
            }
        }
    
    
    
        public void TakeDamage(float value, DamageTypeManager.DamageType damageType)
        {
            SwitchState(GunpowderBarrelStates.Tick);
        }

        public void TakeDamage(float value, DamageTypeManager.DamageType damageType, Vector3 damageSourcePosition)
        {
            TakeDamage(value, damageType);
        }

        public float PulledByHook(Transform Hook, Vector3 pullBeginning, int damage, HookStrategyHandler hookStrategyHandler)
        {
            _currentState = GunpowderBarrelStates.Pulled;
        
            boxCollider.enabled = false;
            circleCollider.enabled = true;

            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, circleColliderRadius, enemyMask);
            if (enemiesInRange.Length > 0) {
                animator.SetTrigger(TriggerBarrel);
                animator.SetTrigger(Explode);
            }
        
            _hookTransform = Hook;
            //var hookStrategyHandler = Hook.GetComponent<HookStrategyHandler>();
            _hookStrategyHandler = hookStrategyHandler;
            hookStrategyHandler.OnHookDisable += HookDisableEvent;
            //SetHookBehaviour(hookStrategyHandler);
            return stopPullDistance;
        }

        public void EndPulledByHook()
        {
            HookDisableEvent();
        }

        public bool IsIntangible() => false;

        private void HookDisableEvent()
        {
            SwitchState(GunpowderBarrelStates.Tick);
            //_hookTransform.GetComponent<HookStrategyHandler>().OnHookDisable -= HookDisableEvent;
            _hookStrategyHandler.OnHookDisable -= HookDisableEvent;
        }

        private void TriggerParticles()
        {
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Play();
            }
        }

        private void ExplodeBarrel()
        {
            TriggerParticles();
            Collider2D[] objectsToDealDamage = Physics2D.OverlapCircleAll(transform.position, 
                explosionRadius, 
                layerMask);
            foreach (var victim in objectsToDealDamage)
            {
                foreach (var modifier in dataModifiers)
                {
                    modifier.statModifier.AffectObject(victim.gameObject, modifier.value);
                }
            }
            ShakeCamera();
        }

        public void StartExplosion()
        {
            animator.SetTrigger(TriggerBarrel);
            animator.SetTrigger(Explode);
        }

        public void DestroyBarrel() => Destroy(gameObject);
    
        public void SetHookBehaviour(HookStrategyHandler hookStrategyHandler)
        {
            hookStrategyHandler.SetHookStrategy(HookBehaviour);
        }

        public Vector3 GetPivotPoint() => transform.position;
        public void ShakeCamera()
        {
            if (Camera.main.TryGetComponent(out CameraFlow shakeScript))
            {
                shakeScript.ForceShake(ShakeMagnitude, ShakeDuration);
            }
            
        }
    }
}

