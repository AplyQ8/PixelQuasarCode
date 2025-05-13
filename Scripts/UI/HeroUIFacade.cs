using UnityEngine;

namespace UI
{
    public class HeroUIFacade : MonoBehaviour
    {
        [field: SerializeField] public HealthBar HealthBar { get; private set; }
        [field: SerializeField] public BloodBar BloodBar { get; private set; }
        [field: SerializeField] public AdrenalineBar AdrenalineBar { get; private set; }
        [field: SerializeField] public AbilityUIPanel AbilityUIPanel { get; private set; }
    }
}