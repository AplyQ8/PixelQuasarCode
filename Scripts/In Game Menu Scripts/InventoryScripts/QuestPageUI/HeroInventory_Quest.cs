using QuestScripts;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace In_Game_Menu_Scripts.InventoryScripts.QuestPageUI
{
    public class HeroInventory_Quest : HeroInventory
    {
        [SerializeField] private QuestSo currentQuest;

        [Header("Quest inventory SO")] [SerializeField]
        private QuestInventorySo questInventory;

        [Header("UI Quest Page")] [SerializeField]
        private UIQuestInventoryPage uiQuestInventoryPage;
        
        [Header("Specific action names")]
        [SerializeField] private LocalizedString activateQuestAction;
        [SerializeField] private LocalizedString showQuestObjectives;
        [SerializeField] private LocalizedString stopObservingAction;
        
        public void AssignNewUIInventories(UIQuestInventoryPage questInventoryPage)
        {
            inventoryData.Initialize(reinitializeInventor);
            uiQuestInventoryPage = questInventoryPage;
            inventoryUI = questInventoryPage;
            questInventoryPage.OnRightMouseClick += OnQuestActionRequest;
            questInventoryPage.OnLeftMouseBtnClick += OnQuestObjectiveRequest;
            questInventoryPage.OnDoubleClick += OnSetObservableQuest;
            questInventory.OnQuestListStateUpdate += UpdateUIPage;
            questInventoryPage.
                InitializeQuestsPage(
                    questInventory.GetCurrentQuestsState().CurrentQuests, 
                    questInventory.GetCurrentQuestsState().ReachedQuests,
                    questInventory.GetCurrentQuestsState().FailedQuests,
                    currentQuest);
            
        }

        public bool AddQuest(QuestSo quest)
        {
            return questInventory.AddQuest(quest);
        }

        private void OnQuestActionRequest(QuestSo quest)
        {
            uiQuestInventoryPage.ShowQuestActions(quest);
            inventoryUI.AddAction(activateQuestAction.GetLocalizedString(), () => OnSetObservableQuest(quest));
            if (currentQuest == quest)
            {
                inventoryUI.AddAction(stopObservingAction.GetLocalizedString(), () => OnStopObserving(quest));
            }
            inventoryUI.AddAction(showQuestObjectives.GetLocalizedString(), () => OnQuestObjectiveRequest(quest));
            
        }

        private void OnQuestObjectiveRequest(QuestSo quest)
        {
            uiQuestInventoryPage.ShowQuestObjectives(quest);
            ItemActionMenu.Instance.Toggle(false);
        }

        private void OnSetObservableQuest(QuestSo quest)
        {
            currentQuest = quest;
            uiQuestInventoryPage.ObserveQuest(quest);
            ItemActionMenu.Instance.Toggle(false);
        }

        private void OnStopObserving(QuestSo quest)
        {
            currentQuest = null;
            uiQuestInventoryPage.StopObserving(quest);
            ItemActionMenu.Instance.Toggle(false);
        }

        private void UpdateUIPage()
        {
            uiQuestInventoryPage.
                InitializeQuestsPage(
                    questInventory.GetCurrentQuestsState().CurrentQuests, 
                    questInventory.GetCurrentQuestsState().ReachedQuests,
                    questInventory.GetCurrentQuestsState().FailedQuests,
                    currentQuest);
        }
    }
}