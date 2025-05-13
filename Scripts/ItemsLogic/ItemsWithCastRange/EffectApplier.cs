using System.Collections.Generic;
using UnityEngine;

namespace ItemsLogic.ItemsWithCastRange
{
    public class EffectApplier : MonoBehaviour
    {
        [SerializeField] private List<ModifierData> modifierData = new List<ModifierData>();
        [SerializeField] private CastRange castRange;
        
        public void ApplyEffect(GameObject objectToApply)
        {
            foreach (var modifier in modifierData)
            {
                modifier.statModifier.AffectObject(objectToApply, modifier.value);
            }
        }

        
        public void Initialize(List<ModifierData> modifiersToApply)
        {
            modifierData = modifiersToApply;
        }

        public void StartApplyingEffect()
        {
            enabled = true;
            castRange.OnEnter += ApplyEffect;
        }
        
        public void StopApplyingEffect()
        {
            this.enabled = false;
            castRange.OnEnter -= ApplyEffect;
        }
    }
}
