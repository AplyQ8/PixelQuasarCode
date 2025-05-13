using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;

public class UI_PA_SlotIndicator : MonoBehaviour
{
    [SerializeField] private UIParticle activationEffect;
    [field: SerializeField] public bool IsActivated { get; private set; }

    private void Awake()
    {
        Deactivate();
    }
    public void Activate()
    {
        activationEffect.Play();
        IsActivated = true;
    }

    public void Deactivate()
    {
        IsActivated = false;
        activationEffect.Stop();
        activationEffect.Clear();
    }
}
