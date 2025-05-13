using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkillGodGroup", menuName = "Skill Tree/Tree Composite/SkillGodGroupSO")]
public class SkillTreeGodGroupSO : SkillTreeGroupSO
{
    [SerializeField] private string description;
    [SerializeField] private bool partiallyUnlocked;
    public override void Unlock()
    {
        //base.Unlock();
        if (IsUnlocked) return;

        if (partiallyUnlocked)
        {
            foreach (var child in children)
            {
                child.Unlock();
            }
            IsUnlocked = true;
            return;
        }
        partiallyUnlocked = true;
    }

    public override string GetDescription() => description;

}
