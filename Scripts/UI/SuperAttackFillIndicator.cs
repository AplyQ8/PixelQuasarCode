using System;
using Main_hero.State_Machine;
using Main_hero.SuperAttackScripts;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SuperAttackFillIndicator : MonoBehaviour
    {
        [SerializeField] private HealthBar gradientManager;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 1.5f;
        [SerializeField] private ParticleSystem particles;
        private bool _isFading;

        [Header("Particle Emission")] 
        [SerializeField] private float minParticleEmission;
        [SerializeField] private float maxParticleEmission;
        
        private Action<float> onHoldingHandler;
        private Action onSuperAttackChange;
        private SuperAttackState superAttackState;
        private ParticleSystem.EmissionModule _emissionModule;
        
        public void Initialize(SuperAttackState state)
        {
            superAttackState = state; 
            onHoldingHandler = UpdateSlider; 
            superAttackState.OnHoldTimeChange += onHoldingHandler;

            onSuperAttackChange = ResetSlider;
            superAttackState.OnSuperAttackChange += onSuperAttackChange;
            superAttackState.OnAttackExecuted += ResetSlider;
            
            // Получение ссылки на EmissionModule.
            if (particles != null)
            {
                _emissionModule = particles.emission;
            }

            ResetSlider();
        }
        
        private void UpdateSlider(float holdTime)
        {
            if (holdTime > 0)
            {
                StopFadeEffect();
                //slider.value = holdTime; 
                gradientManager.SetHealth(holdTime);
                UpdateParticleEmission(holdTime);
            }
            else
            {
                StartFadeEffect();
            }
        }
        private void UpdateParticleEmission(float holdTime)
        {
            if (particles is null) return;

            // Нормализуем holdTime в диапазоне [0, 1].
            float normalizedHoldTime = holdTime / superAttackState.CurrentSuperAttack.MaxHoldTime;

            // Устанавливаем новое значение Emission rateOverTime.
            _emissionModule.rateOverTime = Mathf.Lerp(minParticleEmission, maxParticleEmission, normalizedHoldTime);
        }
        private void ResetSlider()
        {
            _isFading = true;
            //slider.value = 0;
            gradientManager.SetHealth(0);
            canvasGroup.alpha = 0; // Скрываем изначально.
            //slider.maxValue = superAttackState.CurrentSuperAttack.MaxHoldTime;
            gradientManager.SetMaxHealth(superAttackState.CurrentSuperAttack.MaxHoldTime);
            UpdateParticleEmission(0);
        }

        private void StartFadeEffect()
        {
            if (_isFading) return;
            _isFading = true;
            StartCoroutine(FadeOut());
        }

        private void StopFadeEffect()
        {
            if (!_isFading) return;
            _isFading = false;
            StopAllCoroutines();
            canvasGroup.alpha = 1;
        }

        private System.Collections.IEnumerator FadeOut()
        {
            float elapsedTime = 0f;
            float startAlpha = canvasGroup.alpha;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        private void OnDestroy()
        {
            // Отписка от события.
            if (onHoldingHandler != null)
            {
                superAttackState.OnHoldTimeChange -= onHoldingHandler;
            }

            if (onSuperAttackChange != null)
            {
                superAttackState.OnSuperAttackChange -= ResetSlider;
            }
            superAttackState.OnAttackExecuted -= ResetSlider;
        }
    }
}
