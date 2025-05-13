using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace PickableObjects.PickUpSystem
{
    public class PickableItem : MonoBehaviour
    {
        [field: SerializeField] public ItemSO InventoryItem { get; private set; }
        [field: SerializeField] public int Quantity { get; set; } = 1;
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] [CanBeNull] private AudioSource pickUpSound;
        [SerializeField] private float duration = 0.3f;

        private void Start()
        {
            try
            {
                GetComponent<SpriteRenderer>().sprite = InventoryItem.ItemImage;
            }
            catch(NullReferenceException){}
        }

        public void Initialize(ItemSO item, int quantity)
        {
            InventoryItem = item;
            Quantity = quantity;
            sprite.sprite = InventoryItem.ItemImage;
        }
    
        public void DestroyItem()
        {
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(AnimateItemPickUp());

        }

        public ItemSO.ItemTypeEnum GetItemType => InventoryItem.ItemType;

        private IEnumerator AnimateItemPickUp()
        {
            pickUpSound!.Play();
            Vector3 startScale = transform.localScale;
            Vector3 endScale = Vector3.zero;
            float currentTime = 0f;
            while (currentTime < duration)
            {
                transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
                currentTime += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
