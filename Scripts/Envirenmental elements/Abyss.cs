using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class Abyss : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("ObstacleCollider"))
        {
            if (collider.transform.parent.TryGetComponent(out ICanFall fallingObject))
            {
                if(fallingObject.CanFall())
                    fallingObject.DeathFromFalling();
            }
        }
    }
}
