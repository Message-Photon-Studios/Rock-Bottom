using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class AnimationSounds : MonoBehaviour
{
    [SerializeField] SerializedDictionary<string, AudioSource> sounds;

    public void PlayerSounds(string soundKey)
    {
        if(sounds.ContainsKey(soundKey)) 
        {
            sounds[soundKey].Stop();
            sounds[soundKey].Play();
        }
    }
}
