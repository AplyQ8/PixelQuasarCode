using System;
using ItemDrop;
using UnityEngine;

namespace InteractableObjects.Graves
{
    public class GraveLootInventory : LootBagScript
    {
        public event Action OnItemPickUp;
        private protected override void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _heroStateHandler = _player.GetComponent<HeroStateHandler>();
            _heroPickUpSystem = _player.GetComponentInChildren<PickUpSystem>();
            currentState = LootBagState.Closed;
            //_rangeDetector.OnPlayerIsInRange += PlayerIsInRangeEvent;
        }
        private protected override void Update()
        { }

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
            OnItemPickUp?.Invoke();
            pickUpSound.Play();
        }
        
    }
}
