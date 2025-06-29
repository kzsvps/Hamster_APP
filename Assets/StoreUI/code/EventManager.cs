using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject eventCardPrefab;
    public Transform contentParent;
    public ScrollRect scrollRect;
    public GameObject loadingText;

    [Header("Filter Controls")]
    [SerializeField] private Dropdown dropdownType;
    [SerializeField] private Dropdown dropdownCity;
    [SerializeField] private Dropdown dropdownDistance;
    [SerializeField] private InputField searchInput;

    [Header("Detail Panel")]
    public GameObject detailPanel;
    public Text detailTitle;
    public Text detailIntro;
    public Text detailPhone;
    public Text detailAddress;
    public Text detailType;
    public Button closeButton;

    [Header("Debug")]
    public Text debugText;

    private List<EventItem> currentList = new List<EventItem>();
    private int currentPage = 0;
    private int pageSize = 15;
    private bool isLoading = false;
    private bool hasMorePages = true;

    private string currentType = "全部";
    private string currentCity = "全部";
    private string currentSearch = "";

    void Start()
    {
        InitializeUI();
        LoadFilterOptions();
        LoadInitialData();
    }

    void InitializeUI()
    {
        detailPanel.SetActive(false);
        closeButton.onClick.AddListener(CloseDetail);
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        searchInput.onEndEdit.AddListener(delegate { FilterEvents(); });
        dropdownType.onValueChanged.AddListener(delegate { FilterEvents(); });
        dropdownCity.onValueChanged.AddListener(delegate { FilterEvents(); });
        dropdownDistance.onValueChanged.AddListener(delegate { FilterEvents(); });
        UpdateDebugText("初始化完成");
    }

    void LoadFilterOptions()
    {
        UpdateDebugText("載入篩選選項中...");
        StartCoroutine(ShopApiManager.Instance.GetFilterOptions(
            (filters) => {
                UpdateDropdown(dropdownCity, filters.cities);
                UpdateDropdown(dropdownType, filters.types);
                UpdateDropdown(dropdownDistance, filters.distances);
                UpdateDebugText("篩選選項載入成功");
            },
            (error) => {
                UpdateDebugText($"載入失敗: {error}");
            }
        ));
    }

    void UpdateDropdown(Dropdown dropdown, string[] options)
    {
        if (dropdown != null && options != null)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(options));
        }
    }

    void LoadInitialData()
    {
        currentPage = 0;
        hasMorePages = true;
        ClearAllCards();
        currentList.Clear();
        LoadNextPage();
    }

    public void FilterEvents()
    {
        currentType = dropdownType.options[dropdownType.value].text;
        currentCity = dropdownCity.options[dropdownCity.value].text;
        currentSearch = searchInput.text.Trim();
        currentPage = 0;
        hasMorePages = true;
        ClearAllCards();
        currentList.Clear();
        LoadNextPage();
    }

    void LoadNextPage()
    {
        if (!hasMorePages || isLoading) return;
        isLoading = true;
        loadingText.SetActive(true);
        StartCoroutine(ShopApiManager.Instance.GetShops(
            currentPage, pageSize, currentType, currentCity, currentSearch,
            (response) => {
                foreach (var shop in response.shops)
                {
                    EventItem item = new EventItem
                    {
                        id = shop.id,
                        title = shop.title,
                        distance = shop.distance,
                        introduce = shop.introduce,
                        city = shop.city,
                        phone = shop.phone,
                        address = shop.address,
                        type = shop.type
                    };
                    currentList.Add(item);
                    AddEvent(item);
                }
                hasMorePages = response.hasNext;
                currentPage++;
                loadingText.SetActive(false);
                isLoading = false;
            },
            (error) => {
                UpdateDebugText($"載入錯誤: {error}");
                loadingText.SetActive(false);
                isLoading = false;
            }
        ));
    }

    void AddEvent(EventItem e)
    {
        GameObject card = Instantiate(eventCardPrefab, contentParent);
        Transform titleT = card.transform.Find("TitleText");
        if (titleT != null) titleT.GetComponent<Text>().text = e.title;

        Transform distT = card.transform.Find("DistanceText");
        if (distT != null) distT.GetComponent<Text>().text = $"距離你 {e.distance} 公里";

        Transform typeT = card.transform.Find("TypeText");
        if (typeT != null) typeT.GetComponent<Text>().text = e.type;

        Transform cityT = card.transform.Find("LocationText");
        if (cityT != null) cityT.GetComponent<Text>().text = e.city;

        Button btn = card.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(() => ShowDetailPage(e));

        card.GetComponent<Button>()?.onClick.AddListener(() => ShowDetailPage(e));
    }

    void ShowDetailPage(EventItem e)
    {
        StartCoroutine(ShopApiManager.Instance.GetShopDetail(
            e.id,
            (detail) => DisplayDetail(detail),
            (error) => {
                Debug.LogError(error);
                DisplayDetail(e);
            }
        ));
    }

    void DisplayDetail(EventItem e)
    {
        detailTitle.text = e.title;
        detailIntro.text = e.introduce;
        detailPhone.text = $"電話：{e.phone}";
        detailAddress.text = $"地址：{e.address}";
        detailType.text = $"種類：{e.type}";
        detailPanel.SetActive(true);
    }

    void CloseDetail()
    {
        detailPanel.SetActive(false);
    }

    void OnScrollValueChanged(Vector2 pos)
    {
        float contentY = contentParent.GetComponent<RectTransform>().anchoredPosition.y;
        float viewHeight = scrollRect.viewport.rect.height;
        float contentHeight = contentParent.GetComponent<RectTransform>().rect.height;
        if (contentY + viewHeight >= contentHeight - 100f) LoadNextPage();
    }

    void ClearAllCards()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateDebugText(string msg)
    {
        if (debugText != null) debugText.text = $"[{System.DateTime.Now:HH:mm:ss}] {msg}";
        Debug.Log(msg);
    }

    public void RefreshData() => LoadInitialData();
    public void SetSearchKeyword(string keyword) { searchInput.text = keyword; FilterEvents(); }
    public void ClearFilters() { dropdownType.value = 0; dropdownCity.value = 0; dropdownDistance.value = 0; searchInput.text = ""; FilterEvents(); }
}