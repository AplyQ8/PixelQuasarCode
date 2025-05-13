using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] attackSwingSounds;
    [SerializeField] private AudioClip[] dashSounds;
    [Header("Hook/Chain")]
    [SerializeField] private AudioClip[] throwChainSounds;
    [SerializeField] public AudioClip[] returnChainSounds;
    [SerializeField] public AudioClip[] swingSounds;
    [SerializeField] public AudioClip[] chainEndSounds;
    
    [Header("Hit Obstacle")]
    [SerializeField] public AudioClip[] hitObstacleDefaultSounds;
    [SerializeField] public AudioClip[] hitObstacleTreeSounds;
    
    [Header("Steps")]
    [SerializeField] private SoundsPack stepsPack;
    private Dictionary<string, AudioClip[]> stepSounds;
    private int lastStepClipIndex;
    
    [Header("AudioSources")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSource2;
    [SerializeField] private AudioSource audioSource3;
    [SerializeField] private AudioSource audioSource4;
    [SerializeField] private AudioSource audioSource5;

    private SurfaceRecognizer surfaceRecognizer;

    void Start()
    {
        stepSounds = stepsPack.GetAllSounds();
        surfaceRecognizer = transform.parent.GetComponentInChildren<SurfaceRecognizer>();
        lastStepClipIndex = -1;
    }

    public void PlayThrowChainSound()
    {
        audioSource.PlayOneShot(SelectRandomClip(throwChainSounds));
    }
    
    public void PlayReturnChainSound()
    {
        audioSource2.PlayOneShot(SelectRandomClip(swingSounds));
        
        audioSource.PlayOneShot(SelectRandomClip(returnChainSounds));
    }
    
    public void StopChainSound()
    {
        audioSource.Stop();
    }

    public void PlayChainEndSound()
    {
        audioSource3.PlayOneShot(SelectRandomClip(chainEndSounds));
    }

    public void PlayHitObstacleSound(ObstacleType obstacleType=ObstacleType.Default)
    {
        audioSource4.PlayOneShot(SelectRandomClip(GetObstacleHitSounds(obstacleType)));
    }
    
    public void PlayAttackSound()
    {
        audioSource.PlayOneShot(SelectRandomClip(attackSwingSounds));
    }
    
    public void PlayDashSound()
    {
        audioSource.PlayOneShot(SelectRandomClip(dashSounds));
    }

    public void PlayStepsSound()
    {
        SurfaceType currentSurface = surfaceRecognizer.GetCurrentTileSurface();
        audioSource5.Stop();
        AudioClip stepClip = SelectRandomStepsClip(GetSurfaceStepSounds(currentSurface));
        if(stepClip)
            audioSource5.PlayOneShot(stepClip);
    }
    
    private AudioClip[] GetObstacleHitSounds(ObstacleType obstacle)
    {
        AudioClip[] result;
        switch (obstacle)
        {
            case ObstacleType.Tree:
                result = hitObstacleTreeSounds;
                break;
            default:
                result = hitObstacleDefaultSounds;
                break;
        }

        return result;
    }

    private AudioClip[] GetSurfaceStepSounds(SurfaceType surface)
    {
        string surfaceName = SurfaceUtility.SurfaceTypeToString(surface);
        return stepSounds[surfaceName];
    }

    private AudioClip SelectRandomClip(AudioClip[] clips)
    {
        if (clips.Length > 0)
        {
            int randomIndex = Random.Range(0, clips.Length);
            AudioClip selectedClip = clips[randomIndex];

            return selectedClip;
        }

        return null;
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
