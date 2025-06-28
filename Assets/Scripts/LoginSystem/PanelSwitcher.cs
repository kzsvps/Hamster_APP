using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject playerRegisterPanel;
    public GameObject shopRegisterPanel;
    public GameObject ChoosePanel;
    public GameObject PlayerDataPanel;
    public GameObject ShopDataPanel;
    public GameObject ShopRoomPanel;

    void Start()
    {
        // 預設只顯示登入面板
        LoginPanel.SetActive(true);
        playerRegisterPanel.SetActive(false);
        shopRegisterPanel.SetActive(false);
        ChoosePanel.SetActive(false);
        PlayerDataPanel.SetActive(false);
        ShopDataPanel.SetActive (false);
        ShopRoomPanel.SetActive(false); 
    
}

    public void BackToLogin()
    {
        LoginPanel.SetActive(true);
        playerRegisterPanel.SetActive(false);
        shopRegisterPanel.SetActive(false);
        ChoosePanel.SetActive(false);
        PlayerDataPanel.SetActive(false);
        ShopDataPanel.SetActive(false);
        ShopRoomPanel.SetActive (false);
    }
    public void Register()
    {
        LoginPanel.SetActive(false);
        playerRegisterPanel.SetActive(false);
        shopRegisterPanel.SetActive(false);
        ChoosePanel.SetActive(true);
        Debug.Log("切換到選擇角色視窗");
    }
    public void ImPlayer()
    {
        LoginPanel.SetActive(false);
        playerRegisterPanel.SetActive(true);
        shopRegisterPanel.SetActive(false);
        ChoosePanel.SetActive(false);
        PlayerDataPanel.SetActive(false);

    }
    public void ImShop()
    {
        LoginPanel.SetActive(false);
        playerRegisterPanel.SetActive(false);
        shopRegisterPanel.SetActive(true);
        ChoosePanel.SetActive(false);
    }

    public void GoPlayerData()
    {
        LoginPanel.SetActive(false);
        playerRegisterPanel.SetActive(false);
        PlayerDataPanel.SetActive(true);
    }
    public void GoShopData()
    {
        LoginPanel.SetActive(false);
        shopRegisterPanel.SetActive(false);
        ShopDataPanel.SetActive(true);
    }

    public void Login()
    {
        LoginPanel.SetActive(false);
    }
}
