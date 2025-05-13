using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepDistanceEnemy : EnemyScript
{
    
    protected bool farAwayFromComfortZone;
    protected bool inComfortZone;
    
    [Header("Keep Distance Parameters")]
    [SerializeField] protected float innerComfortZoneRadius;
    [SerializeField] protected float outerComfortZoneRadius;
    
    [SerializeField] protected float moveDirCooldown;
    protected float moveDirTimer;
    
    protected System.Random randomDirection = new System.Random();
    
    
    protected void MoveToComfortZone()
    {
        if (farAwayFromComfortZone)
        {
            // destinationPoint = playerBottom.position;
            // MoveToDestination();
            MoveToPlayer();
            
            if ((playerBottom.position - bodyBottom.position).magnitude < outerComfortZoneRadius)
            {
                moveDirTimer = 0;
            }
        }
                

        if ((playerBottom.position - bodyBottom.position).magnitude < innerComfortZoneRadius)
        {
            if (inComfortZone)
            {
                inComfortZone = false;
                moveDirTimer = 0;
            }
            else
            {
                var playerDirection = (Vector2)(playerBottom.position - bodyBottom.position).normalized;
                var movementDirection = rigidBody.velocity.normalized;

                var playerToMovement = movementDirection - playerDirection;
                if (playerToMovement.magnitude < Mathf.Sqrt(2))
                {
                    moveDirTimer = 0;
                }
            }
        }
                
        moveDirTimer -= Time.deltaTime;
        if (moveDirTimer <= 0)
        {
            moveDirTimer = moveDirCooldown;
            ChooseMovementDirection();
        }
    }
    
    
    private void ChooseMovementDirection()
    {
        var playerDirection = (playerBottom.position - bodyBottom.position).normalized;
        
        float angle = EnemyUtility.AngleBetweenTwoPoints(playerBottom.position, bodyBottom.position);
        var directionToPlayer = EnemyUtility.AngleToDirection(angle);

        HashSet<EnemyUtility.Direction> candidateDirections = new HashSet<EnemyUtility.Direction>();
        float distanceToPlayer = (playerBottom.position - bodyBottom.position).magnitude;

        if (distanceToPlayer <= innerComfortZoneRadius)
        {
            farAwayFromComfortZone = false;
            inComfortZone = false;
            moveDirTimer /= 2;
            // dashCooldownTimer *= 0.85f;
            for (int i = 0; i <= 2; i++)
            {
                candidateDirections.Add((EnemyUtility.Direction) (((int) directionToPlayer + 4 + i) % 8));
                candidateDirections.Add((EnemyUtility.Direction) (((int) directionToPlayer + 4 - i) % 8));
            }
        } else if (distanceToPlayer <= outerComfortZoneRadius)
        {
            farAwayFromComfortZone = false;
            inComfortZone = true;
            for (int i = 2; i <= 2; i++)
            {
                candidateDirections.Add((EnemyUtility.Direction) (((int) directionToPlayer + 4 + i) % 8));
                candidateDirections.Add((EnemyUtility.Direction) (((int) directionToPlayer + 4 - i) % 8));
            }
        }
        else
        {
            farAwayFromComfortZone = true;
            inComfortZone = false;
            return;
        }
        
        

        float distanceForCheck = 20;
        EnemyUtility.Direction bestDirection = EnemyUtility.Direction.East;
        float maxDistance = 0;
        bool severalBest = false;
        List<EnemyUtility.Direction> severalBestDirections = new List<EnemyUtility.Direction>();
        foreach (var dir in candidateDirections)
        {
            RaycastHit2D hit = Physics2D.Raycast(bodyBottom.position, EnemyUtility.DirectionToVector(dir),
                distanceForCheck, obstacleReliefMask);
            float distanceToObstacle = hit.distance;
            if (hit.transform == null)
            {
                distanceToObstacle = distanceForCheck;
                severalBest = true;
                severalBestDirections.Add(dir);
            }
            if (!severalBest && distanceToObstacle > maxDistance)
            {
                maxDistance = hit.distance;
                bestDirection = dir;
            }
        }

        if (severalBest)
        {
            int randomIndex = randomDirection.Next(0, severalBestDirections.Count);
            bestDirection = severalBestDirections[randomIndex];
        }

        Vector2 newMovementDirection = EnemyUtility.DirectionToVector(bestDirection);

        rigidBody.velocity = moveScript.GetCurrentMoveSpeed() * newMovementDirection;
    }
}
