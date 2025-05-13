using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace In_Game_Menu_Scripts.InventoryScripts.ItemQuickSlot
{
    public class UIItemEquipmentSlot : MonoBehaviour, 
        IDropHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private UIQuickSlot quickSlot;
        [SerializeField] private Image borderImage;
        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text quantityTxt;
        [SerializeField] private InventoryItem inventoryItem;
        [field: SerializeField] public bool IsEmpty { get; private set; } = true;

        public event Action<UIItemEquipmentSlot>
            OnDrop,
            OnInputTrigger,
            OnItemActionRequested,
            OnItemSelectionRequest,
            OnESBeginDrag,
            OnESEndDrag;
    

        private void Awake()
        {
            ResetData();
            Deselect();
        }
        public void ConnectToQuickSlot(UIQuickSlot quickSlotLink)
        {
            quickSlot = quickSlotLink;
            quickSlotLink.OnInputTrigger += TriggerInputEvent;
        }

        private void TriggerInputEvent()
        {
            OnInputTrigger?.Invoke(this);
        }
    
        public virtual void SetData(Sprite sprite, int quantity, InventoryItem invItem)
        {
            if (sprite is null || quantity == 0)
                ResetData();
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = sprite;
            }

            if (quantityTxt != null)
            {
                quantityTxt.text = quantity.ToString();
            }
        
            inventoryItem = invItem;
            IsEmpty = false;
            quickSlot.SetData(sprite, quantity);
        }
        public void ResetData()
        {
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(false);
            }
        
            IsEmpty = true;
            try
            {
                quickSlot.ResetData();
            }
            catch (NullReferenceException)
            {
                //Catches at the begging, when quick slot is not assigned yet. 
            }
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            OnDrop?.Invoke(this);
        }

        public void UpdateData()
        {
            if (quantityTxt != null)
            {
                quantityTxt.text = inventoryItem.quantity.ToString();
            }
        
            quickSlot.SetData(inventoryItem.quantity);
            if(inventoryItem.quantity <= 0)
                ResetData();
        }

        public void Select()
        {
            if (borderImage == null) return;
            borderImage.enabled = true;
        }

        public void Deselect()
        {
            if (borderImage == null) return;
            borderImage.enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsEmpty)
                return;
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Right:
                    OnItemActionRequested?.Invoke(this);
                    break;
                case PointerEventData.InputButton.Left:
                    OnItemSelectionRequest?.Invoke(this);
                    break;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsEmpty) return;
            OnESBeginDrag?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnESEndDrag?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            //for the sake of work
        }
    }
}
