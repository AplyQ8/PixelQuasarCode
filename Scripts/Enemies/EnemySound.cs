using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySound : MonoBehaviour
{
    [SerializeField] protected AudioClip[] getHookedSounds;
    [SerializeField] protected AudioClip[] getDmgSounds;
    [SerializeField] protected AudioClip[] regularSounds;
    [SerializeField] protected AudioClip[] deathSounds;
    
    [SerializeField] protected AudioSource[] audioSources;
    void Start()
    {
        
    }
    public void PlayRegularSound()
    {
        audioSources[1].PlayOneShot(SelectRandomClip(regularSounds));
    }
    public void PlayGetHookedSound()
    {
        audioSources[1].Stop();
        audioSources[0].PlayOneShot(SelectRandomClip(getHookedSounds));
    }
    
    public void PlayGetDmgSound()
    {
        audioSources[1].Stop();
        audioSources[0].PlayOneShot(SelectRandomClip(getDmgSounds));
    }

    public void PlayDeathSound()
    {
        audioSources[1].Stop();
        // audioSources[0].PlayOneShot(SelectRandomClip(deathSounds));
        
        PlaySoundInPoint(deathSounds, audioSources[0], transform.position);
    }

    protected void PlaySoundInPoint(AudioClip[] sounds, AudioSource source, Vector3 position)
    {
        GameObject audioObject = new GameObject("TempAudioObject");
        audioObject.transform.position = position;
        AudioSource newAudioSource = audioObject.AddComponent<AudioSource>();

        newAudioSource.volume = source.volume;
        newAudioSource.spatialBlend = source.spatialBlend;
        newAudioSource.rolloffMode = source.rolloffMode;
        newAudioSource.minDistance = source.minDistance;
        newAudioSource.maxDistance = source.maxDistance;
            
        TempAudioObject tempAudioObject = audioObject.AddComponent<TempAudioObject>();

        AudioClip sound = SelectRandomClip(sounds);
        newAudioSource.PlayOneShot(sound);
        tempAudioObject.DestroyAfterSeconds(sound.length);
    }

    public void StopAllSound()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.Stop();
        }
    }
    
    
    protected AudioClip SelectRandomClip(AudioClip[] clips)
    {
        if (clips.Length > 0)
        {
            int randomIndex = Random.Range(0, clips.Length);
            AudioClip selectedClip = clips[randomIndex];

            return selectedClip;
        }

        return null;
    }
}
