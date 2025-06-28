using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ShopProfileUpdateRequest
{
    public int shopId;
    public string shopName;
    public string type;
    public string address;
    public string phone;
    public string city;
}

public class ShopProfileManager : MonoBehaviour
{
    public InputField shopNameInput;
    public Dropdown typeDropdown;
    public Dropdown cityDropdown;
    public InputField addressInput;
    public InputField phoneInput;
    public Toggle agreementToggle; 
    public Text statusText;

    private string baseUrl = "https://pet-backend-production-60aa.up.railway.app/api/shop";

    public void OnSubmitButtonClicked()
    {
        int shopId = PlayerPrefs.GetInt("shopId", -1);
        if (shopId == -1)
        {
            statusText.text = "商家ID不存在，請重新登入";
            return;
        }

        // ✅ 檢查是否勾選同意條款
        if (!agreementToggle.isOn)
        {
            statusText.text = "請先勾選同意條款與隱私政策";
            return;
        }

        string shopName = shopNameInput.text.Trim();
        string type = typeDropdown.options[typeDropdown.value].text;
        string city = cityDropdown.options[cityDropdown.value].text;
        string address = addressInput.text.Trim();
        string phone = phoneInput.text.Trim();

        if (string.IsNullOrEmpty(shopName) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phone))
        {
            statusText.text = "請填寫所有欄位";
            return;
        }

        StartCoroutine(UpdateProfileCoroutine(shopId, shopName, type, city, address, phone));
    }

    private IEnumerator UpdateProfileCoroutine(int shopId, string shopName, string type, string city, string address, string phone)
    {
        var data = new ShopProfileUpdateRequest
        {
            shopId = shopId,
            shopName = shopName,
            type = type,
            city = city,
            address = address,
            phone = phone
        };

        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/updateProfile", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                statusText.text = "資料更新成功！";
                //SceneManager.LoadScene("MainLobby"); // 如有需要可切換場景
            }
            else
            {
                string errorMsg = request.downloadHandler.text;
                if (string.IsNullOrEmpty(errorMsg))
                    errorMsg = request.error;

                statusText.text = "更新失敗：" + errorMsg;
            }
        }
    }
}
