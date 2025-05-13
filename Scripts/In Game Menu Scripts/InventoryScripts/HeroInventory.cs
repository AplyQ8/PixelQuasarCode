using System;
using System.Collections;
using System.Collections.Generic;
using In_Game_Menu_Scripts.InventoryScripts;
using PickableObjects.InventoryItems;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class HeroInventory : MonoBehaviour
{
    [SerializeField] private GameObject inGameMenuPref;
    [Header("Player ref")]
    [SerializeField] protected GameObject playerPrefab;
    [SerializeField] private protected UIInventoryPage inventoryUI;
    [SerializeField] private protected InventorySO inventoryData;
    [SerializeField] protected bool reinitializeInventor;
    
    [Header("Initial items")] [SerializeField]
    private protected List<InventoryItem> initialItems = new List<InventoryItem>();

    [Header("Audio Source")] [SerializeField]
    private protected AudioSource audioSource;

    [SerializeField] private protected PlayerInput playerInput;

    // private protected virtual void Start()
    // {
    //     PrepareInventoryData(inventoryData);
    //     PrepareInventoryUI(inventoryUI, inventoryData);
    // }
    protected virtual void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocalizationChange;
    }
    protected virtual void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocalizationChange;
    }
    
    public virtual void AssignNewUIInventories(UIInventoryPage uiInventoryPage)
    {
        inventoryUI = uiInventoryPage;
        PrepareInventoryData(inventoryData);
        PrepareInventoryUI(inventoryUI, inventoryData);
    }

    public virtual void GetCurrentInventoryState()
    {
        foreach (var item in inventoryData.GetCurrentInventoryState())
        {
            inventoryUI.UpdateData(
                item.Key,
                item.Value.item.ItemImage,
                item.Value.quantity,
                item.Value
                );
        }
    }

    private void PrepareInventoryUI(UIInventoryPage invPage, InventorySO invData)
    {
        //Debug.Log(invPage.GetType().Name);
        invPage.InitializeInventoryUI(invData.Size);
        //invPage.OnDescriptionRequested += HandleDescriptionRequest;
        invPage.OnStartDragging += HandleStartDragging;
        invPage.OnSwapItems += HandleSwap;
        invPage.OnItemActionRequested += HandleItemActionRequest;
        invPage.OnItemDoubleClicked += HandleDoubleClick;
        GetCurrentInventoryState();
        
    }

    private void PrepareInventoryData(InventorySO invData)
    {
        invData.Initialize(reinitializeInventor);
        invData.OnInventoryUpdated += UpdateInventoryUI;
        
    }

    #region Action events
    private protected virtual void HandleItemActionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        inventoryUI.ShowItemActions(itemIndex);
        if (inventoryItem.item is IItemAction action)
        {
            if (action.CantBeUsedFromInventory) return;
            if (!action.CanBeUsed) return;
            inventoryUI.AddAction(action.ActionName.GetLocalizedString(), () => PerformAction(itemIndex));
            
        }
        
        
        //Add Full Description Show
    }

    private protected void UseItemMultipleTimes(int itemIndex, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            PerformAction(itemIndex);
        }
        inventoryUI.ResetSelection();
    }

    public void PerformAction(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        if (inventoryItem.item is IItemAction action)
        {
            if (!action.CanBeUsed) return;
            if (action.CantBeUsedFromInventory) return;
            if (!action.PerformAction(gameObject.transform.parent.gameObject)) return;
            inventoryUI.TriggerActionIndicator(itemIndex);
            if (inventoryItem.item is IDestroyableItem destroyableItem)
                inventoryData.RemoveItem(itemIndex, 1);
            audioSource.PlayOneShot(action.ActionSfx);
            if(inventoryData.GetItemAt(itemIndex).IsEmpty)
                inventoryUI.ResetSelection();
        }
        
    }
    
    private protected void HandleSwap(int itemIndex1, int itemIndex2)
    {
        inventoryData.SwapItems(itemIndex1, itemIndex2);
    }

    private void HandleDoubleClick(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        int slotNum = inventoryData.GetFreeQuickSlot();
        if (slotNum != -1 && inventoryItem.quickSlotNumber == -1)
        {
            inventoryItem = inventoryItem.SetQuickSlotNumber(slotNum);
            inventoryData.quickSlots[slotNum] = true;
            inventoryData.SetItemAt(itemIndex, inventoryItem);
        }
    }

    private void HandleStartDragging(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
    }

    public virtual bool RemoveItemByItemSo(ItemSO itemSo)
    {
        return inventoryData.RemoveItemByItemSo(itemSo);
    }

    public virtual bool HasItem(ItemSO itemSo)
    {
        return inventoryData.HasItem(itemSo);
    }

    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            inventoryUI.ResetSelection();
            return;
        }
        ItemSO item = inventoryItem.item;
        inventoryUI.ShowDescription(itemIndex, item.Name.GetLocalizedString(), item.Description.GetLocalizedString());
    }
    
    private protected virtual void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState, InventorySO.InventoryTypeEnum inventoryType)
    {
        UpdateInventoryState(inventoryState, inventoryUI);
    }

    private void UpdateInventoryState(Dictionary<int, InventoryItem> inventoryState, UIInventoryPage inventory)
    {
        inventory.ResetAllItems();
        foreach (var item in inventoryState)
        {
            inventory.UpdateData(
                item.Key,
                item.Value.item.ItemImage,
                item.Value.quantity,
                item.Value
            );
        }
    }

    private protected virtual void OnLocalizationChange(UnityEngine.Localization.Locale newLocale)
    {
        inventoryData.OnLocalizationChange();
    }
    #endregion
}
