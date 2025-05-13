using System;
using System.Collections;
using System.Collections.Generic;
using Status_Effect_System;
using UnityEngine;

public class IncenserBuffZone : MonoBehaviour
{
    private List<IEffectable> _effectables;
    private LayerMask enemyObstacleColliderMask;

    private CircleCollider2D circleCollider;

    private SpriteRenderer spriteRenderer;
    private IEnumerator buffEnemiesRoutine;
    
    [SerializeField] private protected StatusEffectData effect;
    // Start is called before the first frame update
    void Start()
    {
        enemyObstacleColliderMask = LayerMask.GetMask("EnemyObstacleCollider");
        circleCollider = transform.GetComponent<CircleCollider2D>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();

        buffEnemiesRoutine = BuffEnemiesRoutine();
        
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;
        _effectables = new List<IEffectable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartIncense()
    {
        //StartCoroutine(buffEnemiesRoutine);
        spriteRenderer.enabled = true;
        circleCollider.enabled = true;
    }

    public void StopIncense()
    {
        //StopCoroutine(buffEnemiesRoutine);
        spriteRenderer.enabled = false;
        circleCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        
        if (other.TryGetComponent(out IEffectable effectable))
        {
            effectable.ApplyEffect(effect);
            _effectables.Add(effectable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        if (other.TryGetComponent(out IEffectable effectable))
        {
            if (!_effectables.Contains(effectable))
                return;
            effectable.RemoveEffect(effect);
            _effectables.Remove(effectable);
        }
    }

    IEnumerator BuffEnemiesRoutine()
    {
        for (;;)
        {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(TurnOffCollider());
            circleCollider.enabled = true;
        }

    }

    IEnumerator TurnOffCollider()
    {
        yield return new WaitForSeconds(0.1f);
        circleCollider.enabled = false;
    }
}
