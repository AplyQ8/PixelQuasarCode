using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIInventoryItem : MonoBehaviour, 
     IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
{
    [SerializeField] private protected Image itemImage;
    [SerializeField] private protected TMP_Text quantityTxt;
    [SerializeField] private protected Image borderImage;
    [SerializeField] private DescriptionShower descriptionShower;
    [SerializeField] private InventoryItem inventoryItem;
    //[SerializeField] public GUID guid;
    [SerializeField] private string _guid;
    private int _quantity;

    public event Action<UIInventoryItem>
        OnItemClicked,
        OnItemDropOn,
        OnItemBeginDrag,
        OnItemEndDrag,
        OnRightMouseBtnClick,
        OnItemDoubleClicked;

    private protected bool _empty = true;

    private protected virtual void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        descriptionShower = GetComponent<DescriptionShower>();
        ResetData();
        Deselect();
        //guid = GUID.Generate();
        //_guid = guid.ToString();
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeComponents();
        ResetData();
        Deselect();
    }
    public virtual void ResetData()
    {
        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(false);
        }
        _empty = true;
        inventoryItem = InventoryItem.GetEmptyItem();
        descriptionShower.SetData(string.Empty, string.Empty, false);
        Deselect();
    }
    public void Deselect()
    {
        if (borderImage != null)
        {
            borderImage.enabled = false;
        }
        
        try
        {
            descriptionShower.Deactivate();
        }
        catch (Exception)
        {
            //
        }
        
    }
    
    private protected virtual void InitializeComponents()
    {
        if (itemImage == null)
            itemImage = GetComponent<Image>();

        if (quantityTxt == null)
            quantityTxt = GetComponentInChildren<TMP_Text>();

        if (borderImage == null)
            borderImage = GetComponent<Image>();

        if (descriptionShower == null)
            descriptionShower = GetComponent<DescriptionShower>();
    }
    public void Select()
    {
        borderImage.enabled = true;
    }
    public virtual void SetData(Sprite sprite, int quantity, InventoryItem invItem)
    {
        if (sprite == null || quantity == 0)
            return;
        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
        }

        if (quantityTxt != null)
        {
            quantityTxt.text = quantity.ToString();
        }

        _quantity = quantity;
        inventoryItem = invItem;
        _empty = false;
        descriptionShower.SetData(invItem.item.Name.GetLocalizedString(), invItem.item.Description.GetLocalizedString(), true);
        
    }
    public virtual void SetData(Sprite sprite, int quantity)
    {
        if (sprite == null || quantity == 0)
            return;

        InitializeComponents();

        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
        }

        if (quantityTxt != null)
        {
            quantityTxt.text = quantity.ToString();
        }

        _quantity = quantity;
        _empty = false;
    }

    public virtual void ShowDescription(string itemName, string itemDescription)
    {
        descriptionShower.SetDescriptionAndActivate(itemName, itemDescription);
    }
    

    public Sprite GetSprite => itemImage.sprite;
    public int GetQuantity => _quantity;
    public InventoryItem GetInventoryItem => inventoryItem;
    public bool IsEmpty => _empty;
    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f; // Time threshold for double-click, in seconds

    #region Action event methods
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (_empty)
            return;
        OnItemBeginDrag?.Invoke(this);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        OnItemEndDrag?.Invoke(this);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        //for the sake of work
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        OnItemDropOn?.Invoke(this);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Right:
                OnRightMouseBtnClick?.Invoke(this);
                break;
            case PointerEventData.InputButton.Left:
                HandleLeftClick();
                //OnItemDoubleClicked?.Invoke(this);
                //OnItemClicked?.Invoke(this);
                break;
        }
    }

    private protected virtual void HandleLeftClick()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            OnItemDoubleClicked?.Invoke(this); // Trigger double-click event
        }
        else
        {
            OnItemClicked?.Invoke(this); // Trigger single-click event
        }

        lastClickTime = Time.time; // Update the last click time
    }
    #endregion

    public virtual void TriggerActivationEffect()
    { }
}
