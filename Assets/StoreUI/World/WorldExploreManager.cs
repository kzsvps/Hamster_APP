using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WorldExploreManager : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject AdventureSelect;
    public GameObject CreateWorld;
    public GameObject JoinWorld;
    public GameObject Loding;
    public GameObject WorldRoom;
    public GameObject BuildObject;
    public GameObject DialogPopup;

    public InputField createRoomInput;
    public InputField joinRoomInput;

    private Stack<GameObject> pageHistory = new Stack<GameObject>();

    void Start()
    {
        pageHistory.Clear();
        ShowOnly(MainMenu);
    }

    // 顯示頁面並記錄歷史
    void ShowOnlyWithHistory(GameObject target)
    {
        GameObject current = GetCurrentPage();
        if (current != null && current != target)
        {
            pageHistory.Push(current);
        }
        ShowOnly(target);
    }

    // 回上一頁
    public void GoBack()
    {
        if (pageHistory.Count > 0)
        {
            GameObject previousPage = pageHistory.Pop();
            ShowOnly(previousPage);
        }
    }

    void ShowOnly(GameObject target)
    {
        MainMenu.SetActive(target == MainMenu);
        AdventureSelect.SetActive(target == AdventureSelect);
        CreateWorld.SetActive(target == CreateWorld);
        JoinWorld.SetActive(target == JoinWorld);
        Loding.SetActive(target == Loding);
        WorldRoom.SetActive(target == WorldRoom);
        BuildObject.SetActive(target == BuildObject);
        DialogPopup.SetActive(target == DialogPopup);
    }

    GameObject GetCurrentPage()
    {
        if (MainMenu.activeSelf) return MainMenu;
        if (AdventureSelect.activeSelf) return AdventureSelect;
        if (CreateWorld.activeSelf) return CreateWorld;
        if (JoinWorld.activeSelf) return JoinWorld;
        if (Loding.activeSelf) return Loding;
        if (WorldRoom.activeSelf) return WorldRoom;
        if (BuildObject.activeSelf) return BuildObject;
        if (DialogPopup.activeSelf) return DialogPopup;
        return null;
    }

    // UI 操作方法
    public void OnClickMainToAdventure()
    {
        ShowOnlyWithHistory(AdventureSelect);
    }

    public void OnClickAdventureToCreate()
    {
        ShowOnlyWithHistory(CreateWorld);
    }

    public void OnClickAdventureToJoin()
    {
        ShowOnlyWithHistory(JoinWorld);
    }

    public void OnClickCreateConfirm()
    {
        string roomId = createRoomInput.text;
        if (!string.IsNullOrEmpty(roomId))
        {
            StartCoroutine(LoadRoomProcess());
        }
        else
        {
            Debug.LogWarning("請輸入房號");
        }
    }

    public void OnClickJoinConfirm()
{
    string roomId = joinRoomInput.text;
    
    if (string.IsNullOrEmpty(roomId))
    {
        ShowDialog("請輸入房號");
        return;
    }

    // 假設錯誤房號是 "0000"，你可以換成你真實檢查機制
    if (roomId != "1234")  // ✅ 假設「1234」是正確房號
    {
        ShowDialog("房號不存在，請重新輸入");
        return;
    }

    // 房號正確，進入流程
    StartCoroutine(LoadRoomProcess());
}
    void ShowDialog(string message)
    {
        DialogPopup.SetActive(true);

        // 如果 DialogPopup 裡有 Text 元件，顯示錯誤訊息
        Text dialogText = DialogPopup.GetComponentInChildren<Text>();
        if (dialogText != null)
        {
            dialogText.text = message;
        }
    }
public void CloseDialog()
{
    DialogPopup.SetActive(false);
}

public void ExitRoom()
{
    // 清空歷史，避免從 MainMenu 回到 WorldRoom
    pageHistory.Clear();

    // 清空房號（可選）
    createRoomInput.text = "";
    joinRoomInput.text = "";

    // 顯示主畫面
    ShowOnly(MainMenu);
}


    IEnumerator LoadRoomProcess()
    {
        ShowOnlyWithHistory(Loding);
        yield return new WaitForSeconds(2f); // 模擬讀取
        ShowOnlyWithHistory(WorldRoom);
    }
}
