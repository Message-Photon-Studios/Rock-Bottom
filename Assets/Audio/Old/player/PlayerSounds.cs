using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private Animator player;

    [SerializeField] private AudioSource walking;
    [SerializeField] private AudioSource jumping;
    [SerializeField] private AudioSource climbing;
    [SerializeField] private AudioSource sliding;
    [SerializeField] private AudioSource dying;
    [SerializeField] private AudioSource takingDamage;
    [SerializeField] private AudioSource castingSpell;

    private string walkingBool = "walking";
    private string damageBool = "damaged";
    private string grappleBool = "grapple";
    private string dyingBool = "dead";

    private float delayDeath = 0.1f;

    // place looping sounds here and start them
    public void Start()
    {
        walking.Play();
        walking.Pause();

        climbing.Play();
        climbing.Pause();

        sliding.Play();
        sliding.Pause();
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
        

        if (player.GetBool(grappleBool)) 
        {
            if(player.GetInteger("velocityY") > 0)
            {
                StopSliding();
                PlayClimbing();
            }
            if(player.GetInteger("velocityY") < 0)
            {
                StopClimbing();
                PlaySliding();
            }
        }
        else
        {
            StopClimbing();
            StopSliding();
        }

        if (player.GetBool(damageBool) && !player.GetBool(dyingBool)){
            PlayTakingDamage();
        }
    }

    // for each sound there is a play method
    // for looping ones there needs to be a stop method
    // looping sounds are paused and unpaused, as to not start playing from the same place every time. 


    public void PlayWalking()
    {
        walking.UnPause();
    }
    public void StopWalking()
    {
        walking.Pause();
    }

    public void PlayClimbing()
    {
        climbing.UnPause();
    }

    public void StopClimbing()
    {
        climbing.Pause();
    }

    public void PlaySliding()
    {
        sliding.UnPause();
    }

    public void StopSliding()
    {
        sliding.Pause();
    }

    // non loops

    public void PlayJump()
    {
        if(jumping.isPlaying) jumping.Stop();
        jumping.pitch = Random.Range(0.8f, 1.5f);
        jumping.Play();
    }

    public void PlayCastingSpell()
    {
        if(castingSpell.isPlaying) castingSpell.Stop();
        castingSpell.pitch = Random.Range(1.2f, 2.5f);
        castingSpell.Play();
    }
    public void PlayTakingDamage()
    {
        if (!player.GetBool(dyingBool))
        {
            if(takingDamage.isPlaying) takingDamage.Stop();
            takingDamage.pitch = Random.Range(0.8f, 1.5f);
            takingDamage.Play();
        }
    }

    public void PlayDeath()
    {
        if (!dying.isPlaying)
        {
            dying.PlayDelayed(delayDeath);
        }
    }

}
