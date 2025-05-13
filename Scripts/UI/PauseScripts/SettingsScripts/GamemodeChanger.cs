using InteractableObjects.SaveFirecamp;
using UnityEngine;
using UnityEngine.UI;

namespace UI.PauseScripts.SettingsScripts
{
    public class GamemodeChanger : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        
        private void Start()
        {
            // Подписываемся на событие через код (опционально)
            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(OnToggleValueChange);
            }
        }
        public void OnToggleValueChange(bool isOn)
        {
            MimicSaveSystem.Instance.OnGameModeChange(isOn);
        }
    }
}
