using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneGoal : MonoBehaviour
{
    [SerializeField] private GameObject sceneReloaderObject;
    private SceneReloader sceneReloader;
    private ZoneGoalEnemyDetector enemyDetector;
    private Collider2D enemyDetectorCollider;

    private Collider2D collider;
    
    // Start is called before the first frame update
    void Start()
    {
        sceneReloader = sceneReloaderObject.GetComponent<SceneReloader>();
        enemyDetector = transform.Find("EnemyDetector").GetComponent<ZoneGoalEnemyDetector>();
        enemyDetectorCollider = transform.Find("EnemyDetector").GetComponent<CircleCollider2D>();

        collider = transform.GetComponent<Collider2D>();
        
        StartCoroutine(CheckEnemies());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator CheckEnemies()
    {
        while (true)
        {
            enemyDetectorCollider.enabled = false;
            enemyDetector.enemyExist = false;
            enemyDetectorCollider.enabled = true;
            yield return new WaitForSeconds(0.1f);
            if (!enemyDetector.enemyExist)
            {
                NoEnemies();
                break;
            }
            yield return new WaitForSeconds(0.9f);
        }
    }

    private void NoEnemies()
    {
        transform.GetComponent<SpriteRenderer>().color = Color.green;
        collider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D objectCollider)
    {
        if (objectCollider.gameObject.CompareTag("ObstacleCollider") && objectCollider.transform.parent.CompareTag("Player"))
        {
            sceneReloader.LoadNextSceneInBuild();
        }
    }
}
