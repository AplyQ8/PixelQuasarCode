using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelInitializer : MonoBehaviour
{
    [SerializeField] private EffectHandler effectHandler;
    [SerializeField] private UIEffectPanel effectPanelPrefab;
    [SerializeField] private UIEffectPanel currentEffectPanel;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (currentEffectPanel is not null)
            return;
        var panelsOnScene = FindObjectsOfType<UIEffectPanel>();
        if (panelsOnScene.Length is 0)
        {
            currentEffectPanel = Instantiate(effectPanelPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            currentEffectPanel = panelsOnScene[0];
            for (int i = 1; i < panelsOnScene.Length; i++)
            {
                Destroy(panelsOnScene[i]);
            }
        }
        currentEffectPanel.InitializePanel(effectHandler);
    }
}
