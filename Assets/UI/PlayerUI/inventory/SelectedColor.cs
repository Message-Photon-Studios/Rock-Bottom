using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectedColor : MonoBehaviour, ISelectHandler, IDeselectHandler,IPointerClickHandler
{
    [SerializeField] GameObject border;
    [SerializeField] GameColor color;

    public UnityAction<GameColor> onInventoryColorSelected;

    public UnityAction onColorLoaded;

     /// <summary>
    /// When inventory opens, remove any potential border.
    /// </summary>
    private void OnEnable() {
        border.SetActive(false);
        onColorLoaded?.Invoke();
    }

    /// <summary>
    /// On selecting this item, show border and notify inventory that it's the active element. 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        border.SetActive(true);
        onInventoryColorSelected?.Invoke(color);
    }

    /// <summary>
    /// On deselecting the color, hide the border.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        border.SetActive(false);
    }

    /// <summary>
    /// On click, select color.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
