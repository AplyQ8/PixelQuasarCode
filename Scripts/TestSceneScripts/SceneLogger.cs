using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using ObjectLogicInterfaces;
using ObjectLogicRealization.Adrenaline;
using ObjectLogicRealization.Health;

public class SceneLogger : MonoBehaviour
{
    private string deathLogFilePath;

    private string hpLogFilePath;
    private float lostHpValue;
    private IHealth heroHp;

    private string timeLogFilePath;
    private float startTime;

    private string hookDistanceLogFilePath;
    
    private string averageAdrenalineLogFilePath;
    private long adrenalineNotZeroTime;
    private float adrenalineNotZeroTotalValue;
    private HeroAdrenaline heroAdrenalineScript;
    
    private string hookHitSuccessLogFilePath;

    private string hookAverageDmgLogFilePath;
    
    private string attackAverageDmgLogFilePath;
    
    void Start()
    {
        // Specify the path for the log file
        deathLogFilePath = Path.Combine(Application.persistentDataPath, "DeathLog.txt");
        hpLogFilePath = Path.Combine(Application.persistentDataPath, "LostHpLog.txt");
        timeLogFilePath = Path.Combine(Application.persistentDataPath, "TimeSpentLog.txt");
        hookDistanceLogFilePath = Path.Combine(Application.persistentDataPath, "HookHitDistanceLog.txt");
        averageAdrenalineLogFilePath = Path.Combine(Application.persistentDataPath, "AverageAdrenalineLog.txt");
        hookHitSuccessLogFilePath = Path.Combine(Application.persistentDataPath, "HookHitSuccessLog.txt");
        hookAverageDmgLogFilePath = Path.Combine(Application.persistentDataPath, "HookAvgDamageLog.txt");
        attackAverageDmgLogFilePath = Path.Combine(Application.persistentDataPath, "AttackAvgDamageLog.txt");
        
        CreateLogFiles();

        heroHp = GameObject.FindWithTag("Player").GetComponent<IHealth>();
        heroAdrenalineScript = GameObject.FindWithTag("Player").GetComponent<HeroAdrenaline>();
        
        startTime = Time.time;
        adrenalineNotZeroTime = 0;
        adrenalineNotZeroTotalValue = 0;

        StartCoroutine(AverageAdrenalineUpdateCoroutine());
    }

    private void CreateLogFiles()
    {
        string[] allPaths =
        {
            deathLogFilePath, hpLogFilePath, timeLogFilePath,
            hookDistanceLogFilePath, averageAdrenalineLogFilePath, hookHitSuccessLogFilePath,
            hookAverageDmgLogFilePath, attackAverageDmgLogFilePath
        };

        foreach (var logFilePath in allPaths)
        {
            if (!File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, "");
            }
        }
    }
    

    public void UpdateSceneData()
    {
        IncrementSceneDataByPath(deathLogFilePath, 0);
        IncrementSceneDataByPath(hpLogFilePath, heroHp.GetMaxHealthPoints() - heroHp.GetCurrentHealth());
        IncrementSceneDataByPath(timeLogFilePath, Time.time - startTime);
    }
    

    public void IncrementSceneDataByPath(string filePath, float incrementValue)
    {
        // Get the current scene name
        string sceneName = SceneManager.GetActiveScene().name;

        // Read all lines from the log file
        string[] lines = File.ReadAllLines(filePath);
        bool sceneFound = false;

        for (int i = 0; i < lines.Length; i++)
        {
            // Check if the line contains the current scene name
            if (lines[i].StartsWith(sceneName))
            {
                // Split the line to get the number
                string[] parts = lines[i].Split('-');
                if (parts.Length == 2)
                {
                    // Parse the current number and increment it
                    float currentNumber = float.Parse(parts[1].Trim());
                    currentNumber+= incrementValue;

                    // Update the line with the new number
                    lines[i] = $"{sceneName} - {currentNumber}";
                    sceneFound = true;
                }
                break;
            }
        }

        // If the scene was not found, you can choose to add it to the log
        if (!sceneFound)
        {
            lines = AddNewScene(lines, sceneName, incrementValue);
        }

        // Write all lines back to the log file
        File.WriteAllLines(filePath, lines);
    }
    
    

    private string[] AddNewScene(string[] lines, string sceneName, float value)
    {
        // Add a new entry for the scene with an initial value of 1
        string[] newLines = new string[lines.Length + 1];
        lines.CopyTo(newLines, 0);
        newLines[newLines.Length - 1] = $"{sceneName} - {value}";
        return newLines;
    }

    public void IncrementDeathNumber()
    {
        IncrementSceneDataByPath(deathLogFilePath, 1);
    }

    public void IncrementHookHitDistanceData(float distance)
    {
        string[] lines = File.ReadAllLines(hookDistanceLogFilePath);
        int attemptsNumber = 1;
        float totalDistance = distance;
        float averageDistance = distance/attemptsNumber;


        if (lines.Length >= 3)
        {
            string[] parts = lines[0].Split('-');
            attemptsNumber = int.Parse(parts[1].Trim()) + 1;
            
            parts = lines[1].Split('-');
            totalDistance = float.Parse(parts[1].Trim()) + distance;
            
            averageDistance = totalDistance/attemptsNumber;
        }
        
        string[] newLines = new string[3];
        newLines[0] = $"Attempts number - {attemptsNumber}";
        newLines[1] = $"Total distance - {totalDistance}";
        newLines[2] = $"Average distance - {averageDistance}";
        
        File.WriteAllLines(hookDistanceLogFilePath, newLines);
    }
    
    public void IncrementHookAvgDamageData(float damage)
    {
        string[] lines = File.ReadAllLines(hookAverageDmgLogFilePath);
        int attemptsNumber = 1;
        float totalDamage = damage;
        float averageDamage = damage/attemptsNumber;


        if (lines.Length >= 3)
        {
            string[] parts = lines[0].Split('-');
            attemptsNumber = int.Parse(parts[1].Trim()) + 1;
            
            parts = lines[1].Split('-');
            totalDamage = float.Parse(parts[1].Trim()) + damage;
            
            averageDamage = totalDamage/attemptsNumber;
        }
        
        string[] newLines = new string[3];
        newLines[0] = $"Attempts number - {attemptsNumber}";
        newLines[1] = $"Total damage - {totalDamage}";
        newLines[2] = $"Average damage - {averageDamage}";
        
        File.WriteAllLines(hookAverageDmgLogFilePath, newLines);
    }
    
    public void IncrementAttackAvgDamageData(float damage)
    {
        string[] lines = File.ReadAllLines(attackAverageDmgLogFilePath);
        int attemptsNumber = 1;
        float totalDamage = damage;
        float averageDamage = damage/attemptsNumber;


        if (lines.Length >= 3)
        {
            string[] parts = lines[0].Split('-');
            attemptsNumber = int.Parse(parts[1].Trim()) + 1;
            
            parts = lines[1].Split('-');
            totalDamage = float.Parse(parts[1].Trim()) + damage;
            
            averageDamage = totalDamage/attemptsNumber;
        }
        
        string[] newLines = new string[3];
        newLines[0] = $"Attempts number - {attemptsNumber}";
        newLines[1] = $"Total damage - {totalDamage}";
        newLines[2] = $"Average damage - {averageDamage}";
        
        File.WriteAllLines(attackAverageDmgLogFilePath, newLines);
    }
    
    private IEnumerator AverageAdrenalineUpdateCoroutine()
    {
        while (true)
        {
            float heroCurrentAdrenaline = heroAdrenalineScript.CurrentAdrenalineValue;
            if (heroCurrentAdrenaline > 0.0001f)
            {
                adrenalineNotZeroTime++;
                adrenalineNotZeroTotalValue += heroCurrentAdrenaline;
            }
            UpdateAverageAdrenalineData();
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public void UpdateAverageAdrenalineData()
    {
        string[] lines = File.ReadAllLines(averageAdrenalineLogFilePath);
        long notZeroTotalTime = 0;
        float notZeroTotalValue = 0;
        float averageValue = 0;


        if (lines.Length >= 3)
        {
            string[] parts = lines[0].Split('-');
            notZeroTotalTime = long.Parse(parts[1].Trim()) + adrenalineNotZeroTime;
            
            parts = lines[1].Split('-');
            notZeroTotalValue = float.Parse(parts[1].Trim()) + adrenalineNotZeroTotalValue;
            
            averageValue = notZeroTotalValue/notZeroTotalTime;
        }
        
        string[] newLines = new string[3];
        newLines[0] = $"Not zero time intervals - {notZeroTotalTime}";
        newLines[1] = $"Total not zero value - {notZeroTotalValue}";
        newLines[2] = $"Average value - {averageValue}";
        
        File.WriteAllLines(averageAdrenalineLogFilePath, newLines);
    }
    
    
    public void UpdateHookHitSuccessData(bool hookThrow, bool hookHit)
    {
        string[] lines = File.ReadAllLines(hookHitSuccessLogFilePath);
        int attemptsNumber = 0;
        int totalSuccess = 0;
        float successRate = 0;
        if (hookThrow)
            attemptsNumber = 1;
        if (hookHit)
            totalSuccess = 1;


        if (lines.Length >= 3)
        {
            string[] parts = lines[0].Split('-');
            attemptsNumber += int.Parse(parts[1].Trim());
            
            parts = lines[1].Split('-');
            totalSuccess += int.Parse(parts[1].Trim());
            
            successRate = (float)totalSuccess/attemptsNumber;
        }
        
        string[] newLines = new string[3];
        newLines[0] = $"Attempts number - {attemptsNumber}";
        newLines[1] = $"Hit number - {totalSuccess}";
        newLines[2] = $"Success rate - {successRate}";
        
        File.WriteAllLines(hookHitSuccessLogFilePath, newLines);
    }
    
}