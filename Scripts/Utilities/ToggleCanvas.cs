using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class ToggleCanvas : MonoBehaviour
{
    [SerializeField] private GameObject contour;
    [SerializeField] private bool _canOpenBag = false;
    [SerializeField] private DropBagState currentState;
    [SerializeField] private ListenForInput inputListener;
    
    [Header("Message to Display")]
    [SerializeField] private MessageStructure message;

    public enum DropBagState
    {
        Idle,
        Opened,
        Destroying,
        OutOfRange,
        InRange
    }
    private void Start()
    {
        SwitchState(DropBagState.OutOfRange);
        contour.SetActive(false);
    }

    public void SwitchState(DropBagState state)
    {
        switch (state)
        {
            case DropBagState.Idle:
                if (currentState is DropBagState.Destroying)
                    return;
                CanOpenBag(true);
                message.SetKeyCode(inputListener.GetStringKeyCode());
                InteractMessageCanvas.Instance.DisplayMessage(message);
                break;
            case  DropBagState.Opened:
                inputListener.enabled = false;
                contour.SetActive(false);
                CanOpenBag(false);
                InteractMessageCanvas.Instance.ToggleOff();
                break;
            case DropBagState.Destroying:
                inputListener.enabled = false;
                contour.SetActive(false);
                CanOpenBag(false);
                InteractMessageCanvas.Instance.ToggleOff();
                break;
            case DropBagState.OutOfRange:
                inputListener.enabled = false;
                contour.SetActive(false);
                if (currentState is DropBagState.Destroying)
                    return;
                CanOpenBag(false);
                InteractMessageCanvas.Instance.ToggleOff();
                break;
            case DropBagState.InRange:
                if (currentState is DropBagState.Destroying)
                    return;
                CanOpenBag(true);
                break;
        }
        currentState = state;
    }
    
    private void OnMouseOver()
    {
        if (!_canOpenBag)
            return;
        SwitchState(DropBagState.Idle);
        inputListener.enabled = true;
        contour.SetActive(true);
    }

    private void OnMouseExit()
    {
        InteractMessageCanvas.Instance.ToggleOff();
        inputListener.enabled = false;
        contour.SetActive(false);
    }
    
    public void CanOpenBag(bool canOpen) => _canOpenBag = canOpen;

    public void OpenBag()
    {
        SwitchState(DropBagState.Opened);
    }

    public void CloseBag()
    {
        SwitchState(DropBagState.Idle);
    }

    public DropBagState GetState => currentState;
}
