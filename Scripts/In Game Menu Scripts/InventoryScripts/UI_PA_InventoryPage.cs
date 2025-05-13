using System.Collections;
using System.Collections.Generic;
using In_Game_Menu_Scripts.InventoryScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PA_InventoryPage : UIInventoryPage
{
    [SerializeField] private UI_PA_SlotHandler paSlotHandler;
    public override void TriggerActionIndicator(int itemIndex)
    {
        _listOfSlots[itemIndex].TriggerActivationEffect();
    }

    public override void ReassignInventories(GameObject hero)
    {
        paSlotHandler.ReassignSlotHandler(hero.GetComponent<PassiveAbilityHandler>());
    }

    private protected override void OnEnable()
    {
        base.OnEnable();
        paSlotHandler.gameObject.SetActive(false);
        inventoryMenu.OpenQuickSlotPanel(paSlotHandler.gameObject);
    }
    private protected override void OnDisable()
    {
        inventoryMenu.CloseQuickSlotPanel();
    }
}
