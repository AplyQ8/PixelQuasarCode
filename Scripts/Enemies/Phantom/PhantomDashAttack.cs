using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomDashAttack : MonoBehaviour
{
    
    private PhantomScript phantomScript;

    void Start()
    {
        phantomScript = transform.parent.gameObject.GetComponent<PhantomScript>();
    }
    
    
    private void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (collisionObj.gameObject.CompareTag("Player"))
        {
            phantomScript.DashAttackTriggerEnter(collisionObj);
        }
    }
}
