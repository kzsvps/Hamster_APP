using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class ShopRegisterRequest
{
    public string account;  // 電子郵件
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
    public InputField accountInput;      // 電子郵件
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

        // 基本欄位檢查
        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            statusText.text = "請填寫所有欄位";
            return;
        }

        if (password != confirmPassword)
        {
            statusText.text = "密碼與確認密碼不符";
            return;
        }

        if (password.Length < 4)
        {
            statusText.text = "密碼長度至少4碼";
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
                    statusText.text = $"註冊成功，商家ID: {response.shopId}，請繼續填寫基本資料";

                    PlayerPrefs.SetInt("shopId", response.shopId);

                    shopRegisterPanel.SetActive(false);
                    shopDataPanel.SetActive(true);
                }
                else
                {
                    statusText.text = "註冊成功，但回傳資料異常";
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
                statusText.text = "註冊失敗：" + errorMsg;
            }
        }
    }
}
