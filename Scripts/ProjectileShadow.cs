using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShadow : MonoBehaviour
{
    private EnemyProjectile projectileScript;
    // Start is called before the first frame update
    void Start()
    {
        projectileScript = transform.parent.gameObject.GetComponent<EnemyProjectile>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {

        // if (!collider.CompareTag("Enemy") && !collider.CompareTag("Player") && !collider.CompareTag("Intangible"))
        if(collider.CompareTag("Obstacle"))
        {
            projectileScript.DestroyProjectile();
        }
        
    }
}
