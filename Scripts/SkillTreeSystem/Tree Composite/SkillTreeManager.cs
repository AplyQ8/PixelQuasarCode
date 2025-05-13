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
        // Реализация Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Оставляем объект в сценах
        }
        else
        {
            Destroy(gameObject); // Если экземпляр уже существует, уничтожаем этот
        }

        // Если корень не установлен, ничего не делаем
        if (root != null)
        {
            ClearTreeParents(root);
            // Инициализация дерева — добавляем родительские зависимости
            InitializeTree(root, null);
            root.Unlock();
            root.Activate();
        }
    }
    // Рекурсивный метод для инициализации дерева с добавлением родительских ссылок
    private void InitializeTree(SkillTreeComponentSO currentComponent, SkillTreeComponentSO parent)
    {
        // Если есть родитель, добавляем его в список родителей текущего компонента
        if (parent != null)
        {
           
            currentComponent.AddParent(parent);
        }

        // Если текущий компонент — это группа, проходим по ее детям
        if (currentComponent is SkillTreeGroupSO group)
        {
            foreach (SkillTreeComponentSO child in group.children)
            {
                // Рекурсивно инициализируем каждого ребенка, передавая текущий компонент как родителя
                InitializeTree(child, currentComponent);
            }
        }
    }
    // Рекурсивный метод для очистки родителей всех узлов дерева
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

    // Метод для разблокировки компонента
    public void UnlockComponent(SkillTreeComponentSO component)
    {
        component.Unlock();
    }

    // TODO. Метод для активации способности. 
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
