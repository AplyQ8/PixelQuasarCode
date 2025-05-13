using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class SorcererSphere : MonoBehaviour
{
    
    [SerializeField] protected Animator animationController;
    private static readonly int Explosion = Animator.StringToHash("Explosion");
    private static readonly int End = Animator.StringToHash("End");

    private enum SphereState
    {
        Preparation,
        Explosion,
        None
    }

    private SphereState state;
    
    private CircleCollider2D explosionCollider;
    [SerializeField] private float prepDuration;
    private float prepTimer;
    [SerializeField] private float explosionDuration;
    private float explosionTimer;
    
    [SerializeField] private float MaxRadius;
    [SerializeField] private float MinRadius;
    
    [SerializeField] private float damage;

    private Utilities.ObjectPool spawnPool;
    private string spawnId;

    private Transform spriteTransform;
    
    void Awake()
    {
        explosionCollider = transform.GetComponent<CircleCollider2D>();
        explosionCollider.enabled = false;
        state = SphereState.None;

        spriteTransform = transform.Find("Sprite");
    }

    // private void Start()
    // {
    //     StartExplosion();
    // }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case SphereState.Preparation:
                prepTimer -= Time.deltaTime;
                if (prepTimer <= 0)
                {
                    explosionTimer = explosionDuration;
                    explosionCollider.radius = MinRadius;
                    explosionCollider.enabled = true;
                    state = SphereState.Explosion;
                    animationController.SetTrigger(Explosion);
                }
                break;
            
            case SphereState.Explosion:
                UpdateSphereSize();
                
                explosionTimer -= Time.deltaTime;
                // if (explosionTimer <= 0)
                // {
                //     explosionCollider.enabled = false;
                //     state = SphereState.None;
                //     animationController.SetTrigger(End);
                //     BackToPool();
                // }
                break;
        }
    }

    public void StartExplosion()
    {
        prepTimer = prepDuration;
        state = SphereState.Preparation;
    }

    public void EndExplosion()
    {
        explosionCollider.enabled = false;
        state = SphereState.None;
        animationController.SetTrigger(End);
        BackToPool();
    }

    private void UpdateSphereSize()
    {
        float newRadius = MinRadius + (MaxRadius - MinRadius) * (1 - explosionTimer / explosionDuration);
        explosionCollider.radius = newRadius;
        
        // float newSpriteScale = 1 + (4.65f - 1) * (1 - explosionTimer / explosionDuration);
        // spriteTransform.localScale = new Vector3(newSpriteScale, newSpriteScale, newSpriteScale);
    }
    
    
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.CompareTag("Player"))
        {
            if (collider.TryGetComponent<IDamageable>(out IDamageable damageableObject))
            {
                damageableObject.TakeDamage(damage, DamageTypeManager.DamageType.Default);
            }
            explosionCollider.enabled = false;
        }
        
    }
    

    public void SetPool(Utilities.ObjectPool newPool, string id)
    {
        spawnPool = newPool;
        spawnId = id;
    }

    public void BackToPool()
    {
        Destroy(gameObject);
        
        // if (!spawnPool)
        // {
        //     Destroy(gameObject);
        // }
        //
        // spawnPool.AddToPool(spawnId, transform.gameObject);
    }
}
