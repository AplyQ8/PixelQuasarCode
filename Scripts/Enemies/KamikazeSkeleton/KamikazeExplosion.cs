using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class KamikazeExplosion : MonoBehaviour
{
    
    [SerializeField]private float lifeDuration;
    private float lifeTimer;
    
    [SerializeField]private float damageDuration;
    private float damageTimer;
    
    [SerializeField] private ParticleSystem explosionEffect;
    
    [SerializeField] private float damage;
    
    private Collider2D explosionCollider;
    void Awake()
    {
        explosionCollider = GetComponent<CircleCollider2D>();
        lifeTimer = lifeDuration;
        damageTimer = damageDuration;
    }

    void OnEnable()
    {
        lifeTimer = lifeDuration;
        damageTimer = damageDuration;
        explosionEffect.Play();
    }
    
    void Update()
    {
        damageTimer -= Time.deltaTime;
        if (damageTimer <= 0)
        {
            explosionCollider.enabled = false;
        }
        
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0)
        {
            Destroy(gameObject);
        }
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
}
