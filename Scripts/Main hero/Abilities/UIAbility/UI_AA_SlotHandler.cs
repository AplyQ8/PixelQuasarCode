using System.Collections;
using System.Collections.Generic;
using In_Game_Menu_Scripts.InventoryScripts.ItemQuickSlot;
using UnityEngine;

public class UI_AA_SlotHandler : MonoBehaviour
{
    [SerializeField] private ActiveAbilityHandler abilityHandler;
    [SerializeField] private List<UIItemEquipmentSlot> slots = new List<UIItemEquipmentSlot>();
    [SerializeField] private UIItemEquipmentSlot slot;
    [SerializeField] private GameObject contentPanel;

    public void ReassignSlotHandler(ActiveAbilityHandler handler)
    {
        abilityHandler = handler;
        SubscribeToAbilityHandler(abilityHandler);
    }

    private void SubscribeToAbilityHandler(ActiveAbilityHandler handler)
    {
        handler.AddSlots += AddSlots;
        AddSlots(handler.AvailableSlots);
    }

    private void AddSlots(int numOfSlots)
    {
        for (int i = 0; i < numOfSlots; i++)
        {
            UIItemEquipmentSlot newSlot = Instantiate(slot, Vector3.zero, Quaternion.identity);
            newSlot.transform.SetParent(contentPanel.transform);
            newSlot.transform.localScale = Vector3.one;
            slots.Add(newSlot);
        }
    }
}
