using UnityEngine;

namespace ItemDrop
{
    public class LootChest : LootBagScript
    {
        protected override void PickItem(int itemIndex)
        {
            if (itemIndex >= initialItems.Count)
                return;

            var item = initialItems[itemIndex];
            var reminder = _heroPickUpSystem.PickItemFromBag(item.Item, item.Quantity, inventoryType);
            switch (reminder)
            {
                case -1:
                    return;
                case 0:
                    initialItems[itemIndex] = LootBagItem.GetEmptyItem();
                    break;
                default:
                    initialItems[itemIndex] = initialItems[itemIndex].ChangeQuantity(reminder);
                    break;
            }
            BagInventoryUI.Instance.UpdateBagInventoryUI(GetCurrentBagInventoryState());
            pickUpSound.Play();
        }
    }
}