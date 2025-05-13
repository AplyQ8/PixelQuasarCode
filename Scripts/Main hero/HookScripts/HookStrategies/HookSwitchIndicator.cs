using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

namespace Main_hero.HookScripts.HookStrategies
{
    public class HookSwitchIndicator : MonoBehaviour
    {
        [SerializeField] private HookBehaviourSwitcher _hookBehaviourSwitcher;

        [SerializeField] private Image indicator1;
        [SerializeField] private Image indicator2;
        
        
        [SerializedDictionary("Hook Behaviour", "Behaviour Indicator")] 
        public SerializedDictionary<HookBehaviour, BehaviourColors> hookBehaviourIndicators;
        
        
        private void Awake()
        {
            _hookBehaviourSwitcher.OnBehaviourSwitch += BehaviourSwitchEvent;
        }

        private void BehaviourSwitchEvent(HookBehaviour hookBehaviour)
        {
            indicator1.color = hookBehaviourIndicators[hookBehaviour].Color1;
            indicator2.color = hookBehaviourIndicators[hookBehaviour].Color2;
        }

        private void OnDestroy()
        {
            _hookBehaviourSwitcher.OnBehaviourSwitch -= BehaviourSwitchEvent;
        }
    }
    
    [Serializable]
    public class BehaviourColors
    {
        [field: SerializeField] public Color Color1 { get; private set; }
        [field: SerializeField] public Color Color2 { get; private set; }
    }
    
}
