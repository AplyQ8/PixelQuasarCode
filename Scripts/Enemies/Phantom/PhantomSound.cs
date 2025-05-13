using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class PhantomSound : EnemySound
{
    [SerializeField] private AudioClip[] disappearanceSounds;
    [SerializeField] private AudioClip[] appearanceSounds;
    [SerializeField] private AudioClip[] dashSwingSounds;
    [SerializeField] private AudioClip[] swordAttackSounds;
    [SerializeField] private AudioClip[] swordAppearanceSounds;
    [SerializeField] private AudioClip[] dealDmdSounds;

    [SerializeField] float regularSoundCooldown;
    private float regularSoundTimer;

    private bool inDashState;
    private float dashDuration = 4;
    
    void Start()
    {
        StartCoroutine(RegularSoundTimer());
        inDashState = false;
    }

    public void PlayDisappearanceSound()
    {
        audioSources[1].Stop();
        inDashState = true;
        StartCoroutine(NotInDashState());
        audioSources[0].PlayOneShot(SelectRandomClip(disappearanceSounds));
    }
    
    public void PlayAppearanceSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(appearanceSounds));
    }
    
    public void PlayDashSwingSound(Vector3 position)
    {
        audioSources[2].PlayOneShot(SelectRandomClip(dashSwingSounds));
        // PlaySoundInPoint(dashSwingSounds, audioSources[0], position);
    }
    
    public void PlayDashDealDamageSound(Vector3 position)
    {
        audioSources[2].PlayOneShot(SelectRandomClip(dealDmdSounds));
    }
    
    public void PlaySwordAppearanceSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(swordAppearanceSounds));
    }
    
    public void PlaySwordAttackSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(swordAttackSounds));
    }

    public void PlayDealDmgSound()
    {
        audioSources[0].PlayOneShot(SelectRandomClip(dealDmdSounds));
    }

    IEnumerator RegularSoundTimer()
    {
        for (;;)
        {
            regularSoundTimer = Random.Range(20, 100)/100f * regularSoundCooldown;
            yield return new WaitForSeconds(regularSoundTimer);
            if (inDashState)
            {
                yield return new WaitForSeconds(dashDuration);
            }
            PlayRegularSound();
        }
    }

    IEnumerator NotInDashState()
    {
        yield return new WaitForSeconds(dashDuration);
        inDashState = false;
    }
}
