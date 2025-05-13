using UnityEngine;
using System;
using System.Collections.Generic;
using ES3Types;

public class UniqueId : MonoBehaviour
{
    [SerializeField]
    private string uniqueId; // Serialized to persist across scene reloads

    public string UniqueID => uniqueId; // Expose the ID as a read-only property

    // Dictionary to track IDs for uniqueness (Editor-only)
#if UNITY_EDITOR
    public static readonly Dictionary<string, UniqueId> allGuids = new Dictionary<string, UniqueId>();
#endif

    private void Awake()
    {
        // Generate a new ID if it's missing (e.g., new object in a build)
        if (string.IsNullOrEmpty(uniqueId))
        {
            GenerateId();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Ensure ID uniqueness in the Editor
        EnsureUniqueId();
    }

    private void Reset()
    {
        EnsureUniqueId();
    }

    private void EnsureUniqueId()
    {
        string sceneName = gameObject.scene.name + "_";
        bool hasSceneNameAtBeginning = !string.IsNullOrEmpty(uniqueId) &&
                                       uniqueId.StartsWith(sceneName);

        bool isDuplicate = !string.IsNullOrEmpty(uniqueId) &&
                           allGuids.ContainsKey(uniqueId) &&
                           allGuids[uniqueId] != this;
        if (!hasSceneNameAtBeginning || isDuplicate)
        {
            GenerateId(sceneName);
            UnityEditor.EditorUtility.SetDirty(this);
        }

        if (!allGuids.ContainsKey(uniqueId))
        {
            allGuids[uniqueId] = this;
        }
    }
#endif

    private void GenerateId(string scenePrefix = "")
    {
        uniqueId = scenePrefix + Guid.NewGuid().ToString();
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        if (allGuids.ContainsKey(uniqueId))
        {
            allGuids.Remove(uniqueId);
        }
#endif
    }
}
