using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class IndicatorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    
    //Indicator for current button, In main menu it's the acorn.
    [SerializeField] GameObject indicator;
    [SerializeField] TMP_Text text;
    [SerializeField] Color32 active;
    [SerializeField] Color32 nonActive;


    //On enable hide the acorn.
    private void OnEnable() {
        indicator.SetActive(false);
        text.color = nonActive;
    }

    /// <summary>
    /// sets the indicator as active or not depending on what called it.
    /// </summary>
    /// <param name="inp"></param>
    public void ShowIndicator(bool inp) {
        indicator.SetActive(inp);
        if(inp) {
            text.color = active;
        } else {
            text.color = nonActive;
        }
    }

    //On mouse entering the button area.
    public void OnPointerEnter(PointerEventData eventData) {
        ShowIndicator(true);
    }

    //On mouse exiting the button area.
    public void OnPointerExit(PointerEventData eventData) {
        ShowIndicator(false);
    }

    //On seleced with a keyboard/controller.
    public void OnSelect(BaseEventData eventData)
    {
        ShowIndicator(true);
    }

    //On deseleced with a keyboard/controller.
    public void OnDeselect(BaseEventData eventData)
    {
        ShowIndicator(false);
    }
}
