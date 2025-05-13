using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutDoorClosed : MonoBehaviour
{
    private HintCanvas hintUI;

    [SerializeField] private string hintText;
    void Start()
    {
        hintUI = GetComponentInChildren<HintCanvas>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        
        hintUI.SetText(hintText);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        
        hintUI.DisableHint();
    }
}
