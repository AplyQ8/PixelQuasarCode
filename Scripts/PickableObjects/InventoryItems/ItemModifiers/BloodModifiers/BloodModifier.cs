using ObjectLogicInterfaces;
using UnityEngine;

namespace PickableObjects.InventoryItems.ItemModifiers.BloodModifiers
{
    [CreateAssetMenu(menuName = "Stat Modifiers/Blood Modifier")]
    public class BloodModifier : StatModifierSO
    {
        public override void AffectObject(GameObject objectToDealDamage, float value)
        {
            if (objectToDealDamage.TryGetComponent(out IBloodContent bloodContent))
            {
                bloodContent.AddBlood(value);
            }
        }
        public override void RemoveEffect(GameObject objectToRemoveEffect, float value)
        { }
    }
}