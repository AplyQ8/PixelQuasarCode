using System;
using System.Collections;
using System.Collections.Generic;
using PickableObjects.PickUpSystem;
using UnityEditor;
using UnityEngine;
public class PickUpSystem : MonoBehaviour
{
    [field: SerializeField] public InventorySO ItemInventoryData { get; private set; }
    [field: SerializeField] public InventorySO ActiveAbilityInventoryData { get; private set; }
    [field: SerializeField] public InventorySO PassiveAbilityInventoryData { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out PickableItem item)) return;
        if (item == null) 
            return;
        switch (item.GetItemType)
        {
            case ItemSO.ItemTypeEnum.InventoryItem:
                AddItemToInventory(ItemInventoryData, item);
                return;
            case ItemSO.ItemTypeEnum.ActiveAbilityItem:
                AddItemToInventory(ActiveAbilityInventoryData, item);
                return;
            case ItemSO.ItemTypeEnum.PassiveAbilityItem:
                AddItemToInventory(PassiveAbilityInventoryData, item);
                return;
            case ItemSO.ItemTypeEnum.TinderBox:
                AddTinderBox(item);
                break;
        }
    }

    private void AddItemToInventory(InventorySO inventory, PickableItem item)
    {
        int reminder = inventory.AddItem(item.InventoryItem, item.Quantity);
        if (reminder is 0)
            item.DestroyItem();
        else
            item.Quantity = reminder;
    }
    
    private void AddTinderBox(PickableItem item)
    {
        ItemInventoryData.AddTinderBox();
        item.DestroyItem();
        
    }

    public int PickItemFromBag(ItemSO item, int quantity, InventorySO.InventoryTypeEnum inventoryType)
    {
        switch (inventoryType)
        {
            case InventorySO.InventoryTypeEnum.ItemInventory:
                return ItemInventoryData.AddItem(item, quantity);
            case InventorySO.InventoryTypeEnum.PassiveAbilityInventory:
                return PassiveAbilityInventoryData.AddItem(item, quantity);
            case InventorySO.InventoryTypeEnum.ActiveAbilityInventory:
                return ActiveAbilityInventoryData.AddItem(item, quantity);
            default:
                return -1;
        }
    }
    
}
