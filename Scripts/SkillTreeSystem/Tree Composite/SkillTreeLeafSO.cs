using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillLeaf", menuName = "Skill Tree/Tree Composite/SkillLeafSO")]
public class SkillTreeLeafSO : SkillTreeComponentSO
{
    public override bool Activate(SerializedDictionary<SkillResourceType, int> resources, out string message)
    {
        
        message = string.Empty;
        if (IsActivated)
        {
            message = "Skill is activated already!";
            return false; 
        }
        if (!IsUnlocked)
        {
            message = "Skill is locked";
            return false;
        }
        if (!IsResourcesEnough(resources, out message))
            return false;

        try
        {
            skill.Activate();
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Do not have a skill");
        }
        ExtractReources(resources);
        IsActivated = true;
        return true;
    }

    public override void Activate()
    {
        try
        {
            skill.Activate();
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning("Do not have a skill");
        }

    }
}
