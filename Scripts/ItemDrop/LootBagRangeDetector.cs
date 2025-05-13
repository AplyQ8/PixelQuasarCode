using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBagRangeDetector : MonoBehaviour
{
    public Action<bool> OnPlayerIsInRange;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        OnPlayerIsInRange?.Invoke(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        OnPlayerIsInRange?.Invoke(false);
    }


}
