using System;
using System.Collections.Generic;
using System.Linq;
using ObjectLogicInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

//Состояние при котором объект может передвигаться
namespace Main_hero.State_Machine
{
    [Serializable]
    public class NormalState : BaseState
    {
        private Rigidbody2D _rigidbody;
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
        private static readonly int LastVertical = Animator.StringToHash("LastVertical");
        private static readonly int Speed = Animator.StringToHash("Speed");

        #region Input Actions

        [SerializeField] private InputAction openInventory;
        [SerializeField] private InputAction dash;
        [SerializeField] private InputAction hook;
        [SerializeField] private InputAction attack;
        [SerializeField] private InputAction move;
        [SerializeField] private InputAction superAttack;

        #endregion
        
        
        private IMovable _moveScript;
        private bool _superAttackTriggerIsPressed;

        public override void InitializeState(GameObject hero, HeroStateHandler stateHandler, Transform heroTransform,
            Animator animator, PlayerInput playerInput)
        {
            base.InitializeState(hero, stateHandler, heroTransform, animator, playerInput);
            
            _moveScript = Hero.GetComponent<IMovable>();
            _rigidbody = Hero.GetComponent<Rigidbody2D>();
            
            openInventory = playerInput.currentActionMap.FindAction("OpenInventory");

            dash = playerInput.currentActionMap.FindAction("Dash");

            hook = playerInput.currentActionMap.FindAction("Hook");

            attack = playerInput.currentActionMap.FindAction("Attack");

            move = playerInput.currentActionMap.FindAction("Movement");
            superAttack = playerInput.currentActionMap.FindAction("SuperAttackTrigger");
            _superAttackTriggerIsPressed = false;
            // superAttack.started += HandleSuperAttackTriggerIsPressed;
            // superAttack.canceled += HandleSuperAttackTriggerIsReleased;
            
            SubscribeOnActionEvents();
        }

        public override void EnterState()
        {
            base.EnterState();
            SubscribeOnActionEvents();
            _superAttackTriggerIsPressed = false;
        }

        public override void UpdateState(HeroStateHandler stateHandler)
        {
            if (isPaused) return;
            HandleAttacking();
            HandleMovement();
        }

        public override void ExitState()
        {
            base.ExitState();
            UnsubscribeFromActionEvents();
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

        private void HandleAttacking(/*InputAction.CallbackContext context*/)
        {
            // if (_superAttackTriggerIsPressed || superAttack.IsPressed())
            // {
            //     StateHandler.SwitchState(StateHandler.SuperAttackState);
            //     return;
            // }
            // StateHandler.SwitchState(StateHandler.AttackingState);
            if(attack.IsPressed())
                StateHandler.SwitchState(StateHandler.SuperAttackState);
            // if (_superAttackTriggerIsPressed)
            // {
            //     StateHandler.SwitchState(StateHandler.SuperAttackState);
            //     return;
            // }
            // StateHandler.SwitchState(StateHandler.AttackingState);
        }
        private void HandleHooking(InputAction.CallbackContext context)
        {
            StateHandler.SwitchState(StateHandler.HookingState);
        }
        private void HandleOpenInGameMenu(InputAction.CallbackContext context)
        {
            StateHandler.SwitchState(StateHandler.InventoryState);

        }
        private void HandleDash(InputAction.CallbackContext context)
        {
            StateHandler.SwitchState(StateHandler.DashState);
        }

        private void HandleSuperAttackTriggerIsPressed(InputAction.CallbackContext context)
        {
            _superAttackTriggerIsPressed = true;
        }
        private void HandleSuperAttackTriggerIsReleased(InputAction.CallbackContext context)
        {
            _superAttackTriggerIsPressed = false;
        }

        public override void SubscribeOnActionEvents()
        {
            base.SubscribeOnActionEvents();
            openInventory.performed += HandleOpenInGameMenu;
            dash.performed += HandleDash;
            hook.performed += HandleHooking;
            //attack.performed += HandleAttacking;
            superAttack.started += HandleSuperAttackTriggerIsPressed;
            superAttack.canceled += HandleSuperAttackTriggerIsReleased;
        }

        public override void UnsubscribeFromActionEvents()
        {
            base.UnsubscribeFromActionEvents();
            openInventory.performed -= HandleOpenInGameMenu;
            dash.performed -= HandleDash;
            hook.performed -= HandleHooking;
            //attack.performed -= HandleAttacking;
            superAttack.started -= HandleSuperAttackTriggerIsPressed;
            superAttack.canceled -= HandleSuperAttackTriggerIsReleased;
        }

    }
}
