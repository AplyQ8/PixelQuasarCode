using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text hintTextUI;
    
    public void SetText(string hintMessage)
    {
        hintTextUI.text = hintMessage;
        gameObject.SetActive(true);
    }

    public void DisableHint()
    {
        gameObject.SetActive(false);
    }
}
