using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class AbilityState : BaseState
{
    [SerializeField] private AbilityHandler abilityHandler;

    private Rigidbody2D _rigidbody;
    
    #region Animation triggers
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
    private static readonly int LastVertical = Animator.StringToHash("LastVertical");
    private static readonly int Speed = Animator.StringToHash("Speed");
    #endregion

    #region Input actions

    [SerializeField] private InputAction move;
    [SerializeField] private InputAction openInventory;

    #endregion

    private IMovable _moveScript;
    
    public override void InitializeState(GameObject hero, HeroStateHandler stateHandler, Transform heroTransform,
        Animator animator,PlayerInput playerInput)
    {
        base.InitializeState(hero, stateHandler, heroTransform, animator, playerInput);
        _moveScript = Hero.GetComponent<IMovable>();
        abilityHandler = Hero.GetComponentInChildren<AbilityHandler>();
        _rigidbody = Hero.GetComponent<Rigidbody2D>();
        
        openInventory = playerInput.currentActionMap.FindAction("OpenInventory");
        move = playerInput.currentActionMap.FindAction("Movement");
    }

    public override void EnterState()
    {
        base.EnterState();
        SubscribeOnActionEvents();
    }

    public override void UpdateState(HeroStateHandler stateHandler)
    {
        HandleMovement();
    }

    private void HandleOpenGameMenu(InputAction.CallbackContext context)
    {
        StateHandler.SwitchState(StateHandler.InventoryState);
    }

    private void HandleMovement()
    {
        var inputVector = move.ReadValue<Vector2>();
            
        Vector2 moveDirection = inputVector.normalized;
            
        Animator.SetFloat(Horizontal, inputVector.x);
        Animator.SetFloat(Vertical, inputVector.y);
            
        if (inputVector.x != 0 || inputVector.y != 0)
        {
            Animator.SetFloat(LastHorizontal, inputVector.x);
            Animator.SetFloat(LastVertical, inputVector.y);
        }
            
        _rigidbody.velocity = moveDirection * _moveScript.GetCurrentMoveSpeed();
            
        float moveMagnitude = inputVector.magnitude;
        Animator.SetFloat(Speed, moveMagnitude);
    }

    public override void ExitState()
    {
        base.ExitState();
        UnsubscribeFromActionEvents();
    }
    
    public override void SubscribeOnActionEvents()
    {
        base.SubscribeOnActionEvents();
        openInventory.performed += HandleOpenGameMenu;
    }

    public override void UnsubscribeFromActionEvents()
    {
        base.UnsubscribeFromActionEvents();
        openInventory.performed -= HandleOpenGameMenu;
    }
    
}
