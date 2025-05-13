using System;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

namespace ObjectLogicRealization.Move
{
    public class BaseMove : MonoBehaviour, IMovable
    {
        [field: SerializeField] public float MoveSpeed { get; protected set; }
        [field: SerializeField] public float MinMoveSpeed { get; protected set; }

        private protected List<float> _speedEffects = new List<float>(); // Хранит и бафы, и дебафы

        private float _baseMoveSpeed;
        private float _baseMinMoveSpeed;

        private Transform bodyBottom;

        protected virtual void Start()
        {
            bodyBottom = transform.Find("ObstacleCollider");
            _baseMoveSpeed = MoveSpeed;
            _baseMinMoveSpeed = MinMoveSpeed;
        }

        public void SetSpeed(float value)
        {
            MoveSpeed = value;
        }

        public void SpeedUp(float value)
        {
            MoveSpeed += value;
        }

        public void SlowDown(float value)
        {
            MoveSpeed -= value;
            if (MoveSpeed < MinMoveSpeed)
                MoveSpeed = MinMoveSpeed;
        }

        public virtual void SpeedUpByPercent(float value)
        {
            if (value <= 0) return;
            _speedEffects.Add(value); // Добавляем положительный множитель (баф)
            RecalculateSpeed();
        }

        public virtual void SlowDownByPercent(float value)
        {
            if (value <= 0) return;
            _speedEffects.Add(-value); // Добавляем отрицательный множитель (дебаф)
            RecalculateSpeed();
        }

        public virtual void RemoveSpeedEffect(float value)
        {
            _speedEffects.Remove(value); // Удаляет как бафы, так и дебафы
            _speedEffects.Remove(-value); // Удаляет дебаф, если передан положительный множитель
            RecalculateSpeed();
        }

        /// <summary>
        /// Возвращает итоговый множитель скорости с учетом всех бафов и дебафов.
        /// </summary>
        private protected float CurrentSpeedModifier()
        {
            float totalEffect = 0;
            foreach (var speedEffect in _speedEffects)
            {
                totalEffect += speedEffect;
            }
            return 1 + totalEffect; // Итоговый множитель скорости
        }

        private protected virtual void RecalculateSpeed()
        {
            var currentModifier = CurrentSpeedModifier();
            MoveSpeed = Mathf.Max(_baseMoveSpeed * currentModifier, MinMoveSpeed); // Скорость >= MinMoveSpeed
            MinMoveSpeed = Mathf.Max(_baseMinMoveSpeed * currentModifier, 0);
        }
        
        public float GetCurrentMoveSpeed() => MoveSpeed * GameSettings.Instance.speedMultiplier;
        
        public virtual Vector3 GetPivotPosition()
        {
            return bodyBottom.position;
        }
    }
}