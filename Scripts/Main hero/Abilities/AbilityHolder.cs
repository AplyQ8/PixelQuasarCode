using System;
using ObjectLogicInterfaces;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    //[SerializeField] private CastRangeLineRenderer castRangeRenderer;
    [SerializeField] private GameObject hero;
    [SerializeField] private Ability ability;
    [SerializeField] private HeroStateHandler stateHandler;
    [SerializeField] private AbilityState _state = AbilityState.Ready;

    [SerializeField] private float coolDownTime;

    private IBloodContent _heroBlood;
    
    public event Action<AbilityHolder.AbilityState> OnAbilityStateChange;
    public event Action<float> OnCooldownTimerStart;
    public event Action<float> OnCooldownTimerTick;
    public event Action OnCooldownDone;

    public event Action  OnCastAbortion, OnAbilityAvailable;
    public event Action<Ability.AbilityType> OnAbilityActivation;

    public enum AbilityState
    {
        Ready,
        Clicked,
        Cooldown,
        NotReady,
        Activated
    }

    private void Awake()
    {
        //castRangeRenderer.Deactivate();
    }
    public void Update()
    {
        if (ability is null)
            return;
        switch (_state)
        {
            // case AbilityState.Ready:
            //     if (_heroBlood.GetCurrentBloodValue() < ability.bloodConsumption)
            //         SwitchState(AbilityState.NotReady);
            //     break;
            case AbilityState.Clicked:
                switch(ability.type)
                {
                    case Ability.AbilityType.ObjectTargeted:
                        if (Input.GetMouseButton(0))
                        {
                            var rayHit =
                                Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
                            if (!rayHit.collider || 
                                !rayHit.collider.gameObject.CompareTag(ability.TargetTag()) ||
                                Vector3.Distance(transform.position, rayHit.collider.transform.position) > ability.usabilityDistance)
                            {
                                SwitchState(AbilityState.Ready);
                                return;
                            }
                            ability.Activate(hero, rayHit.collider.gameObject);
                            OnAbilityActivation?.Invoke(ability.type);
                            SwitchState(AbilityState.Cooldown);
                        }
                        break;
                    case Ability.AbilityType.PositionTargeted:
                        if (Input.GetMouseButton(0))
                        {
                            ability.Activate(hero, Input.mousePosition);
                            OnAbilityActivation?.Invoke(ability.type);
                            SwitchState(AbilityState.Cooldown);
                        }
                        break;
                }
                break;
            case AbilityState.NotReady:
                if (ability.bloodConsumption <= _heroBlood.GetCurrentBloodValue())
                {
                    SwitchState(AbilityState.Ready);
                    OnAbilityAvailable?.Invoke();
                }
                break;
            case AbilityState.Cooldown:
                coolDownTime -= Time.deltaTime;
                OnCooldownTimerTick?.Invoke(coolDownTime);
                if (coolDownTime <= 0)
                {
                    if (ability.bloodConsumption <= _heroBlood.GetCurrentBloodValue())
                    {
                        OnCooldownDone?.Invoke();
                        SwitchState(AbilityState.Ready);
                        return;
                    }
                    
                    SwitchState(AbilityState.NotReady);
                }
                break;
        }
    }
    
    private void FixedUpdate()
    {
        if (ability == null)
            return;
        if (_heroBlood.GetCurrentBloodValue() < ability.bloodConsumption && _state != AbilityState.Cooldown)
            SwitchState(AbilityState.NotReady);
    }

    public AbilityState GetAbilityState() => _state;

    public Ability GetCurrentAbility() => ability;
    public void SetAbility(Ability newAbility) => ability = newAbility;
    public void SetAbilityState(AbilityState newState)
    {
        SwitchState(newState);
    }

    public void ResetState()
    {
        if (_state is AbilityState.NotReady || _state is AbilityState.Cooldown)
            return;
        SwitchState(AbilityState.Ready);
    }

    public void Initialize(Ability ability, GameObject hero, HeroStateHandler heroStateHandler)
    {
        this.ability = ability;
        this.hero = hero;
        stateHandler = heroStateHandler;
        _heroBlood = this.hero.GetComponent<IBloodContent>();
    }

    public void TriggerInput()
    {
        if (ability is null)
            return;
        switch (_state)
        {
            case AbilityState.Ready:
                if (ability.type is Ability.AbilityType.HeroTargeted)
                {
                    ability.Activate(hero);
                    OnAbilityActivation?.Invoke(ability.type);
                    SwitchState(AbilityState.Cooldown);
                    return;
                }

                if (ability.type is Ability.AbilityType.Durable)
                {
                    ability.Activate(hero);
                    OnAbilityActivation?.Invoke(ability.type);
                    SwitchState(AbilityState.Activated);
                    return;
                }
                SwitchState(AbilityState.Clicked);
                break;
            case AbilityState.Clicked:
                HandleAbilityType();
                break;
            case AbilityState.NotReady:
                return;
            case AbilityState.Activated:
                ability.Activate(hero);
                OnCastAbortion?.Invoke();
                SwitchState(AbilityState.Ready);
                return;
        }
    }

    private void HandleAbilityType()
    {
        switch (ability.type)
        {
            case Ability.AbilityType.ObjectTargeted:
                stateHandler.TryExitAbilityState();
                SwitchState(AbilityState.Ready);
                OnCastAbortion?.Invoke();
                break;
            case Ability.AbilityType.PositionTargeted:
                stateHandler.TryExitAbilityState();
                SwitchState(AbilityState.Ready);
                OnCastAbortion?.Invoke();
                break;
        }
    }

    private void SwitchState(AbilityState newState)
    {
        switch (newState)
        {
            case AbilityState.Ready:
                //Do something
                //castRangeRenderer.Deactivate();
                _state = AbilityState.Ready;
                OnAbilityStateChange?.Invoke(_state);
                break;
            case AbilityState.Clicked:
                //Do something
                if (stateHandler.TryEnterAbilityState())
                {
                    //castRangeRenderer.Activate(ability.usabilityDistance, hero.transform);
                    _state = AbilityState.Clicked;
                    OnAbilityStateChange?.Invoke(_state);
                }
                else
                {
                    SwitchState(AbilityState.Ready);
                }
                break;
            case AbilityState.NotReady:
                //Do something
                //castRangeRenderer.Deactivate();
                if (_state is AbilityState.NotReady)
                    return;
                ability.DeactivateAbility();
                stateHandler.TryExitAbilityState();
                _state = AbilityState.NotReady;
                OnAbilityStateChange?.Invoke(_state);
                break;
            case AbilityState.Cooldown:
                //castRangeRenderer.Deactivate();
                stateHandler.TryExitAbilityState();
                _state = AbilityState.Cooldown;
                OnAbilityStateChange?.Invoke(_state);
                coolDownTime = ability.CoolDown;
                OnCooldownTimerStart?.Invoke(coolDownTime);
                break;
            case AbilityState.Activated:
                _state = AbilityState.Activated;
                OnAbilityStateChange?.Invoke(_state);
                break;
        }
    }

    public void SetReadyState()
    {
        _state = AbilityState.Ready;
    }
    public bool CanRemoveAbility()
    {
        if (_state == AbilityState.Ready || _state == AbilityState.NotReady)
            return true;
        return false;
    }
}
