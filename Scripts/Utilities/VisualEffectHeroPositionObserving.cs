using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VisualEffectHeroPositionObserving : MonoBehaviour
{
    private VisualEffect _vfxRenderer;
    private GameObject _heroObject;

    private void Start()
    {
        _vfxRenderer = GetComponent<VisualEffect>();
        _heroObject = GameObject.FindWithTag("Player");
        
    }

    private void Update()
    {
        if (_heroObject is null) return;
        
        Vector3 localHeroPosition = transform.InverseTransformPoint(_heroObject.transform.position);
        _vfxRenderer.SetVector3("ColliderPosition", localHeroPosition);
    }
}
