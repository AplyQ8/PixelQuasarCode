using System;
using UI.PauseScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace In_Game_Menu_Scripts
{
    public class PauseController : MonoBehaviour
    {
        public static PauseController Instance { get; private set; }
        private PlayerInput _playerInput;
        private InputAction _pause;
        [field: SerializeField] public bool IsPaused { get; private set; }
        public event Action<bool> OnPause;

        private GameStates _currentGameState;

        private enum GameStates
        {
            PauseMenu,
            Settings
        }
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
            }
            _playerInput = GameObject.FindObjectOfType<PlayerInput>();
            _pause = _playerInput.currentActionMap.FindAction("Pause");
        }

        private void Start()
        {
            try
            {
                FindObjectOfType<HeroStateHandler>().InventoryState.OnEnteredState += UnsubscribeFromInputAction;
                FindObjectOfType<HeroStateHandler>().InventoryState.OnExitState += SubscribeOnInputAction;
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("No hero state handler found on Start");
            }
        }
        private void OnEnable()
        {
            SubscribeOnInputAction();
        }
        private void OnDisable()
        {
            UnsubscribeFromInputAction();
        }

        private void OnDestroy()
        {
            try
            {
                FindObjectOfType<HeroStateHandler>().InventoryState.OnEnteredState -= UnsubscribeFromInputAction;
                FindObjectOfType<HeroStateHandler>().InventoryState.OnExitState -= SubscribeOnInputAction;
            }
            catch (NullReferenceException)
            {
                Debug.LogWarning("No hero state handler found on Destroy");
            }
        }

        private void SwitchState(GameStates state)
        {
            switch (state)
            {
                case GameStates.PauseMenu:
                    break;
                case GameStates.Settings:
                    break;
            }
        }
        
        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            TogglePause();
        }

        private void TogglePause()
        {
            IsPaused = !IsPaused;
            OnPause?.Invoke(IsPaused);
            if (IsPaused)
            {
                Time.timeScale = 0f;
                PauseMenuUI.Instance.Open();
                SubScribeOnPauseMenuEvents(PauseMenuUI.Instance);
                return;
            }
            Time.timeScale = 1f;
            PauseMenuUI.Instance.Close();
            UnsubScribeFromPauseMenuEvents(PauseMenuUI.Instance);
        }

        public void DisablePause()
        {
            OnPause?.Invoke(false);
            Time.timeScale = 1f;
            PauseMenuUI.Instance.Close();
            UnsubScribeFromPauseMenuEvents(PauseMenuUI.Instance);
        }

        private void OpenSettingsEvent()
        {
            PauseMenuUI.Instance.Close();
            UnsubScribeFromPauseMenuEvents(PauseMenuUI.Instance);
            _pause.performed -= OnPausePerformed;
            SettingsMenu.Instance.Open();
            _pause.performed += CloseSettingEvent;
        }

        private void CloseSettingEvent(InputAction.CallbackContext context)
        {
            SettingsMenu.Instance.Close();
            SubScribeOnPauseMenuEvents(PauseMenuUI.Instance);
            PauseMenuUI.Instance.Open();
            _pause.performed += OnPausePerformed;
        }

        private void SubScribeOnPauseMenuEvents(PauseMenuUI instance)
        {
            instance.OnContinue += TogglePause;
            instance.OnOpenSettings += OpenSettingsEvent;
        }
        private void UnsubScribeFromPauseMenuEvents(PauseMenuUI instance)
        {
            instance.OnContinue -= TogglePause;
            instance.OnOpenSettings -= OpenSettingsEvent;
        }

        private void SubscribeOnInputAction()
        {
            _pause.performed += OnPausePerformed;
        }
        private void UnsubscribeFromInputAction()
        {
            _pause.performed -= OnPausePerformed;
        }
        
        private void SubscribeOnSettingsMenuEvents()
        {
            
        }

        private void UnsubscribeFromSettingsMenuEvent()
        {
            
        }
        
    }
}
