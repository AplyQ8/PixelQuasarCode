using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [Header("Global Speed Multiplier")]
    [Range(0.1f, 10f)] // Allow speedMultiplier to be adjustable in a reasonable range
    public float speedMultiplier = 1.2f;

    private static GameSettings _instance;

    public static GameSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                // Create a new GameObject to hold the GameSettings instance
                GameObject settingsObject = new GameObject("GameSettings");
                _instance = settingsObject.AddComponent<GameSettings>();

                // Make sure it persists across scenes
                DontDestroyOnLoad(settingsObject);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Ensure only one instance exists
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}