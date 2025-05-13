using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosionLogic : MonoBehaviour
{
    [SerializeField] private List<ModifierData> dataModifiers;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private Animator bombAnimator;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float explosionRadius;
    private static readonly int ExplodeTrigger = Animator.StringToHash("Explode");
    
    public void Explode(Vector3 explosionPosition)
    {
        bombAnimator.SetTrigger(ExplodeTrigger);
        explosionEffect.Play();
        Collider2D[] objectsToDealDamage = Physics2D.OverlapCircleAll(explosionPosition, 
            explosionRadius, 
            layerMask);

        foreach (var victim in objectsToDealDamage)
        {
            foreach (var modifier in dataModifiers)
            {
                modifier.statModifier.AffectObject(victim.gameObject, modifier.value);
            }
        }
        
    }

    private void FixedUpdate()
    {
        if(explosionEffect == null)
            Destroy(gameObject);
    }

    private void SetModifiers(List<ModifierData> dModifiers) => dataModifiers = dModifiers;
    private void SetLayerMask(LayerMask lMask) => layerMask = lMask;
    
    private void SetDistance(float dist) => explosionRadius = dist;

    public void SetExplosionCharacteristics(List<ModifierData> dataModifiers, LayerMask layerMask, float explosionRadius)
    {
        SetModifiers(dataModifiers);
        SetLayerMask(layerMask);
        SetDistance(explosionRadius);
    }
}
