using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class HintScript : MonoBehaviour
{
    [Header("GUI")]
    [SerializeField] private HintCanvas hintUI;
    [SerializeField] private ParticleSystem particles; 
   
    [Header("Info")]
    [SerializeField] private LocalizedString hintMessage;
    [SerializeField] private PlayerInput playerInput;
    
    private bool _hasReadHint = false;
    private bool _inRange = false;
    
    private void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        hintMessage.StringChanged += SetHintText;
    }

    private void OnDisable()
    {
        hintMessage.StringChanged -= SetHintText;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _inRange = true;
        
        SetHintText(hintMessage.GetLocalizedString());
        
        // Останавливаем партикли
        if (particles.isPlaying)
        {
            particles.Stop();
        }

        // Устанавливаем флаг, что подсказка была прочитана
        _hasReadHint = true;
        
    }

    private void SetHintText(string localizedHint)
    {
        if (!_inRange) return;
        // Обновляем подсказку, заменяя все плейсхолдеры
        string formattedMessage = ReplaceActionPlaceholders(localizedHint);

        // Показываем обновленную подсказку на экране
        hintUI.SetText(formattedMessage);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // Убираем подсказку, когда игрок уходит
        hintUI.DisableHint();
        _inRange = false;

    }
    
    private string ReplaceActionPlaceholders(string message)
    {
        // Используем регулярное выражение для поиска плейсхолдеров вида {ActionName}
        return Regex.Replace(message, @"\{(\w+)\}", match =>
        {
            // Получаем название действия из плейсхолдера (например, OpenInventory)
            string actionName = match.Groups[1].Value;

            // Находим соответствующую клавишу для этого действия
            string keyToPress = GetKeyBinding(actionName);
        
            // Возвращаем клавишу вместо плейсхолдера
            return keyToPress;
        });
    }
    
    private string GetKeyBinding(string actionName)
    {
        var action = playerInput.actions.FindAction(actionName);

        if (action != null && action.bindings.Count > 0)
        {
            // Если это QuickSlots, обрабатываем привязки для слотов
            if (actionName == "QuickSlot")
            {
                string bindings = "";
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    var binding = action.bindings[i];
                    bindings += InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                    if (i < action.bindings.Count - 1)
                    {
                        bindings += ", "; // Добавляем запятую между привязками
                    }
                }
                return bindings; // Возвращаем все привязки для quickslots
            }
            else if (action.bindings[0].isComposite)
            {
                string up = GetCompositeBinding(action, "up");
                string down = GetCompositeBinding(action, "down");
                string left = GetCompositeBinding(action, "left");
                string right = GetCompositeBinding(action, "right");
                return $"{up} {left} {down} {right}";
            }
            else
            {
                var binding = action.bindings[0];
                return InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
        }

        return "Unknown"; // Если действие не найдено
    }
    
    // Метод для получения конкретной привязки внутри композитного ввода
    private string GetCompositeBinding(InputAction action, string compositePart)
    {
        // Ищем привязку для определенной части композита (up, down, left, right)
        var bindingIndex = action.bindings.IndexOf(b => b.isPartOfComposite && b.name == compositePart);

        if (bindingIndex != -1)
        {
            var binding = action.bindings[bindingIndex];
            return InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }

        return "?"; // Если привязка не найдена
    }
    
}
