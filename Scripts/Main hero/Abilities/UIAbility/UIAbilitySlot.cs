using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAbilitySlot : MonoBehaviour
{
    [SerializeField] private TMP_Text activationButtonText;
    [SerializeField] private AbilityHolder abilityHolder;
    public event Action<AbilityHolder.AbilityState, UIAbilitySlot> OnAbilityStateChange;
    //public event Action<UIAbilitySlot> OnAbilitySlotActivate;

    [Header("Cooldown Indicators")] [SerializeField]
    private Image cooldownMask;
    //[SerializeField] private TMP_Text cooldownTime;
    private float _totalCooldownTime;

    [SerializeField] private Animator slotAnimator;
    

    #region Animation Triggers

    private static readonly int Unlock = Animator.StringToHash("Unlock");
    private static readonly int Lock = Animator.StringToHash("Lock");

    #endregion

    public enum AbilityAnimationState
    {
        Locked,
        Unlocking,
        Locking,
        Available,
        Casting,
        Activation,
        CastAwaiting,
        ToCooldown,
        FromCooldown,
        InCooldown,
        Forbidden,
        DurableCast
    }

    [SerializeField] private AbilityAnimationState currentAnimationState;
    
    
    public void Initialize(Ability ability, GameObject hero, HeroStateHandler heroStateHandler)
    {
        abilityHolder.Initialize(ability, hero, heroStateHandler);
        abilityHolder.OnAbilityStateChange += AbilityStateChangeEvent;
        abilityHolder.OnCooldownTimerStart += StartCooldownEvent;
        abilityHolder.OnCooldownTimerTick += TickCooldownEvent;
        abilityHolder.OnCooldownDone += CooldownDoneEvent;
        abilityHolder.OnAbilityActivation += AbilityActivationEvent;
        abilityHolder.OnCastAbortion += CastAbortionEvent;
        abilityHolder.OnAbilityAvailable += AbilityAvailableEvent;
        
        cooldownMask.fillAmount = 0f;
        //cooldownTime.gameObject.SetActive(false);
        currentAnimationState = AbilityAnimationState.Locked;
        if (ability != null)
        {
            slotAnimator.SetTrigger(Unlock);
            currentAnimationState = AbilityAnimationState.Available;
        }
        else
        {
            slotAnimator.SetTrigger(Lock);
            abilityHolder.SetReadyState();
            currentAnimationState = AbilityAnimationState.Locked;
        }
        
    }

    private void AbilityAvailableEvent()
    {
        slotAnimator.SetTrigger("AbilityAvailable");
    }

    private void CastAbortionEvent()
    {
        slotAnimator.SetTrigger("AbortCast");
    }

    private void AbilityActivationEvent(Ability.AbilityType abilityType)
    {
        if (abilityType is Ability.AbilityType.Durable)
        {
            if (currentAnimationState is AbilityAnimationState.DurableCast)
                return;
            slotAnimator.SetTrigger("DurableCast");
            return;
        }
        slotAnimator.SetTrigger("Cast");
    }

    public void InitializeNull(KeyCode activationButton)
    {
        activationButtonText.text = activationButton.ToString();
    }

    public void TriggerInput()
    {
        if (currentAnimationState == AbilityAnimationState.Activation)
            return;
        abilityHolder.TriggerInput();
    }

    public void ResetState()
    {
        slotAnimator.SetTrigger("AbortCast");
        abilityHolder.ResetState();
    }

    private void AbilityStateChangeEvent(AbilityHolder.AbilityState currentAbilityState)
    {
        OnAbilityStateChange?.Invoke(currentAbilityState, this);
        switch (currentAbilityState)
        {
            case AbilityHolder.AbilityState.Ready:
                
                break;
            case AbilityHolder.AbilityState.Clicked:
                slotAnimator.SetTrigger("Clicked");
                slotAnimator.SetTrigger("AwaitCast");
                break;
            case AbilityHolder.AbilityState.NotReady:
                slotAnimator.SetTrigger("BlockAbility");
                break;
            case AbilityHolder.AbilityState.Cooldown:
                break;
        }
    }

    public void ChangeAnimationState(AbilityAnimationState animationState) => currentAnimationState = animationState;
    
    #region Cooldown Methods
    private void StartCooldownEvent(float time)
    {
        
        cooldownMask.fillAmount = 1f;
        //cooldownTime.text = time.ToString(CultureInfo.InvariantCulture);
        _totalCooldownTime = time;
        //cooldownTime.gameObject.SetActive(true);
    }

    private void TickCooldownEvent(float currentTime)
    {
        //cooldownTime.text = Mathf.RoundToInt(currentTime).ToString();
        cooldownMask.fillAmount = currentTime / _totalCooldownTime;
    }
    
    private void CooldownDoneEvent()
    {
        cooldownMask.fillAmount = 0f;
        //cooldownTime.gameObject.SetActive(false);
        slotAnimator.SetTrigger("AbilityAvailable");
    }
    #endregion

    public bool CanRemoveAbility()
    {
        return abilityHolder.CanRemoveAbility();
    }
}

