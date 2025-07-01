using UnityEngine;

public class TopDown2D_TestPlayer : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;
    
    [Header("虛擬搖桿設定")]
    public float joystickSize = 120f;
    public float joystickKnobSize = 60f;
    public float joystickDeadZone = 0.1f;
    public Vector2 joystickPosition = new Vector2(100, 100); // 左下角位置
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    private Rigidbody2D rb;
    private Vector2 movement;
    private PlayerHealth playerHealth;
    
    // 視覺組件
    private SpriteRenderer mainSprite;
    private SpriteRenderer eyesSprite;
    private GameObject eyesObject;
    
    // 虛擬搖桿相關
    private bool isDragging = false;
    private Vector2 joystickCenter;
    private Vector2 joystickInput = Vector2.zero;
    private int activeTouchId = -1;
    
    // GUI樣式和材質
    private Texture2D joystickBackgroundTexture;
    private Texture2D joystickKnobTexture;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
        
        playerHealth = GetComponent<PlayerHealth>();
        
        // 創建玩家視覺效果
        CreatePlayerVisuals();
        
        // 初始化搖桿位置
        joystickCenter = new Vector2(joystickPosition.x, Screen.height - joystickPosition.y);
        
        // 初始化搖桿材質
        InitializeJoystickTextures();
        
        Debug.Log("玩家初始化完成！");
    }
    
    void InitializeJoystickTextures()
    {
        // 創建搖桿背景材質
        joystickBackgroundTexture = CreateJoystickTexture((int)joystickSize, new Color(0.2f, 0.2f, 0.2f, 0.5f), new Color(1f, 1f, 1f, 0.8f));
        
        // 創建搖桿把手材質
        joystickKnobTexture = CreateJoystickTexture((int)joystickKnobSize, new Color(0.8f, 0.8f, 0.8f, 0.9f), new Color(0.5f, 0.5f, 0.5f, 1f));
    }
    
    Texture2D CreateJoystickTexture(int size, Color fillColor, Color borderColor)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        float center = size / 2f;
        float radius = size / 2f - 2;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                
                if (distance <= radius - 2)
                {
                    pixels[y * size + x] = fillColor;
                }
                else if (distance <= radius)
                {
                    pixels[y * size + x] = borderColor;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    void CreatePlayerVisuals()
    {
        // 主體 (藍色圓形)
        mainSprite = GetComponent<SpriteRenderer>();
        if (mainSprite == null)
            mainSprite = gameObject.AddComponent<SpriteRenderer>();
            
        // 創建圓形主體
        Texture2D circleTexture = CreateCircleTexture(64, Color.blue);
        Sprite circleSprite = Sprite.Create(circleTexture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        mainSprite.sprite = circleSprite;
        mainSprite.sortingOrder = 1;
        
        // 創建眼睛
        eyesObject = new GameObject("PlayerEyes");
        eyesObject.transform.SetParent(transform);
        eyesObject.transform.localPosition = Vector3.zero;
        
        eyesSprite = eyesObject.AddComponent<SpriteRenderer>();
        Texture2D eyesTexture = CreateEyesTexture();
        Sprite eyesSpriteAsset = Sprite.Create(eyesTexture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        eyesSprite.sprite = eyesSpriteAsset;
        eyesSprite.sortingOrder = 2;
        
        // 添加碰撞體
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
        }
    }
    
    Texture2D CreateCircleTexture(int size, Color color)
    {
        return CreateCircleTexture(size, color, Color.white);
    }
    
    Texture2D CreateCircleTexture(int size, Color color, Color borderColor)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        float center = size / 2f;
        float radius = size / 2f - 2;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                
                if (distance <= radius)
                {
                    // 主色
                    pixels[y * size + x] = color;
                }
                else if (distance <= radius + 2)
                {
                    // 邊框
                    pixels[y * size + x] = borderColor;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    Texture2D CreateEyesTexture()
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        
        // 透明背景 
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        
        // 左眼
        DrawEye(pixels, 64, 20, 45, 4, Color.white, Color.black);
        // 右眼
        DrawEye(pixels, 64, 44, 45, 4, Color.white, Color.black);
        
        // 微笑
        DrawSmile(pixels, 64);
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    void DrawEye(Color[] pixels, int texSize, int centerX, int centerY, int radius, Color eyeColor, Color pupilColor)
    {
        // 畫眼白
        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                if (x >= 0 && x < texSize && y >= 0 && y < texSize)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                    if (distance <= radius)
                    {
                        pixels[y * texSize + x] = eyeColor;
                    }
                }
            }
        }
        
        // 畫瞳孔
        int pupilRadius = radius / 2;
        for (int x = centerX - pupilRadius; x <= centerX + pupilRadius; x++)
        {
            for (int y = centerY - pupilRadius; y <= centerY + pupilRadius; y++)
            {
                if (x >= 0 && x < texSize && y >= 0 && y < texSize)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                    if (distance <= pupilRadius)
                    {
                        pixels[y * texSize + x] = pupilColor;
                    }
                }
            }
        }
    }
    
    void DrawSmile(Color[] pixels, int texSize)
    {
        // 簡單的微笑弧線
        for (int x = 25; x <= 39; x++)
        {
            int y = 25 + (int)(3 * Mathf.Sin((x - 25) * Mathf.PI / 14));
            if (y >= 0 && y < texSize)
            {
                pixels[y * texSize + x] = Color.black;
                if (y > 0) pixels[(y-1) * texSize + x] = Color.black;
            }
        }
    }
    
    void Update()
    {
        HandleInput();
        HandleTouchInput();
        
        if (showDebugInfo && Input.GetKeyDown(KeyCode.G))
        {
            showDebugInfo = !showDebugInfo;
            Debug.Log($"Debug模式: {showDebugInfo}");
        }
    }
    
    void HandleInput()
    {
        // 鍵盤輸入（保留原有功能）
        Vector2 keyboardInput = Vector2.zero;
        keyboardInput.x = Input.GetAxisRaw("Horizontal");
        keyboardInput.y = Input.GetAxisRaw("Vertical");
        
        // 如果沒有觸控輸入，使用鍵盤輸入
        if (joystickInput.magnitude < joystickDeadZone)
        {
            movement = keyboardInput;
        }
        else
        {
            movement = joystickInput;
        }
        
        // 按鍵功能
        if (Input.GetKeyDown(KeyCode.H) && playerHealth != null)
        {
            playerHealth.Heal(20);
            Debug.Log("玩家治療！");
        }
        
        if (Input.GetKeyDown(KeyCode.T) && playerHealth != null)
        {
            Debug.Log($"玩家狀態 - 生命值: {playerHealth.GetCurrentHealth()}/{playerHealth.GetMaxHealth()}");
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = Vector3.zero;
            Debug.Log("玩家位置重置！");
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            showDebugInfo = !showDebugInfo;
            Debug.Log($"Debug模式: {showDebugInfo}");
        }
    }
    
    void HandleTouchInput()
    {
        // 處理觸控輸入
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 touchPos = new Vector2(touch.position.x, Screen.height - touch.position.y);
                
                if (touch.phase == TouchPhase.Began)
                {
                    // 檢查是否點擊在搖桿區域
                    float distanceFromCenter = Vector2.Distance(touchPos, joystickCenter);
                    if (distanceFromCenter <= joystickSize / 2 && activeTouchId == -1)
                    {
                        isDragging = true;
                        activeTouchId = touch.fingerId;
                    }
                }
                else if (touch.fingerId == activeTouchId)
                {
                    if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        if (isDragging)
                        {
                            Vector2 offset = touchPos - joystickCenter;
                            float distance = offset.magnitude;
                            float maxDistance = (joystickSize - joystickKnobSize) / 2;
                            
                            if (distance > maxDistance)
                            {
                                offset = offset.normalized * maxDistance;
                            }
                            
                            // 修正Y軸方向 - 觸控座標已經轉換過，所以Y軸需要反轉
                            joystickInput = new Vector2(offset.x / maxDistance, -offset.y / maxDistance);
                        }
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        isDragging = false;
                        joystickInput = Vector2.zero;
                        activeTouchId = -1;
                    }
                }
            }
        }
        
        // 滑鼠輸入（用於編輯器測試）
        if (Application.isEditor)
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            
            if (Input.GetMouseButtonDown(0))
            {
                float distanceFromCenter = Vector2.Distance(mousePos, joystickCenter);
                if (distanceFromCenter <= joystickSize / 2)
                {
                    isDragging = true;
                }
            }
            else if (Input.GetMouseButton(0) && isDragging)
            {
                Vector2 offset = mousePos - joystickCenter;
                float distance = offset.magnitude;
                float maxDistance = (joystickSize - joystickKnobSize) / 2;
                
                if (distance > maxDistance)
                {
                    offset = offset.normalized * maxDistance;
                }
                
                // 修正Y軸方向 - GUI座標系的Y軸是向下的，但遊戲世界的Y軸是向上的
                joystickInput = new Vector2(offset.x / maxDistance, -offset.y / maxDistance);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                joystickInput = Vector2.zero;
            }
        }
    }
    
    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }
    
    void OnGUI()
    {
        // 繪製虛擬搖桿
        DrawVirtualJoystick();
        
        // Debug資訊
        if (showDebugInfo && playerHealth != null)
        {
            GUI.Label(new Rect(10, Screen.height - 150, 200, 20), $"玩家生命值: {playerHealth.GetCurrentHealth()}/{playerHealth.GetMaxHealth()}");
            GUI.Label(new Rect(10, Screen.height - 130, 200, 20), $"位置: {transform.position}");
            GUI.Label(new Rect(10, Screen.height - 110, 200, 20), $"搖桿輸入: {joystickInput}");
            GUI.Label(new Rect(10, Screen.height - 90, 200, 20), $"移動向量: {movement}");
            GUI.Label(new Rect(10, Screen.height - 70, 200, 20), "控制: WASD/觸控搖桿移動, H治療, T狀態, R重置, G切換Debug");
        }
    }
    
    void DrawVirtualJoystick()
    {
        // 確保材質已創建
        if (joystickBackgroundTexture == null || joystickKnobTexture == null)
        {
            InitializeJoystickTextures();
        }
        
        // 搖桿背景
        float bgX = joystickCenter.x - joystickSize / 2;
        float bgY = joystickCenter.y - joystickSize / 2;
        GUI.DrawTexture(new Rect(bgX, bgY, joystickSize, joystickSize), joystickBackgroundTexture);
        
        // 搖桿把手位置
        Vector2 knobOffset = joystickInput * (joystickSize - joystickKnobSize) / 2;
        float knobX = joystickCenter.x - joystickKnobSize / 2 + knobOffset.x;
        float knobY = joystickCenter.y - joystickKnobSize / 2 - knobOffset.y; // Y軸反轉
        
        GUI.DrawTexture(new Rect(knobX, knobY, joystickKnobSize, joystickKnobSize), joystickKnobTexture);
        
        // Debug: 顯示搖桿區域（可選）
        if (showDebugInfo)
        {
            GUI.Box(new Rect(bgX, bgY, joystickSize, joystickSize), "");
        }
    }
}