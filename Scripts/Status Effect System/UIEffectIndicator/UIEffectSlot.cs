using System;
using System.Collections;
using System.Collections.Generic;
using Status_Effect_System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utilities;


public class UIEffectSlot : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private Image icon;
    private Timer _startBlinkTimer;

    [Header("Blink info")]
    [SerializeField] private float timeToStartBlinking = 3f;
    [SerializeField] private float blinkInterval = 1f;
    [SerializeField] private float minOpacity = 0.1f;
    [SerializeField] private float maxOpacity = 1f;

    [Header("Indicator colors")] 
    [SerializeField] private Color positiveEffectColor;
    [SerializeField] private Color negativeEffectColor;

    private IEnumerator _blinkCoroutine;
    
    public void Initialize(Sprite effectIcon, float effectDuration, StatusEffectData.EffectType effectType)
    {
        slotImage.color = effectType switch
        {
            StatusEffectData.EffectType.Positive => positiveEffectColor,
            StatusEffectData.EffectType.Negative => negativeEffectColor,
            _ => slotImage.color
        };
        icon.sprite = effectIcon;

        _blinkCoroutine = Blink();
        
        _startBlinkTimer = new Timer(effectDuration - timeToStartBlinking);
        _startBlinkTimer.OnTimerDone += StartBlinking;
        _startBlinkTimer.StartTimer();
    }

    public void RestartEffect(float effectDuration)
    {
        StopCoroutine(_blinkCoroutine);
        _startBlinkTimer = effectDuration - timeToStartBlinking is 0 ? 
            new Timer(0) : new Timer(effectDuration - timeToStartBlinking);
        _startBlinkTimer.OnTimerDone += StartBlinking;
        _startBlinkTimer.StartTimer();
    }
    
    private void Update()
    {
        try
        {
            _startBlinkTimer.Tick();
        }
        catch(NullReferenceException){/* no Timer */}
    }

    private void StartBlinking()
    {
        StartCoroutine(_blinkCoroutine);
    }
    
    IEnumerator Blink()
    {
        while (true)
        {
            yield return FadeImage(minOpacity, maxOpacity, blinkInterval / 2); // Fade in
            yield return FadeImage(maxOpacity, minOpacity, blinkInterval / 2); // Fade out
        }
    }
    IEnumerator FadeImage(float startOpacity, float targetOpacity, float duration)
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float newOpacity = Mathf.Lerp(startOpacity, targetOpacity, t);

            slotImage.color = new Color(slotImage.color.r, slotImage.color.g, slotImage.color.b, newOpacity);
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, newOpacity);

            yield return null;
        }
    }

    private void OnDisable()
    {
        try
        {
            StopCoroutine(_blinkCoroutine);
            _startBlinkTimer.StopTimer();
        }
        catch(NullReferenceException) {/*No time assigned*/}
    }
}
