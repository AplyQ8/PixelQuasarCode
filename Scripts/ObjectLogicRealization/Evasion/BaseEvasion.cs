using System.Collections.Generic;
using System.Linq;
using ObjectLogicInterfaces;
using UnityEngine;
using Utilities;

namespace ObjectLogicRealization.Evasion
{
    public class BaseEvasion : MonoBehaviour, IEvasionable
    {
        [SerializeField] private List<float> buffs = new List<float>();
        [Range(0, 1)][SerializeField] private float evasionChance;

        public bool AvoidedDamage()
        {
            return !(evasionChance < 0) && RandomGenerator.Instance.IsInRange(evasionChance);
        }

        public void RecalculateEvasionChance(float value)
        {
            if (value is 0)
                return;
            if (!HasSimilarBuff(value))
            {
                buffs.Add(value);
            }
            var overallBuff = buffs.Aggregate(1f, (current, buff) => current * (1 - buff));
            evasionChance = 1 - overallBuff;
        }
        private bool HasSimilarBuff(float buff)
        {
            if (!buffs.Contains(-buff))
                return false;
            
            buffs.Remove(-buff);
            return true;
        } 
    }
}