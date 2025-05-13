using System;
using System.Collections;
using System.Collections.Generic;
using In_Game_Menu_Scripts.InventoryScripts;
using In_Game_Menu_Scripts.InventoryScripts.QuestPageUI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUIFacade : MonoBehaviour
{
    [SerializeField] private HeroInventoryMenu inventoryMenu;
    [field: Header("UI Inventories")]
    [field: SerializeField] public UIItemInventoryPage ItemInventoryUI { get; private set; }
    [field: SerializeField] public UI_PA_InventoryPage PassiveAbilityInvPage { get; private set; }
    [field: SerializeField] public UI_AA_InventoryPage ActiveAbilityInvPage { get; private set; }

    [field: SerializeField] public UIQuestInventoryPage QuestInventoryPage { get; private set; }

    [field: Header("QuickSlotPanel")] [field: SerializeField]
    public UIQuickSlotEquipmentPanel quickLotPanel;

    public static InventoryUIFacade GetInstance { get; private set; }

    private void Awake()
    {
        GetInstance = this;
    }
    public void Initialize(GameObject hero)
    {
        ItemInventoryUI.ReassignInventories(hero);
        PassiveAbilityInvPage.ReassignInventories(hero);
        ActiveAbilityInvPage.ReassignInventories(hero);
        QuestInventoryPage.ReassignInventories(hero);
    }

    public void OpenInventoryMenu()
    {
        inventoryMenu.Open();
        QuickSlotPanel.GetInstance.ToggleOff();
        //AbilityUIPanel.GetInstance.ToggleOff();
    }

    public void CloseInventoryMenu()
    {
        inventoryMenu.Close();
        try
        {
            QuickSlotPanel.GetInstance.ToggleOn();
            //AbilityUIPanel.GetInstance.ToggleOn();
        }
        catch (NullReferenceException)
        {
            //
        }
        
    }
}
