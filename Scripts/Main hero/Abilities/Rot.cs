using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Rot")]
public class Rot : Ability
{
    [SerializeField] private bool isActive = false;
    [SerializeField] private RotEffect rotEffect;
    [SerializeField] private ParticleSystem rotParticles;
    public event Action Deactivate = delegate {  };

    public override void Activate(GameObject hero)
    {
        Deactivate += DeactivateAbility;
        isActive = !isActive;
        if (isActive)
        {
            SetParticleSystemAttributes();
            rotEffect.SetDistance(usabilityDistance);
            rotEffect.SetBloodConsumption(bloodConsumption);
            rotEffect.SetTarget(hero);
            rotEffect.SetParticleEffect(rotParticles);
            rotEffect.SetAction(Deactivate);
            
            hero.GetComponent<IEffectable>().ApplyEffect(rotEffect);
        }
        else
            hero.GetComponent<IEffectable>().RemoveEffect(rotEffect);
    }

    public override void DeactivateAbility()
    {
        isActive = false;
    }

    // private void DeactivateAbility()
    // {
    //     isActive = false;
    // }

    private void SetParticleSystemAttributes()
    {
        ParticleSystem.ShapeModule psShape = rotParticles.shape;
        psShape.radius = usabilityDistance;
        rotParticles.GetComponent<RotParticleCollision>().SetDamage(damage);
    }
}
