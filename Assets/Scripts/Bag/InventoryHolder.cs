using System.Collections.Generic;
using UnityEngine;

public class InventoryHolder : MonoBehaviour
{
    public inventory playerInventory; // 🔸 指向 ScriptableObject

    public InventoryUI InventoryUI;

    public void AddItem(item newItem)
    {
        playerInventory.itemList.Add(newItem);
        InventoryUI.RefreshUI(playerInventory.itemList);
    }

    public void AddItems(List<item> items)
    {
        foreach (var it in items)
            playerInventory.itemList.Add(it);

        InventoryUI.RefreshUI(playerInventory.itemList);
    }
}
