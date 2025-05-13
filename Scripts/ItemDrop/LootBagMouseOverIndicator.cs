using System.Collections;
using System.Collections.Generic;
using ItemDrop;
using UnityEngine;

public class LootBagMouseOverIndicator : MonoBehaviour
{
    [SerializeField] private LootBagScript lootBagScript;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer parentIcon;
    

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parentIcon = lootBagScript.gameObject.GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
        lootBagScript.MouseEnter += MouseOverEvent;
        lootBagScript.MouseExit += MouseExitEvent;
    }

    private void Update()
    {
        spriteRenderer.sprite = parentIcon.sprite;
    }

    private void MouseOverEvent()
    {
        gameObject.SetActive(true);
    }
    private void MouseExitEvent()
    {
        gameObject.SetActive(false);
    }

    

    private void OnDestroy()
    {
        lootBagScript.MouseEnter -= MouseOverEvent;
        lootBagScript.MouseExit -= MouseExitEvent;
        
    }
}
