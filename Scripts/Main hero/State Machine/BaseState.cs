using System;
using System.Collections;
using System.Collections.Generic;
using In_Game_Menu_Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

//Абстрактный класс состояния пуджа
[Serializable]
public class BaseState
{
    protected GameObject Hero;
    [SerializeField] protected HeroStateHandler StateHandler;
    protected Transform HeroPosition;
    protected Animator Animator;
    protected bool isPaused = false;
    private protected DropBag DropBag;
    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] protected InputAction pause;
    //Метод, отвечающий за триггер входа в состояние
    
    
    public virtual void EnterState()
    {
        //pause = playerInput.currentActionMap.FindAction("Pause");
        //pause.performed += HandlePause;

    }
    //Метод, обрабатывающий какую то информацию, пока объект находится в текущем состоянии
    public virtual void UpdateState(HeroStateHandler stateHandler)
    { }

    public virtual void ExitState()
    {
        UnsubscribeFromPause();
    }
    public virtual void InitializeState(GameObject hero, HeroStateHandler stateHandler, Transform heroTransform,
        Animator animator, PlayerInput playerInput)
    {
        this.playerInput = playerInput;
        Hero = hero;
        Animator = animator;
        HeroPosition = heroTransform;
        StateHandler = stateHandler;
    }
    
    public virtual void SubscribeOnActionEvents()
    {
        
    }

    public virtual void UnsubscribeFromActionEvents()
    {
        
    }

    public void ProceedPause(bool isPaused) => this.isPaused = isPaused;
    
    private void HandlePause(InputAction.CallbackContext context)
    {
        if (PauseController.Instance.IsPaused)
        {
            isPaused = true;
            UnsubscribeFromActionEvents();
            return;
        }

        isPaused = false;
        SubscribeOnActionEvents();
    }

    private void UnsubscribeFromPause()
    {
        if(pause != null)
            pause.performed -= HandlePause;
    }
}

