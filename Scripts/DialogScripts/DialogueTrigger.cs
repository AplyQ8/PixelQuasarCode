using Envirenmental_elements;
using ObjectLogicInterfaces;
using UnityEngine;

namespace DialogScripts
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(IDialogueParticipant))]
    public class DialogueTrigger : MonoBehaviour, IInteractable, IMouseHoverable
    {
        [SerializeField] private TriggerType triggerType;
        [SerializeField] private IDialogueParticipant _dialogueParticipant;
        public bool IsMouseOver { get; private set; } = false;
        public enum TriggerType
        {
            Optional,
            Forced
        }

        private void Awake()
        {
            _dialogueParticipant = GetComponent<IDialogueParticipant>();
        }

        private void Update()
        {
            HandleMouseOver();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (triggerType is TriggerType.Optional) return;
            if (!col.CompareTag("Player")) return;
            DialogueManager.Instance.StartDialogue(_dialogueParticipant);
        }


        public void Interact()
        {
            DialogueManager.Instance.StartDialogue(_dialogueParticipant);
        }
        
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

        private void MouseExitDetection()
        {
            IsMouseOver = false;
        }

        private void MouseEnterDetection()
        {
            IsMouseOver = true;
        }

        public void ChangeTriggerType(TriggerType triggerType) => this.triggerType = triggerType;
    }
}
