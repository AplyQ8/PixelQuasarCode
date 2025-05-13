using System;
using System.Collections.Generic;
using Main_hero.HeroStats;
using ObjectLogicInterfaces;
using ObjectLogicRealization.Adrenaline;
using UnityEngine;

namespace ObjectLogicRealization.Attack
{
    [RequireComponent(typeof(HeroAdrenaline))]
    public class HeroAttack : MonoBehaviour, ICanAttack
    {
        [field: SerializeField] public float MinAttackValue { get; private set; }
        [field: SerializeField] public float MaxAttackValue { get; private set; }
        private float _baseMinAttackValue;
        private float _baseMaxAttackValue;
        
        public bool ReversedAdrenalineLogic { get; set; } = false;
        public event Action OnAttackHit;
        
        
        private HeroAdrenaline _adrenaline;
        private List<float> attackBuffs = new List<float>();

        void Awake()
        {
            _adrenaline = GetComponent<HeroAdrenaline>();
            _baseMaxAttackValue = MaxAttackValue;
            _baseMinAttackValue = MinAttackValue;
        }
    
        public float GetCurrentAttack()
        {
            // return Mathf.Max(MinAttackValue, MaxAttackValue * _adrenaline.GetAdrenalineRatio());
            if(!ReversedAdrenalineLogic)
                return MinAttackValue + (MaxAttackValue - MinAttackValue) * _adrenaline.GetAdrenalineRatio();
            return MaxAttackValue;
        }
        public float GetMaxAttackBoundary() => MaxAttackValue;

        public void EmpowerAttackByPercent(float value)
        {
            if (value < 0) return;
            attackBuffs.Add(value);
            RecalculateAttack();
        }

        public void WeakenAttackByPercent(float value)
        {
            if (value < 0) return;
            attackBuffs.Remove(value);
            RecalculateAttack();
        }

        private float CurrentAttackBuff()
        {
            float totalBuff = 0;
            foreach (var attackBuff in attackBuffs)
            {
                totalBuff += attackBuff;
            }

            return totalBuff;
        }
        private void RecalculateAttack()
        {
            float attackBuff = CurrentAttackBuff();
            MinAttackValue = Mathf.Max(_baseMinAttackValue * (1 + attackBuff), 0);
            MaxAttackValue = Mathf.Max(_baseMaxAttackValue * (1 + attackBuff), 0);

        }
    }
}
