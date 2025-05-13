using System;
using System.Collections.Generic;
using ObjectLogicRealization.Adrenaline;
using Status_Effect_System;
using UnityEngine;

namespace Main_hero
{
    [RequireComponent(typeof(EffectHandler))]
    public class InstabilityEffectApplier : MonoBehaviour
    {
        [SerializeField] private HeroAdrenaline heroAdrenaline;
        [SerializeField] private List<StatusEffectData> instabilityBuffs = new List<StatusEffectData>();

        private EffectHandler _effectHandler;

        private void Start()
        {
            _effectHandler = GetComponent<EffectHandler>();
            heroAdrenaline.OnInstabilityEnter += ApplyEffects;
            heroAdrenaline.OnInstabilityExit += RemoveEffects;
        }

        private void ApplyEffects()
        {
            foreach (var instabilityBuff in instabilityBuffs)
            {
                _effectHandler.ApplyEffect(instabilityBuff);
            }
        }

        private void RemoveEffects()
        {
            foreach (var instabilityBuff in instabilityBuffs)
            {
                _effectHandler.RemoveEffect(instabilityBuff);
            }
        }

        private void OnDestroy()
        {
            heroAdrenaline.OnInstabilityEnter -= ApplyEffects;
            heroAdrenaline.OnInstabilityExit -= RemoveEffects;
        }
    }
}