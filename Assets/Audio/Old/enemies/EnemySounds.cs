using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    public Animator enemy;

    public AudioSource walking;
    [SerializeField] AudioClip[] walkingClips;
    public AudioSource running;
    public AudioSource sleeping;
    public AudioSource dying;
    public AudioSource takingDamage;
    public AudioSource jumping;
    public AudioSource spellHit;

    public AudioSource attack;

    private string walkingBool = "walk";
    private string walkingBool2 = "move";
    private string damageBool = "damaged";
    private string sleepingBool = "sleep";
    private string runningBool = "running";

    // Start is called before the first frame update
    void OnEnable()
    {
        //walking.Play();
        //walking.Pause();

        //running.Play();
        //running.Pause();

        //sleeping.Play();
        //sleeping.Pause();

    }

    // Update is called once per frame
    void Update()
    {
        /*if (enemy.GetBool(walkingBool) || enemy.GetBool(walkingBool2))
        {
            PlayWalking();
        }
        else
        {
            StopWalking();
        }
        */

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
        if(walkingClips.Length <= 0) return;
        if(walking.isPlaying) walking.Pause();
        walking.clip = walkingClips[Random.Range(0, walkingClips.Length)];
        walking.Play();
    }
    public void PlaySleeping()
    {
        if (!sleeping.isPlaying)
        {
            sleeping.Play();
        }
    }

    public void StopSleeping()
    {
        sleeping.Stop();
    }

    public void PlayJump()
    {
        if (!jumping.isPlaying)
        {
            jumping.Play();
        }
    }

    public void PlaySpellHit()
    {
        if (!spellHit.isPlaying)
        {
            spellHit.Play();
        }
    }
    public void PlayDeath()
    {
        if (!dying.isPlaying)
        {
            dying.Play();
        }
    }

    public void PlayAttack()
    {


        attack.Pause();

        attack.pitch = Random.Range(0.8f, 1.5f);
        attack.Play();

    }
}
