using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class UpdateProfileRequest
{
    public int playerId;
    public string nickname;
    public string gender;
    public string region;
    public string birthday;

    public UpdateProfileRequest(int playerId, string nickname, string gender, string region, string birthday)
    {
        this.playerId = playerId;
        this.nickname = nickname;
        this.gender = gender;
        this.region = region;
        this.birthday = birthday;
    }
}

public class PlayerProfileManager : MonoBehaviour
{
    public InputField nicknameInput;
    public Dropdown genderDropdown;
    public Dropdown regionDropdown;
    public InputField birthdayInput; // yyyy-MM-dd �榡
    public Toggle agreeToggle;

    public Text statusText;
    public GameObject profilePanel;
    public GameObject loginPanel;

    private string baseUrl = "https://pet-backend-production-60aa.up.railway.app/api/player";

    public void OnSubmitProfileButtonClicked()
    {
        if (!agreeToggle.isOn)
        {
            statusText.text = "�ЦP�N���ڻP���p�F��";
            return;
        }

        int playerId = PlayerPrefs.GetInt("playerId", -1);
        if (playerId == -1)
        {
            statusText.text = "�䤣�쪱�aID�A�Э��s�n�J";
            return;
        }

        string nickname = nicknameInput.text.Trim();
        string gender = genderDropdown.options[genderDropdown.value].text;
        string region = regionDropdown.options[regionDropdown.value].text;
        string birthday = birthdayInput.text.Trim();

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(birthday))
        {
            statusText.text = "�ж�g�ʺ٩M�ͤ�";
            return;
        }

        StartCoroutine(UpdateProfileCoroutine(playerId, nickname, gender, region, birthday));
    }

    private IEnumerator UpdateProfileCoroutine(int playerId, string nickname, string gender, string region, string birthday)
    {
        UpdateProfileRequest data = new UpdateProfileRequest(playerId, nickname, gender, region, birthday);
        string json = JsonUtility.ToJson(data);

        Debug.Log("Update Profile JSON: " + json);

        using (UnityWebRequest request = new UnityWebRequest(baseUrl + "/updateProfile", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                statusText.text = "��Ƨ�s���\�I";
                // �A�i�H�b�o�̤������L�e���ΰ������ʧ@
            }
            else
            {
                string errorMsg = request.downloadHandler.text;
                if (string.IsNullOrEmpty(errorMsg))
                    errorMsg = request.error;

                Debug.LogError("Update profile failed: " + errorMsg);
                statusText.text = "��Ƨ�s���ѡG" + errorMsg;
            }
        }
    }

    public void OnBackToLoginButtonClicked()
    {
        profilePanel.SetActive(false);
        loginPanel.SetActive(true);
    }
}
