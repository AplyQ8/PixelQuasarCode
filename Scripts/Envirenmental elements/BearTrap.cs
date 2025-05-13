using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class BearTrap : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private int damage;
    [SerializeField] private float embedTime;
    [SerializeField] private bool isActivated;
    private GameObject _victim;
    private static readonly int Activated = Animator.StringToHash("Activated");
    private void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("Enemy") && !col.CompareTag("Player"))
            return;
        if (isActivated || col.GetComponentInChildren<SpriteRenderer>().transform.position.y < transform.position.y)
            return;
        _victim = col.gameObject;
        _victim.GetComponent<Rigidbody2D>().WakeUp();
        isActivated = true;
        GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
        animator.SetBool(Activated, true);
        if (_victim.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(damage, DamageTypeManager.DamageType.Trap);
        }
        if (_victim.TryGetComponent(out IStunable stunable))
        {
            stunable.GetStunned(embedTime);
        }
        StartCoroutine(EmbeddedTime());
    }

    private IEnumerator EmbeddedTime()
    {
        yield return new WaitForSeconds(embedTime);
        Destroy(gameObject);
    }
}
