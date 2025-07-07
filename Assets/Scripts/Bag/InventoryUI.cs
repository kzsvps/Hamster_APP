using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotParent;

    public void RefreshUI(List<item> itemList)
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        foreach (item it in itemList)
        {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            slot.GetComponentInChildren<TextMeshProUGUI>().text = it.itemName;
        }
    }
}
