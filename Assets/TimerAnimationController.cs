using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerAnimationController : MonoBehaviour
{
    [SerializeField] TMP_Text timeText;

    public void EnableText()
    {
        timeText.gameObject.SetActive(true);
    }
}
