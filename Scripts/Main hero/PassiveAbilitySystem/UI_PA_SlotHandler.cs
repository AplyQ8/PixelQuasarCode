using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class UI_PA_SlotHandler : MonoBehaviour
{
    [SerializeField] private PassiveAbilityHandler passiveAbilityHandler;
    [SerializeField] private List<UI_PA_SlotIndicator> slots = new List<UI_PA_SlotIndicator>();
    [SerializeField] private UI_PA_SlotIndicator slot;
    [SerializeField] private GameObject contentPanel;

    // private void Start()
    // {
    //     SubscribeToPassiveAbilityHandler(passiveAbilityHandler);
    // }

    public void ReassignSlotHandler(PassiveAbilityHandler handler)
    {
        passiveAbilityHandler = handler;
        SubscribeToPassiveAbilityHandler(passiveAbilityHandler);
    }
    private void SubscribeToPassiveAbilityHandler(PassiveAbilityHandler handler)
    {
        handler.AddAbility += ActivatePassiveAbilitySlot;
        handler.RemoveAbility += DeactivatePassiveAbilitySlot;
        handler.AddSlots += AddSlots;
        handler.DeleteSlots += DeleteSlots;
        AddSlots(handler.AvailableSlots);
    }
    private void ActivatePassiveAbilitySlot()
    {
        foreach (var slotIndicator in slots.Where(slotIndicator => !slotIndicator.IsActivated))
        {
            slotIndicator.Activate();
            return;
        }
    }

    private void DeactivatePassiveAbilitySlot()
    {
        for (var i = slots.Count - 1; i >= 0; i--)
        {
            if (!slots[i].IsActivated)
                continue;
            slots[i].Deactivate();
            return;
        }
    }

    private void AddSlots(int numOfSlots)
    {
        for (int i = 0; i < numOfSlots; i++)
        {
            UI_PA_SlotIndicator newSlot = Instantiate(slot, Vector3.zero, Quaternion.identity);
            newSlot.transform.SetParent(contentPanel.transform);
            newSlot.transform.localScale = Vector3.one;
            slots.Add(newSlot);
        }
    }

    private void DeleteSlots(int numOfSlots)
    {
        for (int i = 0; i < numOfSlots; i++)
        {
            UI_PA_SlotIndicator deletedSlot = slots[i];
            slots.RemoveAt(slots.Count - i - 1);
            Destroy(deletedSlot);
        }
    }
}
