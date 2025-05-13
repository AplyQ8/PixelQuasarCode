using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAbilityCollider : MonoBehaviour
{
    public event Action<Collider2D> OnCollision;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        OnCollision?.Invoke(col);
    }
}
