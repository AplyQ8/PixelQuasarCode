using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeDashRange : MonoBehaviour
{
    private KamikazeSkeletonScript kamikazeScript;
    private Collider2D collider2D;
    void Start()
    {
        kamikazeScript = transform.parent.gameObject.GetComponent<KamikazeSkeletonScript>();
        // collider2D = GetComponent<Collider2D>();
        // collider2D.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collisionObj)
    {
        if (collisionObj.gameObject.CompareTag("Player"))
        {
            kamikazeScript.StartDashPrep();
        }
    }
}
