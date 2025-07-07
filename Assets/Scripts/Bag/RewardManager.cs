using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardManager : MonoBehaviour
{
    public GameObject rewardPanel;
    public GameObject rewardSlotPrefab;
    public Transform rewardSlotParent;

    public float autoHideTime = 3f;

    public void ShowRewards(List<item> rewards)
    {
        // 清除舊的 UI slot
        foreach (Transform child in rewardSlotParent)
        {
            Destroy(child.gameObject);
        }

        // 顯示每個獎勵並加入背包
        foreach (item i in rewards)
        {
            // ✅ 產生 UI 顯示
            GameObject slot = Instantiate(rewardSlotPrefab, rewardSlotParent);
            slot.GetComponentInChildren<TextMeshProUGUI>().text = i.itemName;

            // ✅ 加入背包（使用 InventoryUtility）
            InventoryUtility.AddItemToBag(i, InventoryManager.instance.myBag);
        }

        rewardPanel.SetActive(true);
        Invoke(nameof(HidePanel), autoHideTime);
    }

    private void HidePanel()
    {
        rewardPanel.SetActive(false);
    }
}
