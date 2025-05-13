using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeVisualizer : MonoBehaviour
{
    public static SkillTreeVisualizer Instance { get; private set; }

    [Header("UI Prefabs")]
    [SerializeField] private GameObject skillNodePrefab; // ������ ��� ����������� ���� ������ (��������, ������)
    [SerializeField] private RectTransform treeContainer; // ��������� ��� ����� ������
    [SerializeField] private GameObject linePrefab; // ������ ��� ����������� ����� ����� ������ (��������, LineRenderer)
    [SerializeField] private Vector2 rootPosition;

    public Color activatedColor = Color.green; // ���� ��� ��������������� ����
    public Color unlockColor = Color.yellow;
    public Color defaultColor = Color.white;   // ���� �� ���������

    // ������� � �������
    [SerializeField] private float horizontalSpacing = 100f;
    [SerializeField] private float verticalSpacing = 100f;

    private Dictionary<SkillTreeComponentSO, GameObject> nodeInstances = new Dictionary<SkillTreeComponentSO, GameObject>();
    private Dictionary<SkillTreeComponentSO, Vector2> nodePositions = new Dictionary<SkillTreeComponentSO, Vector2>();

    void Start()
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
        gameObject.SetActive(false);

    }

    public void ShowSkillTree()
    {
        gameObject.SetActive(true);
        if(SkillTreeManager.Instance.root != null)
        {
            DrawTree(SkillTreeManager.Instance.root, rootPosition);
        }
    }

    // ����������� ����� ��� ��������� ������
    private void DrawTree(SkillTreeComponentSO currentNode, Vector2 position)
    {
        
        // ���������, ��� �� ��� ��������� ���� ����
        if (nodeInstances.ContainsKey(currentNode))
        {
            return;
        }

        // ������� ����
        GameObject node = Instantiate(skillNodePrefab, treeContainer);
        RectTransform nodeRect = node.GetComponent<RectTransform>();
        nodeRect.anchoredPosition = position;

        if (currentNode.IsUnlocked)
        {
            node.GetComponent<Image>().color = unlockColor;
        }

        if (currentNode.IsActivated)
        {
            node.GetComponent<Image>().color = activatedColor;
        }

        // ����������� ������� �� ������ ��������� ����
        Button button = node.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => ActivateSkill(currentNode));
        }

        nodeInstances[currentNode] = node; // ��������� ������ �� ������������ ����
        nodePositions[currentNode] = position; // ��������� �������� ������� ����

        // ���� � ���� ���� ����, ������ ��
        if (currentNode is SkillTreeGroupSO group)
        {
            int childCount = group.children.Count;
            float verticalSpacing = 100f; // ���������� ����� ������ �� ���������
            float horizontalSpacing = 150f; // ���������� ����� ������ �� �����������

            Vector2 startPosition = new Vector2(position.x + horizontalSpacing, position.y);

            for (int i = 0; i < childCount; i++)
            {
                SkillTreeComponentSO child = group.children[i];
                Vector2 childPosition = startPosition + new Vector2(0, -i * verticalSpacing);

                // ���������� ������ �����
                DrawTree(child, childPosition);

                // ������ ����� � ������� �������
                if (nodePositions.TryGetValue(child, out Vector2 actualChildPosition))
                {
                    DrawLine(position, actualChildPosition);
                }
            }
        }
    }

    // ����� ��� ��������� ������
    private void ActivateSkill(SkillTreeComponentSO node)
    {
        string message = string.Empty;
        bool isActivated = SkillTreeManager.Instance.ActivateSkill(node, out message);

        if (nodeInstances.TryGetValue(node, out GameObject nodeObject))
        {
            Image nodeImage = nodeObject.GetComponent<Image>();
            if (nodeImage != null)
            {
                nodeImage.color = isActivated ? activatedColor : defaultColor;
            }
        }

       
        if (!isActivated)
            return;

        UnlockNodes(node);


    }

    // ����� ��� ��������� ����� ����� ����� ������
    private void DrawLine(Vector2 start, Vector2 end)
    {
        GameObject line = Instantiate(linePrefab, treeContainer);
        RectTransform lineRect = line.GetComponent<RectTransform>();

        // ���������� ������ ����������� � �����
        Vector2 direction = end - start;
        float distance = direction.magnitude;

        // ������������� ������� � ������ �����
        lineRect.anchoredPosition = start + direction / 2;
        lineRect.sizeDelta = new Vector2(distance, 5f); // ������ �����
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        lineRect.rotation = Quaternion.Euler(0, 0, angle);

        // ������ ����� ������� (���� ��� �����)
        line.SetActive(true);
    }

    private void UnlockNodes(SkillTreeComponentSO node)
    {
        if (node is SkillTreeGroupSO group)
        {
            int childCount = group.children.Count;
            for (int i = 0; i < childCount; i++)
            {
                SkillTreeComponentSO child = group.children[i];
                if (!child.IsUnlocked)
                    continue;
                nodeInstances[child].GetComponent<Image>().color = unlockColor;
            }
        }    
    }
}
