using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fill;

    public virtual void SetHealth(float healthPoints)
    {
        slider.value = healthPoints;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public virtual void SetMaxHealth(float maxHealthPoints)
    {
        slider.maxValue = maxHealthPoints;
        fill.color = gradient.Evaluate(1f);
    }
}
