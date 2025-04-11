using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class SoundEffectManager : MonoBehaviour
{
    [SerializeField] SerializedDictionary<string, SoundEffect> soundLibrary;


    public void PlaySound(string key, float strength)
    {
        if(key == null) return;
        if(!soundLibrary.ContainsKey(key)) return;


        SoundEffect soundEffect = soundLibrary[key];

        if(!soundEffect.audioSource.isPlaying)
        {
            soundEffect.audioSource.volume = soundEffect.maxVolume*strength;
            soundEffect.audioSource.Play();
        }
    }
    public void PlaySound(string key)
    {
        PlaySound(key, 1f);
    }


    public void StopSound(string key)
    {
        if(key == null) return;
        if(!soundLibrary.ContainsKey(key)) return;

        AudioSource audioSource = soundLibrary[key].audioSource;

        if(!audioSource.isPlaying)
            audioSource.Stop();
    }
}

[System.Serializable]
class SoundEffect
{
    public AudioSource audioSource;
    public float maxVolume;
}
