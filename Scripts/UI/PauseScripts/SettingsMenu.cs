using UnityEngine;

namespace UI.PauseScripts
{
    public class SettingsMenu : MonoBehaviour
    {
        public static SettingsMenu Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
            }
            Close();
        }
        public void Open()
        {
            gameObject.SetActive(true);
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
