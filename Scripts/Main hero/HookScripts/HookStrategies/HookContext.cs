using System;
using ObjectLogicRealization.Adrenaline;
using UnityEngine;

namespace Main_hero.HookScripts.HookStrategies
{
    [Serializable]
    public class HookContext
    {
        public Transform HookTransform;
        public Transform HookEndTransform;
        public Transform PlayerTransform;
        public LayerMask ObstacleMask;
        public Vector2 TargetPosition;
        public HookScript.HookState CurrentHookState;
        public Rigidbody2D hookRigidBody;
        public HookStrategyHandler HookStrategyHandler;
        public HeroAdrenaline Adrenaline;
        
        public bool ObjectCaught;
        public System.Action<float> OnHookHit;
        public System.Action OnHookMiss;
        public System.Action OnHookDisable;

        public void SetState(HookScript.HookState state) => CurrentHookState = state;
    }
}