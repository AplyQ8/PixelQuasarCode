using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventoryItems/PassiveAbilityItem")]
public class PassiveAbilityItem : ItemSO, IItemActivated
{
    [SerializeField] private List<ModifierData> modifiersData = new List<ModifierData>();
    [field: SerializeField] public string ActionName => "Activate";
    [field: SerializeField] public AudioClip ActionSfx { get; private set; }
    [field: SerializeField] public BasePassiveAbilitySO PassiveAbility { get; private set; }

    public bool Activate(GameObject hero)
    {
        return hero.GetComponent<PassiveAbilityHandler>().AddPassiveAbility(PassiveAbility);
    }
}

public interface IItemActivated
{
    bool Activate(GameObject hero);
}
