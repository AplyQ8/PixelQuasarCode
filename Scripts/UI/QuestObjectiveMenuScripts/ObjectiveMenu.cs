using System;
using QuestScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestObjectiveMenuScripts
{
    public class ObjectiveMenu : MonoBehaviour
    {
        public static ObjectiveMenu Instance { get; private set; }
        [SerializeField] private Animator animator;

        [Header("General quest info")] 
        [SerializeField] private TMP_Text questName;
        [SerializeField] private TMP_Text questDescription;
        
        [Header("Objectives")]
        [SerializeField] private UIObjectiveScript objectivePrefab;
        [SerializeField] private RectTransform objectiveContentPanel;

        [Header("Quest indicators")] [SerializeField]
        private Image indicatorField;

        [SerializeField] private Sprite mainQuestIndicator;
        [SerializeField] private Sprite sideQuestIndicator;

        [SerializeField] private Button closeButton;
        //private Action OnMenuCloseEvent;

        private ObjectiveMenuStates _currentState;

        public bool IsOpened { get; private set; }

        private enum ObjectiveMenuStates
        {
            Opened,
            Closed,
            Opening,
            Closing
        }

        #region Animation triggers

        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int Close = Animator.StringToHash("Close");

        #endregion

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
            Instance.gameObject.SetActive(false);
            SwitchToClosedState();
            IsOpened = false;
        }

        public void ToggleOn(QuestSo quest, Action OnMenuClose)
        {
            //if (_currentState is ObjectiveMenuStates.Opening) return;
            Instance.gameObject.SetActive(true);
            if (_currentState is ObjectiveMenuStates.Opened)
            {
                ClearPage();
                questName.text = quest.QuestName.GetLocalizedString();
                questDescription.text = quest.Description.GetLocalizedString();
                foreach (var questObjective in quest.objectives)
                {
                    InitializeObjective(questObjective.Value);
                    if (!questObjective.Value.IsReached) break;
                }

                return;
            }

            switch (quest.QuestType)
            {
                case QuestSo.QuestTypeEnum.MainQuest:
                    indicatorField.sprite = mainQuestIndicator;
                    break;
                default:
                    indicatorField.sprite = sideQuestIndicator;
                    break;
                
            }
            questName.text = quest.QuestName.GetLocalizedString();
            questDescription.text = quest.Description.GetLocalizedString();
            foreach (var questObjective in quest.objectives)
            {
                InitializeObjective(questObjective.Value);
                if (!questObjective.Value.IsReached) break;
            }
            animator.SetTrigger(Open);
            closeButton.onClick.AddListener(() => OnMenuClose());
            //OnMenuCloseEvent = OnMenuClose;
            _currentState = ObjectiveMenuStates.Opening;
        }

        public void ToggleOff()
        {
            if (_currentState is ObjectiveMenuStates.Closing or ObjectiveMenuStates.Closed) return;
            animator.SetTrigger(Close);
            _currentState = ObjectiveMenuStates.Closing;
            // OnMenuCloseEvent?.Invoke();
            // OnMenuCloseEvent = null;
        }

        public void CloseMenuOnButtonClick()
        {
            if (_currentState is ObjectiveMenuStates.Closing or ObjectiveMenuStates.Closed) return;
            animator.SetTrigger(Close);
            _currentState = ObjectiveMenuStates.Closing;
        }

        
        public void SwitchToClosedState()
        {
            _currentState = ObjectiveMenuStates.Closed;
            ClearPage();
            Instance.gameObject.SetActive(false);
            IsOpened = false;
        }

        public void SwitchToOpenedState()
        {
            _currentState = ObjectiveMenuStates.Opened;
            IsOpened = true;
        }

        private void InitializeObjective(ObjectiveInfo objectiveInfo)
        {
            var uiObjective = Instantiate(objectivePrefab, objectiveContentPanel);
            uiObjective.transform.SetSiblingIndex(0);
            uiObjective.Initialize(objectiveInfo);
        }

        private void ClearPage()
        {
            questName.text = String.Empty;
            questDescription.text = String.Empty;
            foreach (RectTransform objective in objectiveContentPanel)
            {
                Destroy(objective.gameObject);
            }
        }

        public void MenuCloseEvent()
        {
            // OnMenuCloseEvent?.Invoke();
            // OnMenuCloseEvent = null;
            closeButton.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
