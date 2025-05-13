using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityHandler : MonoBehaviour

{
    [field: SerializeField] public int AvailableSlots { get; private set; } = 3;
    [SerializeField] private List<Ability> currentAbilities;
    [SerializeField] private HeroComponents hero;


    #region Events

    public event Action
        AddAbility,
        RemoveAbility;

    public event Action<int>
        AddSlots,
        DeleteSlots;

    #endregion
}
