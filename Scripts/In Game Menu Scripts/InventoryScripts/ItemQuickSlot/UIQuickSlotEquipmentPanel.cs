using System.Collections;
using System.Collections.Generic;
using In_Game_Menu_Scripts.InventoryScripts;
using In_Game_Menu_Scripts.InventoryScripts.ItemQuickSlot;
using UnityEngine;

public class UIQuickSlotEquipmentPanel : MonoBehaviour
{
    [SerializeField] private List<UIItemEquipmentSlot> itemEquipmentSlots;
    [SerializeField] private UIItemEquipmentSlot itemEquipmentSlotPrefab;
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private UIItemInventoryPage inventoryUI;

    public void AddEquipmentSlot(UIQuickSlot quickSlot)
    {
        var newEquipmentSlot = Instantiate(itemEquipmentSlotPrefab, Vector3.zero, Quaternion.identity);
        newEquipmentSlot.transform.SetParent(contentPanel.transform);
        newEquipmentSlot.ConnectToQuickSlot(quickSlot);
        inventoryUI.AddEquipmentSlot(newEquipmentSlot);
        itemEquipmentSlots.Add(newEquipmentSlot);
        newEquipmentSlot.transform.localScale = Vector3.one;
    }

}
