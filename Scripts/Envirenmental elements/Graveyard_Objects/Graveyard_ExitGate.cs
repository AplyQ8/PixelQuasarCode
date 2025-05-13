using System;
using System.Collections;
using System.Collections.Generic;
using Envirenmental_elements;
using ObjectLogicInterfaces;
using UnityEngine;

public class Graveyard_ExitGate : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource soundEffect;
    private static readonly int Open = Animator.StringToHash("open");


    public void Interact()
    {
        animator.SetTrigger(Open);
        soundEffect.Play();
    }
}
