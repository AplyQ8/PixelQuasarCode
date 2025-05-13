using System;
using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.Blood
{
    public class HeroBloodScript : MonoBehaviour, IBloodContent
    {
        [field: SerializeField] public float MaxBloodBoundary { get; private set; }
        [field: SerializeField] public float MinBloodBoundary { get; private set; } = 0;
        [field: SerializeField] public float CurrentBloodValue { get; private set; }
        [field: SerializeField] public float BloodConsumptionMultiplier { get; private set; } = 1f;

        public event Action<float>
            OnBloodValueChangeEvent,
            OnBloodBoundaryChange;


        private void Start()
        {
            CurrentBloodValue = MaxBloodBoundary;
            OnBloodBoundaryChange?.Invoke(MaxBloodBoundary);
            OnBloodValueChangeEvent?.Invoke(CurrentBloodValue);
        }

        public void AddBlood(float value)
        {
            CurrentBloodValue += value;
            if (CurrentBloodValue > MaxBloodBoundary)
                CurrentBloodValue = MaxBloodBoundary;
            OnBloodValueChangeEvent?.Invoke(CurrentBloodValue);
        }

        public void SubtractBlood(float value)
        {
            CurrentBloodValue -= value;
            if (CurrentBloodValue < MinBloodBoundary)
                CurrentBloodValue = MinBloodBoundary;
            OnBloodValueChangeEvent?.Invoke(CurrentBloodValue);
        }

        public void FulFill()
        {
            CurrentBloodValue = MaxBloodBoundary;
            OnBloodValueChangeEvent?.Invoke(CurrentBloodValue);
        }

        public void ExpandBloodCapacity(float value)
        {
            MaxBloodBoundary += value;
            OnBloodBoundaryChange?.Invoke(MaxBloodBoundary);
        }

        public void ReduceBloodCapacity(float value)
        {
            MaxBloodBoundary -= value;
            if (MaxBloodBoundary < MinBloodBoundary)
                MaxBloodBoundary = MinBloodBoundary;
            if (CurrentBloodValue > MaxBloodBoundary)
            {
                CurrentBloodValue = MaxBloodBoundary;
                OnBloodValueChangeEvent?.Invoke(CurrentBloodValue);
            }
            OnBloodBoundaryChange?.Invoke(MaxBloodBoundary);
        }

        public void IncreaseBloodConsumption(float value)
        {
            
        }

        public void ReduceBloodConsumption(float value)
        {
            
        }

        public float GetCurrentBloodValue() => CurrentBloodValue;
        public float GetMaxBloodValue() => MaxBloodBoundary;
    }
}