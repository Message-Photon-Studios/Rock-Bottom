using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;
using JetBrains.Annotations;

public class IndicatorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    
    //Indicator for current button, In main menu it's the acorn.
    [SerializeField] RectTransform container;
    [SerializeField] TMP_Text text;
    [SerializeField] Color32 active;
    [SerializeField] Color32 nonActive;

    [CanBeNull] private Coroutine cor;

    //On enable hide the acorn.
    private void OnEnable() {
        StartCoroutine(selectedEffect(false));
    }

    /// <summary>
    /// sets the indicator as active or not depending on what called it.
    /// </summary>
    /// <param name="inp"></param>
    public void ShowIndicator(bool inp)
    {
        StartCoroutine(selectedEffect(inp));
        text.color = inp ? active : nonActive;
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

    public IEnumerator appear(AnimationCurve curve)
    {
        for (float i = 0; i < 1; i += 0.013f)
        {
            container.anchoredPosition = new Vector2(
                Mathf.Lerp(-270, 50, curve.Evaluate(i)),
                container.anchoredPosition.y 
            );
            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator selectedEffect(bool sel)
    {
        var to = sel ? 1.2f : 1;
        var from = sel ? 1 : 1.2f;
        for (float i = 0; i < 1; i += 0.1f)
        {
            // Linearly interpolate scale of the container
            container.localScale = new Vector3(
                              Mathf.Lerp(from, to, i),
                              Mathf.Lerp(from, to, i),
                              Mathf.Lerp(from, to, i)
                          );
            yield return new WaitForSeconds(0.01f);
        }
    }
}
