using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectedBottle : MonoBehaviour, ISelectHandler, IDeselectHandler,IPointerClickHandler
{
    //Border for bottle if selected.
    [SerializeField] GameObject border;

    //Image displayed.
    [SerializeField] Image image;

    //Bottle information.
    [SerializeField] ColorSpell bottleInfo;

    //Send out a notification if selected.
    public UnityAction<ColorSpell> onInventoryBottleSelected;

    /// <summary>
    /// Sets up the prefab with given bottle.
    /// </summary>
    /// <param name="bottle"></param>
    public void Setup(ColorSpell bottle) {
        bottleInfo = bottle;
        image.sprite = bottleInfo.GetBottleSprite().mediumSprite;
        image.SetNativeSize();
    }

    /// <summary>
    /// When inventory opens, remove any potential border.
    /// </summary>
    private void OnEnable() {
        border.SetActive(false);
    }

    /// <summary>
    /// On selecting this bottle, show border and notify inventory that it's the active element. 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        border.SetActive(true);
        onInventoryBottleSelected?.Invoke(bottleInfo);
    }

    /// <summary>
    /// On deselecting the bottle, hide the border.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        border.SetActive(false);
    }

    /// <summary>
    /// On click, select bottle.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}

