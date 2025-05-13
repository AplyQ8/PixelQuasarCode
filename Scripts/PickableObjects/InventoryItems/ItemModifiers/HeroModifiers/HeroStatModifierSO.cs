using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeroStatModifierSO : ScriptableObject
{
    public abstract void AffectHero(GameObject hero, float value);
}
