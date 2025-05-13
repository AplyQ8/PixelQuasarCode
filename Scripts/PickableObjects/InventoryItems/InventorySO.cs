using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class InventorySO : ScriptableObject
{
    [SerializeField] private protected List<InventoryItem> inventoryItems;
    [SerializeField] public bool[] quickSlots;
    [field: SerializeField] public int Size { get; private set; } = 10;
    [field: SerializeField] public InventoryTypeEnum InventoryType { get; private set; }

    [SerializeField] private bool isInitialized = false;
    [SerializeField] protected bool autoQuickSlot;

    public enum InventoryTypeEnum
    {
        ItemInventory,
        PassiveAbilityInventory,
        ActiveAbilityInventory,
        QuestInventory
    }

    public event Action<Dictionary<int, InventoryItem>, InventoryTypeEnum> OnInventoryUpdated;
    public virtual void Initialize(bool reinitialize)
    {
        if (reinitialize)
        {
            inventoryItems = new List<InventoryItem>();
            for (int i = 0; i < Size; i++)
            {
                inventoryItems.Add(InventoryItem.GetEmptyItem());
            }
            QuickSlotData quickSlotData = FindObjectOfType<QuickSlotData>();
            quickSlots = quickSlotData != null ? new bool[quickSlotData.AvailableQuickSlots] : new bool[0];
        }
        autoQuickSlot = true;
        
    }

    public virtual void Initialize(int numberOfSlots)
    {
        if (isInitialized)
            return;
        isInitialized = true;
        inventoryItems = new List<InventoryItem>();
        QuickSlotData quickSlotData = FindObjectOfType<QuickSlotData>();
        quickSlots = quickSlotData != null ? new bool[quickSlotData.AvailableQuickSlots] : new bool[0];
        autoQuickSlot = false;
        for (int i = 0; i < numberOfSlots; i++)
        {
            inventoryItems.Add(InventoryItem.GetEmptyItem());
        }
    }

    #region Add Item Methods
    public virtual int AddItem(ItemSO item, int quantity)
    {
        if (!item.IsStackable)
        {
            while (quantity > 0 && !IsInventoryFull())
            {
                quantity -= AddItemToFirstFreeSlot(item, 1);
                    
            }
            InformAboutChange();
            return quantity;
        }
        quantity = AddStackableItem(item, quantity);
        InformAboutChange();
        return quantity;
    }

    public virtual void AddTinderBox()
    {
        
    }

    private int AddItemToFirstFreeSlot(ItemSO item, int quantity)
    {
        InventoryItem newItem = new InventoryItem(item, quantity);
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
            {
                inventoryItems[i] = newItem;
                return quantity;
            }
        }
        return 0;
    }

    private bool IsInventoryFull()
        => inventoryItems.Any(item => item.IsEmpty) == false;
    
    private int AddStackableItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if(inventoryItems[i].IsEmpty)
                continue;
            if (inventoryItems[i].item.ID.Equals(item.ID))
            {
                int amountPossibleToTake =
                    inventoryItems[i].item.MaxStack - inventoryItems[i].quantity;
                if (quantity > amountPossibleToTake)
                {
                    inventoryItems[i] = inventoryItems[i]
                        .ChangeQuantity(inventoryItems[i].item.MaxStack);
                    quantity -= amountPossibleToTake;
                }
                else
                {
                    inventoryItems[i] = inventoryItems[i]
                        .ChangeQuantity(inventoryItems[i].quantity + quantity);
                    InformAboutChange();
                    return 0;
                }
            }
        }

        while (quantity > 0 && !IsInventoryFull())
        {
            int newQuantity = Math.Clamp(quantity, 0, item.MaxStack);
            quantity -= newQuantity;
            AddItemToFirstFreeSlot(item, newQuantity);
        }

        return quantity;
    }

    public void AddItem(InventoryItem item)
    {
        AddItem(item.item, item.quantity);
    }
    #endregion

    public Dictionary<int, InventoryItem> GetCurrentInventoryState()
    {
        Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if(inventoryItems[i].IsEmpty)
                continue;
            returnValue[i] = inventoryItems[i];
        }

        return returnValue;
    }

    public InventoryItem GetItemAt(int itemIndex)
    {
        return inventoryItems[itemIndex];
    }

    public void SetItemAt(int index, InventoryItem item)
    {
        inventoryItems[index] = item;
        InformAboutChange();
    }

    public void SwapItems(int itemIndex1, int itemIndex2)
    {
        if (itemIndex1 >= inventoryItems.Count || itemIndex2 >= inventoryItems.Count)
            return;
        try
        {
            if ((inventoryItems[itemIndex1].item.ID.Equals(inventoryItems[itemIndex2].item.ID))
                && inventoryItems[itemIndex1].item.IsStackable)
            {
                StackItems(itemIndex1, itemIndex2);
                InformAboutChange();
                return;
            }
        }
        catch (ArgumentOutOfRangeException) { }
        catch(NullReferenceException){ }
        try
        {
            (inventoryItems[itemIndex1], inventoryItems[itemIndex2])
                = (inventoryItems[itemIndex2], inventoryItems[itemIndex1]);
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.Log($"{e}: tries to swap itemIndex_{itemIndex1} with itemIndex_{itemIndex2}");
            return;
        }
        InformAboutChange();
    }

    

    private protected virtual void InformAboutChange()
    {
        OnInventoryUpdated?.Invoke(GetCurrentInventoryState(), InventoryType);
    }

    public virtual void RemoveItem(int itemIndex, int amount)
    {
        if (inventoryItems.Count <= itemIndex) 
            return;
        if (inventoryItems[itemIndex].IsEmpty)
            return;
        var reminder = inventoryItems[itemIndex].quantity - amount;
        if (reminder <= 0)
        {
            inventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
        }
        else
        {
            inventoryItems[itemIndex] = inventoryItems[itemIndex].ChangeQuantity(reminder);
        }
        InformAboutChange();
    }

    public virtual bool RemoveItemByItemSo(ItemSO itemSo)
    {
        foreach (var inventoryItem in inventoryItems)
        {
            if(inventoryItem.item != itemSo) continue;
            int index = inventoryItems.IndexOf(inventoryItem);
            RemoveItem(index, inventoryItem.quantity);
            return true;
        }

        return false;
    }
    public virtual bool RemoveItemByItemSo(ItemSO itemSo, int amount)
    {
        foreach (var inventoryItem in inventoryItems)
        {
            if(inventoryItem.item != itemSo) continue;
            int index = inventoryItems.IndexOf(inventoryItem);
            RemoveItem(index, amount);
            return true;
        }
        return false;
    }

    public bool HasItem(ItemSO itemSo)
    {
        foreach (var inventoryItem in inventoryItems)
        {
            if(inventoryItem.item != itemSo) continue;
            return true;
        }
        return false;
    }

    private void StackItems(int itemIndex1, int itemIndex2)
    {
        int possibleAmountToTake = inventoryItems[itemIndex1].item.MaxStack - inventoryItems[itemIndex1].quantity;
        if (inventoryItems[itemIndex2].quantity > possibleAmountToTake)
        {
            inventoryItems[itemIndex1] = 
                inventoryItems[itemIndex1].ChangeQuantity(inventoryItems[itemIndex1].item.MaxStack);
            inventoryItems[itemIndex2] = 
                inventoryItems[itemIndex2].ChangeQuantity(inventoryItems[itemIndex2].quantity - possibleAmountToTake);
        }
        else
        {
            inventoryItems[itemIndex1] =inventoryItems[itemIndex1]
                .ChangeQuantity(inventoryItems[itemIndex1].quantity + inventoryItems[itemIndex2].quantity);
            RemoveItem(itemIndex2, inventoryItems[itemIndex2].quantity);
        }
    }

    public int FindItemIndex(ItemSO item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if(inventoryItems[i].item is null)
                continue;
            if (inventoryItems[i].item.Equals(item))
            {
                return i;
            }
        }
        return -1;
    }

    public int GetFreeQuickSlot()
    {
        for (int i = 0; i < quickSlots.Length; i++)
        {
            if (!quickSlots[i])
                return i;
        }
        return -1;
    }

    public bool QuickSlotsFull() => quickSlots.All(value => value);
    

    public virtual void OnLocalizationChange() => InformAboutChange();
}

[Serializable]
public struct InventoryItem
{
    public int quantity;
    public ItemSO item;
    public int quickSlotNumber;
    public bool IsEmpty => item == null;
    
    /// <summary>
    /// Default item initialization
    /// </summary>
    /// <param name="item"></param>
    /// <param name="quantity"></param>
    public InventoryItem(ItemSO item, int quantity)
    {
        this.quantity = quantity;
        this.item = item;
        this.quickSlotNumber = -1;
    }
    /// <summary>
    /// Initialization with a quick slot number
    /// </summary>
    /// <param name="item"></param>
    /// <param name="quantity"></param>
    /// <param name="quickSlotNumber"></param>
    public InventoryItem(ItemSO item, int quantity, int quickSlotNumber)
    {
        this.quantity = quantity;
        this.item = item;
        this.quickSlotNumber = quickSlotNumber;
    }

    public InventoryItem ChangeQuantity(int newQuantity)
        => new InventoryItem(this.item, newQuantity, this.quickSlotNumber);
    public static InventoryItem GetEmptyItem()
        => new InventoryItem(null, 0);
    public InventoryItem GetEmptyItemButLeaveQuickSlotNumber()
        => new InventoryItem(null, 0, this.quickSlotNumber);
    public InventoryItem SetQuickSlotNumber(int qSNumber)
        => new InventoryItem(this.item, this.quantity, qSNumber);
    public InventoryItem ResetQuickSlotNumber()
        => new InventoryItem(this.item, this.quantity);

}