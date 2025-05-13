using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;
using Utilities;

public class SpearTrap : BaseTrap
{
    [SerializeField] private GameObject frontTrapView;
    [SerializeField] private Animator backView;
    [SerializeField] private Animator frontView;
    [SerializeField] private TrapStates currentState;
    [SerializeField] private float damageTickRate;
    [SerializeField] private float inactiveTime;
    [SerializeField] private float activeTime;
    private float _elapsed = 0f;
    private static readonly int Appear = Animator.StringToHash("Appear");
    private static readonly int Disappear = Animator.StringToHash("Disappear");

    [SerializeField] private AudioSource bladeSound;
    

    private enum TrapStates
    {
        Active,
        Inactive
    }

    private void Start()
    {
        StartCoroutine(InactiveState());
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        try
        {
            col.gameObject.GetComponent<Rigidbody2D>().WakeUp();
            if (currentState == TrapStates.Inactive)
                return;
            if (col.GetComponentInChildren<SpriteRenderer>().transform.position.y < frontTrapView.transform.position.y)
                return;
            _elapsed += Time.deltaTime;
            if (_elapsed >= damageTickRate)
            {
                _elapsed %= damageTickRate;
                if (col.gameObject.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage, DamageTypeManager.DamageType.Trap);
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    private IEnumerator InactiveState()
    {
        currentState = TrapStates.Inactive;
        yield return new WaitForSeconds(inactiveTime);
        StartCoroutine(ActiveState());
        
        backView.SetTrigger(Appear);
        frontView.SetTrigger(Appear);
    }

    private IEnumerator ActiveState()
    {
        currentState = TrapStates.Active;
        
        yield return new WaitForSeconds(activeTime);
        StartCoroutine(InactiveState());
        backView.SetTrigger(Disappear);
        frontView.SetTrigger(Disappear);
    }

    public void PlaySoundEffect()
    {
        if (!bladeSound.enabled) return;
        bladeSound.Play();
    }
}
