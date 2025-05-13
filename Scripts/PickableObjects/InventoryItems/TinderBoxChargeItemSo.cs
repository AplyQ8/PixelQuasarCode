using System.Collections.Generic;
using UnityEngine;

namespace PickableObjects.InventoryItems
{
    [CreateAssetMenu(menuName = "InventoryItems/TinderBoxChargerSo")]
    public class TinderBoxChargeItemSo: EdibleItemSO, IItemAction, IDestroyableItem
    {
        [Header("Tinder Box Info")]
        [SerializeField] private TinderBoxSo tinderBox;
        [SerializeField] private float chargeValue;
        
        [field: SerializeField] public AudioClip UnsuccessfulCharge { get; private set; }
        
        public override bool PerformAction(GameObject hero)
        {
            if (!tinderBox.Add(chargeValue))
            {
                return false;
            }

            base.PerformAction(hero);
            return true;
        }
    }
}