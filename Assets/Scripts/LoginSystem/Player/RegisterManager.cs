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

        // �����ˬd
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
                    statusText.text = $"���U���\�A���aID: {response.playerId}�A���~���g�򥻸��";

                    PlayerPrefs.SetInt("playerId", response.playerId);

                    playerRegisterPanel.SetActive(false);
                    playerDataPanel.SetActive(true);
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
                Debug.LogError("Register failed: " + errorMsg);
                statusText.text = "���U���ѡG" + errorMsg;
            }
        }
    }
}
