using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private HeroStateHandler stateHandler;
    private HeroSounds heroSounds;

    private void Start()
    {
        heroSounds = transform.parent.GetComponentInChildren<HeroSounds>();
    }

    public void AttackAnimationEnd()
    {
        stateHandler.AttackingState.AttackAnimationEnd();
    }

    public void MakeAnAttack()
    {
        stateHandler.AttackingState.ProceedAttack();
    }

    public void RegisterAnAttack()
    {
        stateHandler.AttackingState.RegisterAttackClick();
    }

    public void StopAttackMovement()
    {
        stateHandler.AttackingState.StopAttackMovement();
    }

    public void PlayStepSound()
    {
        if(stateHandler.GetCurrentState.GetType() != typeof(HookingState))
            heroSounds.PlayStepsSound();
    }
}
