using System;
using System.Collections.Generic;
using ItemDrop;
using PickableObjects.InventoryItems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace In_Game_Menu_Scripts.InventoryScripts
{
    public class HeroInventory_Item : HeroInventory
    {
        [Header("Quick Slot Data")] [SerializeField]
        private QuickSlotData quickSlotData;
    
        [Header("Item quick slot equipment panel ")]
        [SerializeField] private UIQuickSlotEquipmentPanel currentEquipmentPanel;
    
        [Header("Overrode UIInventoryPage")]
        [SerializeField] private UIItemInventoryPage itemInventoryPageUI;

        [Header("Overrode InventorySO")] [SerializeField]
        private ItemInventorySO itemInventorySo;

        [SerializeField] private TinderBoxSo tinderBox;
    
        [Header("Drop containers")]
        [SerializeField] private LootBagScript lootBag;
        [SerializeField] private float lootBagSearchRadius = 3;

        [Header("AudioClips")] [SerializeField]
        private AudioClip dropClip;
    
        [Header("Specific action names")]
        [SerializeField] private LocalizedString equipAction;
        [SerializeField] private LocalizedString dropAction;
        [SerializeField] private LocalizedString unEquipAction;
        [SerializeField] private LocalizedString itemMultipleUsage;
        [SerializeField] private LocalizedString multipleUsage;
        [SerializeField] private LocalizedString multipleDrop;
        [SerializeField] private LocalizedString multipleEquip;
        [SerializeField] private LocalizedString multipleUnEquip;

        [SerializeField] private InputActionReference shiftAction;

        [SerializeField] private bool _shiftIsPressed;
        private List<int> _selectedItems = new List<int>();
        private List<int> _selectedEquipmentSlots = new List<int>();

        protected override void OnEnable()
        {
            base.OnEnable();
            LocalizationSettings.SelectedLocaleChanged += OnLocalizationChange;
            _shiftIsPressed = false;
            SubscribeOnInputActions();
        
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            LocalizationSettings.SelectedLocaleChanged -= OnLocalizationChange;
            UnsubscribeFromInputActions();
        }
    
        public void AssignNewUIInventories(UIItemInventoryPage itemInventoryPage, UIQuickSlotEquipmentPanel quickSlotPanel)
        {
            base.AssignNewUIInventories(itemInventoryPage);
            itemInventoryPageUI = itemInventoryPage;
            itemInventoryPageUI.OnItemDropInQuickSlot += OnDropItemInQuickSlot;
            itemInventoryPageUI.OnItemSetInQuickSlotOnDoubleClick += OnSetItemToQuickSlotOnDoubleClick;
            itemInventoryPageUI.OnItemUsageEvent += ItemUsageEvent;
            itemInventoryPageUI.OnItemDeleteFromQuickSlot += DeleteItemFromQuickSlotRequest;
            itemInventoryPage.OnEquipmentSlotActionRequest += HandleEquipmentSlotActionRequest;
            itemInventoryPage.OnItemSelection += ItemSelectionEvent;
            itemInventoryPageUI.OnEquipmentSlotSelection += HandleEquipmentSlotSelection;
            itemInventoryPageUI.OnEquipmentSlotStartDragging += HandleEquipmentSlotStartDragging;
            itemInventoryPageUI.OnItemESSwap += HandleItemESSwap;
            itemInventoryPageUI.OnESSwap += HandleESSwap;
            currentEquipmentPanel = quickSlotPanel;
            itemInventorySo.OnInformOfItemDropInQuickSlot += SetItemToQuickSlot;
            itemInventorySo.OnDeleteItemFromQuickSlot += DeleteItemFromQuickSlot;
            itemInventorySo.OnTinderBoxInitialization += InitializeTinderBoxEvent;
            itemInventorySo.OnTinderBoxStateChange += TinderBoxStateChangeEvent;
            quickSlotData.InitializeQuickSlotPanel();
            GetCurrentInventoryState();
        
        }
        public void AddUIItemEquipmentSlots(UIQuickSlot quickSlot)
        {
            currentEquipmentPanel.AddEquipmentSlot(quickSlot);
        }

        #region Tindex box related methods

        private void InitializeTinderBoxEvent(TinderBoxSo tinderBoxSo)
        {
            itemInventoryPageUI.InitializeTinderBox(tinderBoxSo);
        }
        private void TinderBoxStateChangeEvent(TinderBoxInfo info)
        {
            itemInventoryPageUI.UpdateTinderBox(info);
        }

        #endregion
    
        #region Quick Slot related methods
        private void OnSetItemToQuickSlotOnDoubleClick(int itemIndex)
        {
            var inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.item is IEquippable { CanBeEquipped: false }) return;
            itemInventorySo.SetItemToQuickSlotOnDoubleClick(itemIndex);
            inventoryUI.ResetSelection();
        }
        private void OnDropItemInQuickSlot(int quickSlotNumber, int itemIndex)
        {
            var inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.item is IEquippable { CanBeEquipped: false }) return;
            itemInventorySo.SetItemToQuickSlot(quickSlotNumber, itemIndex);
        }
        private void SetItemToQuickSlot(int quickSlotNumber, int itemIndex)
        {
            var inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.item is IEquippable { CanBeEquipped: false }) return;
            itemInventoryPageUI.SetItemToQuickSlot(quickSlotNumber, itemIndex);
        }
        private void DeleteItemFromQuickSlot(int quickSlotNumber)
        {
            itemInventoryPageUI.DeleteItemFromQuickSlot(quickSlotNumber);
            inventoryUI.ResetSelection();
        }
        #endregion
        private protected override void OnLocalizationChange(UnityEngine.Localization.Locale newLocale)
        {
            itemInventoryPageUI.OnTinderBoxDescriptionChange(tinderBox);
        }
        private void ItemSelectionEvent(int itemIndex)
        {
            if (inventoryData.GetItemAt(itemIndex).IsEmpty) return;
            if (_shiftIsPressed)
            {
                if (_selectedItems.Contains(itemIndex))
                {
                    inventoryUI.DeselectByIndex(itemIndex);
                    _selectedItems.Remove(itemIndex);
                    return;
                }
                _selectedItems.Add(itemIndex);
                itemInventoryPageUI.MultipleSelection(itemIndex);
                return;
            }
            _selectedItems.Clear();
            itemInventoryPageUI.SingleSelect(itemIndex);
            _selectedItems.Add(itemIndex);
        }
        private void DeleteItemFromQuickSlotRequest(int quickSlotNumber)
        {
            itemInventorySo.DeleteItemFromQuickSlot(quickSlotNumber);
        }
        private void ItemUsageEvent(int quickSlotIndex)
        {
            InventoryItem inventoryItem = itemInventorySo.GetItemByQuickSlotIndex(quickSlotIndex);
            if (inventoryItem.IsEmpty)
                return;
            if (inventoryItem.item is IItemAction action)
            {
                if (action.CantBeUsedFromInventory) return;
                //action.PerformAction(gameObject.transform.parent.gameObject);
                action.PerformAction(playerPrefab);
            }
            if (inventoryItem.item is IDestroyableItem destroyableItem)
            {
                itemInventorySo.RemoveItem(inventoryItem, 1);
            }
            inventoryUI.ResetSelection();
        }
        public void ReassignTinderBox()
        {
            if (!tinderBox.IsInitialized) return;
            itemInventorySo.AddTinderBox();
        
        }
        public void DropItem(int itemIndex, int quantity)
        {
            InventoryItem dropItem = inventoryData.GetItemAt(itemIndex);
            List<InventoryItem> dropItems = new List<InventoryItem>()
            {
                new InventoryItem(dropItem.item, quantity)
            };
            inventoryData.RemoveItem(itemIndex, quantity);
            inventoryUI.ResetSelection();
            var nearestLootBag = FindNearestLootBag();
            if (nearestLootBag != null)
            {
                nearestLootBag.AddItems(InitializeLootList(dropItems));
                audioSource.PlayOneShot(dropClip);
                return;
            }
            InitializeDropBag(InitializeLootList(dropItems));
            audioSource.PlayOneShot(dropClip);
            inventoryUI.ResetSelection();
        }
        private protected override void HandleItemActionRequest(int itemIndex)
        {
            if (itemInventoryPageUI.SelectedMany())
            {
                MultipleSelectionActionRequest(itemIndex);
                return;
            }
            base.HandleItemActionRequest(itemIndex);
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.item is IItemAction { CanBeUsed: true } action and IMultipleUsable)
            {
                if (inventoryItem.item.IsStackable && inventoryItem.quantity > 1)
                {
                    inventoryUI.AddAction(
                        itemMultipleUsage.GetLocalizedString(),
                        () => OpenItemQuantitySelectionMenu( 
                            action.ActionName.GetLocalizedString(),
                            inventoryItem,
                            (quantity) => UseItemMultipleTimes(itemIndex, quantity)
                        )
                    );
                }
            }
            if (inventoryItem.item is IDestroyableItem destroyableItem and IDroppable) 
                inventoryUI.AddAction(
                    dropAction.GetLocalizedString(), 
                    () => DropItem(itemIndex, inventoryItem.quantity));

            if (inventoryItem.quickSlotNumber == -1)
            {
                if (inventoryItem.item is IEquippable equippable)
                {
                    if (equippable.CanBeEquipped)
                    {
                        inventoryUI.
                            AddAction(
                                equipAction.GetLocalizedString(), 
                                () => OnSetItemToQuickSlotOnDoubleClick(itemIndex));
                    }
                }
                
            }
        }
        private void HandleEquipmentSlotSelection(int qsIndex)
        {
            if (!inventoryData.quickSlots[qsIndex]) return;
            _selectedItems.Clear();
            inventoryUI.ResetSelection();
            if (_shiftIsPressed)
            {
                if (_selectedEquipmentSlots.Contains(qsIndex))
                {
                    _selectedEquipmentSlots.Remove(qsIndex);
                    itemInventoryPageUI.DeselectQuickSlotByIndex(qsIndex);
                    return;
                }
                _selectedEquipmentSlots.Add(qsIndex);
                itemInventoryPageUI.MultipleEquipmentSlotSelection(qsIndex);
                return;
            }
            _selectedEquipmentSlots.Clear();
            itemInventoryPageUI.SingleEquipmentSlotSelection(qsIndex);
            _selectedEquipmentSlots.Add(qsIndex);
        }

        private void HandleEquipmentSlotStartDragging(int qsIndex)
        {
            if (!inventoryData.quickSlots[qsIndex]) return;
            InventoryItem inventoryItem = itemInventorySo.GetItemByQuickSlotIndex(qsIndex);
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        
        }

        private void HandleItemESSwap(int itemIndex, int qsIndex)
        {
            if (!inventoryData.quickSlots[qsIndex]) return;
            InventoryItem inventoryItem = itemInventorySo.GetItemByQuickSlotIndex(qsIndex);
            int secondItemIndex = inventoryData.FindItemIndex(inventoryItem.item);
            if (itemIndex == secondItemIndex)
            {
                DeleteItemFromQuickSlotRequest(qsIndex);
                return;
            }
            HandleSwap(itemIndex, secondItemIndex);
            DeleteItemFromQuickSlotRequest(qsIndex);
        }

        private void HandleESSwap(int qsDropInIndex, int qsDraggedIndex)
        {
            itemInventorySo.SwapESItems(qsDropInIndex, qsDraggedIndex);
        }
    
    
        #region MultipleSelectionMethods
        private void MultipleSelectionActionRequest(int itemIndex)
        {
            inventoryUI.ShowItemActions(itemIndex);
            itemInventoryPageUI.AddAction(multipleUsage.GetLocalizedString(), () => MultipleUsageEvent(_selectedItems));
            itemInventoryPageUI.AddAction(multipleEquip.GetLocalizedString(), () => MultipleEquipEvent(_selectedItems));
            itemInventoryPageUI.AddAction(multipleDrop.GetLocalizedString(), () => MultipleDropEvent(_selectedItems));
        }

        private void MultipleEquipmentSLotSelectionRequest(int qsIndex)
        {
            itemInventoryPageUI.ShowEquipmentSlotActionMenu(qsIndex);
            itemInventoryPageUI.AddAction(multipleUsage.GetLocalizedString(), () => MultipleUsageFromQuickSlotsEvent(_selectedEquipmentSlots));
            itemInventoryPageUI.AddAction(multipleUnEquip.GetLocalizedString(), () => MultipleUnEquipEvent(_selectedEquipmentSlots));
        }

        private void MultipleUsageEvent(List<int> itemIndexes)
        {
            foreach (var itemIndex in itemIndexes)
            {
                var inventoryItem = inventoryData.GetItemAt(itemIndex);
                if (inventoryItem.item is IItemAction action)
                {
                    PerformAction(itemIndex);
                }
            }
            inventoryUI.ResetSelection();
            _selectedItems.Clear();
        }
        private void MultipleDropEvent(List<int> itemIndexes)
        {
            foreach (var itemIndex in itemIndexes)
            {
                var inventoryItem = inventoryData.GetItemAt(itemIndex);
                if (inventoryItem.item is IDestroyableItem destroyableItem)
                {
                    DropItem(itemIndex, inventoryItem.quantity);
                }
            }
            inventoryUI.ResetSelection();
            _selectedItems.Clear();
        }
        private void MultipleEquipEvent(List<int> itemIndexes)
        {
            foreach (var itemIndex in itemIndexes)
            {
                if (itemInventorySo.QuickSlotsFull()) return;
                OnSetItemToQuickSlotOnDoubleClick(itemIndex);
            }
            inventoryUI.ResetSelection();
            _selectedItems.Clear();
        }

        private void MultipleUnEquipEvent(List<int> qsIndexes)
        {
            foreach (var qsIndex in qsIndexes)
            {
                DeleteItemFromQuickSlotRequest(qsIndex);
            }
            itemInventoryPageUI.DeselectAllEquipmentSlot();
            _selectedEquipmentSlots.Clear();
        }

        private void MultipleUsageFromQuickSlotsEvent(List<int> qsIndexes)
        {
            foreach (var qsIndex in qsIndexes)
            {
                ItemUsageEvent(qsIndex);
            }
            itemInventoryPageUI.DeselectAllEquipmentSlot();
            _selectedEquipmentSlots.Clear();
        }

        #endregion
        private void OpenItemQuantitySelectionMenu(string actionName, InventoryItem item, Action<int> confirmationAction)
        {
            inventoryUI.ShowItemQuantitySelectionMenu(actionName, item, confirmationAction);
        }
        private void HandleEquipmentSlotActionRequest(int qsIndex)
        {
        
            var item = itemInventorySo.GetItemByQuickSlotIndex(qsIndex);
            if (item.IsEmpty) return;
            if (_selectedEquipmentSlots.Count > 1)
            {
                MultipleEquipmentSLotSelectionRequest(qsIndex);
                return;
            }
            itemInventoryPageUI.ShowEquipmentSlotActionMenu(qsIndex);
            if (item.item is IItemAction { CantBeUsedFromInventory: false } action)
                itemInventoryPageUI.
                    AddAction(action.ActionName.GetLocalizedString(), () => ItemUsageEvent(qsIndex));
            itemInventoryPageUI.
                AddAction(
                    unEquipAction.GetLocalizedString(), 
                    () => DeleteItemFromQuickSlotRequest(qsIndex));
        
        }
        private HeroLootBagScript FindNearestLootBag()
        {
            HeroLootBagScript[] lootBags = FindObjectsOfType<HeroLootBagScript>();
        
            HeroLootBagScript nearestLootBag = null;
            float minDistance = lootBagSearchRadius;
        
            Vector3 currentPosition = transform.position;
        
            foreach (HeroLootBagScript lootBag in lootBags)
            {
                if(!lootBag.IsActive) continue;
                float distance = Vector3.Distance(currentPosition, lootBag.transform.position);
                if (distance <= lootBagSearchRadius && distance < minDistance)
                {
                    nearestLootBag = lootBag;
                    minDistance = distance;
                }
            }
        
            return nearestLootBag;
        }
        private List<LootBagItem> InitializeLootList(List<InventoryItem> items)
        {
            var lootList = new List<LootBagItem>();
            foreach (var lootItem in items)
            {
                lootList.Add(new LootBagItem(lootItem.item, lootItem.quantity));
            }
            return lootList;
        }
        private void InitializeDropBag(List<LootBagItem> lootList)
        {
            var newLootBag = Instantiate(lootBag, transform.position, Quaternion.identity);
            newLootBag.Initialize(lootList);
        }
        private void OnShiftPressed(InputAction.CallbackContext context)
        {
            _shiftIsPressed = true;
        }
        private void OnShiftReleased(InputAction.CallbackContext context)
        {
            _shiftIsPressed = false;
        }
        private void SubscribeOnInputActions()
        {
            shiftAction.action.started += OnShiftPressed;
            shiftAction.action.canceled += OnShiftReleased;
        }
        private void UnsubscribeFromInputActions()
        {
            shiftAction.action.started -= OnShiftPressed;
            shiftAction.action.canceled -= OnShiftReleased;
        }
    }
}
