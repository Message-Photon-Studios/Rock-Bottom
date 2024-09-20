using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class SoundEffectManager : MonoBehaviour
{
    [SerializeField] SerializedDictionary<string, AudioSource> soundLibrary;


    public void PlaySound(string key)
    {
        if(key == null) return;
        if(!soundLibrary.ContainsKey(key)) return;

        AudioSource audioSource = soundLibrary[key];

        if(!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopSound(string key)
    {
        if(key == null) return;
        if(!soundLibrary.ContainsKey(key)) return;

        AudioSource audioSource = soundLibrary[key];

        if(!audioSource.isPlaying)
            audioSource.Stop();
    }
}
