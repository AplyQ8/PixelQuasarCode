using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainAnchor : MonoBehaviour
{
    public Vector2 partOffset = Vector2.zero;
    public float lerpSpeed = 20f;

    private Transform[] chainParts;
    private Transform chainAnchor;

    private void Awake() 
    {
        chainAnchor = GetComponent<Transform>();
        chainParts = GetComponentsInChildren<Transform>();
    }

    private void Update() 
    {
        Transform pieceToFollow = chainAnchor;

        foreach(Transform chainPart in chainParts)
        {
            if (!chainPart.Equals(chainAnchor))
            {
                Vector2 targetPosition = (Vector2) pieceToFollow.position + partOffset;
                Vector2 newPositionLerped = Vector2.Lerp(chainPart.position, targetPosition, Time.deltaTime * lerpSpeed);
               
                chainPart.position = newPositionLerped;
                pieceToFollow = chainPart;
            }
        }
    }
}
