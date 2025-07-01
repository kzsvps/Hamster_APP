using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    [Header("血量設定")]
    public int maxHealth = 100;
    public float healthBarHeight = 0.8f; // 血條距離怪物頭頂的高度
    public Vector2 healthBarSize = new Vector2(1.2f, 0.15f); // 血條尺寸
    
    [Header("視覺效果")]
    public bool showHealthBar = true;
    public Color healthBarBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color healthBarFillColor = Color.red;
    public Color healthBarBorderColor = Color.white;
    
    private int currentHealth;
    private bool isDead = false;
    
    // 血條視覺組件
    private GameObject healthBarObject;
    private SpriteRenderer healthBarBackground;
    private SpriteRenderer healthBarFill;
    private SpriteRenderer healthBarBorder;
    
    // 受傷效果
    private SpriteRenderer monsterSprite;
    private Color originalColor;
    private bool isFlashing = false;
    private float flashTimer = 0f;
    private float flashDuration = 0.2f;
    
    public delegate void OnMonsterDeath(GameObject monster);
    public static event OnMonsterDeath MonsterDied;
    
    void Start()
    {
        currentHealth = maxHealth;
        monsterSprite = GetComponent<SpriteRenderer>();
        if (monsterSprite != null)
        {
            originalColor = monsterSprite.color;
        }
        
        if (showHealthBar)
        {
            CreateHealthBar();
        }
        
        Debug.Log($"怪物血量初始化: {currentHealth}/{maxHealth}");
    }
    
    void Update()
    {
        if (isFlashing)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0)
            {
                isFlashing = false;
                if (monsterSprite != null)
                {
                    monsterSprite.color = originalColor;
                }
            }
        }
        
        UpdateHealthBarVisibility();
    }
    
    void CreateHealthBar()
    {
        // 創建血條父物件
        healthBarObject = new GameObject("HealthBar");
        healthBarObject.transform.SetParent(transform);
        healthBarObject.transform.localPosition = new Vector3(0, healthBarHeight, 0);
        
        // 創建血條背景
        GameObject backgroundObj = new GameObject("Background");
        backgroundObj.transform.SetParent(healthBarObject.transform);
        backgroundObj.transform.localPosition = Vector3.zero;
        
        healthBarBackground = backgroundObj.AddComponent<SpriteRenderer>();
        Texture2D bgTexture = CreateHealthBarTexture((int)(healthBarSize.x * 100), (int)(healthBarSize.y * 100), healthBarBackgroundColor);
        Sprite bgSprite = Sprite.Create(bgTexture, new Rect(0, 0, bgTexture.width, bgTexture.height), new Vector2(0.5f, 0.5f), 100);
        healthBarBackground.sprite = bgSprite;
        healthBarBackground.sortingOrder = 10;
        
        // 創建血條填充
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(healthBarObject.transform);
        fillObj.transform.localPosition = Vector3.zero;
        
        healthBarFill = fillObj.AddComponent<SpriteRenderer>();
        Texture2D fillTexture = CreateHealthBarTexture((int)(healthBarSize.x * 100), (int)(healthBarSize.y * 100), healthBarFillColor);
        Sprite fillSprite = Sprite.Create(fillTexture, new Rect(0, 0, fillTexture.width, fillTexture.height), new Vector2(0.5f, 0.5f), 100);
        healthBarFill.sprite = fillSprite;
        healthBarFill.sortingOrder = 11;
        
        // 創建血條邊框
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(healthBarObject.transform);
        borderObj.transform.localPosition = Vector3.zero;
        
        healthBarBorder = borderObj.AddComponent<SpriteRenderer>();
        Texture2D borderTexture = CreateHealthBarBorderTexture((int)(healthBarSize.x * 100), (int)(healthBarSize.y * 100));
        Sprite borderSprite = Sprite.Create(borderTexture, new Rect(0, 0, borderTexture.width, borderTexture.height), new Vector2(0.5f, 0.5f), 100);
        healthBarBorder.sprite = borderSprite;
        healthBarBorder.sortingOrder = 12;
        
        UpdateHealthBarDisplay();
    }
    
    Texture2D CreateHealthBarTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    Texture2D CreateHealthBarBorderTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    pixels[y * width + x] = healthBarBorderColor;
                }
                else
                {
                    pixels[y * width + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    void UpdateHealthBarDisplay()
    {
        if (healthBarFill == null) return;
        
        float healthPercentage = (float)currentHealth / maxHealth;
        
        // 調整血條填充的縮放
        healthBarFill.transform.localScale = new Vector3(healthPercentage, 1f, 1f);
        
        // 調整血條填充的位置（讓它從左邊開始縮小）
        float offsetX = (1f - healthPercentage) * healthBarSize.x * 0.5f;
        healthBarFill.transform.localPosition = new Vector3(-offsetX, 0, 0);
        
        // 根據血量改變顏色
        if (healthPercentage > 0.6f)
        {
            healthBarFill.color = Color.green;
        }
        else if (healthPercentage > 0.3f)
        {
            healthBarFill.color = Color.yellow;
        }
        else
        {
            healthBarFill.color = Color.red;
        }
    }
    
    void UpdateHealthBarVisibility()
    {
        if (healthBarObject == null) return;
        
        // 只有受傷時或血量不滿時才顯示血條
        bool shouldShow = showHealthBar && (currentHealth < maxHealth || isFlashing);
        healthBarObject.SetActive(shouldShow);
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log($"怪物受到 {damage} 點傷害！剩餘血量: {currentHealth}/{maxHealth}");
        
        // 受傷閃爍效果
        StartFlashEffect();
        
        // 更新血條顯示
        UpdateHealthBarDisplay();
        
        // 檢查是否死亡
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        if (isDead) return;
        
        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        
        UpdateHealthBarDisplay();
        
        Debug.Log($"怪物恢復 {healAmount} 點血量！當前血量: {currentHealth}/{maxHealth}");
    }
    
    void StartFlashEffect()
    {
        if (monsterSprite != null)
        {
            isFlashing = true;
            flashTimer = flashDuration;
            monsterSprite.color = Color.white;
        }
    }
    
    void Die()
    {
        isDead = true;
        Debug.Log("怪物死亡！");
        
        // 觸發死亡事件
        MonsterDied?.Invoke(gameObject);
        
        // 隱藏血條
        if (healthBarObject != null)
        {
            healthBarObject.SetActive(false);
        }
        
        // 死亡效果
        if (monsterSprite != null)
        {
            monsterSprite.color = Color.gray;
        }
        
        // 禁用AI和移動
        TopDown2D_MonsterAI monsterAI = GetComponent<TopDown2D_MonsterAI>();
        if (monsterAI != null)
        {
            monsterAI.enabled = false;
        }
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        // 可以在這裡添加死亡動畫或其他效果
        // 例如：播放死亡音效、掉落道具等
        
        // 延遲銷毀物件
        Destroy(gameObject, 2f);
    }
    
    // 公共方法供其他腳本調用
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
    
    // 設定血條顯示開關
    public void SetHealthBarVisible(bool visible)
    {
        showHealthBar = visible;
        if (healthBarObject != null)
        {
            healthBarObject.SetActive(visible && currentHealth < maxHealth);
        }
    }
}