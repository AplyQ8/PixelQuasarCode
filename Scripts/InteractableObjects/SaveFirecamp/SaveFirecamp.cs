using System;
using System.Text.RegularExpressions;
using ObjectLogicInterfaces;
using PickableObjects.InventoryItems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using IInteractable = Envirenmental_elements.IInteractable;

namespace InteractableObjects.SaveFirecamp
{
    public class SaveFirecamp : MonoBehaviour, IInteractable, IDistanceCheckable, IMouseHoverable
    {
        [Header("Links")]
        [SerializeField] private TinderBoxSo tinderBox;
        [SerializeField] private HintCanvas messageCanvas;
        [SerializeField] private Animator animator;
        
        [Header("Messages")]
        [TextArea(3,5)]
        [SerializeField] private string interactTextField;
        [TextArea(3,5)]
        [SerializeField] private string confirmTextField;
        [TextArea(3,5)]
        [SerializeField] private string successfulSaveAttemptTextField;
        [TextArea(3,5)]
        [SerializeField] private string unsuccessfulSaveAttemptTextField;

        [field: Header("Save Cost")]
        [field: SerializeField]
        public float SaveCost { get; private set; } = 1f;

        [SerializeField] private Transform savePoint;
        
        public bool CanBeInteractedWith { get; private set; }
        public bool IsMouseOver { get; private set; }

        private SaveFireCampStates _currentState;
        private PlayerInput _playerInput;

        #region Events

        public Action MouseEnter, MouseExit;
        private static readonly int FireUp = Animator.StringToHash("FireUp");

        #endregion

        private enum SaveFireCampStates
        {
            NonInteractable,
            Interactable,
            Interacted,
            PostInteraction
        }

        private void Start()
        {
            
            _playerInput = FindObjectOfType<PlayerInput>();
            _currentState = SaveFireCampStates.NonInteractable;
        }
        
        private void Update()
        {
            HandleMouseOver();
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Player")) return;
            CanBeInteractedWith = true;
            ChangeState(SaveFireCampStates.Interactable);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            CanBeInteractedWith = false;
            ChangeState(SaveFireCampStates.NonInteractable);
        }
        
        private void ChangeState(SaveFireCampStates state)
        {
            switch (state)
            {
                case SaveFireCampStates.NonInteractable:
                    messageCanvas.DisableHint();
                    _currentState = SaveFireCampStates.NonInteractable;
                    break;
                case SaveFireCampStates.Interactable:
                    messageCanvas.SetText(ReplacePlaceholders(interactTextField));
                    _currentState = SaveFireCampStates.Interactable;
                    break;
                case SaveFireCampStates.Interacted:
                    messageCanvas.SetText(ReplacePlaceholders(confirmTextField));
                    _currentState = SaveFireCampStates.Interacted;
                    break;
                case SaveFireCampStates.PostInteraction:
                    _currentState = SaveFireCampStates.PostInteraction;
                    break;
            }
        }
        
        public void Interact()
        {
            if (!CanBeInteractedWith) return;
            switch (_currentState)
            {
                case SaveFireCampStates.Interactable:
                    ChangeState(SaveFireCampStates.Interacted);
                    return;
                case SaveFireCampStates.Interacted:
                {
                    
                    if (IsSaved())
                    {
                        messageCanvas.SetText(ReplacePlaceholders(successfulSaveAttemptTextField));
                        animator.SetTrigger(FireUp);
                        MimicSaveSystem.Instance.Save(savePoint);
                        
                    }
                    else
                    {
                        messageCanvas.SetText(ReplacePlaceholders(unsuccessfulSaveAttemptTextField));
                    }
                    ChangeState(SaveFireCampStates.PostInteraction);
                    break;
                }
            }
        }

        private bool IsSaved()
        {
            return tinderBox.Save(SaveCost);
        }
        public float DistanceToPlayer()
        {
            
            var obstacleCollider = GameObject.FindWithTag("Player").transform.Find("ObstacleCollider");
            return Vector2.Distance(transform.position, obstacleCollider.position);
        }

        #region string formatters

        private string ReplacePlaceholders(string message)
        {
            return Regex.Replace(message, @"\{(\w+)\}", match =>
            {
                var placeholder = match.Groups[1].Value;

                return placeholder switch
                {
                    "TinderBoxInfo" => $"{tinderBox.CurrentLoad} out of {tinderBox.MaxLoad}",
                    "SaveCost" => $"{SaveCost}",
                    "ChargeDifference" => $"{SaveCost - tinderBox.CurrentLoad}",
                    _ => GetKeyBinding(placeholder)
                };
            });
        }
        private string GetKeyBinding(string actionName)
        {
            var action = _playerInput.actions.FindAction(actionName);

            if (action == null || action.bindings.Count <= 0) return "Unknown"; // Если действие не найдено
            var binding = action.bindings[0];
            return InputControlPath.ToHumanReadableString(binding.effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
        
        #endregion

        private void HandleMouseOver()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePosition, LayerMask.GetMask("LootBag"));

            if (hit != null && hit.gameObject == gameObject)
            {
                if (!IsMouseOver)
                {
                    MouseEnterDetection();
                }
            }
            else
            {
                if (IsMouseOver)
                {
                    MouseExitDetection();
                }
            }
        }
        
        private void MouseEnterDetection()
        {
            if (!CanBeInteractedWith) return;
            MouseEnter?.Invoke();
            IsMouseOver = true;
        }

        private void MouseExitDetection()
        {
            MouseExit?.Invoke();
            IsMouseOver = false;
        }

        
    }
}
