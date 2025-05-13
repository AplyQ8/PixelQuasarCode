using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ListenForInput : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputAction openBag;

    public UnityEvent triggerEvent;

    private void Awake()
    {
        openBag = playerInput.currentActionMap.FindAction("OpenLootBag");
    }
    private void TriggerOpenEvent(InputAction.CallbackContext context)
    {
        triggerEvent?.Invoke();
        
    }

    private void OnEnable()
    {
        openBag.performed += TriggerOpenEvent;
    }

    private void OnDisable()
    {
        openBag.performed -= TriggerOpenEvent;
    }

    public string GetStringKeyCode()
    {
        return openBag.controls.Count > 0 ? string.Join(", ", openBag.controls.Select(control => control.name)) : string.Empty;
    }

}
