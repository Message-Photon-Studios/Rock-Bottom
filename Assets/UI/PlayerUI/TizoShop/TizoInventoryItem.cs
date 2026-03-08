using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TizoInventoryItem : MonoBehaviour
{
    public string itemName;
    public int amount;
    [SerializeField] public TMP_Text amountText;
    [SerializeField] public Image image;
}
