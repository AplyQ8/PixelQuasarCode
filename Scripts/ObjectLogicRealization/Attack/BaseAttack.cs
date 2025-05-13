using Main_hero.HeroStats;
using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.BaseRealizations
{
    
    public class BaseAttack : MonoBehaviour, ICanAttack
    {
        [field: SerializeField] public float MinAttackValue { get; private set; }
        
        [field: SerializeField] public float MaxAttackValue { get; private set; }
        

        private void Awake()
        {
        }
        public float GetCurrentAttack()
        {
            return MaxAttackValue;
        }

        public float GetMaxAttackBoundary() => MaxAttackValue;
    }
}