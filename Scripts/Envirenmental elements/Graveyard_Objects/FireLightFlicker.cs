using UnityEngine;
using UnityEngine.Rendering.Universal;  // Required for Light2D
using System.Collections;

public class FireLightFlicker : MonoBehaviour
{
    private Light2D light2D;

    [Header("Flicker Settings")]
    [Tooltip("Base intensity of the light.")]
    public float baseIntensity = 1f;
    [Tooltip("Maximum intensity variation.")]
    public float intensityVariation = 0.3f;
    [Tooltip("How often to change the intensity (in seconds).")]
    public float flickerIntervalMin = 0.05f;
    public float flickerIntervalMax = 0.2f; 
    [Tooltip("Speed of the flickering effect.")]
    public float flickerSpeed = 5f;

    private void Start()
    {
        // Get the Light2D component
        light2D = GetComponent<Light2D>();
        if (light2D == null)
        {
            Debug.LogError("Light2D component not found! Make sure this script is attached to the Light2D object.");
            return;
        }
        
        // Start the flicker coroutine
        StartCoroutine(FlickerLight());
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            // Apply flicker effect by adjusting intensity randomly
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
            light2D.intensity = baseIntensity + (noise - 0.5f) * intensityVariation;

            // Calculate a random interval for the next flicker
            float randomInterval = Random.Range(flickerIntervalMin, flickerIntervalMax);

            // Wait for the next flicker update
            yield return new WaitForSeconds(randomInterval);
        }
    }
}