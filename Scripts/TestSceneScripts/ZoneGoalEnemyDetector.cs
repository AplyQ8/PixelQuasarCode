using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneGoalEnemyDetector : MonoBehaviour
{

    public bool enemyExist;
    // Start is called before the first frame update
    void Start()
    {
        enemyExist = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            enemyExist = true;
        }
    }
}
