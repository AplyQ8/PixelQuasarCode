using System;
using System.Collections;
using System.Collections.Generic;
using Animations.Items.DropBagAnims;
using Pathfinding.Ionic.Zip;
using UnityEngine;

public class DropBag : MonoBehaviour
{
    private HeroStateHandler _heroStateHandler;
    private PickUpSystem _heroPickUpSystem;
    [SerializeField] private ToggleCanvas toggleCanvas;
    [SerializeField] private InventorySO.InventoryTypeEnum inventoryType;
    [SerializeField] private DestoryBag bagDestroyingScript;
    [SerializeField] private AudioSource pickUpSound;

    [Header("Initial items")] [SerializeField]
    private protected List<DropBagItem> initialItems = new List<DropBagItem>();
    private void Awake()
    {
        GameObject hero = GameObject.FindWithTag("Player");
        _heroStateHandler = hero.GetComponent<HeroStateHandler>();
        _heroPickUpSystem = hero.GetComponentInChildren<PickUpSystem>();
    }

    public void InitializeInventory(List<DropBagItem> lootList)
    {
        initialItems = lootList;
    }
    public void OpenInventory()
    {
        //if (!_heroStateHandler.EnterBagInventoryState(this))
        //    return;
        toggleCanvas.OpenBag();
        BagInventoryUI.Instance.OpenBagInventoryUI(GetCurrentBagInventoryState());
        BagInventoryUI.Instance.OnPickItem += PickItem;
        BagInventoryUI.Instance.OnCloseBag += CloseBagEvent;
    }

    #region Collider triggers
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        if(toggleCanvas.GetState is ToggleCanvas.DropBagState.Opened)
            _heroStateHandler.ExitBagInventoryState();
        try
        {
            BagInventoryUI.Instance.CloseBagInventoryUI();
            BagInventoryUI.Instance.OnPickItem -= PickItem;
        }
        catch (NullReferenceException)
        {
            //
        }
        toggleCanvas.SwitchState(ToggleCanvas.DropBagState.OutOfRange);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player"))
            return;
        toggleCanvas.SwitchState(ToggleCanvas.DropBagState.InRange);
    }
    #endregion
    

    private void PickItem(int itemIndex)
    {
        if (itemIndex >= initialItems.Count)
            return;
        var item = initialItems[itemIndex];
        var reminder = _heroPickUpSystem.PickItemFromBag(item.Item, item.Quantity , inventoryType);
        switch (reminder)
        {
            case -1:
                return;
            case 0:
                initialItems[itemIndex] = DropBagItem.GetEmptyItem();
                break;
            default:
                initialItems[itemIndex] = initialItems[itemIndex].ChangeQuantity(reminder);
                break;
        }
        BagInventoryUI.Instance.UpdateBagInventoryUI(GetCurrentBagInventoryState());
        pickUpSound.Play();
        if(BagIsEmpty())
            DestroyBag();
    }

    public void PutItemInBag(InventoryItem inventoryItem)
    {
        
        BagInventoryUI.Instance.OpenBagInventoryUI(GetCurrentBagInventoryState());
    }
    public void UnsubscribeFromUIInventory()
    {
        BagInventoryUI.Instance.OnPickItem -= PickItem;
        BagInventoryUI.Instance.OnCloseBag -= CloseBagEvent;
        toggleCanvas.CloseBag();
    }

    private void CloseBagEvent()
    {
        _heroStateHandler.ExitBagInventoryState();
        try
        {
            BagInventoryUI.Instance.CloseBagInventoryUI();
            UnsubscribeFromUIInventory();
        }
        catch (NullReferenceException)
        {
            //
        }
    }
    
    private Dictionary<int, InventoryItem> GetCurrentBagInventoryState()
    {
        Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();
        for (int i = 0; i < initialItems.Count; i++)
        {
            if(initialItems[i].IsEmpty)
                continue;
            returnValue[i] = new InventoryItem(initialItems[i].Item, initialItems[i].Quantity);
        }

        return returnValue;
    }

    private bool BagIsEmpty()
    {
        foreach (var initialItem in initialItems)
        {
            if(initialItem.IsEmpty)
                continue;
            return false;
        }

        return true;
    }

    private void DestroyBag()
    {
        UnsubscribeFromUIInventory();
        toggleCanvas.SwitchState(ToggleCanvas.DropBagState.Destroying);
        BagInventoryUI.Instance.CloseBagInventoryUI();
        bagDestroyingScript.LaunchDestruction();
        _heroStateHandler.ExitBagInventoryState();
    }

}

[Serializable]
public struct DropBagItem
{
    [field: SerializeField] public ItemSO Item { get; private set; }
    [field: SerializeField] public int Quantity { get; private set; }
    public bool IsEmpty => Item == null;
    
    public DropBagItem(ItemSO item, int quantity)
    {
        this.Quantity = quantity;
        this.Item = item;
    }
    public DropBagItem ChangeQuantity(int newQuantity)
        => new DropBagItem(this.Item, newQuantity);
    public static DropBagItem GetEmptyItem()
        => new DropBagItem(null, 0);
    
}

