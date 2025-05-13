using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DistanceActionProbability
{
    public float minDistance; // Minimum distance for this range
    public float maxDistance; // Maximum distance for this range (inclusive)
    public List<ActionProbability> actions; // Actions and their probabilities in this range
}

[System.Serializable]
public class ActionProbability
{
    public string actionName; // Name of the action
    public float probability; // Probability of this action
}

[CreateAssetMenu(fileName = "ActionSelector", menuName = "AI/Action Selector", order = 1)]
public class ActionSelector : ScriptableObject
{
    [Header("Distance-based Action Probabilities")]
    public List<DistanceActionProbability> distanceActionProbabilities;

    /// <summary>
    /// Gets a random action name based on the distance to the player.
    /// </summary>
    /// <param name="distanceToPlayer">The current distance to the player</param>
    /// <returns>A chosen action name</returns>
    public string GetAction(float distanceToPlayer)
    {
        // Find the distance range that applies
        foreach (var range in distanceActionProbabilities)
        {
            if (distanceToPlayer >= range.minDistance && distanceToPlayer <= range.maxDistance)
            {
                return ChooseActionFromProbabilities(range.actions);
            }
        }

        // Default action if no range matches
        Debug.LogWarning("No matching distance range found! Returning default action.");
        return "DefaultAction";
    }

    /// <summary>
    /// Chooses an action based on the provided probabilities.
    /// </summary>
    /// <param name="actions">List of actions with their probabilities</param>
    /// <returns>A chosen action name</returns>
    private string ChooseActionFromProbabilities(List<ActionProbability> actions)
    {
        float totalProbability = 0f;

        // Calculate total probability
        foreach (var action in actions)
        {
            totalProbability += action.probability;
        }

        // Generate a random value between 0 and the total probability
        float randomValue = Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        // Determine which action corresponds to the random value
        foreach (var action in actions)
        {
            cumulativeProbability += action.probability;
            if (randomValue <= cumulativeProbability)
            {
                return action.actionName;
            }
        }

        // Fallback in case no action is selected (should not occur)
        Debug.LogError("No action selected! Check probabilities.");
        return "DefaultAction";
    }
}
