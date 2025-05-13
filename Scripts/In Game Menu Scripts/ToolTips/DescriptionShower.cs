using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

public class DescriptionShower : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] private bool isActive;
    private Coroutine _delayBeforeShowCoroutine;
    [SerializeField] private float delayBeforeShow = 0.3f;
    //private Timer _delayBeforeSHowTimer;

    private void Awake()
    {
        //_delayBeforeShowCoroutine = DelayBeforeShowCoroutine();
    }
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isActive)
            return;
        UIInventoryToolTip.Instance.ShowToolTip(itemName, itemDescription);
        //_delayBeforeShowCoroutine = StartCoroutine(DelayBeforeShowCoroutine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isActive)
            return;
        // if ( _delayBeforeShowCoroutine != null)
        // {
        //     StopCoroutine( _delayBeforeShowCoroutine);
        //     _delayBeforeShowCoroutine = null;
        // }
        UIInventoryToolTip.Instance.HideToolTip();
    }

    public void SetData(string iName, string iDescription, bool isActivated)
    {
        isActive = isActivated;
        itemName = iName;
        itemDescription = iDescription;
    }
    
    
    public void SetDescriptionAndActivate(string iName, string iDescription)
    {
        itemName = iName;
        itemDescription = iDescription;
        isActive = true;
        UIInventoryToolTip.Instance.ShowToolTip(itemName, itemDescription);
    }

    public void Deactivate()
    {
        itemName = string.Empty;
        itemDescription = string.Empty;
        isActive = false;
        UIInventoryToolTip.Instance.HideToolTip();
    }

    private IEnumerator DelayBeforeShowCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeShow);
        UIInventoryToolTip.Instance.ShowToolTip(itemName, itemDescription);
        _delayBeforeShowCoroutine = null;

    }

    
}
