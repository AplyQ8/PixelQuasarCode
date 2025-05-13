using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Fear")]
public class Fear : Ability
{
    [SerializeField] private Collider2D[] objectsToApplyEffect;
    [SerializeField] private FearEffect fear;
    [SerializeField] private float lifeTime;
    [SerializeField] private LayerMask enemyMask;
    public override void Activate(GameObject hero)
    {
        //Set an object to run away from
        fear.SetObjectToRunFrom(hero);
        fear.SetLifeTime(lifeTime);
        //Subtract blood
        hero.GetComponent<IBloodContent>().SubtractBlood(bloodConsumption);
        //Detect objects to apply effect
        objectsToApplyEffect =
            Physics2D.OverlapCircleAll(hero.transform.position, usabilityDistance, enemyMask);
        foreach (var enemies in objectsToApplyEffect)
        {
            enemies.GetComponent<IEffectable>().ApplyEffect(fear);
        }
    }
}
