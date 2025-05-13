using System;
using System.Collections.Generic;
using UnityEngine;

namespace In_Game_Menu_Scripts.InventoryScripts
{
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField] private protected HeroInventoryMenu inventoryMenu;
        [SerializeField] private UIInventoryItem itemPrefab;
        [SerializeField] protected RectTransform contentPanel;
        [SerializeField] private protected MouseFollower mouseFollower;
        //[SerializeField] private protected ItemActionMenu actionMenu;
        //[SerializeField] private protected ItemQuantitySelectionMenu quantitySelectionMenu;
    
        private protected List<UIInventoryItem> _listOfSlots = new List<UIInventoryItem>();
        private UIInventoryItem _selectedItem;
        private protected int _currentlyDraggedItemIndex = -1;

        public event Action<int>
            OnDescriptionRequested,
            OnItemActionRequested,
            OnStartDragging,
            OnItemDoubleClicked;

        public event Action<int, int> OnSwapItems, OnItemMultipleUsage;
    
        private protected virtual void Start()
        {
            mouseFollower.Toggle(false);
        }
        public void InitializeInventoryUI(int startNumberOfSlots)
        {
            for (int i = 0; i < startNumberOfSlots; i++)
            {
                UIInventoryItem newSlot = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                newSlot.transform.SetParent(contentPanel);
                SubscribeOnSlotEvents(newSlot);
                _listOfSlots.Add(newSlot);
                newSlot.transform.localScale = Vector3.one;
            }
        }
        private void AddNewSlot()
        {
            UIInventoryItem newSlot = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            newSlot.transform.SetParent(contentPanel);
            SubscribeOnSlotEvents(newSlot);
            _listOfSlots.Add(newSlot);
        }
    
    
    
        #region Action event methods
        private protected virtual void SubscribeOnSlotEvents(UIInventoryItem slot)
        {
            slot.OnItemClicked += HandleItemSelection;
            slot.OnRightMouseBtnClick += HandleShowItemActions;
            slot.OnItemBeginDrag += HandleBeginDrag;
            slot.OnItemEndDrag += HandleEndDrag;
            slot.OnItemDropOn += HandleSwap;
            slot.OnItemDoubleClicked += HandleDoubleClick;
        }
        private protected virtual void HandleSwap(UIInventoryItem inventoryItemUI)
        {
            //Debug.Log($"Swap was called from cell with GUID: {inventoryItemUI.guid}");
            if (!_listOfSlots.Contains(inventoryItemUI))
                return;
            int index = _listOfSlots.IndexOf(inventoryItemUI);
            if (index == -1)
                return;
            OnSwapItems?.Invoke(index, _currentlyDraggedItemIndex);
        }
    
        private void HandleEndDrag(UIInventoryItem inventoryItemUI)
        {
            ResetDraggedItem();
        }

        private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
        {
            int index = _listOfSlots.IndexOf(inventoryItemUI);
            if (index == -1)
                return;
            _currentlyDraggedItemIndex = index;
            HandleItemSelection(inventoryItemUI);
            OnStartDragging?.Invoke(index);
        }

    
        private protected virtual void HandleShowItemActions(UIInventoryItem inventoryItemUI)
        {
            if (inventoryItemUI.IsEmpty) return;
            OnItemActionRequested?.Invoke(_listOfSlots.IndexOf(inventoryItemUI));
        }

        private protected virtual void HandleDoubleClick(UIInventoryItem inventoryItemUI)
        {
            OnItemDoubleClicked?.Invoke(_listOfSlots.IndexOf(inventoryItemUI));
        }

        private protected virtual void HandleItemSelection(UIInventoryItem inventoryItemUI)
        {
            ResetSelection();
            if (inventoryItemUI.IsEmpty)
            {
                try
                {
                    _selectedItem.Deselect();
                }
                catch (NullReferenceException)
                {
                    //Do nothing
                }
                return;
            }
            inventoryItemUI.Select();
            _selectedItem = inventoryItemUI;
            int index = _listOfSlots.IndexOf(inventoryItemUI);
            if (index is -1)
                return;
            OnDescriptionRequested?.Invoke(index);
        }
        #endregion

        #region Helper methods

        public virtual void TriggerActionIndicator(int itemIndex)
        { }
        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }
        public virtual void UpdateData(int itemIndex, Sprite itemSprite, int itemQuantity, InventoryItem inventoryItem)
        {
            //Means that we have the item in the list
            if(_listOfSlots.Count > itemIndex)
                _listOfSlots[itemIndex].SetData(itemSprite, itemQuantity, inventoryItem);
        }
        private protected virtual void ResetDraggedItem()
        {
            mouseFollower.Toggle(false);
            _currentlyDraggedItemIndex = -1;
        }

        private protected virtual void OnEnable()
        {
            ResetSelection();
        }

        public virtual void ResetSelection()
        {
            try
            {
                _selectedItem.Deselect();
            }
            catch (NullReferenceException)
            {
                //Do nothing
            }
        
            _selectedItem = null;
            //Also possible close description tab
            //actionMenu.Toggle(false);
            ItemActionMenu.Instance.Toggle(false);
            //quantitySelectionMenu.ToggleOff();
            ItemQuantitySelectionMenu.Instance.ToggleOff();
        }

        private void SelectItem(UIInventoryItem itemToSelect)
        {
            itemToSelect.Select();
            _selectedItem = itemToSelect;
        }

        public void DeselectByIndex(int itemIndex)
        {
            try
            {
                _listOfSlots[itemIndex].Deselect();
            }
            catch (NullReferenceException) { }
        }

        private void ShowItemDescription(UIInventoryItem item, string itemName, string itemDescription)
        {
            item.ShowDescription(itemName, itemDescription);
        }

        private protected virtual void OnDisable()
        {
            ResetDraggedItem();
            //actionMenu.Toggle(false);
            ItemActionMenu.Instance.Toggle(false);
        }
        public void ShowDescription(int itemIndex, string itemName, string itemDescription)
        {
            //Show Description
            ResetSelection();
            SelectItem(_listOfSlots[itemIndex]);
            ShowItemDescription(_listOfSlots[itemIndex], itemName, itemDescription);
        }

        public void ResetAllItems()
        {
            foreach (var slot in _listOfSlots)
            {
                slot.ResetData();
            }
            ResetSelection();
        }

        public virtual void ShowItemActions(int itemIndex)
        {
            ItemActionMenu.Instance.Toggle(true);
            ItemActionMenu.Instance.transform.position = _listOfSlots[itemIndex].transform.position;
        }
    
        public virtual void AddAction(string actionName, Action performAction)
        {
            //actionMenu.AddButton(actionName, performAction);
            ItemActionMenu.Instance.AddButton(actionName, performAction);
        }
    

        public virtual void ShowItemQuantitySelectionMenu(string actionName, InventoryItem item, Action<int> confirmationAction)
        {
            //actionMenu.Toggle(false);
            ItemActionMenu.Instance.Toggle(false);
            //quantitySelectionMenu.ToggleOn(actionName, item, confirmationAction);
            ItemQuantitySelectionMenu.Instance.ToggleOn(actionName, item, confirmationAction);
        }
    

        #endregion

        public virtual void ReassignInventories(GameObject hero) { }

    
    }
}
