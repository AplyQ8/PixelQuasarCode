using System;
using UnityEngine;

namespace Utilities
{
    public class Timer
    {
        public event Action OnTimerDone;
        public event Action<float> OnTimeRemaining;
        public float StartTime { get; private set; }
        public float Duration { get; private set; }
        public float TargetTime { get; private set; }

        public bool IsActive { get; private set; }

        public Timer(float duration)
        {
            Duration = duration;
        }

        public void StartTimer()
        {
            StartTime = Time.time;
            TargetTime = StartTime + Duration;
            IsActive = true;
        }
        
        /// <summary>
        /// Запускает таймер на duration секунд
        /// </summary>
        /// <param name="duration">Длительность таймера</param>
        public void StartTimer(float duration)
        {
            Duration = duration;
            StartTimer();
        }

        public void SetLeftTime(float duration)
        {
            StartTime = Time.time;
            TargetTime = StartTime + duration;
        }
        
        public void StopTimer()
        {
            IsActive = false;
        }

        public void Tick()
        {
            if (!IsActive)
                return;

            float timeRemaining = TargetTime - Time.time;

            // Вызываем событие с оставшимся временем
            OnTimeRemaining?.Invoke(Mathf.Max(timeRemaining, 0));

            // Проверяем, завершился ли таймер
            if (timeRemaining <= 0)
            {
                OnTimerDone?.Invoke();
                StopTimer();
            }
        }

        public void ResetDuration(float newDuration)
        {
            StartTime = Time.time;
            TargetTime = StartTime + newDuration;
        }
    }
}