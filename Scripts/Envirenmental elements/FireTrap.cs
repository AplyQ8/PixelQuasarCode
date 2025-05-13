using System;
using System.Collections;
using System.Collections.Generic;
using Status_Effect_System;
using UnityEngine;

public class FireTrap : BaseTrap
{
    [SerializeField] private Animator animController;

    [SerializeField] private GameObject trapBase;
    [SerializeField] private TrapStates currentState;
    [SerializeField] private float inactiveTime;
    [SerializeField] private float activeTime;
    [SerializeField] private float preparationTime;
    private static readonly int Preparation = Animator.StringToHash("Preparation");
    private static readonly int Activate = Animator.StringToHash("Activate");
    private static readonly int EndState = Animator.StringToHash("EndState");
    [SerializeField] private StatusEffectData burnEffect;
    [SerializeField] private AudioSource burnSoundEffect;
    [SerializeField] private AudioSource startFireSoundEffect;
    [SerializeField] private float fireSoundFadeDuration;
    [Range(0, 1)][SerializeField] private float fireEffectVolume;

    private enum TrapStates
    {
        Inactive,
        Preparation,
        Active,
    }

    void Start()
    {
        StartCoroutine(InactiveState());
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        try
        {
            col.GetComponent<Rigidbody2D>().WakeUp();
            if (col.GetComponentInChildren<SpriteRenderer>().transform.position.y <
                trapBase.transform.position.y)
                return;
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

        if (currentState != TrapStates.Active) return;
        if (col.TryGetComponent(out IEffectable effectable))
        {
            effectable.ApplyEffect(burnEffect, damage);
        }
    }

    private IEnumerator InactiveState()
    {
        currentState = TrapStates.Inactive;
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(inactiveTime);
        animController.SetTrigger(Preparation);
        StartCoroutine(PreparationState());
    }

    private IEnumerator PreparationState()
    {
        currentState = TrapStates.Preparation;
        startFireSoundEffect.Play();
        animController.SetTrigger(Preparation);
        yield return new WaitForSeconds(preparationTime);
        StartCoroutine(ActiveState());
    }

    private IEnumerator ActiveState()
    {
        currentState = TrapStates.Active;
        burnSoundEffect.volume = fireEffectVolume;
        burnSoundEffect.Play();
        animController.SetTrigger(Activate);
        yield return new WaitForSeconds(activeTime);
        StartCoroutine(InactiveState());
        animController.SetTrigger(EndState);
    }
    
    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fireSoundFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float volume = Mathf.Lerp(fireEffectVolume, 0f, elapsedTime / fireSoundFadeDuration);
            burnSoundEffect.volume = volume;
            yield return null;
        }
        burnSoundEffect.volume = 0f;
    }
}
