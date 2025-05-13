using System.Collections;
using System.Collections.Generic;
using ObjectLogicRealization.Health;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    [SerializeField] private HealthEvasionableResistible hero;
    [SerializeField] private bool heroCanDie;
    [SerializeField] private TMP_Text heroCanDieIndicator;

    private SceneLogger sceneLogger;
    
    private void Awake()
    {
        heroCanDie = true;
        heroCanDieIndicator = GameObject.Find("HeroCanDieText").GetComponent<TMP_Text>();
        ChangeStringDeathString(heroCanDie);
        hero = GameObject.FindWithTag("Player").GetComponent<HealthEvasionableResistible>();
        hero.OnDeathEvent += OnHeroDeathEvent;

        sceneLogger = GameObject.Find("SceneLogger").GetComponent<SceneLogger>();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartCurrentScene();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadNextSceneInBuild();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadPreviousSceneInBuild();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            HeroDeathCheck();
        }
        
    }

    private void LoadPreviousSceneInBuild()
    {
        sceneLogger.UpdateSceneData();
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int previousSceneIndex = (currentSceneIndex - 1 + SceneManager.sceneCountInBuildSettings) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(previousSceneIndex);
    }

    private void OnHeroDeathEvent()
    {
        if (!heroCanDie)
            return;
        
        sceneLogger.IncrementDeathNumber();
        
        RestartCurrentScene();
    }
    
    private void RestartCurrentScene()
    {
        sceneLogger.UpdateSceneData();
        
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadNextSceneInBuild()
    {
        sceneLogger.UpdateSceneData();
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void HeroDeathCheck()
    {
        heroCanDie = !heroCanDie;
        ChangeStringDeathString(heroCanDie);
        
    }

    private void ChangeStringDeathString(bool hcd)
    {
        var message = "(F5) hero can die: ";
        if (hcd)
        {
            message += $"<color=green>true</color>";
        }
        else
        {
            message += "<color=red>false</color>";
        }

        
        heroCanDieIndicator.text = message;
    }

}
