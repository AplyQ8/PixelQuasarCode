using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutDoorOpen : MonoBehaviour
{
    private ShortcutDoor door;
    private HintCanvas hintUI;

    [SerializeField] private string hintText;
    void Start()
    {
        door = transform.GetComponentInParent<ShortcutDoor>();
        hintUI = GetComponentInChildren<HintCanvas>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        
        door.CanOpenTheDoor(true);
        hintUI.SetText(hintText);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;
        
        door.CanOpenTheDoor(false);
        hintUI.DisableHint();
    }
}
