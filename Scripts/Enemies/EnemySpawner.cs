using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AYellowpaper.SerializedCollections;
using RouteScripts;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public class EnemySpawnInfo
    {
        [SerializeField] public string enemyType;
        [SerializeField] public GameObject patrolRoute;

        private List<PatrolPoint> patrolPoints;
        
        private bool isAlive;
        private float spawnTimer;

        public void SetIsAlive(bool alive){isAlive = alive;}
        public bool GetIsAlive() => isAlive;

        public void SetSpawnTimer(float time){spawnTimer = time;}

        public bool ReduceSpawnTimer(float time)
        {
            if (spawnTimer > 0)
            {
                spawnTimer -= time;
            }
            return spawnTimer > 0;
        }
        public float GetSpawnTimer() => spawnTimer;

        public void SetPatrolPoints()
        {
            patrolPoints = new List<PatrolPoint>();
            foreach (Transform point in patrolRoute.transform)
            {
                patrolPoints.Add(point.GetComponent<PatrolPointContainer>().GetPatrolPoint());
            }
        }
        
        public Vector2 GetSpawnPoint() => patrolPoints[0].GetPointPosition();
        public GameObject GetPatrolRoute() => patrolRoute;

    }

    [SerializeField] private float spawnCooldown;
    [SerializeField] private float timeToUpdate;
    [SerializeField] private float distanceToSpawn;
    [SerializeField] private int initialPoolSize;

    private GameObject player;
    
    [SerializedDictionary("Enemy Id", "Enemy info")]
    public SerializedDictionary<string, EnemySpawnInfo> enemiesInfo;
    
    [SerializedDictionary("Enemy type", "Enemy prefab")]
    public SerializedDictionary<string, GameObject> enemyPrefabs;

    private Utilities.ObjectPool enemyPools;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        enemyPools = gameObject.GetComponent<Utilities.ObjectPool>();

        foreach (var enemy in enemyPrefabs)
        {
            enemyPools.InitializePool(enemy.Key, initialPoolSize, enemy.Value);
        }
        
        foreach(var enemyInfo in enemiesInfo.Values)
        {
            enemyInfo.SetIsAlive(false);
            enemyInfo.SetSpawnTimer(0);
            enemyInfo.SetPatrolPoints();
        }

        StartCoroutine(UpdateSpawn());

    }

    IEnumerator UpdateSpawn()
    {
        for (;;)
        {
            foreach (var enemyInfo in enemiesInfo)
            {
                if (enemyInfo.Value.GetIsAlive())
                {
                    continue;
                }
                if (enemyInfo.Value.ReduceSpawnTimer(timeToUpdate))
                {
                    continue;
                }
                
                var spawnPoint = enemyInfo.Value.GetSpawnPoint();
                if ((spawnPoint - (Vector2)player.transform.position).magnitude > distanceToSpawn)
                {
                    continue;
                }
                
                GameObject newEnemy =
                    enemyPools.SpawnFromPool(enemyInfo.Value.enemyType, spawnPoint, Quaternion.identity);
                newEnemy.GetComponent<EnemyScript>().SetPatrolRoute(enemyInfo.Value.GetPatrolRoute());
                newEnemy.GetComponent<EnemyScript>().SetSpawnVars(this, enemyInfo.Key);
                enemyInfo.Value.SetIsAlive(true);
            }
            yield return new WaitForSeconds(timeToUpdate);
        }
    }

    public void BackToPool(string enemyId, GameObject enemy)
    {
        enemyPools.AddToPool(enemiesInfo[enemyId].enemyType, enemy);
        enemiesInfo[enemyId].SetIsAlive(false);
        enemiesInfo[enemyId].SetSpawnTimer(spawnCooldown);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

