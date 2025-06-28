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
        long playerId = PlayerPrefs.GetInt("playerId", -1); // �q�n�J���x�s�� ID ���X

        if (string.IsNullOrEmpty(hamsterName))
        {
            Debug.LogWarning("�п�J�ܹ��W�r");
            return;
        }

        if (playerId == -1)
        {
            Debug.LogError("�䤣�� playerId�A�нT�{�O�_�n�J");
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
            Debug.Log("�ܹ���i���\�G" + request.downloadHandler.text);
            MainLobbyPanel.SetActive(true);
            NamedPetPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("��i���ѡG" + request.error);
        }
    }

    [System.Serializable]
    public class HamsterCreateRequest
    {
        public string name;
        public long playerId;
    }
}
