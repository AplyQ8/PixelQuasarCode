using System;
using System.Collections.Generic;
using System.Linq;
using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.Resistance
{
    public class BaseResistance : MonoBehaviour, IResistible
    {
        [SerializeField] private List<ResistHolder> resistHolders = new List<ResistHolder>();
        public float GetResistance(DamageTypeManager.DamageType damageType)
        {
            if (damageType is DamageTypeManager.DamageType.Default)
            {
                return GetResistanceHolder(DamageTypeManager.DamageType.Default).Resistance;
            }

            var overallBuff = 1 -
                              (1 - GetResistanceHolder(DamageTypeManager.DamageType.Default).Resistance) *
                              (1 - GetResistanceHolder(damageType).Resistance);
            
            return overallBuff;
        }

        public void ApplyResistance(float value, DamageTypeManager.DamageType damageType)
        {
            GetResistanceHolder(damageType).RecalculateResistance(value);
        }
        
        private ResistHolder GetResistanceHolder(DamageTypeManager.DamageType damageType)
        {
            return resistHolders.FirstOrDefault(resistHolder => resistHolder.damageType.Equals(damageType));
        }
    }

    [Serializable]
    public class ResistHolder
    {
        [field: SerializeField] public DamageTypeManager.DamageType damageType;
        [field: Range(0, 1)] [field: SerializeField] public float Resistance { get; private set; }
        [SerializeField] private List<float> resistanceBuffs = new List<float>();
        public List<float> GetResistanceBuffs() => resistanceBuffs;

        public void RecalculateResistance(float buffValue)
        {
            if (buffValue is 0)
                return;
            if (!HasSimilarBuff(buffValue))
            {
                resistanceBuffs.Add(buffValue);
            }
            var overallBuff = resistanceBuffs.Aggregate(1f, (current, buff) => current * (1 - buff));
            Resistance = 1 - overallBuff;
        }

        private bool HasSimilarBuff(float buff)
        {
            if (!resistanceBuffs.Contains(-buff))
                return false;
            
            resistanceBuffs.Remove(-buff);
            return true;
        }
        
    }
}