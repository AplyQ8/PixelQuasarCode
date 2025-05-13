using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AA_Item : UIInventoryItem
{
    [field: SerializeField] public bool IsActivated { get; private set; }

    private protected override void Awake()
    {
        base.Awake();
        Activate();
    }
    public override void SetData(Sprite sprite, int quantity)
    {
        if (sprite is null || quantity == 0)
            return;
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;
        _empty = false;
    }

    public void Activate() => IsActivated = true;
    public void Deactivate() => IsActivated = false;
}
