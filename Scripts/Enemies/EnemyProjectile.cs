using System;
using System.Collections;
using System.Collections.Generic;
using ObjectLogicInterfaces;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] protected float baseSpeed;
    public Vector2 directionVector;

    [SerializeField]private float lifeDuration;
    private float lifeTimer;
    
    [SerializeField]private float shift;

    [SerializeField] private float damage;

    protected Transform shadow;
    
    private LayerMask obstacleMask;
    private LayerMask playerBottomMask;

    protected BoxCollider2D projCollider;
    protected SpriteRenderer spriteRenderer;
    
    private ObjectPool<EnemyProjectile> pool;

    private AudioSource audioSource;
    [SerializeField] private AudioClip[] hitPlayerSounds;

    protected bool hitPlayer;
    
    // Start is called before the first frame update
    void Awake()
    {
        directionVector = Vector2.left;
        lifeTimer = lifeDuration;

        shadow = transform.Find("Shadow");

        audioSource = transform.GetComponent<AudioSource>();
        projCollider = transform.GetComponent<BoxCollider2D>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        
        obstacleMask = LayerMask.GetMask("Obstacle");
        playerBottomMask = LayerMask.GetMask("PlayerObstacleCollider");

        hitPlayer = false;
    }

    void OnEnable()
    {
        lifeTimer = lifeDuration;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0)
        {
            DestroyProjectile();
        }
    }

    protected virtual void UpdatePosition()
    {
        if(!hitPlayer)
            transform.position += (Vector3)directionVector * (baseSpeed * Time.deltaTime);
    }

    public void SetDirection(Vector2 direction, float rotationAngle)
    {
        directionVector = direction.normalized;
        transform.rotation = Quaternion.Euler(new Vector3(0f,0f,rotationAngle));

        transform.position += (Vector3)directionVector * shift;
    }

    public void SetShadowOffset(float offset)
    {
        shadow.transform.position = (Vector2)transform.position + offset*Vector2.down;
    }

    public void CheckStartCollision(Transform sourceBottom)
    {
        Vector2 direction = shadow.transform.position - sourceBottom.position;
        var rayToObstacle = Physics2D.Raycast(sourceBottom.position, direction.normalized,
            direction.magnitude, obstacleMask);
        if (rayToObstacle)
        {
            var rayToPlayerBottom = Physics2D.Raycast(sourceBottom.position, direction.normalized,
                direction.magnitude, playerBottomMask);

            if (!rayToPlayerBottom || rayToObstacle.distance < rayToPlayerBottom.distance)
            {
                DestroyProjectile();
            }
        }
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.CompareTag("Hook"))
        {
            DestroyProjectile();
            return;
        }
        if (!collider.CompareTag("Player"))
            return;
        
        OnPlayerHit(collider);
        // DestroyProjectile();
        DisableProjectile();
        StartCoroutine(DestroyWithDelay());

    }

    protected virtual void OnPlayerHit(Collider2D player)
    {
        if (HitThroughObstacle(player))
        {
            return;
        }
        
        audioSource?.PlayOneShot(SelectRandomClip(hitPlayerSounds));
        if (player.TryGetComponent<IDamageable>(out IDamageable damageableObject))
        {
            damageableObject.TakeDamage(damage, DamageTypeManager.DamageType.Default);
        }
    }

    protected bool HitThroughObstacle(Collider2D player)
    {
        var shadowToPlayerBottom =  player.transform.Find("ObstacleCollider").position - shadow.transform.position;
        // if (directionVector.y <= 0)
        // {
        //     return false;
        // }
        return Physics2D.Raycast(shadow.transform.position, shadowToPlayerBottom.normalized,
            shadowToPlayerBottom.magnitude, obstacleMask);
    }

    public void SetPool(ObjectPool<EnemyProjectile> newPool)
    {
        pool = newPool;
    }
    
    protected AudioClip SelectRandomClip(AudioClip[] clips)
    {
        if (clips.Length > 0)
        {
            int randomIndex = Random.Range(0, clips.Length);
            AudioClip selectedClip = clips[randomIndex];

            return selectedClip;
        }

        return null;
    }

    public void DisableProjectile()
    {
        projCollider.enabled = false;
        spriteRenderer.enabled = false;
        hitPlayer = true;
        shadow.gameObject.SetActive(false);
    }

    IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(1);
        projCollider.enabled = true;
        spriteRenderer.enabled = true;
        hitPlayer = false;
        shadow.gameObject.SetActive(true);
        DestroyProjectile();
    }

    public void DestroyProjectile()
    {
        try{
            pool.Release(this);
        }
        catch(Exception)
        {
            //
        }
        // Destroy(gameObject);
       
    }
    
}
