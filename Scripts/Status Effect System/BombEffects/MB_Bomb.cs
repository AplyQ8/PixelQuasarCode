using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB_Bomb : MonoBehaviour
{
    [SerializeField] private BaseBombEffect typeOfBomb;

    private void OnCollisionEnter2D(Collision2D col)
    {
        //Some conditions
        if (!col.gameObject.TryGetComponent(out IEffectable effectable)) return;
        effectable.ApplyEffect(typeOfBomb);
        Destroy(gameObject);
    }
    
}
