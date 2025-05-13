using System;
using System.Collections.Generic;
using System.Linq;
using QuestScripts;
using UI.QuestObjectiveMenuScripts;
using UnityEditor;
using UnityEngine;

namespace In_Game_Menu_Scripts.InventoryScripts.QuestPageUI
{
    public class UIQuestInventoryPage : UIInventoryPage
    {
        [SerializeField] private GameObject quickSlotPanel;
        [SerializeField] private GameObject mainQuestItemPrefab;
        [SerializeField] private GameObject sideQuestItemPrefab;
        [SerializeField] private GameObject completedQuestItemPrefab;
        [SerializeField] private GameObject failedQuestItemPrefab;
        private List<UIQuestItem> _mainQuests = new List<UIQuestItem>();
        private List<UIQuestItem> _sideQuests = new List<UIQuestItem>();
        private List<UIQuestItem> _completedQuests = new List<UIQuestItem>(); 
        private List<UIQuestItem> _failedQuests = new List<UIQuestItem>();
        private Dictionary<QuestSo, UIQuestItem> _allQuests = new Dictionary<QuestSo, UIQuestItem>();

        public event Action<QuestSo>
            OnLeftMouseBtnClick,
            OnDoubleClick,
            OnRightMouseClick;
        private protected override void Start()
        {
            base.Start();
            
        }
        public void InitializeQuestsPage(List<QuestSo> activeQuests, List<QuestSo> reachedQuests, List<QuestSo> failedQuests, QuestSo observingQuest)
        {
            ClearPage();
            foreach (var quest in activeQuests)
            {
                AddActiveQuest(quest);
            }
            foreach (var reachedQuest in reachedQuests)
            {
                AddCompletedQuest(reachedQuest);
            }
            foreach (var failedQuest in failedQuests)
            {
                AddFailedQuest(failedQuest);
            }
            SortQuests();
            if (observingQuest == null) return;
            ObserveQuest(observingQuest);
        }

        public void ShowQuestObjectives(QuestSo quest)
        {
            if(!ObjectiveMenu.Instance.IsOpened)
                inventoryMenu.OpenQuickSlotPanel(quickSlotPanel);
            ObjectiveMenu.Instance.ToggleOn(quest, CloseQuestMenuEvent);
        }
        private void AddActiveQuest(QuestSo quest)
        {
            GameObject questItem;
            switch (quest.QuestType)
            {
                case QuestSo.QuestTypeEnum.MainQuest:
                    questItem = Instantiate(mainQuestItemPrefab, contentPanel);
                    var mainQuestItemComponent = questItem.GetComponent<UIQuestItem>();
                    mainQuestItemComponent.Initialize(quest);
                    SubscribeOnQuestEvents(mainQuestItemComponent);
                    _mainQuests.Add(mainQuestItemComponent);
                    _allQuests.Add(quest, mainQuestItemComponent);
                    break;
                case QuestSo.QuestTypeEnum.SideQuest:
                    questItem = Instantiate(sideQuestItemPrefab, contentPanel);
                    var sideQuestItemComponent = questItem.GetComponent<UIQuestItem>();
                    sideQuestItemComponent.Initialize(quest);
                    SubscribeOnQuestEvents(sideQuestItemComponent);
                    _sideQuests.Add(sideQuestItemComponent);
                    _allQuests.Add(quest, sideQuestItemComponent);
                    break;
                default:
                    questItem = Instantiate(sideQuestItemPrefab, contentPanel);
                    var questItemComponent = questItem.GetComponent<UIQuestItem>();
                    questItemComponent.Initialize(quest);
                    SubscribeOnQuestEvents(questItemComponent);
                    _sideQuests.Add(questItemComponent);
                    _allQuests.Add(quest, questItemComponent);
                    break;
            }
            
        }
        private void AddCompletedQuest(QuestSo reachedQuest)
        {
            var questItem = Instantiate(completedQuestItemPrefab, contentPanel);
            var questItemComponent = questItem.GetComponent<UIQuestItem>();
            questItemComponent.Initialize(reachedQuest);
            _completedQuests.Add(questItemComponent);
            _allQuests.Add(reachedQuest, questItemComponent);
        }

        private void AddFailedQuest(QuestSo failedQuest)
        {
            
            var questItem = Instantiate(failedQuestItemPrefab, contentPanel);
            var questItemComponent = questItem.GetComponent<UIQuestItem>();
            questItemComponent.Initialize(failedQuest);
            _failedQuests.Add(questItemComponent);
            if (_allQuests.ContainsKey(failedQuest)) return;
            _allQuests.Add(failedQuest, questItemComponent);
        }

        public void ShowQuestActions(QuestSo quest)
        {
            if (!_allQuests.ContainsKey(quest)) return;
            ItemActionMenu.Instance.Toggle(true);
            ItemActionMenu.Instance.transform.position = _allQuests[quest].gameObject.transform.position;
        }
        public void ObserveQuest(QuestSo quest)
        {
            foreach (var uiQuestItem in _allQuests)
            {
                uiQuestItem.Value.ResetData();
            }

            _allQuests[quest].Observe();
        }
        public void StopObserving(QuestSo quest)
        {
            _allQuests[quest].StopObserving();
        }
        private void SortQuests()
        {
            List<UIQuestItem> sortedQuests = _mainQuests
                .Concat(_sideQuests)
                .Concat(_completedQuests)
                .Concat(_failedQuests)
                .ToList();
            
            for (int i = 0; i < sortedQuests.Count; i++)
            {
                sortedQuests[i].transform.SetSiblingIndex(i);
            }
        }
        private void ClearPage()
        {
            foreach (var uiQuestItem in _allQuests)
            {
                UnsubscribeFromQuestEvents(uiQuestItem.Value);
                Destroy(uiQuestItem.Value.gameObject);
            }
            ClearAllLists();
            
        }
        private void ClearAllLists()
        {
            _allQuests.Clear();
            _mainQuests.Clear();
            _sideQuests.Clear();
            _completedQuests.Clear();
            _failedQuests.Clear();
        }

        public void CloseQuestMenuEvent()
        {
            inventoryMenu.CloseQuickSlotPanel();
            quickSlotPanel.SetActive(false);
        }

        #region Action events

        private void HandleQuestLeftMouseClick(QuestSo questItem)
        {
            OnLeftMouseBtnClick?.Invoke(questItem);
        }

        private void HandleQuestDoubleClick(QuestSo questItem)
        {
            OnDoubleClick?.Invoke(questItem);
        }

        private void HandleQuestRightMouseClick(QuestSo questItem)
        {
            OnRightMouseClick?.Invoke(questItem);
        }

        private void HandleQuestUIItemDeletion(UIQuestItem uiQuestItem)
        {
            uiQuestItem.OnLeftMouseBtnClick -= HandleQuestLeftMouseClick;
            uiQuestItem.OnRightMouseBtnClick -= HandleQuestRightMouseClick;
            uiQuestItem.OnItemDoubleClicked -= HandleQuestDoubleClick;
            Destroy(uiQuestItem);
        }

        #endregion
        
        private void SubscribeOnQuestEvents(UIQuestItem uiQuestItem)
        {
            uiQuestItem.OnLeftMouseBtnClick += HandleQuestLeftMouseClick;
            uiQuestItem.OnRightMouseBtnClick += HandleQuestRightMouseClick;
            uiQuestItem.OnItemDoubleClicked += HandleQuestDoubleClick;
        }
        private void UnsubscribeFromQuestEvents(UIQuestItem uiQuestItem)
        {
            uiQuestItem.OnLeftMouseBtnClick -= HandleQuestLeftMouseClick;
            uiQuestItem.OnRightMouseBtnClick -= HandleQuestRightMouseClick;
            uiQuestItem.OnItemDoubleClicked -= HandleQuestDoubleClick;
        }

        private protected override void OnEnable()
        {
            quickSlotPanel.SetActive(false);
            inventoryMenu.CloseQuickSlotPanel();
        }

        private protected override void OnDisable()
        {
            ObjectiveMenu.Instance.ToggleOff();
        }


        
    }
}