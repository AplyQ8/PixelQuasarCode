using In_Game_Menu_Scripts.InventoryScripts.QuestPageUI;
using ObjectLogicInterfaces;
using UnityEngine;

namespace QuestScripts
{
    public class QuestObjectiveProgressor : MonoBehaviour, IQuestTrigger
    {
        [field: SerializeField] public QuestSo Quest { get; private set; }
        [SerializeField] private bool objectiveHasProgress;
        [SerializeField] private int objectiveID;
        
        public void ActivateTrigger(HeroInventory_Quest questInventory)
        {
           if(objectiveHasProgress) 
               Quest.ProgressObjective(objectiveID);
           else
           {
               Quest.CompleteObjective(objectiveID);
           }
        }
    }
}
