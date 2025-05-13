using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public new string name;
    public AbilityType type;
    public float bloodConsumption;
    public float usabilityDistance;
    public float damage;
    [field: SerializeField] public float CoolDown { get; private set; }

    [SerializeField] private string targetedObjectTag;

    public enum AbilityType
    {
        HeroTargeted,
        PositionTargeted,
        ObjectTargeted,
        Durable
    }

    public virtual void Activate(GameObject hero) { }
    public virtual void Activate(GameObject hero, Vector2 position) { }
    public virtual void Activate(GameObject hero, GameObject target) { }

    public virtual void DeactivateAbility() { }

    public virtual string TargetTag() => targetedObjectTag;
    
}
