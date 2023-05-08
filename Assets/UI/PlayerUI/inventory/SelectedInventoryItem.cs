using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectedInventoryItem : MonoBehaviour, ISelectHandler, IDeselectHandler,IPointerClickHandler
{
    //Border item if selected.
    [SerializeField] GameObject border;

    //Image displayed.
    [SerializeField] Image image;

    //Item.
    [SerializeField] Item itemInfo;

    //Send out a notification if selected.
    public UnityAction<Item> onInventoryItemSelected;

    public UnityAction onItemLoaded;

    /// <summary>
    /// Sets up the prefab with given item.
    /// </summary>
    /// <param name="item"></param>
    public void Setup(Item item) {
        itemInfo = item;
        image.sprite = item.sprite;
        image.SetNativeSize();
    }

    /// <summary>
    /// When inventory opens, remove any potential border.
    /// </summary>
    private void OnEnable() {
        border.SetActive(false);
        onItemLoaded?.Invoke();
    }

    /// <summary>
    /// On selecting this item, show border and notify inventory that it's the active element. 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        border.SetActive(true);
        onInventoryItemSelected?.Invoke(itemInfo);
    }

    /// <summary>
    /// On deselecting the item, hide the border.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        border.SetActive(false);
    }

    /// <summary>
    /// On click, select item.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
