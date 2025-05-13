using System.Collections;
using System.Collections.Generic;
using ObjectLogicRealization.Health;
using UnityEngine;

public class SaveAll : MonoBehaviour
{
    // Start is called before the first frame update
    private const string SaveKeyPrefix = "Pudge_Component_";
    //public GameObject pudge;
    //public GameObject battles;

    public List<GameObject> allEnemies; // List to store all enemies in the scene

    void Start()
    {
        UpdateEnemyList(); // Populate the list at the start
    }

    // Method to update the list of enemies
    public void UpdateEnemyList()
    {
        // Find all objects with the "Enemy" tag and populate the list
        allEnemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //SaveAllComponentsOfPudge();
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            //LoadAllComponentsOfPudge();
            LoadGame();
        }
    }

    public void SaveGame()
    {
        // Save all components and destroyed enemies list
        ES3AutoSaveMgr.Current.Save();
        ES3.Save("destroyedEnemyIDs", EnemyManager.destroyedEnemyIDs);
    }

    public void LoadGame()
    {
        // Load destroyed enemies list
        if (ES3.KeyExists("destroyedEnemyIDs"))
        {
            EnemyManager.destroyedEnemyIDs = ES3.Load<List<string>>("destroyedEnemyIDs");
        }
        // Destroy enemies marked as destroyed
        foreach (GameObject enemy in allEnemies)
        {
            if (enemy != null)
            {
                string id = enemy.GetComponent<UniqueId>().UniqueID;
                if (EnemyManager.destroyedEnemyIDs.Contains(id))
                {
                    Destroy(enemy);
                }
            }
        }

        // Load other components
        ES3AutoSaveMgr.Current.Load();
        ES3AutoSaveMgr.Current.Load();
    }

    //void SaveAllComponentsOfPudge()
    //{
    //    if (pudge != null)
    //    {
    //        // Iterate through all components on the Pudge GameObject
    //        var components = pudge.GetComponents<Component>();
    //        foreach (var component in components)
    //        {
    //            if (component != null && component != pudge.GetComponent<HeroStateHandler>())
    //            {
    //                string key = SaveKeyPrefix + component.GetType().Name;
    //                ES3.Save(key, component);
    //                Debug.Log($"Saved component: {component.GetType().Name}");
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("Pudge not found!");
    //    }
    //    GameObject ui = GameObject.Find("HeroStatsUI(Clone)");
    //    if (ui != null)
    //    {
    //        ES3.Save("ui", ui);
    //    }
    //    saveBattles();
    //}

    //void LoadAllComponentsOfPudge()
    //{
    //    if (pudge != null)
    //    {
    //        var components = pudge.GetComponents<Component>();
    //        foreach (var component in components)
    //        {
    //            if (component != null && component != pudge.GetComponent<HeroStateHandler>())
    //            {
    //                string key = SaveKeyPrefix + component.GetType().Name;

    //                // Deserialize the component if it exists in the save file
    //                if (ES3.KeyExists(key))
    //                {
    //                    ES3.LoadInto(key, component);
    //                }
    //            }
    //        }
    //    }
    //    GameObject ui = GameObject.Find("HeroStatsUI(Clone)");
    //    if (ui != null)
    //    {
    //        ES3.LoadInto("ui", ui);
    //    }
    //    loadBattles();
    //}

    //private void saveBattles()
    //{
    //    var components = battles.GetComponents<Component>();
    //    string name = "battle_";
    //    foreach (var component in components)
    //    {
    //        if (component != null)
    //        {
    //            ES3.Save(name+component.GetType().Name, component);
    //        }
    //    }
    //}

    //private void loadBattles()
    //{
    //    if (battles != null)
    //    {
    //        var components = battles.GetComponents<Component>();
    //        string name = "battle_";
    //        foreach (var component in components)
    //        {
    //            if (component != null)
    //            {
    //                string key = name + component.GetType().Name;
    //                if (ES3.KeyExists(key))
    //                {
    //                    ES3.LoadInto(key, component);
    //                }
    //            }
    //        }
    //    }
    //}
}
