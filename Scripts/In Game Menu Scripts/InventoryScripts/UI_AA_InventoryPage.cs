using System.Collections;
using System.Collections.Generic;
using In_Game_Menu_Scripts.InventoryScripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_AA_InventoryPage : UIInventoryPage
{
    [SerializeField] private UI_AA_SlotHandler aaSlotHandler;
    [SerializeField] private GameObject quickSlotPanel;
    [SerializeField] private List<UI_AA_Item> equipmentSlots = new List<UI_AA_Item>();
    public override void TriggerActionIndicator(int itemIndex)
    {
        _listOfSlots[itemIndex].TriggerActivationEffect();
    }

    public override void ReassignInventories(GameObject hero)
    {
        aaSlotHandler.ReassignSlotHandler(hero.GetComponent<ActiveAbilityHandler>());
    }


    private protected override void OnEnable()
    {
        base.OnEnable();
        aaSlotHandler.gameObject.SetActive(false);
        inventoryMenu.OpenQuickSlotPanel(aaSlotHandler.gameObject);
    }
    private protected override void OnDisable()
    {
        inventoryMenu.CloseQuickSlotPanel();
    }
}
