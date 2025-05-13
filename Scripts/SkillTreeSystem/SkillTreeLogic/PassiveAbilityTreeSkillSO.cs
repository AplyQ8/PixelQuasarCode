using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Skill Logic/PassiveAbilityTreeSkillSO")]
public class PassiveAbilityTreeSkillSO : BaseTreeSkillSO
{
    [SerializeField] private PassiveAbilityItem passiveAbilityItem;
    public override void Activate()
    {
        try
        {
            PickUpSystem pickUpSystem = GameObject.FindWithTag("Player").GetComponentInChildren<PickUpSystem>();
            pickUpSystem.PickItemFromBag(passiveAbilityItem, 1, InventorySO.InventoryTypeEnum.PassiveAbilityInventory);
        }
        catch(NullReferenceException e) {
            Debug.LogWarning("Giving a passive ability caused an exception");
        }
        
        
        
    }
}
