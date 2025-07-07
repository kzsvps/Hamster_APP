using System.Collections.Generic;
using UnityEngine;

public static class InventoryUtility
{
    public static void AddItemToBag(item newItem, inventory bag)
    {
        if (!bag.itemList.Contains(newItem))
        {
            bag.itemList.Add(newItem);
            newItem.itemHeld = 1;
        }
        else
        {
            newItem.itemHeld += 1;
        }

        InventoryManager.RefreshItem();
    }
}
