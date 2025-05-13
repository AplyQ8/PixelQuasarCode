using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class BagInventoryUI : Singleton<BagInventoryUI>
{
    [SerializeField] private RectTransform content;
    [SerializeField] private UIInventoryItem itemPrefab;
    [SerializeField] private int numberOfInitialSlots;
    private List<UIInventoryItem> _listOfSlots = new List<UIInventoryItem>();
    private UIInventoryItem _selectedItem;
    public event Action<int> OnDescriptionRequested, OnPickItem;
    public event Action OnCloseBag;
    private Dictionary<int, InventoryItem> _currentInventoryState;
    private new void Awake()
    {
        base.Awake();
        InitializeSlots();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocalizationChange;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocalizationChange;
    }
    
    private void OnLocalizationChange(UnityEngine.Localization.Locale newLocale)
    {
        UpdateBagInventoryUI(_currentInventoryState);
    }

    private void InitializeSlots()
    {
        for (int i = 0; i < numberOfInitialSlots; i++)
        {
            UIInventoryItem newSlot = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            newSlot.transform.SetParent(content);
            SubscribeOnSlotEvents(newSlot);
            _listOfSlots.Add(newSlot);
        }
    }

    #region Action Methods

    private void SubscribeOnSlotEvents(UIInventoryItem slot)
    {
        slot.OnItemClicked += HandleItemSelection;
        slot.OnRightMouseBtnClick += HandleTakeItem;
    }
    
    private protected virtual void HandleTakeItem(UIInventoryItem inventoryItemUI)
    {
        if (!_listOfSlots.Contains(inventoryItemUI))
            return;
        if (inventoryItemUI.IsEmpty)
            return;
        OnPickItem?.Invoke(_listOfSlots.IndexOf(inventoryItemUI));
    }

    public void PickAllItems()
    {
        foreach (var slot in _listOfSlots)
        {
            HandleTakeItem(slot);
        }
    }

    public void CloseBag()
    {
        OnCloseBag?.Invoke();
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

    public void OpenBagInventoryUI(Dictionary<int, InventoryItem> inventoryState)
    {
        try
        {
            gameObject.SetActive(true);
        }
        catch (Exception)
        {
            return;
        }
        UpdateBagInventoryUI(inventoryState);
        _currentInventoryState = inventoryState;
    }

    public void UpdateBagInventoryUI(Dictionary<int, InventoryItem> inventoryState)
    {
        ResetAllItems();
        foreach (var inventoryItem in inventoryState)
        {
            UpdateData(
                inventoryItem.Key,
                inventoryItem.Value.item.ItemImage,
                inventoryItem.Value.quantity,
                inventoryItem.Value
            );
        }
    }

    protected virtual void UpdateData(int itemIndex, Sprite itemSprite, int itemQuantity, InventoryItem inventoryItem)
    {
        //Means that we have the item in the list
        if (_listOfSlots.Count <= itemIndex)
            return;
        _listOfSlots[itemIndex].SetData(itemSprite, itemQuantity, inventoryItem);
    }
    public void CloseBagInventoryUI()
    {
        try
        {
            gameObject.SetActive(false);
        }
        catch (Exception)
        {
            return;
        }
        foreach (var slot in _listOfSlots)
        {
            slot.ResetData();
        }
        ResetSelection();
    }

    private void ResetSelection()
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
    }

    private void ResetAllItems()
    {
        foreach (var slot in _listOfSlots)
        {
            slot.ResetData();
        }
    }
}
