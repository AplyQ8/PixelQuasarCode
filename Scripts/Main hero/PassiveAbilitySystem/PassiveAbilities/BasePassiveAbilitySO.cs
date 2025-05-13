using System.Collections;
using System.Collections.Generic;
using Status_Effect_System;
using UnityEngine;

[CreateAssetMenu(menuName = "Passive Abilities/Effect appliance ability")]
public class BasePassiveAbilitySO : ScriptableObject
{
    public string Name { get; private set; }
    [SerializeField] private protected StatusEffectData effect;

    public virtual void ApplyEffect(HeroComponents hero)
    {
        hero.Hero.GetComponent<IEffectable>().ApplyEffect(effect);
    }

    public virtual void RemoveEffect(HeroComponents hero)
    {
        hero.Hero.GetComponent<IEffectable>().RemoveEffect(effect);
    }

    public virtual void OverTimeEffect() { }
    
    
}
