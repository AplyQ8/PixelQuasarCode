using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


public abstract class ItemSO : ScriptableObject
{
    public int ID => GetInstanceID();

    #region Properties

    [field: SerializeField] public ItemTypeEnum ItemType { get; private set; }
    [field: SerializeField] public bool IsStackable { get; private set; }
    [field: SerializeField] public int MaxStack { get; private set; } = 1;
    [field: SerializeField] public LocalizedString Name { get; private set; }
    [field: SerializeField] public LocalizedString Description { get; private set; }
    [field: SerializeField] public Sprite ItemImage { get; private set; }
    
    #endregion

    public enum ItemTypeEnum
    {
        InventoryItem,
        ActiveAbilityItem,
        PassiveAbilityItem,
        TinderBox
    }
    
}
