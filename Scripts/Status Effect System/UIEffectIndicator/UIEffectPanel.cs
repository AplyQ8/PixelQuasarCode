using System.Collections;
using System.Collections.Generic;
using Status_Effect_System;
using UnityEngine;
using Utilities;

public class UIEffectPanel : MonoBehaviour
{
    [SerializeField] private UIEffectSlot effectSlotPrefab;
    [SerializeField] private ObjectPool slotPool;
    [SerializeField] private int sizeOfInitialPool;
    [SerializeField] private GameObject sortingPanel;
    [SerializeField] private Dictionary<int, UIEffectSlot> activeSlots;

    private void Awake()
    {
        activeSlots = new Dictionary<int, UIEffectSlot>();
    }

    public void InitializePanel(EffectHandler effectHandler)
    {
        effectHandler.AddEffectEvent += OnAddEffect;
        effectHandler.RemoveEffectEvent += OnRemoveEffect;
        effectHandler.RestartEffectEvent += OnRestartEffect;
        slotPool.InitializePool(effectSlotPrefab.name, sizeOfInitialPool, effectSlotPrefab.gameObject);
    }

    private void OnRestartEffect(int effectID, float duration)
    {
        if (!activeSlots.ContainsKey(effectID))
            return;
        activeSlots[effectID].RestartEffect(duration);
    }

    private void OnAddEffect(int effectID, Sprite icon, float duration, StatusEffectData.EffectType effectType)
    {
        if (activeSlots.ContainsKey(effectID))
        {
            activeSlots[effectID].Initialize(icon, duration, effectType);
            return;
        }
        var pooledObject = slotPool.SpawnFromPool(
            effectSlotPrefab.name,
            transform.position, Quaternion.identity
            );
        if (pooledObject is null)
            return;
        var slot = pooledObject.GetComponent<UIEffectSlot>();
        slot.Initialize(icon, duration, effectType);
        slot.transform.SetParent(sortingPanel.transform);
        activeSlots.Add(effectID, slot);
    }
    
    private void OnRemoveEffect(int effectID)
    {
        if (!activeSlots.ContainsKey(effectID))
            return;
        var slotToDeactivate = activeSlots[effectID];
        activeSlots.Remove(effectID);
        slotPool.AddToPool(effectSlotPrefab.name, slotToDeactivate.gameObject);
    }

    
}
