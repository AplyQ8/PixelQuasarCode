using System;
using In_Game_Menu_Scripts.InventoryScripts.QuestPageUI;
using ObjectLogicInterfaces;
using UnityEngine;

namespace QuestScripts
{
    [RequireComponent(typeof(Collider2D))]
    public class QuestTriggerZone : MonoBehaviour, IQuestTrigger
    {
        [field: SerializeField] public QuestSo Quest { get; private set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            ActivateTrigger(other.gameObject.GetComponentInChildren<HeroInventory_Quest>());
        }

        public void ActivateTrigger(HeroInventory_Quest questInventory)
        {
            if (questInventory.AddQuest(Quest))
            {
                return;
            }
        }
    }
}
