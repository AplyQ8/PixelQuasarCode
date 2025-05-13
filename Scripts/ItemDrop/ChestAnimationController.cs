using System.Collections;
using System.Collections.Generic;
using ItemDrop;
using UnityEngine;

public class ChestAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private LootBagScript lootBagScript;


    private void Awake()
    {
        lootBagScript.OnOpen += OpenEvent;
        lootBagScript.OnClose += CloseEvent;
    }

    private void OpenEvent()
    {
        animator.SetTrigger("Open");
    }
    private void CloseEvent()
    {
        animator.SetTrigger("Close");
    }
    private void OnDisable()
    {
        lootBagScript.OnOpen -= OpenEvent;
        lootBagScript.OnClose -= CloseEvent;
    }
}
