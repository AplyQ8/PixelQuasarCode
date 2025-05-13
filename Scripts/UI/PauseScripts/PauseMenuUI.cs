using System;
using In_Game_Menu_Scripts;
using InteractableObjects.SaveFirecamp;
using UnityEngine;

namespace UI.PauseScripts
{
    public class PauseMenuUI : MonoBehaviour
    {
        public static PauseMenuUI Instance { get; private set; }

        #region Actions

        public event Action OnContinue;
        public event Action OnOpenSettings;

        #endregion

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

        public void Continue()
        {
            OnContinue?.Invoke();
        }

        public void LoadLastSave()
        {
            FindObjectOfType<MimicSaveSystem>().LoadLastSave();
        }

        public void OpenSettings()
        {
            OnOpenSettings?.Invoke();
        }
        
        public void ExitGame()
        {
            Debug.Log("Quiting the game...");
            Application.Quit();
        }
        
    }
}
