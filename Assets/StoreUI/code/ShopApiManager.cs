using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShopApiManager : MonoBehaviour
{
    [Header("API設定")]
     public string baseUrl = "https://pet-backend-production-60aa.up.railway.app/api/shop";


    private static ShopApiManager instance;
    public static ShopApiManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ShopApiManager>() ?? new GameObject("ShopApiManager").AddComponent<ShopApiManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    public IEnumerator GetShops(int page, int size, string type, string city, string search,
        System.Action<ShopListResponse> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/list?page={page}&size={size}";
        if (!string.IsNullOrEmpty(type) && type != "全部") url += $"&type={UnityWebRequest.EscapeURL(type)}";
        if (!string.IsNullOrEmpty(city) && city != "全部") url += $"&city={UnityWebRequest.EscapeURL(city)}";
        if (!string.IsNullOrEmpty(search)) url += $"&search={UnityWebRequest.EscapeURL(search)}";

        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    onSuccess?.Invoke(JsonUtility.FromJson<ShopListResponse>(req.downloadHandler.text));
                }
                catch (System.Exception e)
                {
                    onError?.Invoke($"JSON解析錯誤: {e.Message}");
                }
            }
            else
            {
                onError?.Invoke($"API錯誤: {req.error}");
            }
        }
    }

    public IEnumerator GetShopDetail(int id, System.Action<EventItem> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/{id}";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("Content-Type", "application/json");
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    onSuccess?.Invoke(JsonUtility.FromJson<EventItem>(req.downloadHandler.text));
                }
                catch (System.Exception e)
                {
                    onError?.Invoke($"詳情解析錯誤: {e.Message}");
                }
            }
            else
            {
                onError?.Invoke($"獲取詳情失敗: {req.error}");
            }
        }
    }

    public IEnumerator GetFilterOptions(System.Action<FilterOptions> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/filters";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    onSuccess?.Invoke(JsonUtility.FromJson<FilterOptions>(req.downloadHandler.text));
                }
                catch (System.Exception e)
                {
                    onError?.Invoke($"篩選解析錯誤: {e.Message}");
                }
            }
            else
            {
                onError?.Invoke($"獲取篩選失敗: {req.error}");
            }
        }
    }
}
