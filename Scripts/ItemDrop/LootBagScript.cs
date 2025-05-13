using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;
using IInteractable = Envirenmental_elements.IInteractable;

namespace ItemDrop
{
    public class LootBagScript : MonoBehaviour, IInteractable, IDistanceCheckable, IMouseHoverable
    {
        private protected HeroStateHandler _heroStateHandler;
        private protected PickUpSystem _heroPickUpSystem;

        [SerializeField] private protected InventorySO.InventoryTypeEnum inventoryType;
        [SerializeField] private protected AudioSource pickUpSound;
        [SerializeField] protected LootBagRangeDetector _rangeDetector;

        private protected GameObject _player;

        [Header("Initial items")]
        [SerializeField]
        private protected List<LootBagItem> initialItems = new List<LootBagItem>();

        #region Events
        public Action OnDestroy, MouseEnter, MouseExit, OnOpen, OnClose;
        #endregion

        [field: SerializeField] public bool CanBeOpened { get; private set; } = true;
        [field: SerializeField] public bool IsMouseOver { get; private set; } = false;

        [SerializeField] public LootBagState currentState;

        public enum LootBagState
        {
            Opened,
            Closed
        }

        private protected virtual void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _heroStateHandler = _player.GetComponent<HeroStateHandler>();
            _heroPickUpSystem = _player.GetComponentInChildren<PickUpSystem>();

            _rangeDetector.OnPlayerIsInRange += PlayerIsInRangeEvent;
            currentState = LootBagState.Closed;
        }

        private protected virtual void Update()
        {
            HandleMouseOver();
        }

        private void HandleMouseOver()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePosition, LayerMask.GetMask("LootBag"));

            if (hit != null && hit.gameObject == gameObject)
            {
                if (!IsMouseOver)
                {
                    MouseEnterDetection();
                }
            }
            else
            {
                if (IsMouseOver)
                {
                    MouseExitDetection();
                }
            }
        }

        public void Initialize(List<LootBagItem> lootList)
        {
            initialItems = lootList;
        }

        public void AddItems(List<LootBagItem> lootList)
        {
            foreach (var lootItem in lootList)
            {
                initialItems.Add(lootItem);
            }
            BagInventoryUI.Instance.UpdateBagInventoryUI(GetCurrentBagInventoryState());
        }

        public void OpenLootBag()
        {
            if (!_heroStateHandler.EnterBagInventoryState(this))
                return;

            BagInventoryUI.Instance.OpenBagInventoryUI(GetCurrentBagInventoryState());
            BagInventoryUI.Instance.OnPickItem += PickItem;
            BagInventoryUI.Instance.OnCloseBag += CloseBagEvent;
            currentState = LootBagState.Opened;
            OnOpen?.Invoke();
        }

        protected virtual void PickItem(int itemIndex)
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
            if (BagIsEmpty())
                DestroyBag();
        }

        public void UnsubscribeFromUIInventory()
        {
            BagInventoryUI.Instance.OnPickItem -= PickItem;
            BagInventoryUI.Instance.OnCloseBag -= CloseBagEvent;
        
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
            currentState = LootBagState.Closed;
            OnClose?.Invoke();
        }

        public void CloseLootBag()
        {
            try
            {
                BagInventoryUI.Instance.CloseBagInventoryUI();
                UnsubscribeFromUIInventory();
            }
            catch (NullReferenceException)
            {
                //
            }
            currentState = LootBagState.Closed;
            OnClose?.Invoke();
        }

        private protected Dictionary<int, InventoryItem> GetCurrentBagInventoryState()
        {
            Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();
            for (int i = 0; i < initialItems.Count; i++)
            {
                if (initialItems[i].IsEmpty)
                    continue;
                returnValue[i] = new InventoryItem(initialItems[i].Item, initialItems[i].Quantity);
            }
            return returnValue;
        }

        private bool BagIsEmpty()
        {
            foreach (var initialItem in initialItems)
            {
                if (initialItem.IsEmpty)
                    continue;
                return false;
            }
            return true;
        }

        private protected virtual void DestroyBag()
        {
            UnsubscribeFromUIInventory();
            BagInventoryUI.Instance.CloseBagInventoryUI();
            _heroStateHandler.ExitBagInventoryState();
            OnDestroy?.Invoke();
            _rangeDetector.OnPlayerIsInRange -= PlayerIsInRangeEvent;
            MouseExitDetection();
            CanBeOpened = false;
        }

        // ?????????? ?????? DistanceToPlayer ?? IDistanceCheckable
        public float DistanceToPlayer()
        {
            var obstacleCollider = _player.transform.Find("ObstacleCollider");
            return Vector2.Distance(transform.position, obstacleCollider.position);
        }

        // ?????????? ?????? Interact ?? IInteractable
        public void Interact()
        {
            if (CanBeInteractedWith)
            {
                OpenLootBag();
            }
        }

        // ?????????? ???????? CanBeInteractedWith ?? IInteractable
        public bool CanBeInteractedWith => CanBeOpened;

        private void MouseEnterDetection()
        {
            if (!CanBeOpened) return;
            MouseEnter?.Invoke();
            IsMouseOver = true;
        }

        private void MouseExitDetection()
        {
            MouseExit?.Invoke();
            IsMouseOver = false;
        }

        protected virtual void PlayerIsInRangeEvent(bool isInRange)
        {
            if (isInRange)
            {
                CanBeOpened = true;
            }
            else
            {
                if (currentState == LootBagState.Closed)
                    return;
                CanBeOpened = false;
                MouseExitDetection();
                CloseBagEvent();
            }
        }
    }

    [Serializable]
    public struct LootBagItem
    {
        [field: SerializeField] public ItemSO Item { get; private set; }
        [field: SerializeField] public int Quantity { get; private set; }
        public bool IsEmpty => Item == null;

        public LootBagItem(ItemSO item, int quantity)
        {
            this.Quantity = quantity;
            this.Item = item;
        }
        public LootBagItem ChangeQuantity(int newQuantity)
            => new LootBagItem(this.Item, newQuantity);
        public static LootBagItem GetEmptyItem()
            => new LootBagItem(null, 0);

    }
}