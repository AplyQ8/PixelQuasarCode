using System;
using System.Collections;
using System.Collections.Generic;
using InteractableObjects.SaveFirecamp;
using ObjectLogicInterfaces;
using UnityEngine;
using IInteractable = Envirenmental_elements.IInteractable;

public class ShortcutDoor : MonoBehaviour, IInteractable, IDistanceCheckable
{
    [SerializeField] private ParticleSystem particles;
    private Animator _animator;
    private bool canBeOpened;
    private static bool opened;
    private BoxCollider2D _boxCollider2D;
    private SpriteRenderer _spriteRenderer;
    
    private GameObject player;
    private GameObject doorOpenObject;
    private GameObject doorClosedObject;
    private static readonly int Unlock = Animator.StringToHash("Unlock");

    void Start()
    {
        canBeOpened = false;
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        doorOpenObject = GetComponentInChildren<ShortcutDoorOpen>().gameObject;
        doorClosedObject = GetComponentInChildren<ShortcutDoorClosed>().gameObject;

        _animator = GetComponent<Animator>();
        
        player = GameObject.FindWithTag("Player");
        
        if(opened)
            OpenTheDoor();
    }

    public void Interact()
    {
        if (canBeOpened)
        {
            OpenTheDoor();
        }
    }
    
    public bool CanBeInteractedWith => canBeOpened;

    public bool IsOpened => opened;
    
    public float DistanceToPlayer()
    {
        var obstacleCollider = player.transform.Find("ObstacleCollider");
        return Vector2.Distance(transform.position, obstacleCollider.position);
    }

    public void OpenTheDoor()
    {
        opened = true;
        _animator.SetTrigger(Unlock);
        doorOpenObject.SetActive(false);
        doorClosedObject.SetActive(false);
        particles.Stop();
    }

    public void DisableCollider()
    {
        _boxCollider2D.enabled = false;
    }

    public void CanOpenTheDoor(bool canOpen)
    {
        canBeOpened = canOpen;
    }
}
