using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InventoryItems/Throwable Items/ThrowableItemType/HandBombData")]
public class HandBomb : ThrowableItemDataSO
{
    [SerializeField] private FakeHeightThrowableObject bombPrefab;
    [SerializeField] private float explosionRadius;
    public override bool Throw(GameObject hero, Vector2 groundVelocity, float verticalVelocity, List<ModifierData> dataModifiers, LayerMask layerMask)
    {
        var stateManager = hero.GetComponent<HeroStateHandler>();
        if (stateManager.GetCurrentState == stateManager.InventoryState)
            return false;
        try
        {
            InitializeBomb(
                hero.transform.position,
                groundVelocity, 
                verticalVelocity,
                dataModifiers,
                layerMask);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void InitializeBomb(Vector3 staringPosition, Vector2 groundVelocity, float verticalVelocity, 
        List<ModifierData> dataModifiers, LayerMask layerMask){
        var instantiatedBomb =Instantiate(bombPrefab, staringPosition, Quaternion.identity);
        instantiatedBomb.Initialize(groundVelocity, verticalVelocity);
        var explosionScript = instantiatedBomb.gameObject.GetComponent<BombExplosionLogic>();
        explosionScript.SetExplosionCharacteristics(dataModifiers, layerMask, explosionRadius);
    }
}
