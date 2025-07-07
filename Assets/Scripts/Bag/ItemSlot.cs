using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public item item;
    public Image slotImage;
    public TMP_Text slotNum;

    public void SetItem(item newItem)
    {
        item = newItem;
        slotImage.sprite = item.itemImage;

        if (item.itemHeld > 1)
            slotNum.text = item.itemHeld.ToString();
        else
            slotNum.text = "";
    }

    public void OnClickShowDescription()
    {
        TooltipUI.Instance.ShowTooltip(item);
    }
}
