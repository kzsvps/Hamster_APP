using System;

[System.Serializable]
public class EventItem
{
    public int id;
    public string title;
    public string distance;
    public string introduce;
    public string city;
    public string phone;
    public string address;
    public string type;
}

[System.Serializable]
public class ShopListResponse
{
    public ShopData[] shops;
    public int currentPage;
    public int totalPages;
    public long totalElements;
    public bool hasNext;
}

[System.Serializable]
public class ShopData
{
    public int id;
    public string title;
    public string distance;
    public string introduce;
    public string city;
    public string phone;
    public string address;
    public string type;
}

[System.Serializable]
public class FilterOptions
{
    public string[] cities;
    public string[] types;
    public string[] distances;
}
