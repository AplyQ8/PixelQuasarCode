using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInventoryToolTip : Singleton<UIInventoryToolTip>
{
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;
    //private static UIInventoryToolTip _instance;
    //public static UIInventoryToolTip Instance => _instance;
    
    private void Start()
    {
        Cursor.visible = true;
        itemName = transform.GetChild(0).GetComponent<TMP_Text>();
        itemDescription = transform.GetChild(1).GetComponent<TMP_Text>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void ShowToolTip(string iName, string iDescription)
    {
        itemName.text = iName;
        itemDescription.text = iDescription;
        gameObject.SetActive(true);
        
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
        itemName.text = string.Empty;
        itemDescription.text = string.Empty;
    }
    
}
