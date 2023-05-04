using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSounds : MonoBehaviour
{
    //based on https://www.youtube.com/watch?v=lqyzGntF5Hw

    public AudioClip[] sounds;
    private AudioSource source;

    private float time;
    private float start;

    [Range(0.1f, 0.5f)]
    public float volumeChangeMultiplier = 0.2f;

    [Range(0.1f, 0.5f)]
    public float pitchChangeMultiplier = 0.2f;

    private float delay = 0;
    //change this to increase the range of random delays
    private float minDelay = 2f;
    private float maxDelay = 10f;

    void Start()
    {
        source = GetComponent<AudioSource>();
        start = Time.time;
        time = Time.time;
    }

    void Update()
    {
        time = Time.time;
        if (time > start + delay){
            source.clip = sounds[Random.Range(0, sounds.Length)];
            source.volume = Random.Range(1 - volumeChangeMultiplier, 1);
            source.pitch = Random.Range(1 - pitchChangeMultiplier, 1);
            source.PlayOneShot(source.clip);
            start = Time.time;
            delay = Random.Range(minDelay, maxDelay);
        }
        
    }
}
