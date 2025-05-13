using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class DeathGodThrownHalberd : EnemyProjectile
{

    private Vector3 goalPoint;
    [SerializeField] private float distanceThreshold;
    
    protected override void UpdatePosition()
    {
        if(!hitPlayer)
            transform.position += (Vector3)directionVector * (baseSpeed * Time.deltaTime);

        float distanceToGoal = (transform.position - goalPoint).magnitude;
        if (distanceToGoal <= distanceThreshold)
        {
            ThrowFinish();
        }
    }

    public void SetGoalPoint(Vector3 point)
    {
        goalPoint = point;
    }
    
    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        
        if (!collider.CompareTag("Player"))
            return;
        
        OnPlayerHit(collider);

    }

    public void ThrowFinish()
    {
        DisableProjectile();
        StartCoroutine(DestroyWithDelay());
    }
    
    IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
