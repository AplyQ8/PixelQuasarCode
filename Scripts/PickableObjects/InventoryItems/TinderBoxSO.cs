using System;
using UnityEngine;
using UnityEngine.Localization;

namespace PickableObjects.InventoryItems
{
    [CreateAssetMenu(menuName = "InventoryItems/Tinder-Box")]
    public class TinderBoxSo: ItemSO, IDestroyableItem
    {
        [field: SerializeField] public float MaxLoad { get; private set; } = 3;
        

        public float CurrentLoad
        {
            get => currentLoad;
            private set
            {
                currentLoad = (float)Math.Round(Mathf.Min(value, MaxLoad), 1);
                OnStateChange?.Invoke(new TinderBoxInfo(CurrentLoad, MaxLoad));
            }
        } 

        [SerializeField] private float currentLoad;

        [field: SerializeField] private AudioClip ActionSfx { get; set; }
        public bool IsInitialized { get; private set; } = true;

        public event Action<TinderBoxInfo> OnStateChange; 

        public bool Add(float charge)
        {
            if (!IsInitialized)
                return false;
            if (Math.Abs(CurrentLoad - MaxLoad) < 0.01)
                return false;

            CurrentLoad += charge;
            OnStateChange?.Invoke(new TinderBoxInfo(CurrentLoad, MaxLoad));
            
            return true;
        }

        public void ExtendMaxLoad(float value)
        {
            MaxLoad += value;
            OnStateChange?.Invoke(new TinderBoxInfo(CurrentLoad, MaxLoad));
        }
        
        public bool Save(float charge)
        {
            if (CurrentLoad < charge)
                return false;
            CurrentLoad -= charge;
            return true;
        }

        public void Initialize()
        {
            CurrentLoad = CurrentLoad;
            IsInitialized = true;
        }

        public void UpdateState()
        {
            OnStateChange?.Invoke(new TinderBoxInfo(CurrentLoad, MaxLoad));
        }

        public LocalizedString DestroyableActionName { get; }
        public void OnDestroyEvent(GameObject hero)
        { }
    }

    public struct TinderBoxInfo
    {
        public float MaxLoad { get; private set; }
        public float CurrentLoad { get; private set; }

        public TinderBoxInfo(float currentLoad, float maxLoad)
        {
            MaxLoad = maxLoad;
            CurrentLoad = currentLoad;
        }
    }
}