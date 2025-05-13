using UnityEngine;
using UnityEngine.Localization;

namespace DialogScripts
{
    [CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Dialogue/Line")]
    public class DialogueLine : ScriptableObject
    {
        public LocalizedString text;          // Текст реплики
        public float displayTime;     // Время отображения реплики
    }
}
