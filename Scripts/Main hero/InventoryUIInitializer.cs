using System.Collections;
using System.Collections.Generic;
using In_Game_Menu_Scripts.InventoryScripts;
using In_Game_Menu_Scripts.InventoryScripts.QuestPageUI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUIInitializer : MonoBehaviour
{
    [Header("Hero Object")] [SerializeField]
    private GameObject hero;
    
    [Header("UI Facade")]
    [SerializeField] private InventoryUIFacade inventoryUIFacadePrefab;
    [SerializeField] private InventoryUIFacade currentInventoryUIFacade;

    [Header("Hero Inventories")] 
    [SerializeField] private HeroInventory_Item itemInventory;
    [SerializeField] private HeroInventory pAbilityInventory;
    [SerializeField] private HeroInventory aAbilityInventory;
    [SerializeField] private HeroInventory_Quest questInventory;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentInventoryUIFacade is not null)
            return;
        var invFacadeOnScene = FindObjectOfType<InventoryUIFacade>();
        currentInventoryUIFacade = invFacadeOnScene ? 
            invFacadeOnScene : Instantiate(inventoryUIFacadePrefab, transform.position, Quaternion.identity);
        
        currentInventoryUIFacade.Initialize(hero);
        AssignNewUIInventoriesToControllers();
        currentInventoryUIFacade.CloseInventoryMenu();
    }

    private void AssignNewUIInventoriesToControllers()
    {
        itemInventory.AssignNewUIInventories(currentInventoryUIFacade.ItemInventoryUI, currentInventoryUIFacade.quickLotPanel);
        pAbilityInventory.AssignNewUIInventories(currentInventoryUIFacade.PassiveAbilityInvPage);
        itemInventory.ReassignTinderBox();
        //aAbilityInventory.AssignNewUIInventories(currentInventoryUIFacade.ActiveAbilityInvPage);
        questInventory.AssignNewUIInventories(currentInventoryUIFacade.QuestInventoryPage);
    }
    
}
