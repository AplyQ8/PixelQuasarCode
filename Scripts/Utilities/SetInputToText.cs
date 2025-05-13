using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetInputToText : MonoBehaviour
{
    [SerializeField] private string message;
    [SerializeField] private ListenForInput inputListener;
    [SerializeField] private TMP_Text textField;

    private void Awake()
    {
        //textField.text = inputListener.GetStringKeyCode();
        
    }
}
