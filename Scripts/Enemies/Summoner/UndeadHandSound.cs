using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndeadHandSound : EnemySound
{
    [SerializeField] protected AudioClip[] appearanceSounds;
    [SerializeField] protected AudioClip[] throwSounds;
    
    public void PlayAppearanceSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(appearanceSounds));
    }
    
    public void PlayThrowSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(throwSounds));
    }
}
