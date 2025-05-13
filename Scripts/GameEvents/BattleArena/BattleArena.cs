using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

public class BattleArena: MonoBehaviour
{
    
    [SerializeField] private float fieldOfViewSize;

    [Serializable]
    public class EnemySpawnInfo
    {
        [SerializeField] public string enemyType;
        [SerializeField] public GameObject spawnPoint;
        [SerializeField] public float spawnTime;
    }
    
    [Serializable]
    public class WaveInfo
    {
        [SerializedDictionary("Enemy Id", "Enemy info")]
        public SerializedDictionary<string, EnemySpawnInfo> enemiesInfo;
        
        private List<EnemySpawnInfo> enemiesInfoList;

        public void SortEnemiesInfoList()
        {
            enemiesInfoList = enemiesInfo.Select(enemyInfo => enemyInfo.Value).ToList();
            enemiesInfoList.Sort((x, y) => x.spawnTime.CompareTo(y.spawnTime));
        }

        public List<EnemySpawnInfo> GetEnemiesInfoList() => enemiesInfoList;
    }

    [SerializeField] List<WaveInfo> wavesInfo;
    
    [SerializedDictionary("Enemy type", "Enemy prefab")]
    public SerializedDictionary<string, GameObject> enemyPrefabs;

    private bool battleStarted;
    private float battleTimer;
    
    private int waveIndex; 
    private int nextSpawnIndex;
    private float iterationDuration;

    private bool spawnEnd;

    private List<GameObject> spawnedEnemies;
    
    private Utilities.ObjectPool enemyPools;

    private GameEvent battleArenaEvent;

    public event Action BattleEndEvent;
    
    void Start()
    {
        battleStarted = false;
        spawnEnd = false;
        
        battleArenaEvent = transform.GetComponent<GameEvent>();

        foreach (var waveInfo in wavesInfo)
        {
            waveInfo.SortEnemiesInfoList();
        }
        
        enemyPools = gameObject.GetComponent<Utilities.ObjectPool>();

        foreach (var enemy in enemyPrefabs)
        {
            enemyPools.InitializePool(enemy.Key, 5, enemy.Value);
        }

        waveIndex = 0;
        nextSpawnIndex = 0;
        iterationDuration = 1;
    }

    
    void Update()
    {
        
    }

    private void StartBattle()
    {
        if(battleArenaEvent)
            battleArenaEvent.StartEvent();
        
        battleStarted = true;
        StartCoroutine(UpdateWaveSpawn());
    }
    
    IEnumerator UpdateWaveSpawn()
    {
        spawnEnd = false;
        List<EnemySpawnInfo> enemiesInfoList = wavesInfo[waveIndex].GetEnemiesInfoList();
        spawnedEnemies = new List<GameObject>();
        
        for (;;)
        {
            while (enemiesInfoList[nextSpawnIndex].spawnTime <= battleTimer)
            {
                SpawnEnemy(enemiesInfoList[nextSpawnIndex]);
                nextSpawnIndex++;
                if (nextSpawnIndex == enemiesInfoList.Count)
                {
                    spawnEnd = true;
                    break;
                }
            }

            if (spawnEnd)
            {
                break;
            }
            yield return new WaitForSeconds(iterationDuration);
            battleTimer += iterationDuration;
        }
        
        StartCoroutine(CheckWaveEnd());
    }

    IEnumerator CheckWaveEnd()
    {
        for (;;)
        {
            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                if (!spawnedEnemies[i].activeSelf)
                {
                    spawnedEnemies.RemoveAt(i);
                    i -= 1;
                }
            }
            yield return new WaitForSeconds(1);

            if (spawnedEnemies.Count == 0)
            {
                break;
            }
        }

        waveIndex += 1;

        if (waveIndex == wavesInfo.Count)
        {
            EndBattle();
        }
        else
        {
            battleTimer = 0;
            nextSpawnIndex = 0;
            StartCoroutine(UpdateWaveSpawn());
        }
    }

    private void EndBattle()
    {
        if (battleArenaEvent)
        {
            battleArenaEvent.EndEvent();
            BattleEndEvent?.Invoke();
        }
    }

    private void SpawnEnemy(EnemySpawnInfo enemyInfo)
    {

        var spawnPoint = enemyInfo.spawnPoint.transform.position;
            
        GameObject newEnemy =
            enemyPools.SpawnFromPool(enemyInfo.enemyType, spawnPoint, Quaternion.identity);
        spawnedEnemies.Add(newEnemy);
        // newEnemy.GetComponent<EnemyScript>().SetPatrolRoute(enemyInfo.Value.GetPatrolRoute());
        newEnemy.GetComponent<EnemyScript>().SetSpawnVars(enemyPools, enemyInfo.enemyType);
        newEnemy.GetComponent<FieldOfView>().radius = fieldOfViewSize;
        newEnemy.GetComponent<FieldOfView>().angle = 360;
        newEnemy.GetComponent<BattleDetector>().radius = fieldOfViewSize;
    }
    

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && !battleStarted)
        {
            StartBattle();
        }
        
    }
}
