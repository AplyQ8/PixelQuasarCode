using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionStartRange : MonoBehaviour
{
    private KamikazeSkeletonScript kamikazeScript;
    void Start()
    {
        kamikazeScript = transform.parent.gameObject.GetComponent<KamikazeSkeletonScript>();
    }

    private void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (collisionObj.gameObject.CompareTag("Player"))
        {
            kamikazeScript.StartExplosionPrep();
        }
    }
}
