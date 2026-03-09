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
            StartCoroutine(instance.FadeOutAndDie());
            instance = this;
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

    public IEnumerator FadeOutAndDie()
    {
        float time = 0;
        while(time < fadeOutTime)
        {
            musicSourceStart.volume -= Time.deltaTime * (1/fadeOutTime);
            musicSourceLoop.volume -= Time.deltaTime * (1/fadeOutTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
