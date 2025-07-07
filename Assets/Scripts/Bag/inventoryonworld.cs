using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryonworld : MonoBehaviour
{
    public item thisItem;
    public inventory playerinventory;

    public void AddNewItem()
    {
        if (!playerinventory.itemList.Contains(thisItem))
        {
            playerinventory.itemList.Add(thisItem);
        }
        else
        {
            thisItem.itemHeld += 1;
        }

        InventoryManager.RefreshItem();
    }
}
