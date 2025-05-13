using System;
using ObjectLogicInterfaces;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Main_hero
{
    public class HeroStatUIReferences : MonoBehaviour
    {
        
        [Header("HeroUIFacade")]
        [SerializeField] private HeroUIFacade heroUI;
        [SerializeField] private HeroUIFacade currentHeroUIInstance;
        [field: SerializeField] public HealthBar HealthBar { get; private set; }
        [field: SerializeField] public BloodBar BloodBar { get; private set; }
        [field: SerializeField] public AdrenalineBar AdrenalineBar { get; private set; }
        [field: SerializeField] public AbilityUIPanel AbilityUIPanel { get; private set; }

        private void Awake()
        {
            LinkToHeroScripts();
            GetUIInstance();
            GetUIReferences();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GetUIInstance();
            GetUIReferences();
            GetCurrentStatState();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void GetUIInstance()
        {
            if (currentHeroUIInstance != null)
                return;
            var uiPanelsOnScene = FindObjectsOfType<HeroUIFacade>();
            
            if (uiPanelsOnScene.Length <= 0)
            {
                currentHeroUIInstance = Instantiate(heroUI, transform.position, Quaternion.identity);
            }
            else
            {
                currentHeroUIInstance = uiPanelsOnScene[0];
                for (int i = 1; i < uiPanelsOnScene.Length; i++)
                {
                    Destroy(uiPanelsOnScene[i]);
                }
            }
        }
        private void GetUIReferences()
        {
            HealthBar = currentHeroUIInstance.HealthBar;
            BloodBar = currentHeroUIInstance.BloodBar;
            AdrenalineBar = currentHeroUIInstance.AdrenalineBar;
            AbilityUIPanel = currentHeroUIInstance.AbilityUIPanel;
        }

        private void LinkToHeroScripts()
        {
            gameObject.GetComponent<IDamageable>().OnHealthChange += OnHealthChange;
            gameObject.GetComponent<IDamageable>().OnHealthBoundariesChange += OnHealthBoundaryChange;

            gameObject.GetComponent<IBloodContent>().OnBloodValueChangeEvent += OnBloodValueChange;
            gameObject.GetComponent<IBloodContent>().OnBloodBoundaryChange += OnBloodBoundaryChange;

            gameObject.GetComponent<IAdrenalineContent>().OnAdrenalineValueChange += OnAdrenalineValueChange;
            gameObject.GetComponent<IAdrenalineContent>().OnAdrenalineBoundaryChange += OnAdrenalineBoundaryChange;
        }

        private void GetCurrentStatState()
        {
            var healthScript = gameObject.GetComponent<IHealth>();
            var bloodScript = gameObject.GetComponent<IBloodContent>();
            var adrenalineScript = gameObject.GetComponent<IAdrenalineContent>();
            
            HealthBar.SetMaxHealth(healthScript.GetMaxHealthPoints());
            HealthBar.SetHealth(healthScript.GetCurrentHealth());
            
            BloodBar.SetMaxBlood(bloodScript.GetMaxBloodValue());
            BloodBar.SetBlood(bloodScript.GetCurrentBloodValue());
            
            AdrenalineBar.SetMaxAdrenalineValue(adrenalineScript.GetMaxAdrenalineBoundary());
            AdrenalineBar.SetAdrenaline(adrenalineScript.GetCurrentAdrenalineValue());
        }

        #region Event Methods
        private void OnHealthChange(float value)
        {
            HealthBar.SetHealth(value);
        }

        private void OnHealthBoundaryChange(float value)
        {
            HealthBar.SetMaxHealth(value);
        }

        private void OnBloodValueChange(float value)
        {
            BloodBar.SetBlood(value);
        }

        private void OnBloodBoundaryChange(float value)
        {
            BloodBar.SetMaxBlood(value);
        }

        private void OnAdrenalineValueChange(float value)
        {
            AdrenalineBar.SetAdrenaline(value);
        }

        private void OnAdrenalineBoundaryChange(float value)
        {
            AdrenalineBar.SetMaxAdrenalineValue(value);
        }
        #endregion
    }
}