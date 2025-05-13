using PickableObjects.InventoryItems;
using UnityEngine;

namespace PickableObjects.PickUpSystem
{
    public class PickableTinderBox : MonoBehaviour
    {
        [SerializeField] private TinderBoxSo tinderBoxSo;
        [SerializeField] private float duration = 0.3f;

        private void Awake()
        {
            GetComponent<SpriteRenderer>().sprite = tinderBoxSo.ItemImage;
        }
        
        
    }
}