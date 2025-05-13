using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;
using UnityEngine.UI;

public class AdrenalineBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    private IAdrenalineContent _heroAdrenaline;

    private void Awake()
    {
        _heroAdrenaline = GameObject.FindWithTag("Player").GetComponent<IAdrenalineContent>();
        slider.maxValue = _heroAdrenaline.GetMaxAdrenalineBoundary();
        slider.value = _heroAdrenaline.GetCurrentAdrenalineValue();

        _heroAdrenaline.OnAdrenalineValueChange += SetAdrenaline;
    }
    public void SetAdrenaline(float currentAdrenaline)
    {
        slider.value = currentAdrenaline;
    }

    public void SetMaxAdrenalineValue(float maxAdrenalineValue)
    {
        slider.maxValue = maxAdrenalineValue;
    }
}
