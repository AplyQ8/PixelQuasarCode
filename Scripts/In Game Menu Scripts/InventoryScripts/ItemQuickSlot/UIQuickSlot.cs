using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuickSlot : MonoBehaviour
{
    private string ActivationKeyCode { get; set; }
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityTxt;
    private bool _empty = true;
    public event Action OnInputTrigger;

    private void Awake()
    {
        ResetData();
    }
    public void TriggerInput()
    {
        OnInputTrigger?.Invoke();
    }
    public virtual void SetData(Sprite sprite, int quantity)
    {
        if (sprite is null || quantity == 0)
            ResetData();
        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
        }
        if (quantityTxt != null)
        {
            quantityTxt.text = quantity.ToString();
        }
        _empty = false;
    }

    public void SetData(int quantity)
    {
        quantityTxt.text = quantity.ToString();
        if(quantity <= 0)
            ResetData();
    }
    public void ResetData()
    {
        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(false);
        }
        
        _empty = true;
    }

    public void SetActivationKeyCodeName(string activationKeyCode)
    {
        // ActivationKeyCode = activationKeyCode.ToString();
        // if (ActivationKeyCode.Contains("Alpha"))
        //     ActivationKeyCode = ActivationKeyCode.Substring(ActivationKeyCode.Length - 1);
        ActivationKeyCode = activationKeyCode;
    }
}
