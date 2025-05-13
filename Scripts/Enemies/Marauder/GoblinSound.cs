using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class GoblinSound : EnemySound
{
    [SerializeField] private AudioClip[] attackPrepSounds;
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] dealDmdSounds;

    [SerializeField] float regularSoundCooldown;
    private float regularSoundTimer;
    
    [Header("Steps")]
    [SerializeField] private SoundsPack stepsPack;
    private Dictionary<string, AudioClip[]> stepSounds;
    private int lastStepClipIndex;
    
    private SurfaceRecognizer surfaceRecognizer;
    
    void Start()
    {
        stepSounds = stepsPack.GetAllSounds();
        lastStepClipIndex = -1;
        surfaceRecognizer = transform.parent.GetComponentInChildren<SurfaceRecognizer>();
        
        StartCoroutine(RegularSoundTimer());
    }
    
    public void PlayAttackPrepSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(attackPrepSounds));
    }
    public void PlayAttackSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(attackSounds));
    }

    public void PlayDealDmgSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(dealDmdSounds));
    }
    
    public void PlayStepsSound()
    {
        SurfaceType currentSurface = surfaceRecognizer.GetCurrentTileSurface();
        audioSources[2].Stop();
        AudioClip stepClip = SelectRandomStepsClip(GetSurfaceStepSounds(currentSurface));
        if(stepClip)
            audioSources[2].PlayOneShot(stepClip);
    }

    IEnumerator RegularSoundTimer()
    {
        for (;;)
        {
            regularSoundTimer = Random.Range(20, 100)/100f * regularSoundCooldown;
            yield return new WaitForSeconds(regularSoundTimer);
            PlayRegularSound();
        }
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
