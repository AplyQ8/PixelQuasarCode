using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using In_Game_Menu_Scripts;
using In_Game_Menu_Scripts.InventoryScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class QuickSlotData : MonoBehaviour
{
    [field: SerializeField] public int AvailableQuickSlots { get; private set; }
    [SerializeField] private QuickSlotPanel quickSlotPanelPrefab;
    [SerializeField] private HeroStateHandler heroStateHandler;
    [SerializeField] private QuickSlotPanel currentQuickSlotPanel;
    [SerializeField] private HeroInventory_Item itemInventory;
    [SerializeField] private List<string> keyCodes;
    
    [SerializeField] private InputActionAsset actionMap;
    [SerializeField] private HeroStateHandler _stateHandler;
    private InputActionMap _quickSlotActionMap;
    private PlayerInput _playerInput;
    private InputAction qSAction;

    private void Awake()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
        
        //_quickSlotActionMap = actionMap.FindActionMap("Player");
        qSAction = _playerInput.currentActionMap.FindAction("QuickSlot");

        // if (qSAction == null) return;
        //
        // PopulateQuickSlotList(qSAction);
        // SubscribeOnActionEvents();
        if (qSAction != null)
        {
            PopulateQuickSlotList(qSAction);
            SubscribeOnActionEvents();
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        _stateHandler.InventoryState.OnEnteredState += UnSubscribeFromActionEvents;
        _stateHandler.InventoryState.OnExitState += SubscribeOnActionEvents;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        PauseController.Instance.OnPause += PauseEvent;
        // if (currentQuickSlotPanel is not null)
        //     return;
        // var quickSlotPanelOnScene = FindObjectOfType<QuickSlotPanel>();
        // if (quickSlotPanelOnScene is null)
        // {
        //     currentQuickSlotPanel = Instantiate(quickSlotPanelPrefab, transform.position, Quaternion.identity);
        // }
        // else
        // {
        //     currentQuickSlotPanel = quickSlotPanelOnScene;
        // }
        //InitializeQuickSlotPanel();
        if (currentQuickSlotPanel == null)
        {
            var quickSlotPanelOnScene = FindObjectOfType<QuickSlotPanel>();
            currentQuickSlotPanel = quickSlotPanelOnScene ? quickSlotPanelOnScene : Instantiate(quickSlotPanelPrefab, transform.position, Quaternion.identity);
        }
        
    }

    private void OnSceneUnloaded(Scene scene)
    {
        //PauseController.Instance.OnPause -= PauseEvent;
        if (PauseController.Instance != null)
            PauseController.Instance.OnPause -= PauseEvent;

        UnSubscribeFromActionEvents();

        // Убедиться, что ссылки на старые объекты удалены
        currentQuickSlotPanel = null;
        qSAction = null;
    }
    
    public void InitializeQuickSlotPanel()
    {
        try
        {
            var quickSlotPanels = FindObjectsOfType<QuickSlotPanel>();
            foreach (var slotPanel in quickSlotPanels)
            {
                Destroy(slotPanel);
            }
        }
        catch (NullReferenceException)
        {
            //No slot panel
        }
        
        
        currentQuickSlotPanel = Instantiate(quickSlotPanelPrefab, transform.position, Quaternion.identity);
        currentQuickSlotPanel.Initialize(keyCodes, itemInventory);
    }

    private void TriggerInput(InputAction.CallbackContext context)
    {
        if (!heroStateHandler.CanUseQuickSlot()) return;
        var bindingIndex = context.action.GetBindingIndexForControl(context.control);
        var actionName = context.action.bindings[bindingIndex].path;

        var qsInfo = keyCodes.FirstOrDefault(info => info.Equals(actionName));

        if (qsInfo != null)
        {
            currentQuickSlotPanel.TriggerInput(actionName);
        }
        
        
    }

    private void PopulateQuickSlotList(InputAction qSAction)
    {
        keyCodes.Clear();

        foreach (var binding in qSAction.bindings)
        {
            var actionName = binding.path;
            keyCodes.Add(actionName);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        if (PauseController.Instance != null)
            PauseController.Instance.OnPause -= PauseEvent;
        UnSubscribeFromActionEvents();
        _stateHandler.InventoryState.OnEnteredState -= UnSubscribeFromActionEvents;
        _stateHandler.InventoryState.OnExitState -= SubscribeOnActionEvents;
    }

    private void OnEnable()
    {
        SubscribeOnActionEvents();
    }

    private void OnDisable()
    {
        UnSubscribeFromActionEvents();
    }

    private void PauseEvent(bool isPaused)
    {
        if(isPaused)
            UnSubscribeFromActionEvents();
        else
        {
            SubscribeOnActionEvents();
        }
    }
    private void SubscribeOnActionEvents()
    {
        if (qSAction != null)
        {
            qSAction.performed += TriggerInput;
        }
    }

    private void UnSubscribeFromActionEvents()
    {
        if (qSAction != null)
        {
            qSAction.performed -= TriggerInput;
        }
    }
    
}
