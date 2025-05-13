using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using ObjectLogicInterfaces;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

// [ExecuteAlways] // Ensures it updates in the Editor
public class DeathGodArena : MonoBehaviour
{
    [Header("Ellipse Parameters")]
    public float semiMajorAxis = 10f; // Horizontal radius (a)
    public float semiMinorAxis = 5f;  // Vertical radius (b)
    public Color ellipseColor = Color.green; // Color of the ellipse
    
    [Header("Spawn Enemies Parameters")]
    [SerializedDictionary("Enemy Id", "Enemy info")]
    public SerializedDictionary<string, EnemySpawnInfo> enemiesInfo;

    public int maxEnemies;
    public float spawnCooldown;
    public int startSpawnsNumber;
    
    private Collider2D arenaCollider;
    private LayerMask enemyLayer;
    
    private ObjectPool summonPool;

    [Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        public float spawnProbability;
    }

    public GameObject deathGod;

    private void Start()
    {
        arenaCollider = GetComponent<Collider2D>();
        
        summonPool = GetComponent<ObjectPool>();
        foreach (var enemyName in enemiesInfo.Keys)
        {
            summonPool.InitializePool(enemyName, 3, enemiesInfo[enemyName].enemyPrefab);
        }
        
        enemyLayer = LayerMask.GetMask("Enemy");
        
    }

    public void StartSpawningEnemies()
    {
        StartCoroutine(SpawnEnemies());
    }

    public void StopSpawningEnemies()
    {
        StopAllCoroutines();
        KillAllEnemies();
    }

    IEnumerator SpawnEnemies()
    {
        for (;;)
        {
            int enemiesNumber = CountEnemiesInArena();
            if (enemiesNumber == 0)
            {
                for(int i = 0; i < startSpawnsNumber; i++)
                    SpawnRandomEnemy();
            }
            else if(enemiesNumber < maxEnemies)
                SpawnRandomEnemy();
            
            yield return new WaitForSeconds(Random.Range(0.75f*spawnCooldown, 1.25f*spawnCooldown));
        }
    }

    private void SpawnRandomEnemy()
    {
        string enemyName = GetRandomEnemyName();
        Vector3 spawnPoint = GetRandomPointOnArena();
        
        GameObject newEnemy =
            summonPool.SpawnFromPool(enemyName, spawnPoint, Quaternion.identity);
        newEnemy.GetComponent<EnemyScript>().SetSpawnVars(summonPool, enemyName);
        newEnemy.transform.SetParent(null);
    }
    
    private string GetRandomEnemyName()
    {
        // Calculate total probability
        float totalProbability = 0f;
        foreach (var entry in enemiesInfo)
        {
            totalProbability += entry.Value.spawnProbability;
        }

        // Generate a random value in the range [0, totalProbability]
        float randomValue = Random.Range(0f, totalProbability);

        // Find the corresponding enemy
        float cumulativeProbability = 0f;
        foreach (var entry in enemiesInfo)
        {
            cumulativeProbability += entry.Value.spawnProbability;
            if (randomValue <= cumulativeProbability)
            {
                return entry.Key; // Return the name of the selected enemy
            }
        }

        return null; // This line should not be reached if probabilities are correctly set
    }
    
    public int CountEnemiesInArena()
    {
        // Array to hold detected colliders
        Collider2D[] results = new Collider2D[50]; // Adjust size if necessary

        // Use OverlapCollider to detect objects within the collider bounds
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useLayerMask = true;
        filter.useTriggers = true;

        int count = arenaCollider.OverlapCollider(filter, results);

        // Count colliders with the "Enemy" tag (if needed)
        int enemyCount = 0;
        for (int i = 0; i < count; i++)
        {
            if (results[i].CompareTag("Enemy"))
            {
                enemyCount++;
            }
        }

        count -= 1; // Т.к. босс не должен учитываться
        return count;
    }

    private void KillAllEnemies()
    {
        // Array to hold detected colliders
        Collider2D[] results = new Collider2D[50]; // Adjust size if necessary

        // Set up a filter for the enemy layer
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useLayerMask = true;
        filter.useTriggers = true;

        // Find all colliders within the arena
        int count = arenaCollider.OverlapCollider(filter, results);

        // Iterate through detected colliders
        for (int i = 0; i < count; i++)
        {
            Collider2D collider = results[i];

            // Skip if it's the deathGod
            if (collider.gameObject == deathGod)
                continue;

            // Try to get the IDamageable component and apply lethal damage
            if (collider.TryGetComponent(out IDamageable damageableObject))
            {
                damageableObject.TakeDamage(9999999, DamageTypeManager.DamageType.Default);
            }
        }
    }
    
    
    
    private void OnDrawGizmos()
    {
        // Set the Gizmo color
        Gizmos.color = ellipseColor;

        // Draw the ellipse
        Vector3 previousPoint = Vector3.zero;
        int segments = 100; // Number of points to draw the ellipse
        for (int i = 0; i <= segments; i++)
        {
            // Calculate the angle for this segment
            float angle = (i / (float)segments) * Mathf.PI * 2;

            // Parametric equation for an ellipse
            float x = semiMajorAxis * Mathf.Cos(angle);
            float y = semiMinorAxis * Mathf.Sin(angle);

            // Convert to world position
            Vector3 currentPoint = transform.position + new Vector3(x, y, 0);

            // Draw a line from the previous point to the current point
            if (i > 0)
            {
                Gizmos.DrawLine(previousPoint, currentPoint);
            }

            // Update the previous point
            previousPoint = currentPoint;
        }
    }

    public Vector3 GetRandomPointOnArena()
    {
        var center = transform.position;
        var horizontalRadius = semiMajorAxis;
        var verticalRadius = semiMinorAxis;
        
        // Generate a random angle (0 to 2π)
        float angle = Random.Range(0f, Mathf.PI * 2);

        // Generate a random radius using square root to ensure uniform distribution
        float radius = Mathf.Sqrt(Random.Range(0f, 1f));

        // Calculate the x and y coordinates using the parametric equation of an ellipse
        float x = center.x + radius * Mathf.Cos(angle) * horizontalRadius;
        float y = center.y + radius * Mathf.Sin(angle) * verticalRadius;

        return new Vector2(x, y);
    }
}

