using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform positionToFly;
    [SerializeField] private float speed;
    [SerializeField] private ProjectilePooler currentPooler;
    [SerializeField] private float projectileLifeTime = 5;
    private Timer _projectileLifeTimer;
    private List<ModifierData> _modifiers;

    public void Initialize(ProjectilePooler pooler)
    {
        currentPooler = pooler;
        gameObject.SetActive(false);
        currentPooler.EnqueueProjectile(this);
        _projectileLifeTimer = new Timer(projectileLifeTime);
        _projectileLifeTimer.OnTimerDone += ReturnIntoPool;
    }

    public void Launch(Transform target, float initialSpeed, List<ModifierData> modifiers)
    {
        _modifiers = modifiers;
        positionToFly = target;
        speed = initialSpeed;
        _projectileLifeTimer.StartTimer();
    }

    private void Update()
    {
        _projectileLifeTimer.Tick();
    }
    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            positionToFly.position, 
            speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        foreach (var modifier in _modifiers)
        {
            modifier.statModifier.AffectObject(col.gameObject, modifier.value);
        }
        ReturnIntoPool();
    }

    private void ReturnIntoPool()
    {
        gameObject.SetActive(false);
        currentPooler.EnqueueProjectile(this);
        transform.position = currentPooler.gameObject.transform.position;
    }
    
}
