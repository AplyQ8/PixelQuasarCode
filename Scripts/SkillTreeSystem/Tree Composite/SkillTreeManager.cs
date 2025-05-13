using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance { get; private set; }

    [Header("Skill Tree Settings")]
    public SkillTreeComponentSO root;

    [SerializedDictionary("Resource Type", "Value")]
    public SerializedDictionary<SkillResourceType, int> resources;

    void Awake()
    {
        // ���������� Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ��������� ������ � ������
        }
        else
        {
            Destroy(gameObject); // ���� ��������� ��� ����������, ���������� ����
        }

        // ���� ������ �� ����������, ������ �� ������
        if (root != null)
        {
            ClearTreeParents(root);
            // ������������� ������ � ��������� ������������ �����������
            InitializeTree(root, null);
            root.Unlock();
            root.Activate();
        }
    }
    // ����������� ����� ��� ������������� ������ � ����������� ������������ ������
    private void InitializeTree(SkillTreeComponentSO currentComponent, SkillTreeComponentSO parent)
    {
        // ���� ���� ��������, ��������� ��� � ������ ��������� �������� ����������
        if (parent != null)
        {
           
            currentComponent.AddParent(parent);
        }

        // ���� ������� ��������� � ��� ������, �������� �� �� �����
        if (currentComponent is SkillTreeGroupSO group)
        {
            foreach (SkillTreeComponentSO child in group.children)
            {
                // ���������� �������������� ������� �������, ��������� ������� ��������� ��� ��������
                InitializeTree(child, currentComponent);
            }
        }
    }
    // ����������� ����� ��� ������� ��������� ���� ����� ������
    private void ClearTreeParents(SkillTreeComponentSO component)
    {
        component.ClearParents();

        if (component is SkillTreeGroupSO group)
        {
            foreach (SkillTreeComponentSO child in group.children)
            {
                ClearTreeParents(child);
            }
        }
    }

    // ����� ��� ������������� ����������
    public void UnlockComponent(SkillTreeComponentSO component)
    {
        component.Unlock();
    }

    // TODO. ����� ��� ��������� �����������. 
    public bool ActivateSkill(SkillTreeComponentSO component, out string message)
    {
        message = string.Empty;
        if (!component.Activate(resources, out message))
        {
            //Instantiate error message
            return false;
        }
        return true;
    }
}

public enum SkillResourceType
{
    
}
