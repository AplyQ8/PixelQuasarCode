using System;
using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.Move
{
    public class EnemyMove : BaseMove
    {
        [field: SerializeField] public float PatrolSpeed { get; private set; }
        [field: SerializeField] public float BaseSpeed { get; private set; }
        
        public void SetPatrolSpeed()
        {
            var speedModifier = CurrentSpeedModifier();
            if (speedModifier < 1) return;
            MoveSpeed = PatrolSpeed;
        }
        
        public void SetBaseSpeed()
        {
            var speedModifier = CurrentSpeedModifier();
            if (speedModifier < 1) return;
            MoveSpeed = BaseSpeed;
        }

        public void SpeedUpBaseSpeed(float value)
        {
            BaseSpeed += value;
        }
        
        public void SlowDownBaseSpeed(float value)
        {
            BaseSpeed -= value;
        }
    }
}