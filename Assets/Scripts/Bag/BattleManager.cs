using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public RewardManager rewardManager;
    public item redPotion;
    public inventory playerInventory;

    void Start()
    {
        EndBattle();  // 遊戲一開始自動執行
    }

    public void EndBattle()
    {
        List<item> rewards = new List<item>() { redPotion };
        rewardManager.ShowRewards(rewards);
    }
}
