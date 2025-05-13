using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using AudioSettings = UI.PauseScripts.SettingsScripts.AudioSettings;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    // [SerializeField] private TrackVersions endOfAllPaths;
    // [SerializeField] private TrackVersions whisperOfYore;
    // [SerializeField] private TrackVersions graveyardAmbient;
    // [SerializeField] private TrackVersions virtuousSlaughter;
    
    private AudioSource endOfAllPaths;
    private AudioSource endOfAllPathsLoop;
    private AudioSource whisperOfYore;
    private AudioSource graveyardAmbient;
    private AudioSource virtuousSlaughter;
    
    [SerializedDictionary("Title", "Audio Source")]
    public SerializedDictionary<string, AudioSource> music;

    private AudioSource currentPlayingSource;
    

    [SerializeField] private int endOfAllPathsLoopsNumber;
    [SerializeField] private int whisperOfYoreLoopsNumber;
    [SerializeField] private int graveyardAmbientLoopsNumber;
    
    
    [Range(0f, 1f)]
    public float musicVolume;
    private Dictionary<string, float> defaultVolumes;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        defaultVolumes = new Dictionary<string, float>();
        
        foreach (var title in music.Keys)
        {
            defaultVolumes[title] = music[title].volume;
        }
        UpdateVolume();

        endOfAllPaths = music["End Of All Paths"];
        endOfAllPathsLoop = music["End Of All Paths Loop"];
        whisperOfYore = music["Whisper Of Yore"];
        graveyardAmbient = music["Graveyard Ambient"];
        virtuousSlaughter = music["Virtuous Slaughter"];

        StartCoroutine(UpdateVolumeRoutine());
        StartAndKillDelay();
        //FindObjectOfType<AudioSettings>().SetMusicVolume(musicVolume);
    }

    private void Update()
    {
        // UpdateVolume();
    }

    public void SetMusicVolume(float volume) => musicVolume = volume;
    
    private void PlayRegularTrack(AudioSource source)
    {
        source.Play();
        currentPlayingSource = source;
    }
    
    private void PlayLoopTrack(AudioSource source, int numberOfLoops=-1, AudioSource lastLoopSource=null)
    {
        if (numberOfLoops == 0)
            return;
        
        source.Play();
        currentPlayingSource = source;

        if (numberOfLoops != -1)
        {
            StartCoroutine(StopLoopTrack(numberOfLoops, lastLoopSource));
        }
    }
    
    IEnumerator StopLoopTrack(int numberOfLoops, AudioSource lastLoopSource=null)
    {
        if (lastLoopSource)
            numberOfLoops -= 1;
        
        while (numberOfLoops > 0)
        {
            yield return new WaitForSeconds(currentPlayingSource.clip.length);
            numberOfLoops--;
        }
        
        currentPlayingSource.Stop();
        if(lastLoopSource)
            PlayRegularTrack(lastLoopSource);
    }
    
    private void FadeTrack(float fadeTime = 2)
    {
        StartCoroutine(FadeTrackRoutine(fadeTime));
    }

    IEnumerator FadeTrackRoutine(float fadeTime)
    {
        float startVolume = currentPlayingSource.volume;
        for (float i = 0; i < fadeTime; i += 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
            currentPlayingSource.volume = startVolume * (1 - i / fadeTime);
        }
        currentPlayingSource?.Stop();
        currentPlayingSource.volume = startVolume;
    }
    

    
    
    IEnumerator ScheduleAction(float time, Action func)
    {
        yield return new WaitForSeconds(time);
        func();
    }

    private void StartEndOfAllPaths()
    {
        CancelCoroutines();
        currentPlayingSource?.Stop();
        PlayLoopTrack(endOfAllPathsLoop, endOfAllPathsLoopsNumber, endOfAllPaths);
        
        float finishTime = endOfAllPathsLoop.clip.length*(endOfAllPathsLoopsNumber-1) + 
                           endOfAllPaths.clip.length + 5;
        StartCoroutine(ScheduleAction(finishTime, StartWhisperOfYore));
    }

    private void StartWhisperOfYore()
    {
        CancelCoroutines();
        currentPlayingSource?.Stop();
        PlayLoopTrack(whisperOfYore, whisperOfYoreLoopsNumber);
        float finishTime = whisperOfYore.clip.length*whisperOfYoreLoopsNumber + 3;
        StartCoroutine(ScheduleAction(finishTime, StartGraveyardAmbient));
    }
    
    private void StartGraveyardAmbient()
    {
        CancelCoroutines();
        currentPlayingSource?.Stop();
        PlayLoopTrack(graveyardAmbient, graveyardAmbientLoopsNumber);
        float finishTime = graveyardAmbient.clip.length*graveyardAmbientLoopsNumber + 3;
        StartCoroutine(ScheduleAction(finishTime, StartWhisperOfYore));
    }
    
    public void StartArenaOst()
    {
        FadeTrack(2);
        StartCoroutine(ScheduleAction(2.5f, StartVirtuousSlaughter));
    }
    
    public void StopArenaOst()
    {
        FadeTrack(2);
        StartCoroutine(ScheduleAction(4f, StartGraveyardAmbient));
    }
    
    private void StartVirtuousSlaughter()
    {
        CancelCoroutines();
        currentPlayingSource?.Stop();
        PlayLoopTrack(virtuousSlaughter);
    }
    
    public void FadeAndStartEndOfAllPaths()
    {
        FadeTrack(2);
        StartCoroutine(ScheduleAction(2.5f, StartEndOfAllPaths));
    }

    private void StartAndKillDelay()
    {
        //time is 0 to kill the delay of the first WaitForSeconds call
        StartCoroutine(ScheduleAction(0f, StartEndOfAllPaths));
    }


    private void CancelCoroutines()
    {
        StopAllCoroutines();
        StartCoroutine(UpdateVolumeRoutine());
    }
    
    IEnumerator UpdateVolumeRoutine()
    {

        for (;;)
        {
            yield return new WaitForSeconds(0.1f);
            UpdateVolume();
        }

    }

    private void UpdateVolume()
    {
        foreach (var title in music.Keys)
        {
            music[title].volume = defaultVolumes[title] * musicVolume;
        }
    }
    
}
