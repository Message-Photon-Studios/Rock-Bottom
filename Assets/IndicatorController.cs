using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IndicatorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    
    //Indicator for current button, In main menu it's the acorn.
    [SerializeField] GameObject indicator;

    //On enable hide the acorn.
    private void OnEnable() {
        indicator.SetActive(false);
    }

    /// <summary>
    /// sets the indicator as active or not depending on what called it.
    /// </summary>
    /// <param name="inp"></param>
    private void ShowIndicator(bool inp) {
        indicator.SetActive(inp);
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
