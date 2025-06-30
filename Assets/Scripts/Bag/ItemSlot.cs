using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public item item;

    public void SetItem(item newItem)
    {
        item = newItem;
    }

    public void OnClickShowDescription()
    {
        TooltipUI.Instance.ShowTooltip(item);
    }
}
