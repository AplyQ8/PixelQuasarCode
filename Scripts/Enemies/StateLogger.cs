using UnityEngine;

public class StateLogger : StateMachineBehaviour
{
    // Called when the state starts
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"Entered State: {GetStateName(stateInfo)} on layer {layerIndex}");
    }

    // Called when the state exits
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"Exited State: {GetStateName(stateInfo)} on layer {layerIndex}");
    }

    private string GetStateName(AnimatorStateInfo stateInfo)
    {
        string[] stateNames = { "AgrIdle", "AgrMove", "Stun", "Get Damage", "Attack" };
        foreach (var stateName in stateNames)
        {
            if (stateInfo.IsName(stateName))
                return stateName;
        }

        return "Bruh";
    }
}