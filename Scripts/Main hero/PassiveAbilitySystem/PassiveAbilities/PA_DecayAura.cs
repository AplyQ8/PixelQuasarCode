using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive Abilities/DecayAura")]
public class PA_DecayAura : BasePassiveAbilitySO
{
    [SerializeField] private float damage = 1;
    [SerializeField] private float tickRate = 1;
    [SerializeField] private Collider2D[] objectsToDealDamage;
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private RotEffect decay;
    
    public override void ApplyEffect(HeroComponents hero)
    {
        decay.SetDistance(distance);
        decay.SetBloodConsumption(0);
        decay.SetTarget(hero.Hero);
        hero.EffectHandler.ApplyEffect(decay);
    }

    public override void RemoveEffect(HeroComponents hero)
    {
        hero.EffectHandler.RemoveEffect(decay);
    }
}
