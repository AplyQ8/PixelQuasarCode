using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class DeadManDagger : MonoBehaviour
{
    private CircleCollider2D circleCollider2D;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer extraSpriteRenderer;

    [SerializeField] private float prepTime;
    [SerializeField] private float attackTime;
    [SerializeField] private float damage;
    void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        extraSpriteRenderer = transform.Find("ExtraSprite").GetComponent<SpriteRenderer>();

        // StartCoroutine(TestStart());
    }

    // IEnumerator TestStart()
    // {
    //     for (;;)
    //     {
    //         yield return new WaitForSeconds(5);
    //         StartAttack();
    //     }
    // }

    public void StartAttack()
    {
        spriteRenderer.enabled = true;
        StartCoroutine(Preparation());
    }

    IEnumerator Preparation()
    {
        yield return new WaitForSeconds(prepTime);
        Activate();
    }

    private void Activate()
    {
        circleCollider2D.enabled = true;
        extraSpriteRenderer.enabled = true;
        StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(attackTime);
        circleCollider2D.enabled = false;
        spriteRenderer.enabled = false;
        extraSpriteRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("ObstacleCollider"))
            return;

        var colliderParent = collider.transform.parent;
        if(!colliderParent.CompareTag("Player"))
            return;
        
        if (colliderParent.TryGetComponent(out IDamageable damageableObject))
        {
            damageableObject.TakeDamage(damage, DamageTypeManager.DamageType.Default);
        }
    }
}
