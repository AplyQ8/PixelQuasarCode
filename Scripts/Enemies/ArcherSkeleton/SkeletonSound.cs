using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSound : EnemySound
{
    [SerializeField] protected AudioClip[] bowLoadSounds;
    [SerializeField] protected AudioClip[] bowSwingSounds;
    
    [Header("Steps")]
    [SerializeField] private SoundsPack stepsPack;
    private Dictionary<string, AudioClip[]> stepSounds;
    private int lastStepClipIndex;
    [SerializeField] protected AudioClip[] stepBonesCrackSounds;
    
    
    private SurfaceRecognizer surfaceRecognizer;
    
    void Start()
    {
        stepSounds = stepsPack.GetAllSounds();
        lastStepClipIndex = -1;
        surfaceRecognizer = transform.parent.GetComponentInChildren<SurfaceRecognizer>();
    }
    
    public void PlayBowLoadSound()
    {
        audioSources[2].PlayOneShot(SelectRandomClip(bowLoadSounds));
    }
    
    public void StopBowLoadSound()
    {
        audioSources[2].Stop();
    }
    
    public void PlayBowSwingSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(bowSwingSounds));
    }

    private void PlayBonesCrackSound()
    {
        audioSources[4].Stop();
        audioSources[4].PlayOneShot(SelectRandomClip(stepBonesCrackSounds));
    }
    
    
    public void PlayStepsSound()
    {
        SurfaceType currentSurface = surfaceRecognizer.GetCurrentTileSurface();
        audioSources[3].Stop();
        AudioClip stepClip = SelectRandomStepsClip(GetSurfaceStepSounds(currentSurface));
        if(stepClip)
            audioSources[3].PlayOneShot(stepClip);
        PlayBonesCrackSound();
    }
    
    private AudioClip[] GetSurfaceStepSounds(SurfaceType surface)
    {
        string surfaceName = SurfaceUtility.SurfaceTypeToString(surface);
        return stepSounds[surfaceName];
    }
    
    private AudioClip SelectRandomStepsClip(AudioClip[] clips)
    {
        if (clips.Length > 1)
        {
            int randomIndex = Random.Range(0, clips.Length);
            while (randomIndex == lastStepClipIndex)
            {
                randomIndex = Random.Range(0, clips.Length);
            }

            lastStepClipIndex = randomIndex;
            AudioClip selectedClip = clips[randomIndex];
            return selectedClip;
        }
        
        if (clips.Length == 1)
        {
            return clips[0];
        }

        return null;
    }
}
