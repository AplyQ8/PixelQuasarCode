using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main_hero;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class AbilityHandler : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap _abilityActionMap;
    
    [Header("UI References")] [SerializeField]
    private HeroStatUIReferences uiReferences;
    [SerializeField] private AbilityUIPanel currentUIPanel;
    
    [Header("Hero References")]
    [SerializeField] private GameObject hero;
    [SerializeField] private HeroStateHandler stateHandler;

    [Header("Ability Information")]
    [SerializeField] private List<AbilityInformation> abilityList =  new List<AbilityInformation>();

    [Header("Initial Abilities")] [SerializeField]
    private List<Ability> initialAbilities = new List<Ability>();
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Находим карту действий для героя
        _abilityActionMap = inputActions.FindActionMap("Player");
    
        // Получаем действие "Abilities" из карты действий "Hero"
        var abilityAction = _abilityActionMap.FindAction("AbilityTriggers");

        if (abilityAction != null)
        {
            PopulateAbilityList(abilityAction); // Автоматически заполняем список способностей
            abilityAction.performed += TriggerAbility; // Подписка на выполнение действия
        }
        InitializeAbilitiesFromInitialList();
    }
    private void TriggerAbility(InputAction.CallbackContext context)
    {
        if(stateHandler.CanUseAbility())
        {
            var bindingIndex = context.action.GetBindingIndexForControl(context.control);
            var actionName = context.action.bindings[bindingIndex].path;

            var abilityInfo = abilityList.FirstOrDefault(info => info.ActionName == actionName);
            if (abilityInfo != null)
            {
                currentUIPanel.TriggerInput(actionName);
            }
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GetUIInstance();
    }
    private void GetUIInstance()
    {
        if (currentUIPanel is not null)
            return;
        var abilityUIPanelOnCurrentScene = FindObjectOfType<AbilityUIPanel>();
        if (abilityUIPanelOnCurrentScene == null)
        {
            //currentUIPanel = Instantiate(abilityPanelPrefab, transform.position, Quaternion.identity);
            currentUIPanel = uiReferences.AbilityUIPanel;
            InitializeSlot();
            AssignAbilitiesToSlots();
        }
        else
        {
            currentUIPanel = abilityUIPanelOnCurrentScene;
            InitializeSlot();
            AssignAbilitiesToSlots();
        }
        
    }
    private void InitializeSlot()
    {
        var actionNames = abilityList.Select(info => info.ActionName).ToList();
        currentUIPanel.InitializeActivationButtons(actionNames);
    }
    private void AssignAbilitiesToSlots()
    {
        foreach (var information in abilityList)
        {
            currentUIPanel.InitializeSlot(
                information.ActionName,  // Передаем путь привязки
                information.Ability,
                hero,
                stateHandler
            );
        }
    }
    
    private void InitializeAbilitiesFromInitialList()
    {
        // Список для слотов, где способности еще не установлены
        var slotsToFill = abilityList.Where(info => info.Ability == null).ToList();
    
        if (slotsToFill.Count == 0)
        {
            Debug.LogWarning("No available slots to initialize abilities.");
            return;
        }

        // Создайте копию initialAbilities для безопасного изменения
        var abilitiesToInitialize = new List<Ability>(initialAbilities);

        // Проходим по слотам и заполняем их способностями
        foreach (var slotInfo in slotsToFill)
        {
            if (abilitiesToInitialize.Count == 0)
                break;

            // Получите первую способность из списка
            var ability = abilitiesToInitialize[0];
            abilitiesToInitialize.RemoveAt(0);

            // Обновляем AbilityInformation для слота
            slotInfo.SetAbility(ability);
        }

        // Обновляем UI, чтобы отразить изменения
        UpdateUIWithAbilities();

        // Если осталось еще способностей и слоты заполнены, их можно игнорировать или обрабатывать по-другому
        if (abilitiesToInitialize.Count > 0)
        {
            Debug.LogWarning($"Some abilities from initialAbilities were not added due to slot limitations.");
        }
    }

    private void UpdateUIWithAbilities()
    {
        // Обновляем UI панель, чтобы отразить изменения
        var actionNames = abilityList.Select(info => info.ActionName).ToList();
        currentUIPanel.InitializeActivationButtons(actionNames);

        foreach (var info in abilityList)
        {
            if (info.Ability != null)
            {
                currentUIPanel.InitializeSlot(
                    info.ActionName, 
                    info.Ability,
                    hero,
                    stateHandler
                );
            }
        }
    }
    
    private void PopulateAbilityList(InputAction abilityAction)
    {
        abilityList.Clear();

        foreach (var binding in abilityAction.bindings)
        {
            var actionName = binding.path;

            // Если уже существует Ability с таким же ActionName, обновляем его
            var abilityInfo = abilityList.FirstOrDefault(info => info.ActionName == actionName);
            if (abilityInfo == null)
            {
                abilityInfo = new AbilityInformation(actionName);
                abilityList.Add(abilityInfo);
            }
        }

        // Обновляем UI
        var actionNames = abilityList.Select(info => info.ActionName).ToList();
        currentUIPanel.InitializeActivationButtons(actionNames);
    }
    /// <summary>
    /// Updates bindings
    /// </summary>
    private void UpdateBindings()
    {
        var abilityAction = _abilityActionMap.FindAction("AbilityTriggers");

        if (abilityAction != null)
        {
            PopulateAbilityList(abilityAction);
            AssignAbilitiesToSlots();
        }
    }
    public void AddAbility(Ability newAbility, string actionName)
    {
        // Шаг 1: Находим AbilityInformation по ActionName
        var existingAbilityInfo = abilityList.FirstOrDefault(info => info.ActionName == actionName);

        if (existingAbilityInfo == null)
        {
            Debug.LogWarning($"No action found with ActionName {actionName}. Ability not added.");
            return;
        }

        // Шаг 2: Проверяем, не занята ли уже способность
        if (existingAbilityInfo.Ability != null)
        {
            Debug.LogWarning($"Slot with ActionName {actionName} is already occupied by another ability.");
            return;
        }

        // Шаг 3: Записываем новую способность в найденный слот
        existingAbilityInfo.SetAbility(newAbility);

        // Шаг 4: Обновляем UI, если он инициализирован
        if (currentUIPanel != null)
        {
            // Инициализируем новый слот в UI
            currentUIPanel.InitializeSlot(
                actionName,  // Используем ActionName для идентификации
                newAbility,
                hero,
                stateHandler
            );
        }
        else
        {
            Debug.LogWarning("currentUIPanel is not initialized.");
        }
    }

    public void RemoveAbility(string actionName)
    {
        // Шаг 1: Находим AbilityInformation по ActionName
        var existingAbilityInfo = abilityList.FirstOrDefault(info => info.ActionName == actionName);

        if (existingAbilityInfo == null)
        {
            Debug.LogWarning($"No action found with ActionName {actionName}. Unable to remove ability.");
            return;
        }

        // Шаг 2: Проверяем, занята ли ячейка способностью
        if (existingAbilityInfo.Ability == null)
        {
            Debug.LogWarning($"No ability assigned to ActionName {actionName}.");
            return;
        }
        
        //Шаг 3. Проверяем можем ли мы удалить способность
        if (!currentUIPanel.CanRemoveAbility(actionName))
        {
            Debug.Log($"Can not remove ability {actionName} right now");
            return;
        }

        // Шаг 4: Освобождаем слот, приравнивая Ability к null
        existingAbilityInfo.SetAbility(null);

        // Шаг 5: Обновляем UI, если он инициализирован
        if (currentUIPanel != null)
        {
            // Обновляем слот в UI, чтобы отразить отсутствие способности
            currentUIPanel.InitializeSlot(
                actionName,  // Используем ActionName для идентификации
                null,        // Передаем null, чтобы очистить слот
                hero,
                stateHandler
            );
        }
        else
        {
            Debug.LogWarning("currentUIPanel is not initialized.");
        }
    }


}

[Serializable]
public class AbilityInformation
{
    [field: SerializeField] public Ability Ability { get; private set; }
    [field: SerializeField] public string ActionName { get; set; } // Путь привязки

    public AbilityInformation(Ability ability, string actionName)
    {
        Ability = ability;
        ActionName = actionName;
    }

    public AbilityInformation(string actionName)
    {
        ActionName = actionName;
    }

    public void SetAbility(Ability ability) => Ability = ability;
}

