using QuestScripts;
using UnityEngine;

namespace In_Game_Menu_Scripts.InventoryScripts.QuestPageUI
{
    public class UIInactiveQuestItem : UIQuestItem
    {
        public override void Initialize(QuestSo quest)
        {
            questNameText.text = quest.QuestName.GetLocalizedString();
        }
        public override void ResetData()
        { }
    }
}