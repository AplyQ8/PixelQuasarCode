using TMPro;
using UnityEngine;

namespace DialogScripts
{
    public class UIBubbleDialogueController : MonoBehaviour
    {
        [SerializeField] private TMP_Text dialogueText; // Ссылка на текстовый компонент для отображения реплик

        public void ShowText(string text)
        {
            dialogueText.text = text;
            dialogueText.gameObject.SetActive(true);
        }

        public void HideText()
        {
            dialogueText.gameObject.SetActive(false);
        }
    }
}
