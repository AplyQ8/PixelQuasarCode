using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class PassiveAbilityHandler : MonoBehaviour, IPassiveAbilityHolder
{
    [SerializeField] private List<BasePassiveAbilitySO> currentPassiveAbilities;
    [field: SerializeField] public int AvailableSlots { get; private set; } = 3;
    [SerializeField] private HeroComponents hero;
    private PassiveAbilityHandler _passiveAbilityHandler;

    #region Events

    public event Action
        AddAbility,
        RemoveAbility;

    public event Action<int>
        AddSlots,
        DeleteSlots;

    #endregion

    public bool AddPassiveAbility(BasePassiveAbilitySO passiveAbility)
    {
        if (currentPassiveAbilities.Count >= AvailableSlots)
            return false;
        if (currentPassiveAbilities.Contains(passiveAbility))
        {
            RemovePassiveAbility(passiveAbility);
            return true;
        }
        currentPassiveAbilities.Add(passiveAbility);
        AddAbility?.Invoke();
        passiveAbility.ApplyEffect(hero);
        return true;
    }

    public void RemovePassiveAbility(BasePassiveAbilitySO passiveAbility)
    {
        passiveAbility.RemoveEffect(hero);
        RemoveAbility?.Invoke();
        currentPassiveAbilities.Remove(passiveAbility);
    }

    public void ExpandSlots(int numOfSlots)
    {
        AvailableSlots += numOfSlots;
        AddSlots?.Invoke(numOfSlots);
    }

    public void NarrowDownSlots(int numOfSlots)
    {
        for (int i = AvailableSlots - 1; i >= AvailableSlots - numOfSlots; i--)
        {
            try
            {
                RemovePassiveAbility(currentPassiveAbilities[i]);
            }
            catch (NullReferenceException)
            {
                //No ability in slot
            }
            currentPassiveAbilities.RemoveAt(i);
        }
        AvailableSlots -= numOfSlots;
        DeleteSlots?.Invoke(numOfSlots);
        if (AvailableSlots <= 0) AvailableSlots = 0;
        
    }

    public void ExpandPassiveAbilityPool(int numberOfSlots)
    {
        ExpandSlots(numberOfSlots);
    }

    public void ReducePassiveAbilityPool(int numberOfSlots)
    {
        NarrowDownSlots(numberOfSlots);
    }
}
[Serializable]
public class HeroComponents
{
    [field: SerializeField] public GameObject Hero { get; private set; }
    [field: SerializeField] public PassiveAbilityCollider Collider { get; private set; }
    [field: SerializeField] public EffectHandler EffectHandler { get; private set; }
}
