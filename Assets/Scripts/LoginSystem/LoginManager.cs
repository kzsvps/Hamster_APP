using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[System.Serializable]
public class LoginRequest
{
    public string account;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public string role;
    public long id;
    public string nickname;

    public string shopName;
    public string type;
    public string address;
    public string phone;
    public string city;
}

public class LoginManager : MonoBehaviour
{
    public InputField accountInput;
    public InputField passwordInput;
    public Text statusText;

    public GameObject LoginPanel;
    public GameObject RoomPanel;        // 玩家畫面
    public GameObject ShopRoomPanel;    // 商家畫面

    private string loginUrl = "https://pet-backend-production-60aa.up.railway.app/api/login";

    public void OnLoginButtonClicked()
    {
        string account = accountInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
        {
            statusText.text = "請輸入帳號與密碼";
            return;
        }

        LoginRequest req = new LoginRequest { account = account, password = password };
        string json = JsonUtility.ToJson(req);

        StartCoroutine(AttemptLogin(json));
    }

    private IEnumerator AttemptLogin(string json)
    {
        using (UnityWebRequest request = new UnityWebRequest(loginUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log("伺服器回應：" + request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                LoginResponse res = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

                if (res.role == "player")
                {
                    statusText.text = "歡迎玩家：" + res.nickname;

                    // ✅ 儲存玩家資訊
                    PlayerPrefs.SetInt("playerId", (int)res.id);
                    PlayerPrefs.SetString("nickname", res.nickname);
                    PlayerPrefs.SetString("role", res.role);
                    PlayerPrefs.Save();

                    // ✅ 轉跳主場景
                    SceneManager.LoadScene("MainLobbyScene");
                }
                else if (res.role == "shop")
                {
                    statusText.text = "歡迎商家：" + res.shopName;

                    // ✅ 儲存商家資訊
                    PlayerPrefs.SetInt("shopId", (int)res.id);
                    PlayerPrefs.SetString("shopName", res.shopName);
                    PlayerPrefs.SetString("type", res.type);
                    PlayerPrefs.SetString("address", res.address);
                    PlayerPrefs.SetString("phone", res.phone);
                    PlayerPrefs.SetString("city", res.city);
                    PlayerPrefs.SetString("role", res.role);
                    PlayerPrefs.Save();

                    // ✅ 開啟商家畫面
                    ShopRoomPanel.SetActive(true);
                    RoomPanel.SetActive(false);
                    LoginPanel.SetActive(false);
                }


                LoginPanel.SetActive(false);
            }
            else
            {
                statusText.text = "登入失敗：" + request.error;
            }
        }
    }
}
