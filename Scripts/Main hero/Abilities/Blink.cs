using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;
[CreateAssetMenu(menuName = "Abilities/Blink")]
public class Blink : Ability
{
    public override void Activate(GameObject hero, Vector2 position)
    {
        try
        {
            position = Camera.main.ScreenToWorldPoint(position);
        }
        catch (NullReferenceException) { }

        if (Vector2.Distance(hero.transform.position, position) > usabilityDistance)
        {
            var heroPos = (Vector2)hero.transform.position;
            var facing = position - heroPos;
            var destination = heroPos + facing.normalized * usabilityDistance;
            hero.transform.position = destination;
            hero.GetComponent<IBloodContent>().SubtractBlood(bloodConsumption);
            return;
        }
        hero.transform.position = position;
        hero.GetComponent<IBloodContent>().SubtractBlood(bloodConsumption);
    }
}
