using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EventManager : MonoBehaviour
{
    public GameObject eventCardPrefab;
    public Transform contentParent;
    public ScrollRect scrollRect;

    public GameObject loadingText;
    [SerializeField] private Dropdown dropdownType;
    [SerializeField] private Dropdown dropdownCity;
    [SerializeField] private Dropdown dropdownDistance;
    [SerializeField] private InputField searchInput;

    private List<EventItem> allEvents = new List<EventItem>();
    private List<EventItem> currentList = new List<EventItem>();

    private int currentPage = 0;
    private int pageSize = 15;
    private bool isLoading = false;
    private bool isFinished = false;

    // 詳細資料 UI（全部改為 Text）
    public GameObject detailPanel;
    public Text detailTitle;
    public Text detailIntro;
    public Text detailPhone;
    public Text detailAddress;
    public Text detailType;
    public Button closeButton;

    void Start()
    {
        // 假資料生成
        for (int i = 1; i <= 100; i++)
        {
            allEvents.Add(new EventItem
            {
                title = $"活動 {i}",
                type = "體驗",
                address = "台北市 XX 路 XX 號",
                phone = "0912-345-678",
                distance = Random.Range(0.5f, 10f).ToString("0.0"),
                city = "台北市",
                introduce = "這是一段模擬的活動介紹文字"
            });
        }

        detailPanel.SetActive(false); // 預設關閉
        closeButton.onClick.AddListener(CloseDetail);
        currentList = new List<EventItem>(allEvents);
        LoadNextPage();
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private int GetKeywordScore(string title, string keyword)
    {
        keyword = keyword.ToLower();
        title = title.ToLower();

        if (title == keyword) return 100;
        if (title.StartsWith(keyword)) return 80;
        if (title.Contains(keyword)) return 50;
        return 0;
    }

    public void FilterEvents()
    {
        string type = dropdownType.value == 0 ? "全部" : dropdownType.options[dropdownType.value].text;
        string city = dropdownCity.value == 0 ? "全部" : dropdownCity.options[dropdownCity.value].text;
        string distance = dropdownDistance.value == 0 ? "全部" : dropdownDistance.options[dropdownDistance.value].text;
        string keyword = searchInput.text.Trim();

        Debug.Log($"查詢條件：{type}, {city}, {distance}, {keyword}");

        ClearAllCards();
        currentPage = 0;
        isFinished = false;

        currentList = allEvents
            .FindAll(e =>
                (type == "全部" || e.type == type) &&
                (city == "全部" || e.city == city) &&
                (distance == "全部" || e.distance == distance) &&
                (string.IsNullOrEmpty(keyword) || e.title.Contains(keyword))
            )
            .OrderByDescending(e =>
                !string.IsNullOrEmpty(keyword) ? GetKeywordScore(e.title, keyword) : 0
            )
            .ToList();

        LoadNextPage();
    }

    void ClearAllCards()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    void OnScrollValueChanged(Vector2 scrollPos)
    {
        if (isLoading || isFinished) return;

        float contentY = contentParent.GetComponent<RectTransform>().anchoredPosition.y;
        float viewHeight = scrollRect.viewport.rect.height;
        float contentHeight = contentParent.GetComponent<RectTransform>().rect.height;

        if (contentY + viewHeight >= contentHeight - 100f)
        {
            StartCoroutine(LoadNextPageDelayed());
        }
    }

    IEnumerator LoadNextPageDelayed()
    {
        isLoading = true;
        loadingText.SetActive(true);

        yield return new WaitForSeconds(1f);

        int startIndex = currentPage * pageSize;
        int endIndex = Mathf.Min(startIndex + pageSize, currentList.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            AddEvent(currentList[i]);
        }

        currentPage++;
        isLoading = false;
        loadingText.SetActive(false);
    }

    void LoadNextPage()
    {
        StartCoroutine(LoadNextPageDelayed());
    }

    void AddEvent(EventItem e)
    {
        GameObject card = Instantiate(eventCardPrefab, contentParent);

        Text titleText = card.transform.Find("TitleText")?.GetComponent<Text>();
        Text distanceText = card.transform.Find("DistanceText")?.GetComponent<Text>();
        Text typeText = card.transform.Find("TypeText")?.GetComponent<Text>();
        Text locationText = card.transform.Find("LocationText")?.GetComponent<Text>();

        if (titleText != null) titleText.text = e.title;
        if (distanceText != null) distanceText.text = $"距離你 {e.distance} 公里";
        if (typeText != null) typeText.text = e.type;
        if (locationText != null) locationText.text = e.city;

        Button btn = card.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => ShowDetailPage(e));
            Debug.Log($"✅ 按鈕已綁定：{e.title}");
        }
        else
        {
            Debug.LogWarning("⚠️ EventCard 上缺少 Button 組件");
        }
    }

    void ShowDetailPage(EventItem e)
    {
        Debug.Log("✔ 點擊卡片：" + e.title);
        detailTitle.text = e.title;
        detailIntro.text = e.introduce;
        detailPhone.text = $"電話：{e.phone}";
        detailAddress.text = $"地址：{e.address}";
        detailType.text = $"種類：{e.type}";
        detailPanel.SetActive(true);
    }

    public void CloseDetail()
    {
        detailPanel.SetActive(false);
    }
}
