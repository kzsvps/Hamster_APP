using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class ShopRegisterRequest
{
    public string account;  // �q�l�l��
    public string password;
}

[System.Serializable]
public class ShopRegisterResponse
{
    public string account;
    public int shopId;
}

public class ShopRegisterManager : MonoBehaviour
{
    public InputField accountInput;      // �q�l�l��
    public InputField passwordInput;
    public InputField confirmPasswordInput;
    public Text statusText;

    public GameObject shopRegisterPanel;
    public GameObject shopDataPanel;

    private string baseUrl = "https://pet-backend-production-60aa.up.railway.app/api/shop";

    public void OnRegisterButtonClicked()
    {
        string account = accountInput.text.Trim();
        string password = passwordInput.text.Trim();
        string confirmPassword = confirmPasswordInput.text.Trim();

        // ������ˬd
        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            statusText.text = "�ж�g�Ҧ����";
            return;
        }

        if (password != confirmPassword)
        {
            statusText.text = "�K�X�P�T�{�K�X����";
            return;
        }

        if (password.Length < 4)
        {
            statusText.text = "�K�X���צܤ�4�X";
            return;
        }

        StartCoroutine(RegisterCoroutine(account, password));
    }

    private IEnumerator RegisterCoroutine(string account, string password)
    {
        ShopRegisterRequest data = new ShopRegisterRequest { account = account, password = password };
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/register", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    ShopRegisterResponse response = JsonUtility.FromJson<ShopRegisterResponse>(request.downloadHandler.text);
                    statusText.text = $"���U���\�A�ӮaID: {response.shopId}�A���~���g�򥻸��";

                    PlayerPrefs.SetInt("shopId", response.shopId);

                    shopRegisterPanel.SetActive(false);
                    shopDataPanel.SetActive(true);
                }
                else
                {
                    statusText.text = "���U���\�A���^�Ǹ�Ʋ��`";
                }
            }
            else
            {
                string errorMsg = request.downloadHandler.text;
                if (string.IsNullOrEmpty(errorMsg))
                {
                    errorMsg = request.error;
                }
                Debug.LogError("Shop register failed: " + errorMsg);
                statusText.text = "���U���ѡG" + errorMsg;
            }
        }
    }
}
