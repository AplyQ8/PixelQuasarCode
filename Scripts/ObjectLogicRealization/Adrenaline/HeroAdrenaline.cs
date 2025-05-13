using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main_hero.HookScripts.HookStrategies;
using Main_hero.State_Machine;
using ObjectLogicInterfaces;
using ObjectLogicRealization.Attack;
using UnityEngine;

namespace ObjectLogicRealization.Adrenaline
{
    [RequireComponent(typeof(ICanAttack))]
    public class HeroAdrenaline : MonoBehaviour, IAdrenalineContent
    {
        public event Action<float>
            OnAdrenalineValueChange,
            OnAdrenalineBoundaryChange;

        public event Action OnAdrenalineLowestBoundary, OnAdrenalineHighestBoundary;

        public event Action OnInstabilityEnter, OnInstabilityExit;

        private event Action OnOverTimeReductionCoroutineEnd;

        [field: Header("Adrenaline related properties")]
        [field: SerializeField] public float MaxAdrenalineBoundary { get; private set; }
        [field: SerializeField] public float MinAdrenalineBoundary { get; private set; } = 0;
        [field: SerializeField] private float currentAdrenalineValue;
        public float CurrentAdrenalineValue
        {
            get => currentAdrenalineValue;
            private set
            {
                currentAdrenalineValue = Mathf.Clamp(value, MinAdrenalineBoundary, MaxAdrenalineBoundary);

                if (Math.Abs(currentAdrenalineValue - MinAdrenalineBoundary) < 0.1)
                {
                    OnAdrenalineLowestBoundary?.Invoke();
                }
                else if (Math.Abs(currentAdrenalineValue - MaxAdrenalineBoundary) < 0.1)
                {
                    OnAdrenalineHighestBoundary?.Invoke();
                }
            }
        }
        [field: SerializeField] public float AdrenalineReductionPerSecond { get; private set; }
        [SerializeField] private List<AdrenalineModifier> adrenalineModifiers = new List<AdrenalineModifier>();
        
        [field: Header("Instability related properties")]
        [field: SerializeField] public float AdrenalineReductionAfterInstability { get; private set; }
        [field: SerializeField] public float InstabilityDuration { get; private set; }

        [field: Header("Adrenaline state")]
        [field: SerializeField]
        public AdrenalineState CurrentState { get; private set; }

        private IEnumerator _adrenalineReductionCoroutine;
        private IEnumerator _instabilityCoroutine;
        private AttackingState _attackState;
        private SuperAttackState _superAttackState;

        public enum AdrenalineState
        {
            Normal,
            Instability,
            AfterInstability
        }
        private void Start()
        {
            _attackState = GetComponent<HeroStateHandler>().AttackingState;
            SubscribeOnActionEvents();
            GetComponent<HeroStateHandler>().DashState.OnDash += DashUseEvent;
            OnAdrenalineBoundaryChange?.Invoke(MaxAdrenalineBoundary);
            OnAdrenalineValueChange?.Invoke(CurrentAdrenalineValue);
            OnOverTimeReductionCoroutineEnd += SwitchToNormal;
            SwitchState(AdrenalineState.Normal);
            
        }

        private void SwitchState(AdrenalineState adrenalineState)
        {
            switch (adrenalineState)
            {
                case AdrenalineState.Normal:
                    TriggerHighestAdrenalineBoundary();
                    StopAdrenalineReductionCoroutine();
                    StartNormalReductionCoroutine();
                    break;
                case AdrenalineState.Instability:
                    UnsubscribeFromActionEvents();
                    StopTriggeringHighestAdrenalineBoundary();
                    StopAdrenalineReductionCoroutine();
                    OnInstabilityEnter?.Invoke();
                    StartInstabilityCoroutine();
                    break;
                case AdrenalineState.AfterInstability:
                    SubscribeOnActionEvents();
                    StopInstabilityCoroutine();
                    OnInstabilityExit?.Invoke();
                    StartInstabilityReductionCoroutine();
                    break;
            }

            CurrentState = adrenalineState;
        }

        #region Event Methods

        private void AttackHitEvent()
        {
            ChangeAdrenaline(AdrenalineModificatorManager.AdrenalineModificator.ReversedAttackLogic);
        }
        private void DashUseEvent()
        {
            ChangeAdrenaline(AdrenalineModificatorManager.AdrenalineModificator.Dash);
        }

        private void SuperAttackEvent()
        {
            ChangeAdrenaline(AdrenalineModificatorManager.AdrenalineModificator.SuperAttack);
        }

        #endregion
        
        #region Adrenaline Change Methods
        
        public void ChangeAdrenaline(AdrenalineModificatorManager.AdrenalineModificator adrenalineModificator)
        {
            var modifier = GetModifierByModificatorType(adrenalineModificator);
            if (modifier is null)
                return;
            switch (modifier.ValueType)
            {
                case AdrenalineModifier.ValueTypeEnum.Add:
                    AddAdrenaline(modifier.AdrenalineValue);
                    break;
                case AdrenalineModifier.ValueTypeEnum.Subtract:
                    SubtractAdrenaline(modifier.AdrenalineValue);
                    break;
                default:
                    return;
            }
            OnAdrenalineValueChange?.Invoke(CurrentAdrenalineValue);
        }
        
        public float GetAdrenalineRatio() => CurrentAdrenalineValue / MaxAdrenalineBoundary;

        private void SubtractAdrenaline(float value)
        {
            CurrentAdrenalineValue = Mathf.Max(0, CurrentAdrenalineValue - value);
            OnAdrenalineValueChange?.Invoke(CurrentAdrenalineValue);
            // if(CurrentAdrenalineValue == 0)
            //     OnAdrenalineLowestBoundary?.Invoke();
            // if (Math.Abs(CurrentAdrenalineValue - MaxAdrenalineBoundary) < 0.1)
            //     OnAdrenalineHighestBoundary?.Invoke();
        }

        private void AddAdrenaline(float value)
        {
            CurrentAdrenalineValue += value;
            if (CurrentAdrenalineValue > MaxAdrenalineBoundary)
                CurrentAdrenalineValue = MaxAdrenalineBoundary;
            OnAdrenalineValueChange?.Invoke(CurrentAdrenalineValue);
        }
        
        public void ExpandAdrenalineBoundary(float value)
        {
            MaxAdrenalineBoundary += value;
            OnAdrenalineBoundaryChange?.Invoke(MaxAdrenalineBoundary);
        }

        public void DecreaseAdrenalineBoundary(float value)
        {
            MaxAdrenalineBoundary -= value;
            if (MaxAdrenalineBoundary < MinAdrenalineBoundary)
                MaxAdrenalineBoundary = MinAdrenalineBoundary;
            if (CurrentAdrenalineValue > MaxAdrenalineBoundary)
            {
                CurrentAdrenalineValue = MaxAdrenalineBoundary;
                OnAdrenalineValueChange?.Invoke(CurrentAdrenalineValue);
            }
            OnAdrenalineBoundaryChange?.Invoke(MaxAdrenalineBoundary);
        }

        public void IncreaseAdrenalineReduction(float value)
        {
            AdrenalineReductionPerSecond += value;
        }

        public void DecreaseAdrenalineReduction(float value)
        {
            AdrenalineReductionPerSecond -= value;
            if (AdrenalineReductionPerSecond < 0)
                AdrenalineReductionPerSecond = 0;
        }
        public float GetCurrentAdrenalineValue() => CurrentAdrenalineValue;
        public float GetMaxAdrenalineBoundary() => MaxAdrenalineBoundary;

        #endregion
        
        private IEnumerator ReduceAdrenalineOverTime(float adrenalineReduction)
        {
            while (true)
            {
                float reductionPerFrame = adrenalineReduction * Time.deltaTime;
                SubtractAdrenaline(reductionPerFrame);
                OnAdrenalineValueChange?.Invoke(CurrentAdrenalineValue);
                yield return null;
            }
        }
        
        private IEnumerator ReduceAdrenalineOverTime(float adrenalineReduction, float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float reductionPerFrame = adrenalineReduction * Time.deltaTime;
                SubtractAdrenaline(reductionPerFrame);
                OnAdrenalineValueChange?.Invoke(CurrentAdrenalineValue);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            OnOverTimeReductionCoroutineEnd?.Invoke();
            
        }

        #region Event Subscription Methods

        private void SubscribeOnActionEvents()
        {
            _attackState.OnAttack += AttackHitEvent;
            //_superAttackState.OnAttackExecuted += SuperAttackEvent;
        }

        private void UnsubscribeFromActionEvents()
        {
            _attackState.OnAttack -= AttackHitEvent;
            //_superAttackState.OnAttackExecuted -= SuperAttackEvent;
        }

        private void TriggerHighestAdrenalineBoundary()
        {
            OnAdrenalineHighestBoundary += SwitchToInstability;
        }

        private void StopTriggeringHighestAdrenalineBoundary()
        {
            OnAdrenalineHighestBoundary -= SwitchToInstability;
        }

        #endregion

        #region Instability Methods
        private void StopAdrenalineReductionCoroutine()
        {
            if (_adrenalineReductionCoroutine is null) return;
            StopCoroutine(_adrenalineReductionCoroutine);
        }
        private void StartInstabilityReductionCoroutine()
        {
            _adrenalineReductionCoroutine = 
                ReduceAdrenalineOverTime(AdrenalineReductionAfterInstability, MaxAdrenalineBoundary/AdrenalineReductionAfterInstability);
            StartCoroutine(_adrenalineReductionCoroutine);
        }
        private void StartNormalReductionCoroutine()
        {
            _adrenalineReductionCoroutine = ReduceAdrenalineOverTime(AdrenalineReductionPerSecond);
            StartCoroutine(_adrenalineReductionCoroutine);
        }
        private void AdrenalineReachesLowestBoundaryAfterInstabilityEvent()
        {
            OnAdrenalineLowestBoundary -= AdrenalineReachesLowestBoundaryAfterInstabilityEvent;
            SwitchState(AdrenalineState.Normal);
        }
        private IEnumerator InstabilityCoroutine()
        {
            yield return new WaitForSeconds(InstabilityDuration);
            SwitchState(AdrenalineState.AfterInstability);
        }
        private void StartInstabilityCoroutine()
        {
            _instabilityCoroutine = InstabilityCoroutine();
            StartCoroutine(_instabilityCoroutine);
        }
        private void StopInstabilityCoroutine()
        {
            StopCoroutine(_instabilityCoroutine);
            _instabilityCoroutine = null;
        }
        private void SwitchToInstability() => SwitchState(AdrenalineState.Instability);
        private void SwitchToNormal() => SwitchState(AdrenalineState.Normal);

        #endregion

        #region Helper methods

        private AdrenalineModifier GetModifierByModificatorType(AdrenalineModificatorManager.AdrenalineModificator adrenalineModificator)
        {
            return adrenalineModifiers.FirstOrDefault(
                adrenalineModifier => adrenalineModifier.AdrenalineModificator.Equals(adrenalineModificator));
            
        }
        public bool IsAdrenalineEnough(AdrenalineModificatorManager.AdrenalineModificator adrenalineModificator)
        {
            var modifier = GetModifierByModificatorType(adrenalineModificator);
            if (modifier.ValueType is AdrenalineModifier.ValueTypeEnum.Add) return true;
            return CurrentAdrenalineValue >= modifier.AdrenalineValue;
        }

        #endregion

        private void OnDisable()
        {
            OnOverTimeReductionCoroutineEnd -= SwitchToNormal;
            UnsubscribeFromActionEvents();
            StopTriggeringHighestAdrenalineBoundary();
        }
    }

    [Serializable]
    public class AdrenalineModifier
    {
        [field: SerializeField] public float AdrenalineValue { get; private set; }
        [field: SerializeField] public AdrenalineModificatorManager.AdrenalineModificator AdrenalineModificator 
        { get; private set; }
        [field: SerializeField] public ValueTypeEnum ValueType { get; private set; }

        public enum ValueTypeEnum
        {
            Add,
            Subtract
        }
    }
}