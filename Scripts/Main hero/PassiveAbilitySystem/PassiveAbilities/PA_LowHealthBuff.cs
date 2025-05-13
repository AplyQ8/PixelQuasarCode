using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "Passive Abilities/LowHealthBuff")]
public class PA_LowHealthBuff : BasePassiveAbilitySO
{
    [SerializeField] private float speedValue = 2f;
    [SerializeField] private float hpThreshold = 0.2f;
    private GameObject _hero;
    private IHealth _heroHealthScript;

    public override void ApplyEffect(HeroComponents hero)
    {
        _hero = hero.Hero;
        _hero.GetComponent<IDamageable>().OnHealthChange += ApplyBuff;
        _heroHealthScript = _hero.GetComponent<IHealth>();
    }

    public override void RemoveEffect(HeroComponents hero)
    {
        _hero.GetComponent<IDamageable>().OnHealthChange -= ApplyBuff;
    }

    private void ApplyBuff(float currentHealth)
    {
        if (currentHealth < _heroHealthScript.GetMaxHealthPoints() * hpThreshold)
        {
            _hero.GetComponent<IEffectable>().ApplyEffect(effect);
        }
        else
        {
            _hero.GetComponent<IEffectable>().RemoveEffect(effect);
        }
    }

}
