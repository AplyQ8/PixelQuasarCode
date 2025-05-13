using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Tree/Skill Logic/ActiveAbilityTreeSkillSO")]
public class ActiveAbilityTreeSkillSO : BaseTreeSkillSO
{
    [SerializeField] private ActiveAbilityItem activeAbilityItem;
    public override void Activate()
    {
        try
        {
            PickUpSystem pickUpSystem = GameObject.FindWithTag("Player").GetComponentInChildren<PickUpSystem>();
            pickUpSystem.PickItemFromBag(activeAbilityItem, 1, InventorySO.InventoryTypeEnum.ActiveAbilityInventory);
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning("Giving a active ability caused an exception");
        }
    }
}
