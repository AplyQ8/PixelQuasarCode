using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolDeath : MonoBehaviour
{
    protected EnemyScript enemyScript;
    protected UniqueId UniqueId;
    
    public void Death()
    {
        string id = UniqueId.UniqueID;
        if (!EnemyManager.destroyedEnemyIDs.Contains(id))
        {
            EnemyManager.destroyedEnemyIDs.Add(id);
        }
        EnemySpawner spawner = enemyScript.GetSpawner();
        Utilities.ObjectPool spawnPool = enemyScript.GetSpawnPool();
        if (spawner is not null)
        {
            string spawnId = enemyScript.GetSpawnId();
            StartCoroutine(DelayedBackToPool(spawner, spawnId, 0));
        }
        else if (spawnPool is not null)
        {
            string spawnId = enemyScript.GetSpawnId();
            StartCoroutine(DelayedBackToPool(spawnPool, spawnId, 0));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DelayedBackToPool(EnemySpawner spawner, string spawnId, float delay)
    {
        yield return new WaitForSeconds(delay);
        spawner.BackToPool(spawnId, transform.gameObject);
    }
    IEnumerator DelayedBackToPool(Utilities.ObjectPool pool, string spawnId, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (pool != null)
        {
            pool.AddToPool(spawnId, transform.gameObject);
        }
        else
        {
            Destroy(gameObject, 0);
        }
    }
}
