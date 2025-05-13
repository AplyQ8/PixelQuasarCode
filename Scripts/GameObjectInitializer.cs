using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameObjectInitializer : MonoBehaviour
{
    [SerializeField] private List<GameObject> necessaryGameObjects;
    
    private void Awake()
    {
        
        foreach (var necessaryGameObject in necessaryGameObjects)
        {
            Instantiate(necessaryGameObject, gameObject.transform, true);
        }
        //DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var initializers = FindObjectsOfType<GameObjectInitializer>();
        if (initializers.Length > 0)
        {
            foreach (var initializer in initializers)
            {
                Destroy(initializer);
            }
        }
    }
}
