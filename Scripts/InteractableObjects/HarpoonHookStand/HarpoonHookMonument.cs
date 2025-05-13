using System;
using Main_hero.HookScripts.HookStrategies;
using ObjectLogicInterfaces;
using UnityEngine;

namespace InteractableObjects.HarpoonHookStand
{
    public class HarpoonHookMonument : MonoBehaviour, IHookable, IForceHookBehaviourChanger
    {
        [field: SerializeField] public HookBehaviour HookBehaviour { get; set; }
        public float PulledByHook(Transform Hook, Vector3 pullBeginning, int damage, HookStrategyHandler hookStrategyHandler)
        {
            return Single.Epsilon;
        }
        public void EndPulledByHook()
        { }
        public bool IsIntangible() => false;
        
        public void SetHookBehaviour(HookStrategyHandler hookStrategyHandler)
        { }
        
    }
}
