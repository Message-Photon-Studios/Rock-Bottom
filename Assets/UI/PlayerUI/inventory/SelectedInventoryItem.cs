using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectedInventoryItem : MonoBehaviour, ISelectHandler, IDeselectHandler,IPointerClickHandler
{
    //
    [SerializeField] GameObject border;
    [SerializeField] Image image;
    [SerializeField] Item itemInfo;

    public UnityAction<Item> onInventoryItemSelected;

    public void Setup(Item item) {
        itemInfo = item;
        image.sprite = item.sprite;
        image.SetNativeSize();
    }

    private void OnEnable() {
        border.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        border.SetActive(true);
        onInventoryItemSelected?.Invoke(itemInfo);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        border.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
