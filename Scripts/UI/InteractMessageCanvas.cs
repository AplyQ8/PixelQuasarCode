using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InteractMessageCanvas : MonoBehaviour
    {
        public static InteractMessageCanvas Instance { get; private set; }
        [SerializeField] private TMP_Text textBox;
        
        private void Awake()
        {
            // Ensure that there is only one instance of the singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DisplayMessage(new MessageStructure("", "", ""));
            ToggleOff();
        }

        public void DisplayMessage(MessageStructure message)
        {
            ToggleOn();
            textBox.text = $"{message.MessageBeforeKeyCode} <b><u>{message.KeyCode}</u></b> {message.MessageAfterKeyCode}";
        }

        private void ToggleOn() => gameObject.SetActive(true);
        public void ToggleOff()
        {
            try
            {
                gameObject.SetActive(false);
            }
            catch (MissingReferenceException)
            {
                //
            }
            
        }
    }

    [Serializable]
    public struct MessageStructure
    {
        public string MessageBeforeKeyCode;
        [HideInInspector] public string KeyCode;
        public string MessageAfterKeyCode;


        public MessageStructure(string messageBeforeKeyCode, string keyCode, string messageAfterKeyCode)
        {
            this.MessageBeforeKeyCode = messageBeforeKeyCode;
            this.KeyCode = keyCode;
            this.MessageAfterKeyCode = messageAfterKeyCode;
        }

        public void SetKeyCode(string keyCode) => KeyCode = keyCode;
    }
    
    
}
