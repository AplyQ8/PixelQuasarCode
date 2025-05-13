using System;
using System.Collections;
using System.Collections.Generic;
using Status_Effect_System;
using UnityEngine;

[Serializable]
public class ModifierData
{
    public StatModifierSO statModifier;
    public float value;
}

[Serializable]
public class EffectModifierData
{
    public StatusEffectData effectData;
    public float lifeTime;
    public ParticleSystem effectParticles;
}
