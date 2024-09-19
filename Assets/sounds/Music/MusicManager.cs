using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;
using AYellowpaper.SerializedCollections;

public class MusicManager : MonoBehaviour
{
    [SerializeField] float changeMusicSpeed;
    
    [SerializeField] AudioSource player;
    [SerializeField] SerializedDictionary<string, BackgroundMusic> musicLibrary;
    BackgroundMusic currentMusic;
    private float maxVolume;
    private int musicIndex = 0;
    private bool musicStopped = true;

    void Start()
    {
        maxVolume = player.volume;
    }
    public void PlayMusic(string key)
    {
        if(key == null) return;
        if(!musicLibrary.ContainsKey(key)) return;
        AudioClip newMusic = musicLibrary[key].music[0];
        if(player.clip == newMusic) return;
        StartCoroutine(ChangeMusic(newMusic));
        musicIndex = 0;
        musicStopped = false;
        currentMusic = musicLibrary[key];
    }

    IEnumerator ChangeMusic(AudioClip newMusic)
    {
        if(player.isPlaying)
        {
            while(player.volume > 0)
            {
                player.volume -= changeMusicSpeed*0.1f*maxVolume;
                yield return new WaitForSeconds(0.1f);
            }
        }
        player.volume = 0;
        player.clip = newMusic;
        player.Play();
        
        while(player.volume < maxVolume)
        {
            player.volume += changeMusicSpeed*0.1f*maxVolume;
            yield return new WaitForSeconds(0.1f);
        }
        player.volume = maxVolume;
    }

    void Update()
    {
        if(!musicStopped && !player.isPlaying && currentMusic != null)
        {
            if(musicIndex < currentMusic.music.Length -1)
            {
                musicIndex++;
                player.clip = currentMusic.music[musicIndex];
                player.Play();
            } else if (currentMusic.loopMusic)
            {
                musicIndex = 0;
                player.clip = currentMusic.music[musicIndex];
                player.Play();
            } else musicStopped = true;
        }
    }
}

[System.Serializable]
class BackgroundMusic
{
    public bool loopMusic;
    public AudioClip[] music;
}
