using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillTreeComponentSO : ScriptableObject
{
    [Header("Tree Skill")]
    [SerializeField] protected BaseTreeSkillSO skill;
    [field: SerializeField] public Sprite Icon { get; private set; }

    [Tooltip("List of parents")]
    [SerializeField] protected List<SkillTreeComponentSO> parents = new List<SkillTreeComponentSO>();

    [SerializedDictionary("Resource Type", "Value")]
    protected SerializedDictionary<SkillResourceType, int> requiredResources;

    [field: SerializeField] public bool IsUnlocked { get; protected set; }
    [field: SerializeField] public bool IsActivated { get; protected set; }

    // ����� ��� ��������, ����� �� �������������� ���������
    public virtual bool CanUnlock()
    {
        foreach (var parent in parents)
        {
            if (!parent.IsActivated)
            {
                return false;
            }
        }
        return true;
    }

    // ����� ��� ������������� ����������
    public virtual void Unlock()
    {
        if (IsUnlocked) return;
        if (CanUnlock())
        {
            IsUnlocked = true;
        }
    }

    public virtual string GetDescription() => skill.GetDescription();

    public virtual void AddParent(SkillTreeComponentSO skillTreeComponent)
    {
        if (parents.Contains(skillTreeComponent))
            return;
        parents.Add(skillTreeComponent); 
    }

    // ����������� ����� ��� ��������� ����������
    public abstract bool Activate(SerializedDictionary<SkillResourceType, int> resources, out string message);
    public abstract void Activate();

    protected bool IsResourcesEnough(SerializedDictionary<SkillResourceType, int> resources, out string message)
    {
        message = string.Empty;
        foreach(var resource in requiredResources)
        {
            if (resources[resource.Key] < resource.Value)
            {
                message = $"Not enough {resource.Key}";
                return false;
            }
        }
        return true;
    }

    protected void ExtractReources(SerializedDictionary<SkillResourceType, int> resources)
    {
        foreach(var resource in requiredResources)
        {
            resources[resource.Key] -= resource.Value;
        }
    }

    public List<SkillTreeComponentSO> GetParents() => parents;

    // ����� ��� ������� ���������
    public void ClearParents()
    {
        parents.Clear();
    }


}
