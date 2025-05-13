using System.Text.RegularExpressions;
using UnityEngine.InputSystem;

public class ActionReplacer
{
    private PlayerInput _playerInput;
    private static ActionReplacer _instance;
    public static ActionReplacer Instance => _instance ??= new ActionReplacer();

    private ActionReplacer()
    { }
    
    public string ReplaceActionPlaceholders(string message, PlayerInput playerInput)
    {
        // Используем регулярное выражение для поиска плейсхолдеров вида {ActionName}
        return Regex.Replace(message, @"\{(\w+)\}", match =>
        {
            // Получаем название действия из плейсхолдера (например, OpenInventory)
            string actionName = match.Groups[1].Value;

            // Находим соответствующую клавишу для этого действия
            string keyToPress = GetKeyBinding(actionName, playerInput);
        
            // Возвращаем клавишу вместо плейсхолдера
            return keyToPress;
        });
    }
    private string GetKeyBinding(string actionName, PlayerInput playerInput)
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