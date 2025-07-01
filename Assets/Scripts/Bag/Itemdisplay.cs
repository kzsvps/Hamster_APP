using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    public item itemData;        // 拖 ScriptableObject 進來
    public Image itemImage;      // 拖 UI Image 元件進來

    void Start()
    {
        if (itemData != null && itemImage != null)
        {
            itemImage.sprite = itemData.itemImage;
        }
    }
}
