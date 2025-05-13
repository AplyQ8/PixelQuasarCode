using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Skill Logic/CharacteristicTreeSkillSO")]
public class CharacteristicsTreeSkillSO : BaseTreeSkillSO
{
    [SerializedDictionary("Modifier", "Value")]
    public SerializedDictionary<HeroStatModifierSO, float> modifiers;

    public override void Activate()
    {
        GameObject hero = GameObject.FindWithTag("Player");
        foreach(var modifier in modifiers)
        {
            modifier.Key.AffectHero(hero, modifier.Value);
        }
    }
}
