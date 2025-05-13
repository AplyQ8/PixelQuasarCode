using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyProjectileSpawner : MonoBehaviour
{

    public ObjectPool<EnemyProjectile> pool;
    public EnemyProjectile projectilePref;
    
    // Start is called before the first frame update
    void Start()
    {
        pool = new ObjectPool<EnemyProjectile>(CreateProjectile, OnTakeProjectileFromPool,
            OnReturnProjectileToPool, OnDestroyProjectile, true, 10, 15);
        
    }

    private EnemyProjectile CreateProjectile()
    {
        EnemyProjectile arrow = Instantiate(projectilePref, transform.position, Quaternion.identity);
        arrow.SetPool(pool);
        return arrow;
    }

    private void OnTakeProjectileFromPool(EnemyProjectile arrow)
    {
        arrow.transform.position = transform.position;
        arrow.gameObject.SetActive(true);
    }

    private void OnReturnProjectileToPool(EnemyProjectile arrow)
    {
        arrow.gameObject.SetActive(false);
    }
    
    private void OnDestroyProjectile(EnemyProjectile arrow)
    {
        if(arrow != null)
            Destroy(arrow.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

