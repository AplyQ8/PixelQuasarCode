using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatModifierSO : ScriptableObject
{
    public abstract void AffectObject(GameObject objectToDealDamage, float value);

    public abstract void RemoveEffect(GameObject objectToRemoveEffect, float value);
}
