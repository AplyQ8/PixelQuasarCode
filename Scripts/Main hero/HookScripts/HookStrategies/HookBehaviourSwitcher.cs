using System;
using System.Collections.Generic;
using System.Linq;
using ObjectLogicRealization.Adrenaline;
using UnityEngine;
using Utilities;

namespace Main_hero.HookScripts.HookStrategies
{
    [RequireComponent(typeof(HookStrategyHandler))]
    public class HookBehaviourSwitcher : MonoBehaviour
    {
        [SerializeField] private HeroAdrenaline heroAdrenaline;
        private HookStrategyHandler _hookStrategyHandler;

        [SerializeField] private HookBehaviour defaultHookBehaviour;
        [SerializeField] private List<HookBehaviour> availableHookBehaviours;
        
        private List<HookBehaviour> _nextPotentialHookBehaviours;
        private HookBehaviour _currentHookBehaviour;

        public event Action<HookBehaviour> OnBehaviourSwitch;

        private HookBehaviourSwitcherState _currentState;

        private enum HookBehaviourSwitcherState
        {
            Normal,
            Instable
        }

        private void Awake()
        {
            _hookStrategyHandler = GetComponent<HookStrategyHandler>();
            
            _nextPotentialHookBehaviours = new List<HookBehaviour>();
            SubscribeOnActionEvents();

            _currentHookBehaviour = defaultHookBehaviour;
           _hookStrategyHandler.SetHookStrategy(defaultHookBehaviour);
           OnBehaviourSwitch?.Invoke(_currentHookBehaviour);
        }

        private void HookHitEvent(HookBehaviour behaviour)
        {
            _hookStrategyHandler.OnHookDisable += HookDisableEvent;
        }

        private void CreateNextPotentialHookBehaviourList(HookBehaviour excepted)
        {
            _nextPotentialHookBehaviours.Clear();
            if (availableHookBehaviours.Count == 1)
            {
                _nextPotentialHookBehaviours.Add(availableHookBehaviours[0]);
                return;
            }
            foreach (var hookBehaviour in availableHookBehaviours.Where(hookBehaviour => hookBehaviour != excepted))
            {
                _nextPotentialHookBehaviours.Add(hookBehaviour);
            }
        }

        private void InstabilityEnterEvent()
        {
            _currentState = HookBehaviourSwitcherState.Instable;
            CreateNextPotentialHookBehaviourList(_currentHookBehaviour);
            _hookStrategyHandler.SetHookStrategy(RandomizeNextHookBehaviour());
            OnBehaviourSwitch?.Invoke(_currentHookBehaviour);
            
        }
        private void InstabilityExitEvent()
        {
            _currentState = HookBehaviourSwitcherState.Normal;
            _hookStrategyHandler.OnHookDisable -= HookDisableEvent;
            if (!_hookStrategyHandler.IsActive)
            {
                _hookStrategyHandler.SetHookStrategy(defaultHookBehaviour);
                _currentHookBehaviour = defaultHookBehaviour;
                OnBehaviourSwitch?.Invoke(_currentHookBehaviour);
                return;
            }
            _hookStrategyHandler.OnHookDisable += SetDefaultHookAfterReturn;
        }

        private HookBehaviour RandomizeNextHookBehaviour()
        {
            var newBehaviourIndex = RandomGenerator.Instance.RandomValueInRange(0, _nextPotentialHookBehaviours.Count);
            _currentHookBehaviour = _nextPotentialHookBehaviours[newBehaviourIndex];
            return _currentHookBehaviour;
        }
        private void HookDisableEvent()
        {
            if (_hookStrategyHandler.BehaviourWasForciblyChanged)
            {
                _hookStrategyHandler.SetHookStrategy(_currentHookBehaviour);
                _hookStrategyHandler.OnHookDisable -= HookDisableEvent;
                return;
            }
            
            if (_currentState == HookBehaviourSwitcherState.Normal) return;
            
            CreateNextPotentialHookBehaviourList(_currentHookBehaviour);
            _hookStrategyHandler.SetHookStrategy(RandomizeNextHookBehaviour());
            OnBehaviourSwitch?.Invoke(_currentHookBehaviour);
            _hookStrategyHandler.OnHookDisable -= HookDisableEvent;
        }
        private void SubscribeOnActionEvents()
        {
            _hookStrategyHandler.OnHookHit += HookHitEvent;
            heroAdrenaline.OnInstabilityEnter += InstabilityEnterEvent;
            heroAdrenaline.OnInstabilityExit += InstabilityExitEvent;
        }
        private void UnsubscribeFromActionEvents()
        {
            _hookStrategyHandler.OnHookHit -= HookHitEvent;
            heroAdrenaline.OnInstabilityEnter -= InstabilityEnterEvent;
            heroAdrenaline.OnInstabilityExit -= InstabilityExitEvent;
        }
        private void SetDefaultHookAfterReturn()
        {
            _hookStrategyHandler.SetHookStrategy(defaultHookBehaviour);
            _currentHookBehaviour = defaultHookBehaviour;
            OnBehaviourSwitch?.Invoke(_currentHookBehaviour);
            _hookStrategyHandler.OnHookDisable -= SetDefaultHookAfterReturn;
        }
        private void OnDestroy()
        {
            UnsubscribeFromActionEvents();
        }
    }
}
