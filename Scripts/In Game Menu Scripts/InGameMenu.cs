using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private List<UIElementToManipulateWith> manipulateObjects;
    private void Awake()
    {
        //gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        foreach (var uiElement in manipulateObjects)
        {
            if(uiElement.ToggleUpWithEnablingMainMenu)
                uiElement.UIElement.SetActive(true);
            if(uiElement.ToggleDownWithEnablingMainMenu)
                uiElement.UIElement.SetActive(false);
        }
    }

    private void OnDisable()
    {
        foreach (var uiElement in manipulateObjects)
        {
            if(uiElement.ToggleUpWithDisablingMainMenu)
                uiElement.UIElement.SetActive(true);
            if(uiElement.ToggleDownWithDisablingMainMenu)
                uiElement.UIElement.SetActive(false);
        }
    }
}

[Serializable]
public class UIElementToManipulateWith
{
    [field: SerializeField] public GameObject UIElement { get; private set; }
    [field: Header("Toggling up")]
    [field: SerializeField] public bool ToggleUpWithEnablingMainMenu { get; private set; }
    [field: SerializeField] public bool ToggleUpWithDisablingMainMenu { get; private set; }
    [field: Header("Toggling down")]
    [field: SerializeField] public bool ToggleDownWithEnablingMainMenu { get; private set; }
    [field: SerializeField] public bool ToggleDownWithDisablingMainMenu { get; private set; }


}

