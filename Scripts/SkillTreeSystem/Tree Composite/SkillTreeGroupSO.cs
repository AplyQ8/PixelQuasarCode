using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillGroup", menuName = "Skill Tree/Tree Composite/SkillGroupSO")]
public class SkillTreeGroupSO : SkillTreeComponentSO
{
    [Header("Group Properties")]
    public List<SkillTreeComponentSO> children = new List<SkillTreeComponentSO>();

    
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
        IsActivated = true;
        ExtractReources(resources);
        foreach (var child in children)
        {            
            child.Unlock();
        }
        
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
        
        foreach (var child in children)
        {
            
            child.Unlock();
        }
    }
}
