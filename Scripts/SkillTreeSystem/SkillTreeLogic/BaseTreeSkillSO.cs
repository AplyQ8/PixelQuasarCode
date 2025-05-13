using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTreeSkillSO : ScriptableObject
{
    [SerializeField] protected string description;
    public abstract void Activate();

    public virtual string GetDescription() => description;
}
