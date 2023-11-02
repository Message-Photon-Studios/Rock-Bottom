using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClockTimerController : MonoBehaviour
{
    [SerializeField] TMP_Text timeText;
    private PlayerStats player;

    /* Here are the useful methods to get the current clock time.
     * To get the time as a float in seconds: player.clockTime;
     * To get a formated string of the time in the format "min:"sec" example "02:31": player.GetClockTimeString()
     */

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        timeText.text = player.GetClockTimeString();
    }
}
