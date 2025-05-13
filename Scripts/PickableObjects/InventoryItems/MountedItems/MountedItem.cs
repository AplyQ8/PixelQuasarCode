using System;
using System.Collections.Generic;
using ItemsLogic.ItemsWithCastRange;
using UnityEngine;
using UnityEngine.Localization;

namespace PickableObjects.InventoryItems.MountedItems
{
    [CreateAssetMenu(menuName = "InventoryItems/MountedItems")]
    public class MountedItem : ItemSO, IItemAction, IDestroyableItem
    {
        [field: SerializeField] public bool CanBeUsed { get; private set; } = true;
        [field: SerializeField] public bool CantBeUsedFromInventory { get; private set; }
        [field: SerializeField] public LocalizedString ActionName { get; private set; }
        [field: SerializeField] public AudioClip ActionSfx { get; private set; }

        [SerializeField] private List<ModifierData> modifiers = new List<ModifierData>();

        [SerializeField] private MountItemInfo mountItemInfo;
        //Script reference 

        public bool PerformAction(GameObject hero)
        {
            var setItem = Instantiate(mountItemInfo.ObjectInitializer, hero.transform.position, Quaternion.identity);
            setItem.InitializeSettableItem(modifiers, mountItemInfo.Durability);
            return true;
        }

        public void OnDestroyEvent(GameObject hero)
        { }
    }
    
    [Serializable]
    public class MountItemInfo
    {
        [field: SerializeField] public SettableObjectInitializer ObjectInitializer { get; private set; }
        [field: SerializeField] public float Durability { get; private set; }
    }
}