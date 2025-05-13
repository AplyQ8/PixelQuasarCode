using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIComponentInitializer : MonoBehaviour
{
    [SerializeField] private List<Canvas> uiComponents;

    private void Awake()
    {
        
        foreach (var uiComponent in uiComponents)
        {
            Instantiate(uiComponent, gameObject.transform, true);
        }
        //DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var initializers = FindObjectsOfType<UIComponentInitializer>();
        if (initializers.Length > 0)
        {
            foreach (var initializer in initializers)
            {
                Destroy(initializer);
            }
        }
    }
    
}
