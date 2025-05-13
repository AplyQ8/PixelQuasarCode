using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using ObjectLogicInterfaces;
using Status_Effect_System;
using UnityEngine;

public class EffectHandler : MonoBehaviour, IEffectable
{
    [SerializedDictionary("Effect ID", "Effect Instance")] [SerializeField]
    private SerializedDictionary<int, EffectInstance> activeDurableEffects;
    
    [SerializedDictionary("Effect ID", "Effect Instance")] [SerializeField]
    private SerializedDictionary<int, EffectInstance> activePermanentEffects;
    
    [SerializedDictionary("Effect ID", "Effect Instance")] [SerializeField]
    private SerializedDictionary<int, EffectInstance> effectsToRemove;
    
    [SerializedDictionary("VFX ID", "VFX Instance")] [SerializeField]
    private SerializedDictionary<int, ParticleSystem> activeVFX;
    
    public event Action<int, Sprite, float, StatusEffectData.EffectType> AddEffectEvent;
    public event Action<int> RemoveEffectEvent;
    public event Action<int, float> RestartEffectEvent;
    

    void Awake()
    {
        activeDurableEffects = new SerializedDictionary<int, EffectInstance>();
        effectsToRemove = new SerializedDictionary<int, EffectInstance>();
        activeVFX = new SerializedDictionary<int, ParticleSystem>();
        
        if (TryGetComponent(out IDamageable damageable))
        {
            damageable.OnDeathEvent += RemoveAllCurrentEffect;
        }
    }

    private void OnEnable()
    {
        if (activeDurableEffects.Count == 0)
            return;
        RemoveAllCurrentEffect();
    }

    private void Update()
    {
        foreach (var effectInstance in activeDurableEffects)
        {
            
            var isDone = effectInstance.Value.HandleEffect(gameObject);
            
            if(effectInstance.Value.IsExpired || isDone)
                RemoveEffect(effectInstance.Value.Effect);
        }

        foreach (var effectInstance in effectsToRemove)
        {
            activeDurableEffects.Remove(effectInstance.Key);
        }
        effectsToRemove.Clear();
    }

    public void ApplyEffect(StatusEffectData effect)
    {
        switch (effect.DurabilityType)
        {
            case StatusEffectData.EffectDurability.Durable:
                break;
            case StatusEffectData.EffectDurability.Permanent:
                break;
            case StatusEffectData.EffectDurability.Stackable:
                if (activeDurableEffects.TryGetValue(effect.ID, out var existingEffect))
                {
                    existingEffect.IncreaseStack(gameObject);
                    RestartEffectEvent?.Invoke(effect.ID, existingEffect.Lifetime);
                    return;
                }
                break;
        }

        if (EffectAlreadyInDictionary(effect.ID)) 
            return;

        activeDurableEffects.Add(effect.ID, new EffectInstance(effect));
        activeDurableEffects[effect.ID].StartEffect(gameObject);
        AddEffectEvent?.Invoke(effect.ID, effect.icon, effect.currentLifeTime, effect.effectType);
        TryCreateVFX(effect.ID, activeDurableEffects[effect.ID].Particles);
    }
    public void ApplyEffect(StatusEffectData effect, int value)
    {
        ApplyEffect(effect, (float)value);
    }
    public void ApplyEffect(StatusEffectData effect, float value)
    {
        if (EffectAlreadyInDictionary(effect.ID)) 
            return;
        activeDurableEffects.Add(effect.ID,  new EffectInstance(effect));
        activeDurableEffects[effect.ID].StartEffect(gameObject, value);
        AddEffectEvent?.Invoke(effect.ID, effect.icon, effect.currentLifeTime, effect.effectType);
        TryCreateVFX(effect.ID,  activeDurableEffects[effect.ID].Particles);
    }
    

    private void TryCreateVFX(int vfxID, ParticleSystem vfx)
    {
        if (null == vfx)
            return;
        ParticleSystem particleEffect = Instantiate(vfx, gameObject.transform);
        activeVFX.Add(vfxID, particleEffect);
        particleEffect.transform.position = gameObject.transform.position;
        particleEffect.Play();

    }

    private void TryRemoveVFX(int vfxID)
    {
        if (!activeVFX.ContainsKey(vfxID))
            return;
        activeVFX[vfxID].Stop();
        activeVFX[vfxID].Clear();
        activeVFX.Remove(vfxID);

    }
    
    public void RemoveEffect(StatusEffectData effect)
    {
        if (!activeDurableEffects.ContainsKey(effect.ID))
            return;
        activeDurableEffects[effect.ID].EndEffect(gameObject);
        //if (effectsToRemove.ContainsKey(effect.ID)) return;
        effectsToRemove.Add(effect.ID, activeDurableEffects[effect.ID]);
        TryRemoveVFX(effect.ID);
        RemoveEffectEvent?.Invoke(effect.ID);
    }

    private void RemoveAllCurrentEffect()
    {
        foreach (var effect in activeDurableEffects)
        {
            RemoveEffect(effect.Value.Effect);
        }
    }

    private bool EffectAlreadyInDictionary(int id)
    {
        if (!activeDurableEffects.ContainsKey(id)) return false;
        
        activeDurableEffects[id].RestartEffect(gameObject);
        RestartEffectEvent?.Invoke(
            id, activeDurableEffects[id].Lifetime);
        return true;

    }

    private void OnDisable()
    {
        if (TryGetComponent(out IDamageable damageable))
        {
            damageable.OnDeathEvent -= RemoveAllCurrentEffect;
        }
    }
}

[Serializable]
public class EffectInstance
{
    [field: SerializeField] public StatusEffectData Effect { get; private set; }
    [field: SerializeField] public float Lifetime { get; private set; }
    [field: SerializeField] public ParticleSystem Particles { get; private set; }
    [field: SerializeField] public int Stacks { get; private set; } = 1;

    public EffectInstance(StatusEffectData newEffect)
    {
        Effect = newEffect;
        Lifetime = Effect.LifeTime;
        Particles = newEffect.particles;
    }

    #region Start Effect Methods
    
    public void StartEffect(GameObject objectToApplyEffect)
    {
        Effect.StartEffect(objectToApplyEffect);
        Lifetime = Effect.LifeTime;
    }
    public void StartEffect(GameObject objectToApplyEffect, float value)
    {
        Effect.SetValues(value);
        Effect.StartEffect(objectToApplyEffect);
        Lifetime = Effect.LifeTime;
    }

    public void StartEffect(GameObject objectToApplyEffect, int value)
    {
        StartEffect(objectToApplyEffect, (float)value);
    }
    
    #endregion
    
    public void IncreaseStack(GameObject objectToApplyEffect)
    {
        if (Effect is StackableStatusEffect stackableEffect)
        {
            if (Stacks < stackableEffect.MaxStacks)
            {
                Stacks++;
                //Lifetime = stackableEffect.StackDuration * Stacks;
                Lifetime = stackableEffect.LifeTime;
                stackableEffect.IncreaseStack(objectToApplyEffect, Stacks);
            }
            
        }
    }
    
    public void ReduceStack(GameObject objectToApplyEffect)
    {
        if (Effect is StackableStatusEffect stackableEffect)
        {
            if (Stacks > 0)
            {
                stackableEffect.ReduceStack(objectToApplyEffect, Stacks);
                Stacks--;
                //Lifetime = stackableEffect.StackDuration * Stacks;
                Lifetime = stackableEffect.LifeTime;

                if (Stacks == 0)
                {
                    EndEffect(objectToApplyEffect);
                }
            }
        }
    }

    
    public void RestartEffect(GameObject objectToApplyEffect)
    {
        if (Effect is StackableStatusEffect)
        {
            IncreaseStack(objectToApplyEffect);
        }
        else
        {
            Effect.RestartEffect(objectToApplyEffect);
            Lifetime = Effect.LifeTime;
        }
    }
    
    public bool HandleEffect(GameObject objectToApplyEffect)
    {
        var isDone = Effect.HandleEffect(objectToApplyEffect);
        if (isDone) return true;
        
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0)
        {
            ReduceStack(objectToApplyEffect);
        }
        return false;
    }

    public void EndEffect(GameObject objectToApplyEffect)
    {
        Effect.EndEffect(objectToApplyEffect);
    }

    public bool IsExpired
        => EffectIsExpired();

    private bool EffectIsExpired()
    {
        if (Effect.DurabilityType is StatusEffectData.EffectDurability.Permanent)
            return false;
        return Lifetime <= 0;
    }
}
