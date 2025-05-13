using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Random = UnityEngine.Random;

namespace PickableObjects.InventoryItems
{
    [CreateAssetMenu(menuName = "InventoryItems/Throwable Items/ThrowableItem")]
    public class ThrowableItemSO : ItemSO, IItemAction, IDestroyableItem
    {
        [SerializeField] private List<ModifierData> modifiersData = new List<ModifierData>();
        [SerializeField] private ThrowData throwableItem;
        [field: SerializeField] public bool CanBeUsed { get; private set; } = true;
        [field: SerializeField] public bool CantBeUsedFromInventory { get; private set; }
        [field: SerializeField] public LocalizedString ActionName { get; private set; }
        [field: SerializeField] public AudioClip ActionSfx { get; private set; }
        public bool PerformAction(GameObject hero)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mouseDir = (mousePosition - hero.transform.position).normalized;
            return throwableItem.throwData.Throw(hero, 
                mouseDir * Random.Range(
                    throwableItem.groundDispenseVelocity.x, 
                    throwableItem.groundDispenseVelocity.y), 
                Random.Range(throwableItem.verticalDispenseVelocity.x, throwableItem.verticalDispenseVelocity.y),
                modifiersData,
                throwableItem.layerMask);
        }

        public void OnDestroyEvent(GameObject hero)
        { }
    }
    [Serializable]
    public class ThrowData
    {
        public ThrowableItemDataSO throwData;
        public Vector2 groundDispenseVelocity;
        public Vector2 verticalDispenseVelocity;
        public LayerMask layerMask;
    }
}