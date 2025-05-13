using System;
using System.Linq;
using UnityEngine;

namespace PickableObjects.InventoryItems
{
    [CreateAssetMenu]
    public class ItemInventorySO : InventorySO
    {
        //Добавляем TinderBox и подписываемся на его эвенты. 
        //TinderBox - scriptableObject и мы можем с ним взаимодействовать напрямую
        [SerializeField] private TinderBoxSo tinderBox;
    
        public event Action<int>
            OnDeleteItemFromQuickSlot;

        public event Action<int, int> OnInformOfItemDropInQuickSlot;

        public event Action<TinderBoxInfo> OnTinderBoxStateChange;
        public event Action<TinderBoxSo> OnTinderBoxInitialization;
        public void SetItemToQuickSlot(int quickSlotNumber, int itemIndex)
        {
            if (itemIndex >= inventoryItems.Count || itemIndex < 0)
                return;
            // if (inventoryItems[itemIndex].quickSlotNumber != -1)
            //     return;
            // inventoryItems[itemIndex] = inventoryItems[itemIndex].SetQuickSlotNumber(quickSlotNumber);
            // quickSlots[quickSlotNumber] = true;
            // InformAboutQuickSlotChange(quickSlotNumber, itemIndex);
            // if (inventoryItems[itemIndex].quickSlotNumber != -1)
            // {
            //     //Взять предмет по quickSlotNumber
            //     //Удалить его из quick slot 
            //     //Поставить текущий предмет в quickSlot
            //     //Inform
            //     //var qsItem = GetItemByQuickSlotIndex(quickSlotNumber);
            //     DeleteItemFromQuickSlot(quickSlotNumber);
            //     inventoryItems[itemIndex] = inventoryItems[itemIndex].SetQuickSlotNumber(quickSlotNumber);
            //     quickSlots[quickSlotNumber] = true;
            //     InformAboutQuickSlotChange(quickSlotNumber, itemIndex);
            //     return;
            // }
            if (inventoryItems[itemIndex].quickSlotNumber != -1)
            {
                int qsChangeIndex = inventoryItems[itemIndex].quickSlotNumber;
                DeleteItemFromQuickSlot(qsChangeIndex);
                //InformAboutQuickSlotChange(qsChangeIndex, itemIndex);
            }
            DeleteItemFromQuickSlot(quickSlotNumber);
            inventoryItems[itemIndex] = inventoryItems[itemIndex].SetQuickSlotNumber(quickSlotNumber);
            quickSlots[quickSlotNumber] = true;
            InformAboutQuickSlotChange(quickSlotNumber, itemIndex);
        }

        public void SetItemToQuickSlotOnDoubleClick(int itemIndex)
        {
            if (itemIndex >= inventoryItems.Count || itemIndex < 0)
                return;
            if (inventoryItems[itemIndex].quickSlotNumber != -1)
                return;
            if (QuickSlotsFull()) return;
            int quickSlotIndex = GetFreeQuickSlot();
            inventoryItems[itemIndex] = inventoryItems[itemIndex].SetQuickSlotNumber(quickSlotIndex);
            quickSlots[quickSlotIndex] = true;
            InformAboutQuickSlotChange(quickSlotIndex, itemIndex);
        }

        public override int AddItem(ItemSO item, int quantity)
        {
            int res = base.AddItem(item, quantity);
            if (base.autoQuickSlot)
            {
                int index = FindItemIndex(item);
                int quickSlotIndex = GetFreeQuickSlot();
                if (index != -1 && quickSlotIndex != -1)
                {
                    SetItemToQuickSlot(quickSlotIndex, index);
                }
            }
            return res;
        }

        public override void AddTinderBox()
        {
            tinderBox.OnStateChange += TinderBoxStateChangeEvent;
            OnTinderBoxInitialization?.Invoke(tinderBox);
            tinderBox.Initialize();
        }

        private void TinderBoxStateChangeEvent(TinderBoxInfo info)
        {
            OnTinderBoxStateChange?.Invoke(info);
        }

        private void InformAboutQuickSlotChange(int quickSlotNumber, int itemIndex)
        {
            OnInformOfItemDropInQuickSlot?.Invoke(quickSlotNumber, itemIndex);
        }
        public void SwapESItems(int qsDropInIndex, int qsDraggedIndex)
        {
            if (qsDropInIndex >= quickSlots.Length || qsDraggedIndex >= quickSlots.Length)
                return;
            var item1 = GetItemByQuickSlotIndex(qsDropInIndex);
            var item2 = GetItemByQuickSlotIndex(qsDraggedIndex);
            if (item1.IsEmpty)
            {
                //item2.SetQuickSlotNumber(qsDropInIndex);
                SetItemToQuickSlot(qsDropInIndex, FindItemIndex(item2.item));
                DeleteItemFromQuickSlot(qsDraggedIndex);
                return;
            }
            DeleteItemFromQuickSlot(qsDraggedIndex);
            DeleteItemFromQuickSlot(qsDropInIndex);
            SetItemToQuickSlot(qsDropInIndex, FindItemIndex(item2.item));
            SetItemToQuickSlot(qsDraggedIndex, FindItemIndex(item1.item));

        }
        public override void RemoveItem(int itemIndex, int amount)
        {
            if (inventoryItems.Count <= itemIndex) 
                return;
            if (inventoryItems[itemIndex].IsEmpty)
                return;
            var reminder = inventoryItems[itemIndex].quantity - amount;
            if (reminder <= 0)
            {
                if (inventoryItems[itemIndex].quickSlotNumber == -1)
                {
                    inventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
                }
                else
                {
                    OnDeleteItemFromQuickSlot?.Invoke(inventoryItems[itemIndex].quickSlotNumber);
                    InventoryItem item = inventoryItems[itemIndex];
                    quickSlots[inventoryItems[itemIndex].quickSlotNumber] = false;
                    inventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
                    ReplaceItemWithTheSameInQuickSlot(item, item.quickSlotNumber);
                }
                
            }
            else
            {
                inventoryItems[itemIndex] = inventoryItems[itemIndex].ChangeQuantity(reminder);
            }
            InformAboutChange();
        
        }

        public InventoryItem GetItemByQuickSlotIndex(int quickSlotIndex)
        {
            foreach (var inventoryItem in inventoryItems)
            {
                if (inventoryItem.quickSlotNumber == quickSlotIndex)
                    return inventoryItem;
            }
            return InventoryItem.GetEmptyItem();
        }
    

        public void RemoveItem(InventoryItem inventoryItem, int amount)
        {
            RemoveItem(inventoryItems.IndexOf(inventoryItem), amount);
        }

        

        public void DeleteItemFromQuickSlot(int quickSlotIndex)
        {
            InventoryItem itemToDeleteFromQuickSlot = GetItemByQuickSlotIndex(quickSlotIndex);
            if (itemToDeleteFromQuickSlot.IsEmpty)
                return;
            inventoryItems[inventoryItems.IndexOf(itemToDeleteFromQuickSlot)] =
                inventoryItems[inventoryItems.IndexOf(itemToDeleteFromQuickSlot)].ResetQuickSlotNumber();
            OnDeleteItemFromQuickSlot?.Invoke(quickSlotIndex);
            quickSlots[quickSlotIndex] = false;
        }

        private void ReplaceItemWithTheSameInQuickSlot(InventoryItem deletedItem, int quickSlotIndex)
        {
            //Find in inventoryItems the same item by ID, 
            //Check if it is in quick slot
            //Yes -> continue
            //No -> set quickSlotIndex to found item
            foreach (var inventoryItem in from inventoryItem 
                         in inventoryItems 
                     where !inventoryItem.IsEmpty 
                     where inventoryItem.item.ID.Equals(deletedItem.item.ID) 
                     where inventoryItem.quickSlotNumber != 1 
                     select inventoryItem)
            {
                SetItemToQuickSlot(quickSlotIndex, inventoryItems.IndexOf(inventoryItem));
                return;
            }
        }
    }
}


