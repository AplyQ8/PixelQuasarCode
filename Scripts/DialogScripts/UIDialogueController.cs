using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DialogScripts
{
    public class UIDialogueController : MonoBehaviour
    {
        public static UIDialogueController Instance { get; private set; }
        [SerializeField] private TMP_Text dialogueTextField;
        [SerializeField] private TMP_Text scrollDialogueText;
        [SerializeField] private TMP_Text buttonToScrollText;
        [SerializeField] private TMP_Text participantNameText;
        //PlayerInputActionMap to indicate wich button to press
        public bool IsTyping { get; private set;  } = false;
        [SerializeField] private float textSpeed = 0.05f;
        [SerializeField] private InputActionReference nextDialogueAction;
        
        private IEnumerator _typeTextCoroutine;
        

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
            }
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            UpdateNextDialogueKeyText();
        }

        public void NextDialogue()
        {
            DialogueManager.Instance.DisplayNextLine();
        }
        private void UpdateNextDialogueKeyText()
        {
            if (nextDialogueAction != null && nextDialogueAction.action != null)
            {
                // Получаем клавишу, назначенную для действия "NextDialogue"
                var bindingDisplayString = nextDialogueAction.action.GetBindingDisplayString();
                buttonToScrollText.text = $"[{bindingDisplayString}]";  // Обновляем текст в UI
            }
        }

        public void DisplayTextSlowly(DialogueInfoStruct dialogueInfo, bool isLastLine)
        {
            participantNameText.text = dialogueInfo.ParticipantName;
            _typeTextCoroutine = TypeText(dialogueInfo.TextLine);
            if (isLastLine)
                scrollDialogueText.text = "End";
            else
            {
                scrollDialogueText.text = "Next";
            }
            StartCoroutine(_typeTextCoroutine);
        }

        public void DisplayFullText(DialogueInfoStruct dialogueInfo)
        {
            IsTyping = false;
            StopCoroutine(_typeTextCoroutine);
            participantNameText.text = dialogueInfo.ParticipantName;
            dialogueTextField.text = dialogueInfo.TextLine;
        }

        private IEnumerator TypeText(string text)
        {
            dialogueTextField.text = ""; // Очистить текстовое поле
            IsTyping = true;

            string displayedText = ""; // Хранит текст без тегов для показа постепенно
            int tagBalance = 0;        // Используется для отслеживания открытых/закрытых тегов
            string currentTag = "";    // Хранит текущий открытый тег

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // Обработка тегов
                if (c == '<') tagBalance++;
                if (tagBalance > 0) currentTag += c;

                if (c == '>')
                {
                    tagBalance--;
                    if (tagBalance == 0)
                    {
                        displayedText += currentTag; // Добавляем весь тег сразу
                        currentTag = "";            // Сбрасываем текущий тег
                    }
                    continue;
                }

                // Если это не часть тега, добавляем символ
                if (tagBalance == 0)
                {
                    displayedText += c;
                    dialogueTextField.text = displayedText + currentTag; // Вставляем открытые теги (если есть)
                    yield return new WaitForSeconds(textSpeed);
                }
            }

            IsTyping = false;
            _typeTextCoroutine = null;
        }

        public void CloseDialogueWindow()
        {
            IsTyping = false;
            gameObject.SetActive(false);
        }

        public void OpenDialogueWindow()
        {
            IsTyping = false;
            gameObject.SetActive(true);
        }

        public void EndDialogue()
        {
            IsTyping = false;
            DialogueManager.Instance.EndDialogue();
        }
        
    }
}
