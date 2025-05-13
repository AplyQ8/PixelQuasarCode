using System.Collections;
using System.Collections.Generic;
using RouteScripts;
using UnityEngine;

public class PatrolPointContainer : MonoBehaviour
{

    [SerializeField] private Vector2 stopoverLookingDirection;
    [SerializeField] private float stopoverDuration;

    private PatrolPoint patrolPoint;
    void Awake()
    {
        if (transform.parent.childCount == 1)
        {
            stopoverDuration = 10;
        }
        patrolPoint = new PatrolPoint(transform.position, stopoverLookingDirection, stopoverDuration);
    }

    public PatrolPoint GetPatrolPoint() => patrolPoint;

}
