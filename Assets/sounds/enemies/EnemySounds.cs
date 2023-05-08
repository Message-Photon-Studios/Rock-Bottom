using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    public Animator enemy;

    public AudioSource walking;
    public AudioSource running;
    public AudioSource sleeping;
    public AudioSource dying;
    public AudioSource takingDamage;
    public AudioSource jumping;

    private string walkingBool = "walk";
    private string damageBool = "damaged";
    private string dyingBool = "dead";
    private string sleepingBool = "sleep";
    private string runningBool = "running";

    // Start is called before the first frame update
    void OnEnable()
    {
        walking.Play();
        walking.Pause();

        running.Play();
        running.Pause();

        sleeping.Play();
        sleeping.Pause();

    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.GetBool(walkingBool))
        {
            PlayWalking();
        }
        else
        {
            StopWalking();
        }

        if (enemy.GetBool(sleepingBool))
        {
            PlaySleeping();
        }
        else
        {
            StopSleeping();
        }
    }

    public void PlayWalking()
    {
        walking.UnPause();
    }

    public void StopWalking()
    {
        walking.Pause();
    }

    public void PlaySleeping()
    {
        sleeping.UnPause();
    }

    public void StopSleeping()
    {
        sleeping.Pause();
    }

    public void PlayJump()
    {
        if (!jumping.isPlaying)
        {
            jumping.Play();
        }
    }

    public void PlayDeath()
    {
        if (!dying.isPlaying)
        {
            dying.Play();
        }
    }
}
