using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ClockTimerController : MonoBehaviour
{
    [SerializeField] TMP_Text timeText;
    [SerializeField] GameObject timer;
    [SerializeField] UIController UIController;
    [SerializeField] RectTransform rectTransform;
    Animator animator;
    bool tracker = false;
    bool active = false;
    private PlayerStats player;

    /* Here are the useful methods to get the current clock time.
     * To get the time as a float in seconds: player.clockTime;
     * To get a formated string of the time in the format "min:"sec" example "02:31": player.GetClockTimeString()
     * 
     * If you want tho check if the current level has a clock timer enabled checkteh bool: GameObject.FindObjectWithTag("GameManager").GetComponent<GameManager>().allowsClockTimer
     * You might want to use this to hide the timer ui when the timer does not exist, for example in the tutorial, '
     * it is however not recomended to check this in the Update loop, insted check when a level is loaded. This is because the GameObject.FindObjectWithTag method is kinda slow. 
     */

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>(); 
        UIController.UILoaded += LoadClock;
        GameManager.instance.onLevelLoaded += ResetAnimator;
        animator = timer.GetComponent<Animator>();
    }

    private void OnDisable()
    {
        GameManager.instance.onLevelLoaded -= ResetAnimator;
        UIController.UILoaded -= LoadClock;
    }

    // Update is called once per frame
    void Update()
    {
        /*(String text, float size, Color color) clockVars;
        clockVars = GameManager.instance.GetClockTimeString();
        timeText.text = clockVars.text;
        timeText.color = clockVars.color;
        rectTransform.sizeDelta = new Vector2(clockVars.size, rectTransform.sizeDelta.y);*/

        if (LevelManager.instance.allowsClockTimer && active)
        {
            (int min, int sec) time;
            time = GameManager.instance.getTime();
            if ((time.min == 4 || time.min == 3 || time.min == 2) && time.sec > 30 && !tracker)
            {
                animator.SetBool("UnBlink", false);
                animator.SetBool("Blink", true);
                tracker = true;
            }
            if ((time.min == 4 || time.min == 3 || time.min == 2) && time.sec < 30 && tracker)
            {
                animator.SetBool("UnBlink", true);
                animator.SetBool("Blink", false);
                tracker = false;
            }
            if (time.min <= 1) animator.SetBool("WakeUp", true);
            if (time.min == 0) animator.SetBool("FasterLook", true);
            if (time.min == 0 && time.sec <= 20) {
                animator.SetBool("Glitch", true);
            }
            if (time.min <= 0 && time.sec <= 0)
            {
                animator.SetBool("Done", true);
                (String text, float size, Color color) clockVars;
                clockVars = GameManager.instance.GetClockTimeString();
                timeText.text = clockVars.text;
                timeText.color = clockVars.color;
            } else 
            {
                animator.SetBool("Done", false);
                timeText.text = "";
                timeText.gameObject.SetActive(false);
            }
            
        }
    }

    //Sets clocks active state acording to if it's active or not.
    private void LoadClock() {
        if (LevelManager.instance.allowsClockTimer)
        {
            animator.SetBool("Active", true);
            active = true;
        }
    }

    //Resets all animations and checks if the timer should start again
    private void ResetAnimator()
    {
        animator.Rebind();
        animator.Update(0f);
        LoadClock();
    }

    //Triggers the closing animation and keeps the timer closed until started again
    public void PauseTimer()
    {
        animator.SetBool("Active", false);
        active = false;
    }

    //Resumes all animations
    public void ResumeTimer()
    {
        animator.SetBool("Active", true);
        active = true;
    }

    
}
