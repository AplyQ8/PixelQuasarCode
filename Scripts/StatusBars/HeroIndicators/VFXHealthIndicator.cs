using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace StatusBars.HeroIndicators
{
    public class VFXHealthIndicator : MonoBehaviour
    {
        [SerializeField] private Slider observingSlider;
        [SerializeField] private Slider nativeSlider;
        [Range(-1, 1)] [SerializeField] private float offset;
        [SerializeField] private float smoothSpeed = 0.5f;

        private IEnumerator _smoothChangeCoroutine;

        private void Awake()
        {
            observingSlider.onValueChanged.AddListener(delegate { SetSliderValue(); });
        }
        
        private void SetSliderValue()
        {
            var newSliderValue = observingSlider.value + offset;
            if (Math.Abs(observingSlider.value - observingSlider.maxValue) < 0.01 || observingSlider.value == 0)
                newSliderValue = observingSlider.value;
            
            if(_smoothChangeCoroutine is not null)
                StopCoroutine(_smoothChangeCoroutine);
            
            _smoothChangeCoroutine = SmoothSliderValueChange(newSliderValue);
            StartCoroutine(_smoothChangeCoroutine);
        }

        private IEnumerator SmoothSliderValueChange(float newSliderValue)
        {
            float currentHealthValue = nativeSlider.value;
            float elapsedTime = 0;
            while (elapsedTime < smoothSpeed)
            {
                nativeSlider.value = Mathf.Lerp(currentHealthValue, newSliderValue, elapsedTime / smoothSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            nativeSlider.value = newSliderValue;
        }
    }
}
