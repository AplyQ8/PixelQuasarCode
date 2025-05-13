using System;
using System.Collections;
using System.Collections.Generic;
using In_Game_Menu_Scripts.InventoryScripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickSlotPanel : MonoBehaviour
{
    private Dictionary<string, UIQuickSlot> _quickSlots = new Dictionary<string, UIQuickSlot>();
    [SerializeField] private UIQuickSlot quickSlotsPrefab;
    [SerializeField] private GameObject sortingPanel;
    public static QuickSlotPanel GetInstance { get; private set; }

    private void Awake()
    {
        
        
    }
    private void Start()
    {
        GetInstance = this;
        

    }
    public void Initialize(List<string> triggers, HeroInventory_Item heroInventory)
    {
        // for (int i = 0; i < numOfQuickSlots; i++)
        // {
        //     var newSlot = Instantiate(quickSlotsPrefab, Vector3.zero, Quaternion.identity);
        //     newSlot.transform.SetParent(sortingPanel.transform);
        //     //Add to dictionary
        //     newSlot.SetActivationKeyCodeName(triggers[i]);
        //     _quickSlots.Add(triggers[i], newSlot);
        //     heroInventory.AddUIItemEquipmentSlots(newSlot);
        //     newSlot.transform.localScale = Vector3.one;
        // }

        foreach (var trigger in triggers)
        {
            var newSlot = Instantiate(quickSlotsPrefab, Vector3.zero, Quaternion.identity);
            newSlot.transform.SetParent(sortingPanel.transform);
            
            newSlot.SetActivationKeyCodeName(trigger);
            _quickSlots.Add(trigger, newSlot);
            heroInventory.AddUIItemEquipmentSlots(newSlot);
            newSlot.transform.localScale = Vector3.one;
        }
    }

    public void TriggerInput(string keyCode)
    {
        try
        {
            _quickSlots[keyCode].TriggerInput();
        }
        catch (KeyNotFoundException)
        {
            Debug.Log($"There is no item bind with this keyCode: {keyCode}");
        }
        
    }

    public void ToggleOn()
    {
        try
        {
            sortingPanel.SetActive(true);
        }
        catch (MissingReferenceException)
        {
            //Object has been deleted
        }
        
    }

    public void ToggleOff()
    {
        try
        {
            sortingPanel.SetActive(false);
        }
        catch (MissingReferenceException)
        {
            //Object has been deleted
        }
    }
}
