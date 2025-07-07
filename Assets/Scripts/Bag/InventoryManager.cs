using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public inventory myBag;                  // ScriptableObject 背包
    public GameObject slotGrid;              // UI 格子容器
    public ItemSlot slotPrefab;              // 預製物品格子

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        RefreshItem();
    }

    public static void CreateNewItem(item item)
    {
        GameObject obj = Instantiate(instance.slotPrefab.gameObject, instance.slotGrid.transform);
        ItemSlot slot = obj.GetComponent<ItemSlot>();

        if (slot != null)
            slot.SetItem(item);
        else
            Debug.LogError("❌ ItemSlot component not found!");
    }

    public static void RefreshItem()
    {
        foreach (Transform child in instance.slotGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (item i in instance.myBag.itemList)
        {
            CreateNewItem(i);
        }
    }
}
