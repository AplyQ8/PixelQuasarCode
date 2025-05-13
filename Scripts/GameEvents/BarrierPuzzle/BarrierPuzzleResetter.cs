using System.Collections;
using System.Collections.Generic;
using Envirenmental_elements;
using ObjectLogicInterfaces;
using UnityEngine;

public class BarrierPuzzleResetter : MonoBehaviour, IInteractable, IDistanceCheckable
{
    
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Interact()
    {
        ResetBarrierPuzzle();
    }
    
    public bool CanBeInteractedWith => true;
    
    public float DistanceToPlayer()
    {
        var obstacleCollider = player.transform.Find("ObstacleCollider");
        return Vector2.Distance(transform.position, obstacleCollider.position);
    }

    private void ResetBarrierPuzzle()
    {
        // Start with the direct children of the parent
        foreach (Transform child in transform.parent)
        {
            // Check and reset BarrierBreaker on the direct child
            BarrierBreaker barrierBreaker = child.GetComponent<BarrierBreaker>();
            if (barrierBreaker != null)
            {
                barrierBreaker.Reset();
            }

            // Check grandchildren for BarrierWall components and reset them
            foreach (Transform grandchild in child)
            {
                BarrierWall barrierWall = grandchild.GetComponent<BarrierWall>();
                if (barrierWall != null)
                {
                    barrierWall.Reset();
                }
            }
        }
    }
    
    
}
