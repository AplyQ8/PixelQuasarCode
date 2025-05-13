using System;
using DialogScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Main_hero.State_Machine
{
    [Serializable]
    public class DialogueState: BaseState
    {
        
        private Rigidbody2D _rigidbody;
        private static readonly int Speed = Animator.StringToHash("Speed");
        #region InputActions
        [SerializeField] private InputAction nextDialogue;
        #endregion

        public override void InitializeState(GameObject hero, HeroStateHandler stateHandler, Transform heroTransform,
            Animator animator, PlayerInput playerInput)
        {
            base.InitializeState(hero, stateHandler, heroTransform, animator, playerInput);
            nextDialogue = playerInput.currentActionMap.FindAction("NextDialogue");
            _rigidbody = hero.GetComponent<Rigidbody2D>();
        }

        public override void EnterState()
        {
            base.EnterState();
            _rigidbody.velocity = new Vector2(0, 0);
            Animator.SetFloat(Speed, 0);
            SubscribeOnActionEvents();
        }
        

        public override void ExitState()
        {
            base.ExitState();
            UnsubscribeFromActionEvents();
        }

        private void NextDialogue(InputAction.CallbackContext context)
        {
            DialogueManager.Instance.DisplayNextLine();
        }
        
        
        public override void SubscribeOnActionEvents()
        {
            base.SubscribeOnActionEvents();
            nextDialogue.performed += NextDialogue;
        }
        public override void UnsubscribeFromActionEvents()
        {
            base.UnsubscribeFromActionEvents();
            nextDialogue.performed -= NextDialogue;
        }
    }
}