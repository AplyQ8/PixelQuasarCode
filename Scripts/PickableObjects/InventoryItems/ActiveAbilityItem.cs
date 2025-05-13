using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityItem : ItemSO, IItemActivated
{
    [field: SerializeField] public string ActionName => "Activate";
    [field: SerializeField] public AudioClip ActionSfx { get; private set; }
    [field: SerializeField] public Ability Ability { get; private set; }
    public bool Activate(GameObject hero)
    {
        return true;
    }

    
}
