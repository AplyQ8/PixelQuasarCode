using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargoyleSound : EnemySound
{
    [SerializeField] protected AudioClip[] wingsSounds;
    [SerializeField] protected AudioClip[] dashSounds;
    [SerializeField] protected AudioClip[] smallDashSounds;
    [SerializeField] protected AudioClip[] fallingSounds;
    [SerializeField] protected AudioClip[] fallImpactSounds;
    [SerializeField] protected AudioClip[] healingSounds;
    [SerializeField] protected AudioClip[] healingGetHookedSounds;
    [SerializeField] protected AudioClip[] agroSounds;
    
    public void PlayWingsSound()
    {
        audioSources[1].PlayOneShot(SelectRandomClip(wingsSounds));
    }

    public void PlayDashSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(wingsSounds));
    }
    
    public void PlaySmallDashSound()
    {
        audioSources[2].Stop();
        audioSources[0].PlayOneShot(SelectRandomClip(smallDashSounds));
    }
    
    public void PlayFallingSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(fallingSounds));
    }
    
    public void PlayFallImpactSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(fallImpactSounds));
    }
    
    public void PlayHealingSound()
    {
        audioSources[2].PlayOneShot(SelectRandomClip(healingSounds));
    }
    
    public void PlayHealingGetHookedSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(healingGetHookedSounds));
    }
    
    public void PlayAgroSound()
    {
        audioSources[3].PlayOneShot(SelectRandomClip(agroSounds));
    }
    
}
