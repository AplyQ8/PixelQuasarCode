using QuestScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.QuestObjectiveMenuScripts
{
    public class UIObjectiveScript : MonoBehaviour
    {
        [SerializeField] private TMP_Text objectiveText;
        [SerializeField] private Toggle checkMark;

        public void Initialize(ObjectiveInfo objectiveInfo)
        {
            checkMark.isOn = false;
            objectiveText.text = objectiveInfo.Description.GetLocalizedString();
            Stylize(objectiveInfo);
        }

        private void Stylize(ObjectiveInfo objectiveInfo)
        {
            if (objectiveInfo.HasProgress)
            {
                objectiveText.text = $"{objectiveInfo.Description.GetLocalizedString()} ({objectiveInfo.CurrentProgress}/{objectiveInfo.TotalNumberOfSteps})";
            }
            if (!objectiveInfo.IsReached) return;
            
            checkMark.isOn = true;
            objectiveText.text = $"<s>{objectiveText.text}</s>";
        }
    }
}
