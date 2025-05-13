using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

namespace QuestScripts
{
    [CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest")]
    public class QuestSo : ScriptableObject
    {
        [field: SerializeField] public LocalizedString QuestName { get; private set; }
        [field: SerializeField] public LocalizedString Description { get; private set; }
        [SerializedDictionary("Objective ID", "Objective Info")]
        public SerializedDictionary<int, ObjectiveInfo> objectives;
        [field: SerializeField] public int CurrentProgress { get; private set; } = 0;
        [field: SerializeField] public QuestTypeEnum QuestType { get; private set; }

        [field: SerializeField] public QuestStateEnum QuestState { get; private set; } = QuestStateEnum.Inactive;

        public enum QuestTypeEnum
        {
            MainQuest,
            SideQuest
        }

        public enum QuestStateEnum
        {
            Inactive,
            Active,
            Completed,
            Failed
        }

        public event Action<QuestSo> OnQuestUpdate;

        public void CompleteObjective(int objectiveID)
        {
            objectives[objectiveID].ReachObjective();
            CalculateProgress();
            OnQuestUpdate?.Invoke(this);
        }

        public void ProgressObjective(int objectiveID)
        {
            objectives[objectiveID].ProgressObjective();
            CalculateProgress();
            OnQuestUpdate?.Invoke(this);
        }

        public void ProgressObjective(int objectiveID, int numberOfSteps)
        {
            objectives[objectiveID].ProgressObjective(numberOfSteps);
            CalculateProgress();
            OnQuestUpdate?.Invoke(this);
        }

        private void CalculateProgress()
        {
            int reachedSteps = 0;
            foreach (var objective in objectives)
            {
                if (objective.Value.IsReached)
                    reachedSteps += 1;
            }

            CurrentProgress = Mathf.RoundToInt(((float)reachedSteps / objectives.Count) * 100);
            if(CurrentProgress == 100) CompleteQuest();
        }

        public void ActivateQuest()
        {
            QuestState = QuestStateEnum.Active;
            OnQuestUpdate?.Invoke(this);
        }

        public void CompleteQuest()
        {
            if (QuestState is QuestStateEnum.Failed) return;
            QuestState = QuestStateEnum.Completed;
            OnQuestUpdate?.Invoke(this);
        }

        public void FailQuest()
        {
            if (QuestState is QuestStateEnum.Completed) return;
            QuestState = QuestStateEnum.Failed;
            OnQuestUpdate?.Invoke(this);
        }


        public ObjectiveInfo GetCurrentObjective() => objectives.First().Value;
        
    }

    [Serializable]
    public class ObjectiveInfo
    {
        [field: Header("general Info")]
        [field: SerializeField] public bool IsReached { get; private set; } = false;
        [field: SerializeField] public LocalizedString Description { get; private set; }

        [field: Header("Objective progress")] [field: SerializeField]
        public bool HasProgress { get; private set; }
        [field: SerializeField] public int CurrentProgress { get; private set; }
        [field: SerializeField] public int TotalNumberOfSteps { get; private set; }

        public void ProgressObjective()
        {
            if (IsReached) return;
            CurrentProgress++;
            if(CurrentProgress == TotalNumberOfSteps) ReachObjective();
        }

        public void ProgressObjective(int numberOfSteps)
        {
            if (IsReached) return;
            CurrentProgress += numberOfSteps;
            if(CurrentProgress == TotalNumberOfSteps) ReachObjective();
        }

        public void ReachObjective() => IsReached = true;

    }
    
}
