using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
    [SerializeField] private int poolSize;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Queue<Projectile> _projectiles;

    private void Awake()
    {
        _projectiles = new Queue<Projectile>();
        InitializePool(poolSize);
    }

    public void InitializeProjectile(Transform targetToMove, float projectileSpeed, List<ModifierData> modifiers)
    {
        if(NoProjectilesInPool)
            InitializePool(poolSize / 2);
        var projectileToLaunch = _projectiles.Dequeue();
        projectileToLaunch.gameObject.SetActive(true);
        projectileToLaunch.Launch(targetToMove, projectileSpeed, modifiers);
    }

    public void EnqueueProjectile(Projectile projectile)
    {
        _projectiles.Enqueue(projectile);
    }

    private void InitializePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            var newProjectile = Instantiate(projectilePrefab, transform.position, quaternion.identity);
            newProjectile.transform.SetParent(gameObject.transform);
            newProjectile.Initialize(this);
        }
    }

    private bool NoProjectilesInPool => _projectiles.Count is 0;
}
