using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu]
public class SoundsPack : ScriptableObject
{
    [SerializedDictionary("Sounds Name", "Sounds")]
    public SerializedDictionary<string, AudioClip[]> sounds;

    public Dictionary<string, AudioClip[]> GetAllSounds() => sounds;
}
