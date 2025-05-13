using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class HighlightSelectedButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Button _button;
        private Color _originalColor;
        private Color _highlightedColor;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _highlightedColor = _button.colors.highlightedColor;
            _originalColor = _button.colors.normalColor;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _button.targetGraphic.color = _highlightedColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _button.targetGraphic.color = _originalColor;
        }
    }
}
