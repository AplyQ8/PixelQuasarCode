using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class DeathGodMeleeAttack : MonoBehaviour
{

    private Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private float damage;
    
    // Start is called before the first frame update
    void Start()
    {
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        collider2D.enabled = false;
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAttack()
    {
        collider2D.enabled = true;
        spriteRenderer.enabled = true;
    }
    
    public void StartAttack(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        
        StartAttack();
    }
    
    public void EndAttack()
    {
        collider2D.enabled = false;
        spriteRenderer.enabled = false;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player"))
            return;
        
        if (collider.TryGetComponent(out IDamageable damageableObject))
        {
            damageableObject.TakeDamage(damage, DamageTypeManager.DamageType.Default);
        }
    }
}
