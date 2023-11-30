using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClockTimerController : MonoBehaviour
{
    [SerializeField] TMP_Text timeText;
    [SerializeField] GameObject timer;
    [SerializeField] UIController UIController;
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
    }

    // Update is called once per frame
    void Update()
    {
        timeText.text = player.GetClockTimeString();
    }

    //Sets clocks active state acording to if it's active or not.
    private void LoadClock() {
        timer.SetActive(GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().allowsClockTimer);
    }
}
