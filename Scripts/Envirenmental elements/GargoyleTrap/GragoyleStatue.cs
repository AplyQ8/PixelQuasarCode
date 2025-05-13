using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GragoyleStatue : MonoBehaviour
{
    [SerializeField] private Animator animationController;
    [SerializeField] private GargoyleStates currentState;
    [SerializeField] private List<Transform> spottedTargets;
    [SerializeField] private float attackCooldown;
    [SerializeField] private ProjectilePooler projectilePooler;
    [SerializeField] private float projectileSpeed;
    private Timer _attackCooldownTimer;
    [SerializeField] private Transform _nearestTarget;

    [SerializeField] private List<ModifierData> modifiers;

    #region Animation triggers

    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int TargetIsSpotted = Animator.StringToHash("TargetIsSpotted");

    #endregion
    
    public enum GargoyleStates
    {
        Idle,
        Activate,
        Attack
    }

    private void Awake()
    {
        currentState = GargoyleStates.Idle;
        spottedTargets = new List<Transform>();
        _attackCooldownTimer = new Timer(attackCooldown);
        _attackCooldownTimer.OnTimerDone += AttackEvent;
    }

    private void Update()
    {
        _attackCooldownTimer.Tick();
    }

    public void TargetSpotted(List<Transform> updatedListOfTargets)
    {
        spottedTargets = updatedListOfTargets;
        animationController.SetBool(TargetIsSpotted, true);
    }
    public void NoTarget()
    {
        spottedTargets.Clear();
        animationController.SetBool(TargetIsSpotted, false);
    }

    private void AttackEvent()
    {
        currentState = GargoyleStates.Attack;
        _nearestTarget = GetNearestTarget();
        if (_nearestTarget is null)
        {
            animationController.SetBool(TargetIsSpotted, false);
            return;
        }
        animationController.SetTrigger(Attack);
        
    }

    public void ThrowProjectile()
    {
        projectilePooler.InitializeProjectile(_nearestTarget, projectileSpeed, modifiers);
        _nearestTarget =  null;
    }

    public void EnterState(GargoyleStates state)
    {
        switch (state)
        {
            case GargoyleStates.Idle:
                currentState = GargoyleStates.Idle;
                _attackCooldownTimer.StopTimer();
                break;
            case GargoyleStates.Attack:
                currentState = GargoyleStates.Attack;
                _attackCooldownTimer.StopTimer();
                break;
            case GargoyleStates.Activate:
                currentState = GargoyleStates.Activate;
                _attackCooldownTimer.StartTimer();
                break;
        }
    }

    private Transform GetNearestTarget()
    {
        Transform nearestObject = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform objTransform in spottedTargets)
        {
            float distance = Vector3.Distance(objTransform.position, transform.position);
            if (!(distance < minDistance)) continue;
            minDistance = distance;
            nearestObject = objTransform;
        }
        return nearestObject;
    }
}
