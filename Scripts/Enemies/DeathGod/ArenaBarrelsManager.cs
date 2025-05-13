using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaBarrelsManager : MonoBehaviour
{
    [Header("Barrel Settings")]
    public GameObject barrelPrefab; // Prefab for the explosive barrel
    public float spawnInterval = 5f; // Time interval between spawns

    private List<ArenaBarrelSpawnPoint> spawnPoints;

    void Start()
    {
        // Initialize spawn points
        spawnPoints = new List<ArenaBarrelSpawnPoint>();

        foreach (Transform child in transform)
        {
            ArenaBarrelSpawnPoint spawnPoint = child.GetComponent<ArenaBarrelSpawnPoint>();
            if (spawnPoint != null)
            {
                spawnPoints.Add(spawnPoint);
            }
        }
        
    }

    public void StartSpawningBarrels()
    {
        StartCoroutine(SpawnBarrelsRoutine());
    }
    
    public void StopSpawningBarrels()
    {
        StopAllCoroutines();
        ExplodeAllBarrels();
    }

    private void ExplodeAllBarrels()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            spawnPoint.ExplodeBarrel();
        }
    }

    private IEnumerator SpawnBarrelsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            SpawnBarrel();
        }
    }

    private void SpawnBarrel()
    {
        // Find an available spawn point
        List<ArenaBarrelSpawnPoint> availablePoints = spawnPoints.FindAll(sp => sp.IsVacant);

        if (availablePoints.Count == 0) return;

        // Choose a random spawn point
        ArenaBarrelSpawnPoint chosenPoint = availablePoints[Random.Range(0, availablePoints.Count)];

        // Spawn the barrel
        GameObject barrel = Instantiate(barrelPrefab, chosenPoint.transform.position, Quaternion.identity);
        chosenPoint.AssignBarrel(barrel);
    }
}