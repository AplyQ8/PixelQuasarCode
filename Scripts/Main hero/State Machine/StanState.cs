using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class StanState : BaseState
{
    private float _duration;
    private static readonly int Stanned = Animator.StringToHash("Stanned");
    private Rigidbody2D _heroRigidBody;
    private Timer _stunTimer;

    public StanState(float duration, GameObject hero, Animator animator, HeroStateHandler stateHandler)
    {
        StateHandler = stateHandler;
        Animator = animator;
        _duration = duration;
        _heroRigidBody = hero.GetComponent<Rigidbody2D>();
        _stunTimer = new Timer(duration);
        _stunTimer.OnTimerDone += EndOfStun;
    }
    
    public override void EnterState()
    {
        base.EnterState();
        _heroRigidBody.velocity = Vector2.zero;
        _stunTimer.StartTimer();
        //Start animation;
        Animator.SetBool(Stanned, true);
    }

    public override void UpdateState(HeroStateHandler stateHandler)
    {
        // _duration -= Time.deltaTime;
        // if (!(_duration <= 0)) return;
        // stateHandler.SwitchState(stateHandler.NormalState);
        _stunTimer.Tick();
    }

    private void EndOfStun()
    {
        _stunTimer.OnTimerDone -= EndOfStun;
        StateHandler.SwitchState(StateHandler.NormalState);
    }

    public override void ExitState()
    {
        base.ExitState();
        Animator.SetBool(Stanned, false);
        Animator.StopPlayback();
    }
}
