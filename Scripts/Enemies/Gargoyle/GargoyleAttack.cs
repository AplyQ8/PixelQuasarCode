using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class GargoyleAttack : MonoBehaviour
{
    private ICanAttack _attackScript;
    private Collider2D attackCollider;
    private SpriteRenderer visualisationSpriteRenderer;
    [SerializeField] private float attackDuration;
    [SerializeField] private float stunDuration;
    [SerializeField] private ParticleSystem _particleSystem;

    void Start()
    {
        _attackScript = gameObject.transform.parent.GetComponent<ICanAttack>();
        attackCollider = GetComponent<CircleCollider2D>();
        visualisationSpriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        
    }

    public void StartAttack()
    {
        attackCollider.enabled = true;
        //visualisationSpriteRenderer.enabled = true;
        _particleSystem.Play();
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
            damageableObject.TakeDamage(_attackScript.GetCurrentAttack(), DamageTypeManager.DamageType.Default);
        }

        if (collider.TryGetComponent(out IStunable stunableObject))
        {
            stunableObject.GetStunned(stunDuration);
        }
    }
}
