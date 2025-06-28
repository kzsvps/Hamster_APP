using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class RegisterRequest
{
    public string account;
    public string password;

    public RegisterRequest(string account, string password)
    {
        this.account = account;
        this.password = password;
    }
}

[System.Serializable]
public class RegisterResponse
{
    public string account;
    public int playerId;
}

public class RegisterManager : MonoBehaviour
{
    public InputField accountInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;
    public Text statusText;

    public GameObject playerRegisterPanel;
    public GameObject playerDataPanel;

    private string baseUrl = "https://pet-backend-production-60aa.up.railway.app/api/player";

    public void OnRegisterButtonClicked()
    {
        string account = accountInput.text.Trim();
        string password = passwordInput.text.Trim();
        string confirmPassword = confirmPasswordInput.text.Trim();

        Debug.Log($"Register attempt - Account: {account}, Password: {(string.IsNullOrEmpty(password) ? "(empty)" : "******")}");

        // 欄位基本檢查
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
        RegisterRequest data = new RegisterRequest(account, password);
        string json = JsonUtility.ToJson(data);

        Debug.Log("Register JSON: " + json);

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
                    RegisterResponse response = JsonUtility.FromJson<RegisterResponse>(request.downloadHandler.text);
                    statusText.text = $"註冊成功，玩家ID: {response.playerId}，請繼續填寫基本資料";

                    PlayerPrefs.SetInt("playerId", response.playerId);

                    playerRegisterPanel.SetActive(false);
                    playerDataPanel.SetActive(true);
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
                Debug.LogError("Register failed: " + errorMsg);
                statusText.text = "註冊失敗：" + errorMsg;
            }
        }
    }
}
