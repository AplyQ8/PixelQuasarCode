using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fill;
    
    public void SetBlood(float bloodValue)
    {
        slider.value = bloodValue;
    }

    public void SetMaxBlood(float maxBloodValue)
    {
        slider.maxValue = maxBloodValue;
        
    }
}
