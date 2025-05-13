using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FakeHeightThrowableObject : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Transform transformObject;
    [SerializeField] private Transform transformBody;
    [SerializeField] private Transform transformShadow;
    [SerializeField] private Transform particleTransform;

    [Header("Flight characteristics")] 
    [SerializeField] private Vector2 groundVelocity;
    [SerializeField] private float verticalVelocity;
    [SerializeField] private float gravityScale;
    
    [SerializeField] private bool isGrounded = false;

    private IEnumerator _flightCoroutine;
    private ComponentCollision _componentCollision;

    public UnityEvent<Vector3> onGroundEvent;
    
    
    private void Awake()
    {
        _flightCoroutine = Flight();
    }
    
    private IEnumerator Flight()
    {
        var delay = new WaitForEndOfFrame();
        while (true)
        {
            UpdatePosition();
            CheckGroundHit();
            yield return delay;
        }
    }

    public void Initialize(Vector2 groundVelocity, float verticalVelocity)
    {
        _componentCollision = new ComponentCollision(false, false);
        this.groundVelocity = groundVelocity;
        this.verticalVelocity = verticalVelocity;
        StartCoroutine(_flightCoroutine);
    }
    private void UpdatePosition()
    {
        if (!isGrounded)
        {
            verticalVelocity += gravityScale * Time.deltaTime;
            transformBody.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
            particleTransform.position += new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
        }
        transformObject.position += (Vector3)groundVelocity * Time.deltaTime;
    }
    private void CheckGroundHit()
    {
        if (!(transformBody.position.y < transformObject.position.y) || isGrounded) return;
        
        transformShadow.gameObject.SetActive(false);
        transformBody.position = transformShadow.position;
        isGrounded = true;
        StopCoroutine(_flightCoroutine);
        onGroundEvent.Invoke(transformBody.position);
    }

    public void BodyObstacleCollision(bool isTriggered)
    {
        _componentCollision.BodyIsHit = isTriggered;
        CheckCollision();
    }
    public void ShadowObstacleCollision(bool isTriggered)
    {
        _componentCollision.ShadowIsHit = isTriggered;
        CheckCollision();
    }

    private void CheckCollision()
    {
        if (!_componentCollision.BothHt)
            return;
        StopCoroutine(_flightCoroutine);
        transformShadow.gameObject.SetActive(false);
        onGroundEvent?.Invoke(transformBody.position);
    }

    
    
}

public struct ComponentCollision
{
    public bool BodyIsHit { private get; set; }
    public bool ShadowIsHit { private get; set; }
    
    public ComponentCollision(bool bodyIsHit, bool shadowIsHit)
    {
        BodyIsHit = bodyIsHit;
        ShadowIsHit = shadowIsHit;
    }

    public bool BothHt => BodyIsHit && ShadowIsHit;
}
