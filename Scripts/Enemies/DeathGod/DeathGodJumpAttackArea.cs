using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class DeathGodJumpAttackArea : MonoBehaviour
{
    
    private Collider2D attackCollider;
    private SpriteRenderer visualisationSpriteRenderer;
    private float damage;
    [SerializeField] private float attackDuration;
    
    
    void Start()
    {
        attackCollider = GetComponent<CircleCollider2D>();
        visualisationSpriteRenderer = GetComponent<SpriteRenderer>();
        damage = GetComponentInParent<DeathGod>().GetJumpAttackDamage();
    }
    
    public void StartAttack()
    {
        attackCollider.enabled = true;
        visualisationSpriteRenderer.enabled = true;
        StartCoroutine(EndAttack(attackDuration));
    }
    
    IEnumerator EndAttack(float duration)
    {
        yield return new WaitForSeconds(duration);
        attackCollider.enabled = false;
        visualisationSpriteRenderer.enabled = false;
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
