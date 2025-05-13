
using System;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PA_Item : UIInventoryItem
{
    [field: SerializeField] public bool IsActivated { get; private set; }
    [SerializeField] private UIParticle activationEffect;
    private protected override void Awake()
    {
        base.Awake();
        Deactivate();
    }
    
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (IsActivated)
            return;
        base.OnBeginDrag(eventData);
    }
    public override void SetData(Sprite sprite, int quantity)
    {
        if (sprite is null || quantity == 0)
            return;
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;
        _empty = false;
    }

    public override void TriggerActivationEffect()
    {
        if(IsActivated) Deactivate();
        else Activate();
    }

    private void Activate()
    {
        IsActivated = true;
        activationEffect.Play();
    }

    private void Deactivate()
    {
        IsActivated = false;
        activationEffect.Stop();
        activationEffect.Clear();
    }
}
