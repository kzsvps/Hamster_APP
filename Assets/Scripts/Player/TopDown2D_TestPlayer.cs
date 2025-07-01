using UnityEngine;

public class TopDown2D_TestPlayer : MonoBehaviour
{
    [Header("���ʳ]�w")]
    public float moveSpeed = 5f;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    private Rigidbody2D rb;
    private Vector2 movement;
    private PlayerHealth playerHealth;
    
    // ��ı�ե�
    private SpriteRenderer mainSprite;
    private SpriteRenderer eyesSprite;
    private GameObject eyesObject;
    
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
        
        // �Ыت��a��ı�ĪG
        CreatePlayerVisuals();
        
        Debug.Log("���a��l�Ƨ����I");
    }
    
    void CreatePlayerVisuals()
    {
        // �D�� (�Ŧ���)
        mainSprite = GetComponent<SpriteRenderer>();
        if (mainSprite == null)
            mainSprite = gameObject.AddComponent<SpriteRenderer>();
            
        // �Ыض�ΥD��
        Texture2D circleTexture = CreateCircleTexture(64, Color.blue);
        Sprite circleSprite = Sprite.Create(circleTexture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        mainSprite.sprite = circleSprite;
        mainSprite.sortingOrder = 1;
        
        // �Ыز���
        eyesObject = new GameObject("PlayerEyes");
        eyesObject.transform.SetParent(transform);
        eyesObject.transform.localPosition = Vector3.zero;
        
        eyesSprite = eyesObject.AddComponent<SpriteRenderer>();
        Texture2D eyesTexture = CreateEyesTexture();
        Sprite eyesSpriteAsset = Sprite.Create(eyesTexture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        eyesSprite.sprite = eyesSpriteAsset;
        eyesSprite.sortingOrder = 2;
        
        // �K�[�I����
        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
        }
    }
    
    Texture2D CreateCircleTexture(int size, Color color)
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
                    // �D��
                    pixels[y * size + x] = color;
                }
                else if (distance <= radius + 2)
                {
                    // ���
                    pixels[y * size + x] = Color.white;
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
        
        // �z���I��
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        
        // ����
        DrawEye(pixels, 64, 20, 45, 4, Color.white, Color.black);
        // �k��
        DrawEye(pixels, 64, 44, 45, 4, Color.white, Color.black);
        
        // �L��
        DrawSmile(pixels, 64);
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    void DrawEye(Color[] pixels, int texSize, int centerX, int centerY, int radius, Color eyeColor, Color pupilColor)
    {
        // �e����
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
        
        // �e����
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
        // ²�檺�L�����u
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
        
        if (showDebugInfo && Input.GetKeyDown(KeyCode.G))
        {
            showDebugInfo = !showDebugInfo;
            Debug.Log($"Debug�Ҧ�: {showDebugInfo}");
        }
    }
    
    void HandleInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        
        // ����\��
        if (Input.GetKeyDown(KeyCode.H) && playerHealth != null)
        {
            playerHealth.Heal(20);
            Debug.Log("���a�v���I");
        }
        
        if (Input.GetKeyDown(KeyCode.T) && playerHealth != null)
        {
            Debug.Log($"���a���A - �ͩR��: {playerHealth.GetCurrentHealth()}/{playerHealth.GetMaxHealth()}");
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = Vector3.zero;
            Debug.Log("���a��m���m�I");
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            showDebugInfo = !showDebugInfo;
            Debug.Log($"Debug�Ҧ�: {showDebugInfo}");
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
        if (showDebugInfo && playerHealth != null)
        {
            GUI.Label(new Rect(10, 10, 200, 20), $"���a�ͩR��: {playerHealth.GetCurrentHealth()}/{playerHealth.GetMaxHealth()}");
            GUI.Label(new Rect(10, 30, 200, 20), $"��m: {transform.position}");
            GUI.Label(new Rect(10, 50, 200, 20), "����: WASD����, H�v��, T���A, R���m, G����Debug");
        }
    }
}