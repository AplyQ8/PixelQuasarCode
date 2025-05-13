using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItemDataSO : ScriptableObject
{
    public virtual bool Throw(GameObject hero, 
        Vector2 groundVelocity, float verticalVelocity, 
        List<ModifierData> dataModifiers,
        LayerMask layerMask) { return true; }
}
