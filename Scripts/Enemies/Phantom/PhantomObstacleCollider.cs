using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomObstacleCollider : MonoBehaviour
{

    private PhantomScript phantomScript;
    
    private Transform bodyBottom;
    // Start is called before the first frame update
    void Start()
    {
        phantomScript = transform.parent.GetComponent<PhantomScript>();
        bodyBottom = transform.parent.Find("ObstacleCollider");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    private void OnTriggerEnter2D(Collider2D collisionObj)
    {
        phantomScript.ObstacleColliderEnterTrigger(collisionObj);
    }

    public void SetOffset(Vector2 offset)
    {
        transform.position = bodyBottom.position + (Vector3)offset;
    }
    
}
