using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace In_Game_Menu_Scripts
{
    public class ItemActionMenu : MonoBehaviour, IPointerExitHandler
    {
        [SerializeField] private GameObject actionButtonPrefab;
        public static ItemActionMenu Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void AddButton(string actionName, Action onClickAction)
        {
            GameObject button = Instantiate(actionButtonPrefab, transform);
            button.GetComponent<Button>().onClick.AddListener(() => onClickAction());
            button.GetComponentInChildren<TMP_Text>().text = actionName;
        }

        public void Toggle(bool val)
        {
            if (val)
                RemoveOldButtons();

            if (Instance is null || !Instance.gameObject)
            {
                Instance = this;
            }

            Instance.gameObject.SetActive(val);
        }

        private void RemoveOldButtons()
        {
            foreach (Transform childObjects in transform)
            {
                Destroy(childObjects.gameObject);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Toggle(false);
        }
    }
}