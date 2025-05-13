using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInventory_PA : HeroInventory
{
    private protected override void HandleItemActionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;
        if (inventoryItem.item is not IItemActivated activation)
            return;
        if (activation.Activate(gameObject.transform.parent.gameObject))
        {
            inventoryUI.TriggerActionIndicator(itemIndex);
        }
    }
}
