using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class HamsterAdoptionManager : MonoBehaviour
{
    public InputField nameInputField;
    public Button confirmButton;
    public GameObject NamedPetPanel;
    public GameObject MainLobbyPanel;

    private string apiUrl = "https://pet-backend-production-60aa.up.railway.app/api/hamster/create";

    void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmClick);
    }

    public void OnConfirmClick()
    {
        string hamsterName = nameInputField.text;
        long playerId = PlayerPrefs.GetInt("playerId", -1); // 從登入後儲存的 ID 取出

        if (string.IsNullOrEmpty(hamsterName))
        {
            Debug.LogWarning("請輸入倉鼠名字");
            return;
        }

        if (playerId == -1)
        {
            Debug.LogError("找不到 playerId，請確認是否登入");
            return;
        }

        StartCoroutine(CreateHamster(hamsterName, playerId));
    }

    IEnumerator CreateHamster(string name, long playerId)
    {
        HamsterCreateRequest requestData = new HamsterCreateRequest
        {
            name = name,
            playerId = playerId
        };

        string jsonData = JsonUtility.ToJson(requestData);
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("倉鼠領養成功：" + request.downloadHandler.text);
            MainLobbyPanel.SetActive(true);
            NamedPetPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("領養失敗：" + request.error);
        }
    }

    [System.Serializable]
    public class HamsterCreateRequest
    {
        public string name;
        public long playerId;
    }
}
