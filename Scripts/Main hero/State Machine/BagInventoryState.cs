using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

[Serializable]
public class BagInventoryState : BaseState
{
    #region Animation triggers

    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
    private static readonly int LastVertical = Animator.StringToHash("LastVertical");
    private static readonly int Speed = Animator.StringToHash("Speed");

    #endregion
    
    private Rigidbody2D _heroRigidbody;
    private HeroInventory _heroInventory;
    private DropBag _bag;
    private IMovable _moveScript;

    public BagInventoryState(GameObject hero)
    {

        _moveScript = hero.GetComponent<IMovable>();
        _heroInventory = hero.GetComponentInChildren<HeroInventory>();
        
        try
        {
            _heroRigidbody = hero.GetComponent<Rigidbody2D>();
        }
        catch (NullReferenceException)
        {
            Debug.Log("Hero does not have rigidbody component");
        }
    }
    
    public override void UpdateState(HeroStateHandler stateHandler)
    {
        HandleMovement();
        ListenForInputToExitBagInventoryState();
        ListerForInputToEnterInventoryState();
    }
    private void HandleMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(inputX, inputY).normalized;
        Animator.SetFloat(Horizontal, inputX);
        Animator.SetFloat(Vertical, inputY);
        if(Input.GetAxisRaw("Horizontal") == 1 || 
           Input.GetAxisRaw("Horizontal") == -1 || 
           Input.GetAxisRaw("Vertical") == 1 || 
           Input.GetAxisRaw("Vertical") == -1)
        {
            Animator.SetFloat(LastHorizontal, 
                inputX);
            Animator.SetFloat(LastVertical, 
                inputY);
        }

        if(StateHandler.HookingState.GetStateOfTheHook() == HookScript.HookState.Inactive)
        { 
            _heroRigidbody.velocity =
                new Vector2(
                    moveDirection.x * _moveScript.GetCurrentMoveSpeed(),
                    moveDirection.y * _moveScript.GetCurrentMoveSpeed());
        }
        else
        {
            _heroRigidbody.velocity =
                new Vector2(
                    moveDirection.x * _moveScript.GetCurrentMoveSpeed(),
                    moveDirection.y * _moveScript.GetCurrentMoveSpeed());
        }

        float moveMagnitude = (float)Math.Sqrt(Math.Pow(inputX, 2) + Math.Pow(inputY, 2));
        Animator.SetFloat(Speed, moveMagnitude);
    }

    private void ListenForInputToExitBagInventoryState()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;
        CloseBag();
        StateHandler.SwitchState(StateHandler.NormalState);
    }

    private void ListerForInputToEnterInventoryState()
    {
        if (!Input.GetKeyDown(KeyCode.G))
            return;
        CloseBag();
        StateHandler.SwitchState(StateHandler.InventoryState);
    }

    private void CloseBag()
    {
        _bag.UnsubscribeFromUIInventory();
        BagInventoryUI.Instance.CloseBagInventoryUI();
    }
    public void SetDropBag(DropBag dropBag) => _bag = dropBag;
}
