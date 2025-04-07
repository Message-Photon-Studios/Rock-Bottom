using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    [SerializeField] AudioClip startingMusic;
    [SerializeField] AudioClip loopingMusic;

    [SerializeField] AudioSource musicSourceStart;
    [SerializeField] AudioSource musicSourceLoop;

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
}
