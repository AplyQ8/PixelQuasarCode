using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace PickableObjects.InventoryItems
{
    [CreateAssetMenu(menuName = "InventoryItems/EdibleItem")]
    public class EdibleItemSO : ItemSO, IItemAction, IDestroyableItem, IEquippable, IMultipleUsable, IDroppable
    {
        [SerializeField] private List<ModifierData> modifiersData = new List<ModifierData>();
        [SerializeField] private List<ModifierData> destroyEffects = new List<ModifierData>();
        [field: SerializeField] public bool CanBeEquipped { get; private set; } = true;
        [field: SerializeField] public bool CanBeUsed { get; private set; } = true;
        [field: SerializeField] public bool CantBeUsedFromInventory { get; private set; }
        [field: SerializeField] public LocalizedString ActionName { get; private set; }
        [field: SerializeField] public AudioClip ActionSfx { get; private set; }
        
        public virtual bool PerformAction(GameObject hero)
        {
            if (!CanBeUsed) return false;
            foreach (var modifierData in modifiersData)
            {
                modifierData.statModifier.AffectObject(hero, modifierData.value);
            }

            return true;
        }

        public void OnDestroyEvent(GameObject hero)
        {
            foreach (var destroyEffect in destroyEffects)
            {
                destroyEffect.statModifier.AffectObject(hero, destroyEffect.value);
            }
        }
    }

    public interface IDestroyableItem
    {
        public void OnDestroyEvent(GameObject hero);
    }

    public interface IItemAction
    {
        
        public bool CanBeUsed { get;} 
        public bool CantBeUsedFromInventory { get; }
        public LocalizedString ActionName { get; }
        public AudioClip ActionSfx { get; }
        bool PerformAction(GameObject hero);
    }

    public interface IEquippable
    {
        public bool CanBeEquipped { get;} 
    }

    public interface IMultipleUsable { }
    public interface IDroppable { }
}