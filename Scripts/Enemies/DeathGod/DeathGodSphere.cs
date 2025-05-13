using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;

public class DeathGodSphere : MonoBehaviour
{
    
    private enum SphereState
    {
        Moving,
        Preparation,
        Expansion,
        Active,
        None
    }
    
    private SphereState state;
    
    [SerializeField] protected float timeToDestination;
    
    [SerializeField] private float prepDuration;
    private float prepTimer;
    [SerializeField] private float expansionDuration;
    private float expansionTimer;
    [SerializeField] private float activeDuration;
    private float activeTimer;
    [SerializeField] private float damageCooldown;
    
    [SerializeField] private float MaxRadius;
    [SerializeField] private float MinRadius;
    [SerializeField] private float damage;
    
    private Vector3 goalPoint;

    private CircleCollider2D circleCollider;
    private Rigidbody2D rigidbody2D;
    private Transform spriteTransform;
    
    void Awake()
    {
        circleCollider = transform.GetComponent<CircleCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteTransform = transform.Find("Sprite");
        
        circleCollider.enabled = false;
        state = SphereState.None;
        
        SetGoalPoint(new Vector3(-33, 0, 0));
    }

    public void SetGoalPoint(Vector3 point)
    {
        goalPoint = point;
        var directionVector = (goalPoint - transform.position).normalized;
        float speed = (goalPoint - transform.position).magnitude / timeToDestination;
        rigidbody2D.velocity = directionVector * speed;
        state = SphereState.Moving;
    }
    

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case SphereState.Moving:
                if ((transform.position - goalPoint).magnitude < 0.1)
                {
                    rigidbody2D.velocity = Vector2.zero;
                    prepTimer = prepDuration;
                    state = SphereState.Preparation;
                }
                break;
            case SphereState.Preparation:
                prepTimer -= Time.deltaTime;
                if (prepTimer <= 0)
                {
                    expansionTimer = expansionDuration;
                    circleCollider.radius = MinRadius;
                    circleCollider.enabled = true;
                    state = SphereState.Expansion;
                }
                break;
            case SphereState.Expansion:
                UpdateSphereSize();
                
                expansionTimer -= Time.deltaTime;
                if (expansionTimer <= 0)
                {
                    activeTimer = activeDuration;
                    state = SphereState.Active;
                }
                break;
            case SphereState.Active:
                activeTimer -= Time.deltaTime;
                if (activeTimer <= 0)
                    Destroy(gameObject);
                break;
                
        }
    }
    
    private void UpdateSphereSize()
    {
        float newRadius = MinRadius + (MaxRadius - MinRadius) * (1 - expansionTimer / expansionDuration);
        circleCollider.radius = newRadius;
        
        float newSpriteScale = 0.2f + (2f - 0.2f) * (1 - expansionTimer / expansionDuration);
        spriteTransform.localScale = new Vector3(newSpriteScale, newSpriteScale, newSpriteScale);
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.CompareTag("Player"))
        {
            if (collider.TryGetComponent<IDamageable>(out IDamageable damageableObject))
            {
                damageableObject.TakeDamage(damage, DamageTypeManager.DamageType.Default);
            }
            circleCollider.enabled = false;
            StartCoroutine(DamageCooldown());
        }
        
    }

    IEnumerator DamageCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        circleCollider.enabled = true;
    }
}
