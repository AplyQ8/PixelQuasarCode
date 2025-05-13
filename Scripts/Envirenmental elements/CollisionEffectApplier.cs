using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEffectApplier : MonoBehaviour
{
    [SerializeField] private List<ModifierData> effects;

    private void OnTriggerStay2D(Collider2D other)
    {
        try
        {
            other.GetComponent<Rigidbody2D>().WakeUp();
            if (other.transform.Find("ObstacleCollider").transform.position.y <
                gameObject.transform.position.y)
                return;
        }
        catch (Exception ex)
        {
            // ignored
        }

        foreach (var effect in effects)
        {
            effect.statModifier.AffectObject(other.gameObject, effect.value);
        }
    }
}
