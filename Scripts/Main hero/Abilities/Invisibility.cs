using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using Status_Effect_System;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Invisibility")]
public class Invisibility : Ability
{
    [SerializeField] private StatusEffectData effect;
    public override void Activate(GameObject hero)
    {
        hero.GetComponent<IBloodContent>().SubtractBlood(bloodConsumption);
        hero.GetComponent<IEffectable>().ApplyEffect(effect);
    }
}
