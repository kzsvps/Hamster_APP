using UnityEngine;
using UnityEngine.EventSystems;
public class InventoryUIController : MonoBehaviour
{
    public GameObject inventoryPanel;  // 拖曳 InventoryPanel 到這個欄位

    private bool isOpen = false;

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
        EventSystem.current.SetSelectedGameObject(null);
    }
}
