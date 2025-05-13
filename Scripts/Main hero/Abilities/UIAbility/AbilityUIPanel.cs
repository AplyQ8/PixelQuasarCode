using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class AbilityUIPanel : MonoBehaviour
{
    [SerializeField] private GameObject sortingPanel;

    public List<UIAbilitySlot> UIAbilitySlotsabilitySlots;
    
    [SerializedDictionary("KeyCode", "AbilitySlot")]
    public SerializedDictionary<string, UIAbilitySlot> abilitySlots;
    [SerializeField] [CanBeNull] private UIAbilitySlot currentActivatedAbility;

    public static AbilityUIPanel GetInstance { get; private set; }

    private void Start()
    {
        GetInstance = this;
    }

    public void InitializeSlot(string actionName, Ability ability, GameObject hero, HeroStateHandler heroStateHandler)
    {
        if (abilitySlots.ContainsKey(actionName))
        {
            abilitySlots[actionName].Initialize(ability, hero, heroStateHandler);
            abilitySlots[actionName].OnAbilityStateChange += AbilityStateChangeEvent;
        }
        else
        {
            Debug.LogWarning($"No UI slot found for action {actionName}");
        }
    }

    public void InitializeActivationButtons(List<string> actButtons)
    {
        if (actButtons.Count > UIAbilitySlotsabilitySlots.Count)
            throw new Exception("Ability UI Panel: abilities more than expected");

        abilitySlots.Clear(); // Очищаем и пересоздаем словарь привязок

        for (int i = 0; i < actButtons.Count; i++)
        {
            abilitySlots.Add(actButtons[i], UIAbilitySlotsabilitySlots[i]);
        }
    }

    public void TriggerInput(string actionName)
    {
        if (!abilitySlots.ContainsKey(actionName))
        {
            Debug.LogWarning($"No ability slot found for action {actionName}");
            return;
        }

        if (currentActivatedAbility != null && currentActivatedAbility != abilitySlots[actionName])
        {
            currentActivatedAbility.ResetState();
        }
        abilitySlots[actionName].TriggerInput();
    }

    private void AbilityStateChangeEvent(AbilityHolder.AbilityState currentAbilityState, UIAbilitySlot abilitySlot)
    {
        currentActivatedAbility = currentAbilityState == AbilityHolder.AbilityState.Clicked ? abilitySlot : null;
    }

    public void DeactivateWaitingActionAbility()
    {
        if (currentActivatedAbility == null)
            return;

        foreach (var uiAbilitySlot in abilitySlots.Where(uiAbilitySlot => uiAbilitySlot.Value == currentActivatedAbility))
        {
            uiAbilitySlot.Value.ResetState();
        }
        currentActivatedAbility = null;
    }
    // Существующие поля и методы...

    /// <summary>
    /// Деактивирует способность по ActionName.
    /// </summary>
    /// <param name="actionName">Имя действия для деактивации способности.</param>
    public void DeactivateAbility(string actionName)
    {
        if (abilitySlots.TryGetValue(actionName, out var slot))
        {
            slot.ResetState(); // Метод для сброса состояния слота
        }
        else
        {
            Debug.LogWarning($"No ability slot found for ActionName: {actionName}");
        }
    }

    public void ToggleOn()
    {
        sortingPanel.SetActive(true);
    }

    public void ToggleOff()
    {
        sortingPanel.SetActive(false);
    }

    public bool CanRemoveAbility(string actionName)
    {
        return abilitySlots[actionName].CanRemoveAbility();
    }
}

