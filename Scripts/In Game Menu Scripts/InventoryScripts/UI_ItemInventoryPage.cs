using System;
using System.Collections.Generic;
using In_Game_Menu_Scripts.InventoryScripts.ItemQuickSlot;
using In_Game_Menu_Scripts.InventoryScripts.TinderBoxScripts;
using PickableObjects.InventoryItems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace In_Game_Menu_Scripts.InventoryScripts
{
    public class UIItemInventoryPage : UIInventoryPage
    {
        [SerializeField] private List<UIItemEquipmentSlot> equipmentSlots = new List<UIItemEquipmentSlot>();
        [SerializeField] private List<int> selectedItemIndexes = new List<int>();
        [SerializeField] private GameObject quickSlotPanel;
        private List<int> _selectedQuickSlots = new List<int>();
    
        [SerializeField] private UITinderBoxSlot tinderBoxSlotPrefab;
        private UITinderBoxSlot _currentTinderBoxSlot;
        private int _currentlyDraggedESIndex;

        public event Action<int, int> OnItemDropInQuickSlot, OnItemESSwap, OnESSwap;
        public event Action<int> OnItemSetInQuickSlotOnDoubleClick;
        public event Action<int> OnItemUsageEvent;
        public event Action<int> OnItemDeleteFromQuickSlot;
        public event Action<int> OnEquipmentSlotActionRequest;
        public event Action<int> OnItemSelection;
        public event Action<int> OnEquipmentSlotSelection;
        public event Action<int> OnEquipmentSlotStartDragging;

        #region TinderBoxMethods

        private protected override void Start()
        {
            base.Start();
            _currentlyDraggedESIndex = -1;
        }
        public void InitializeTinderBox(TinderBoxSo tinderBoxSo)
        {
            if (_currentTinderBoxSlot != null)
                return;
            UITinderBoxSlot tinderBoxSlot = Instantiate(tinderBoxSlotPrefab, Vector3.zero, Quaternion.identity);
            tinderBoxSlot.gameObject.transform.SetParent(contentPanel);
            tinderBoxSlot.gameObject.transform.SetSiblingIndex(0);
            _currentTinderBoxSlot = tinderBoxSlot;
            _currentTinderBoxSlot.Initialize(tinderBoxSo);
        }

        public void UpdateTinderBox(TinderBoxInfo info)
        {
            _currentTinderBoxSlot.UpdateTinderBox(info);
        }

        public void OnTinderBoxDescriptionChange(TinderBoxSo tinderBoxSo)
        {
            _currentTinderBoxSlot.OnDescriptionChange(tinderBoxSo);
        }
    
        #endregion
    
        public void AddEquipmentSlot(UIItemEquipmentSlot equipmentSlot)
        {
            equipmentSlot.OnDrop += HandleSwapES;
            equipmentSlot.OnInputTrigger += OnEquipmentSlotUsage;
            equipmentSlot.OnItemActionRequested += OnEquipmentSlotActionRequested;
            equipmentSlot.OnItemSelectionRequest += HandleQuickSlotSelection;
            equipmentSlot.OnESBeginDrag += HandleBeginDragES;
            equipmentSlot.OnESEndDrag += HandleEndDragES;
            equipmentSlots.Add(equipmentSlot);
        }

        private void HandleSwapES(UIItemEquipmentSlot equipmentSlot)
        {
        
            // if (!equipmentSlot.IsEmpty)
            //     return;
        
            if (!equipmentSlots.Contains(equipmentSlot))
                return;
            if (_currentlyDraggedESIndex != -1)
            {
                int dropSlotIndex = equipmentSlots.IndexOf(equipmentSlot);
                OnESSwap?.Invoke(dropSlotIndex, _currentlyDraggedESIndex);
                return;
            }
            //if(equipmentSlot.IsEmpty)
            OnItemDropInQuickSlot?.Invoke(equipmentSlots.IndexOf(equipmentSlot), _currentlyDraggedItemIndex);
        
        }
        private protected override void HandleSwap(UIInventoryItem inventoryItemUI)
        {
        
            if (_currentlyDraggedESIndex != -1)
            {
                if (!_listOfSlots.Contains(inventoryItemUI)) return;
                int index = _listOfSlots.IndexOf(inventoryItemUI);
                if (index == -1) return;
                OnItemESSwap?.Invoke(index, _currentlyDraggedESIndex);
                return;
            }
            base.HandleSwap(inventoryItemUI);
        }

        private void HandleBeginDragES(UIItemEquipmentSlot equipmentSlot)
        {
            if (!equipmentSlots.Contains(equipmentSlot))
                return;
            if (equipmentSlot.IsEmpty)
                return;
            _currentlyDraggedESIndex = equipmentSlots.IndexOf(equipmentSlot);
            HandleQuickSlotSelection(equipmentSlot);
            OnEquipmentSlotStartDragging?.Invoke(_currentlyDraggedESIndex);
        
        }

        private void HandleEndDragES(UIItemEquipmentSlot equipmentSlot)
        {
            ResetDraggedES();
        }
        private void OnEquipmentSlotUsage(UIItemEquipmentSlot equipmentSlot)
        {
            if (equipmentSlot.IsEmpty)
                return;
            OnItemUsageEvent?.Invoke(equipmentSlots.IndexOf(equipmentSlot));
        }

        private void DeleteFromEquipmentSlot(UIItemEquipmentSlot equipmentSlot)
        {
            OnItemDeleteFromQuickSlot?.Invoke(equipmentSlots.IndexOf(equipmentSlot));
        }
        public void SetItemToQuickSlot(int quickSlotNumber, int itemIndex)
        {
            equipmentSlots[quickSlotNumber].SetData(
                _listOfSlots[itemIndex].GetSprite,
                _listOfSlots[itemIndex].GetQuantity,
                _listOfSlots[itemIndex].GetInventoryItem
            );
            DeselectAllEquipmentSlot();
        }

        private void OnEquipmentSlotActionRequested(UIItemEquipmentSlot equipmentSlot)
        {
            if (!equipmentSlots.Contains(equipmentSlot))
                return;
            //ShowEquipmentSlotActions(equipmentSlot);
            OnEquipmentSlotActionRequest?.Invoke(equipmentSlots.IndexOf(equipmentSlot));

        }
        public override void UpdateData(int itemIndex, Sprite itemSprite, int itemQuantity, InventoryItem inventoryItem)
        {
            //Means that we have the item in the list
            if (_listOfSlots.Count <= itemIndex) 
                return;
            _listOfSlots[itemIndex].SetData(itemSprite, itemQuantity, inventoryItem);
            if (inventoryItem.quickSlotNumber != -1)
            {
                try
                {
                    equipmentSlots[inventoryItem.quickSlotNumber].SetData(itemSprite, itemQuantity, inventoryItem);
                }
                catch (ArgumentOutOfRangeException)
                {
                    //Quick slot panel has not been initialized yet
                }
            }
        }

        public void DeleteItemFromQuickSlot(int quickSlotIndex)
        {
            equipmentSlots[quickSlotIndex].ResetData();
        }
    
        public void UpdateQuickSlots()
        {
            foreach (var equipmentSlot in equipmentSlots)
            {
                equipmentSlot.UpdateData();
            }
        }

        public void ShowEquipmentSlotActionMenu(int qsIndex)
        {
            //actionMenu.Toggle(true);
            ItemActionMenu.Instance.Toggle(true);
            //actionMenu.transform.position = equipmentSlots[qsIndex].transform.position;
            ItemActionMenu.Instance.transform.position = equipmentSlots[qsIndex].transform.position;
        }

        private protected override void HandleItemSelection(UIInventoryItem inventoryItemUI)
        {
            int index = _listOfSlots.IndexOf(inventoryItemUI);
            OnItemSelection?.Invoke(index);
        }

        private void HandleQuickSlotSelection(UIItemEquipmentSlot equipmentSlot)
        {
            var qsIndex = equipmentSlots.IndexOf(equipmentSlot);
            OnEquipmentSlotSelection?.Invoke(qsIndex);
        }

        private protected override void OnEnable()
        {
            base.OnEnable();
            quickSlotPanel.SetActive(false);
            inventoryMenu.OpenQuickSlotPanel(quickSlotPanel);
        }

        private protected override void OnDisable()
        {
            inventoryMenu.CloseQuickSlotPanel();
        }

        public void SingleSelect(int itemIndex)
        {
            ResetSelection();
            if (_listOfSlots[itemIndex].IsEmpty) return;
            _listOfSlots[itemIndex].Select();
            selectedItemIndexes.Add(itemIndex);
        }

        public void MultipleSelection(int itemIndex)
        {
            if (_listOfSlots[itemIndex].IsEmpty) return;
            _listOfSlots[itemIndex].Select();
            selectedItemIndexes.Add(itemIndex);
        }

        public override void ResetSelection()
        {
            foreach (var itemIndex in selectedItemIndexes)
            {
                try
                {
                    _listOfSlots[itemIndex].Deselect();
                }
                catch (NullReferenceException) { }
            }
            selectedItemIndexes.Clear();
            //actionMenu.Toggle(false);
            try
            {
                ItemActionMenu.Instance.Toggle(false);
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("Item action panel has not been found");
            }
            //quantitySelectionMenu.ToggleOff();
            try
            {
                ItemQuantitySelectionMenu.Instance.ToggleOff();
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("Quantity selection menu has been found");
            }
        }

        private void ResetDraggedES()
        {
            mouseFollower.Toggle(false);
            _currentlyDraggedESIndex = -1;
        }

        #region Equipemnt Slot Selection methods

        public void SingleEquipmentSlotSelection(int qsIndex)
        {
            DeselectAllEquipmentSlot();
            _selectedQuickSlots.Clear();
            _selectedQuickSlots.Add(qsIndex);
            equipmentSlots[qsIndex].Select();
        }

        public void MultipleEquipmentSlotSelection(int qsIndex)
        {
            _selectedQuickSlots.Add(qsIndex);
            equipmentSlots[qsIndex].Select();
        }

        public void DeselectQuickSlotByIndex(int qsIndex)
        {
            _selectedQuickSlots.Remove(qsIndex);
            equipmentSlots[qsIndex].Deselect();
        }

        public void DeselectAllEquipmentSlot()
        {
            foreach (var selectedQuickSlot in _selectedQuickSlots)
            {
                equipmentSlots[selectedQuickSlot].Deselect();
            }
        }

        #endregion
    
        public bool SelectedMany() => selectedItemIndexes.Count > 1;
        
    }
}
