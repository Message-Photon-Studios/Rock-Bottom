using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public Animator player;

    public AudioSource walking;
    public AudioSource jumping;
    public AudioSource climbing;
    public AudioSource sliding;
    public AudioSource dying;
    public AudioSource takingDamage;
    [SerializeField] private AudioSource defaultAttack;
    public AudioSource castingSpell;

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
        jumping.Play();
    }

    public void PlayCastingSpell()
    {
        castingSpell.Play();
    }

    public void PlayDefaultAttack()
    {
        defaultAttack.Play();
    }
    public void PlayTakingDamage()
    {
        if (!takingDamage.isPlaying && !player.GetBool(dyingBool))
        {
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
