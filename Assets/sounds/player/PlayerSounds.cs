using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public Animator player;

    public AudioSource walking;
    public AudioSource jumping;
    public AudioSource climbing;

    public AudioSource takingDamage;

    private string walkingBool = "walking";
    private string damageBool = "damaged";

    public void Start()
    {
        

    }


    public void Update()
    {
        if (player.GetBool(walkingBool))
        {
            PlayWalking();
        }
        else
        {
            StopWalking();
        }

        if (player.GetBool(damageBool)){
            PlayTakingDamage();
        }
    }

    // for each sound there is a play method
    // for looping ones there needs to be a stop method

    public void PlayWalking()
    {
        if (!walking.isPlaying)
        {
            walking.Play();
        }
        
    }
    public void StopWalking()
    {
        walking.Stop();
    }

    public void PlayJump()
    {
        jumping.Play();
    }


    public void PlayTakingDamage()
    {
        if (!takingDamage.isPlaying)
        {
            takingDamage.Play();
        }
    }

}
