using System.Collections;
using System.Collections.Generic;
using Status_Effect_System;
using UnityEngine;

public interface IEffectable
{
    public void ApplyEffect(StatusEffectData effect);
    public void ApplyEffect(StatusEffectData effect, int value);
    public void ApplyEffect(StatusEffectData effect, float value);
    public void RemoveEffect(StatusEffectData effect);

}
