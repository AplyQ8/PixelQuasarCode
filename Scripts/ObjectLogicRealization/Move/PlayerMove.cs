using System;
using UnityEngine;

namespace ObjectLogicRealization.Move
{
    public class PlayerMove : BaseMove
    {
        [SerializeField] private float speedWhileHooking;
        [SerializeField] private float speedWhileSuperAttack;
        [SerializeField] private GameObject pivotPoint;

        private float _baseSpeedWhileHooking;

        protected override void Start()
        {
            base.Start();
            _baseSpeedWhileHooking = speedWhileHooking;
        }
        
        public override Vector3 GetPivotPosition()
        {
            return pivotPoint.transform.position;
        }
        
        private protected override void RecalculateSpeed()
        {
            base.RecalculateSpeed();
            var currentSpeedBuff = CurrentSpeedModifier();
            speedWhileHooking = Math.Max(_baseSpeedWhileHooking * (1 + currentSpeedBuff), 0);
        }

        public float GetHookingMoveSpeed()
        {
            return speedWhileHooking * GameSettings.Instance.speedMultiplier;
        }

        public float GetSpeedWhileSuperAttack()
        {
            return speedWhileSuperAttack * GameSettings.Instance.speedMultiplier;
        }
    }
}