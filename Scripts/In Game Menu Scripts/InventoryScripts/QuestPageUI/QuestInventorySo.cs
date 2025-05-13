using System;
using System.Collections.Generic;
using QuestScripts;
using UnityEditor;
using UnityEngine;

namespace In_Game_Menu_Scripts.InventoryScripts.QuestPageUI
{
    [CreateAssetMenu]
    public class QuestInventorySo : InventorySO
    {
        [SerializeField] private List<QuestSo> currentQuests;
        [SerializeField] private List<QuestSo> completedQuests;
        [SerializeField] private List<QuestSo> failedQuest;
        
        public event Action OnQuestListStateUpdate;

        public override void Initialize(bool reinitialize)
        {
            if (reinitialize)
            {
                currentQuests.Clear();
                completedQuests.Clear();
                failedQuest.Clear();
                return;
            }

            foreach (var currentQuest in currentQuests)
            {
                currentQuest.OnQuestUpdate += OnQuestProgressUpdate;
            }
        }
        public Quests GetCurrentQuestsState()
        {
            return new Quests(currentQuests, completedQuests, failedQuest);
        }

        private protected override void InformAboutChange()
        {
            OnQuestListStateUpdate?.Invoke();
        }

        public bool AddQuest(QuestSo quest)
        {
            if (ContainsQuest(quest)) return false;
            quest.ActivateQuest();
            currentQuests.Add(quest);
            quest.OnQuestUpdate += OnQuestProgressUpdate;
            OnQuestListStateUpdate?.Invoke();
            return true;
        }

        private bool ContainsQuest(QuestSo quest)
        {
            if (currentQuests.Contains(quest)) return true;
            if (completedQuests.Contains(quest)) return true;
            if (failedQuest.Contains(quest)) return true;
            return false;
        }

        private void OnQuestProgressUpdate(QuestSo quest)
        {
            if (!currentQuests.Contains(quest)) return;
            switch (quest.QuestState)
            {
                case QuestSo.QuestStateEnum.Completed:
                    currentQuests.Remove(quest);
                    completedQuests.Add(quest);
                    quest.OnQuestUpdate -= OnQuestProgressUpdate;
                    break;
                case QuestSo.QuestStateEnum.Failed:
                    currentQuests.Remove(quest);
                    failedQuest.Add(quest);
                    quest.OnQuestUpdate -= OnQuestProgressUpdate;
                    break;
            }
            OnQuestListStateUpdate?.Invoke();
        }
    }

    public class Quests
    {
        public List<QuestSo> CurrentQuests { get; private set; }
        public List<QuestSo> ReachedQuests { get; private set; }
        public List<QuestSo> FailedQuests { get; private set; }

        public Quests(List<QuestSo> currentQuests, List<QuestSo> reachedQuests, List<QuestSo> failedQuests)
        {
            CurrentQuests = currentQuests;
            ReachedQuests = reachedQuests;
            FailedQuests = failedQuests;
        }
    }
}