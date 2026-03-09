using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    public static BackgroundMusicController instance = null;
    [SerializeField] bool takeOverMusic = true;
    [SerializeField] float fadeOutTime = 1f;
    [SerializeField] AudioClip startingMusic;
    [SerializeField] AudioClip loopingMusic;

    [SerializeField] AudioSource musicSourceStart;
    [SerializeField] AudioSource musicSourceLoop;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        } else if (takeOverMusic)
        {
            instance.StartCoroutine(instance.FadeOutAndDie());
            instance = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if(startingMusic == null && loopingMusic == null) return;


        musicSourceStart.loop = false;
        musicSourceLoop.loop = true;
        musicSourceStart.clip = startingMusic;
        musicSourceLoop.clip = loopingMusic;
        musicSourceLoop.Stop();
        double startingTime = AudioSettings.dspTime+0.5f;
        double firstSongTime = (double)startingMusic.samples / (double)startingMusic.frequency;
        double loopTime = firstSongTime + startingTime;
        musicSourceStart.PlayScheduled(startingTime);
        musicSourceLoop.PlayScheduled(loopTime);
    }

    public void SetNewMusicLoop(AudioClip music, float fadeTime) => SetNewMusicLoop(music, music, fadeTime);
    public void SetNewMusicLoop(AudioClip startMusic, AudioClip loopMusic, float fadeTime)
    {
        StartCoroutine(FadeChangeMusic(startMusic, loopMusic, fadeTime));
    }

    IEnumerator FadeChangeMusic(AudioClip newStartMusic, AudioClip newLoopMusic, float fadeTime)
    {
        float time = 0;
        while(time < fadeTime)
        {
            musicSourceStart.volume -= Time.deltaTime * (1/fadeOutTime);
            musicSourceLoop.volume -= Time.deltaTime * (1/fadeOutTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        musicSourceStart.Stop();
        musicSourceLoop.Stop();
        startingMusic = newStartMusic;
        loopingMusic = newLoopMusic;
        musicSourceStart.volume = 1;
        musicSourceLoop.volume = 1;
        Start();
    }

    public IEnumerator FadeOutAndDie()
    {
        float time = 0;
        while(time < fadeOutTime)
        {
            musicSourceStart.volume -= Time.deltaTime * (1/fadeOutTime);
            musicSourceLoop.volume -= Time.deltaTime * (1/fadeOutTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

    public void StopMusic()
    {
        musicSourceStart.Stop();
        musicSourceLoop.Stop();
    }
}
