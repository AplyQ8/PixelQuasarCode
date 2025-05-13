using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicStartEvent : TriggerableEvent
{
    [SerializeField] private MusicManager musicManager;
    public override void StartEvent()
    {
        musicManager.FadeAndStartEndOfAllPaths();
        enabled = false;
    }
    
    
}
