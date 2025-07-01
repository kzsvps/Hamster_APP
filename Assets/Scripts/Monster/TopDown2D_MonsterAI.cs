using UnityEngine;

public class TopDown2D_MonsterAI : MonoBehaviour
{
    [Header("AI�]�w")]
    public Transform player;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float moveSpeed = 3f;
    public float patrolRadius = 3f;
    public float attackCooldown = 1f;
    public int attackDamage = 10;
    
    [Header("��ı�]�w")]
    public bool showDebugRanges = true;
    
    private enum MonsterState
    {
        Patrol,
        Chase,
        Attack,
        Return
    }
    
    private MonsterState currentState = MonsterState.Patrol;
    private Vector2 startPosition;
    private Vector2 patrolTarget;
    private float lastAttackTime;
    private Rigidbody2D rb;
    
    // ��ı�ե�
    private SpriteRenderer mainSprite;
    private SpriteRenderer faceSprite;
    private GameObject faceObject;
    private SpriteRenderer hornSprite;
    private GameObject hornObject;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
        
        startPosition = transform.position;
        SetNewPatrolTarget();
        
        // �ЫةǪ���ı�ĪG
        CreateMonsterVisuals();
        
        Debug.Log("�Ǫ���l�Ƨ����I");
    }
    
    void CreateMonsterVisuals()
    {
        // �D�� (�����αa�y��)
        mainSprite = GetComponent<SpriteRenderer>();
        if (mainSprite == null)
            mainSprite = gameObject.AddComponent<SpriteRenderer>();
            
        // �ЫةǪ��D��
        Texture2D bodyTexture = CreateMonsterBodyTexture();
        Sprite bodySprite = Sprite.Create(bodyTexture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        mainSprite.sprite = bodySprite;
        mainSprite.sortingOrder = 1;
        
        // �ЫةǪ��y��
        faceObject = new GameObject("MonsterFace");
        faceObject.transform.SetParent(transform);
        faceObject.transform.localPosition = Vector3.zero;
        
        faceSprite = faceObject.AddComponent<SpriteRenderer>();
        Texture2D faceTexture = CreateMonsterFaceTexture();
        Sprite faceSpriteAsset = Sprite.Create(faceTexture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        faceSprite.sprite = faceSpriteAsset;
        faceSprite.sortingOrder = 2;
        
        // �Ыب�
        hornObject = new GameObject("MonsterHorns");
        hornObject.transform.SetParent(transform);
        hornObject.transform.localPosition = new Vector3(0, 0.2f, 0);
        
        hornSprite = hornObject.AddComponent<SpriteRenderer>();
        Texture2D hornTexture = CreateHornTexture();
        Sprite hornSpriteAsset = Sprite.Create(hornTexture, new Rect(0, 0, 64, 32), new Vector2(0.5f, 0.5f), 64);
        hornSprite.sprite = hornSpriteAsset;
        hornSprite.sortingOrder = 3;
        
        // �K�[�I����
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 0.8f);
        }
    }
    
    Texture2D CreateMonsterBodyTexture()
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        
        // �Ыرa�y�������
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                Color pixelColor = Color.clear;
                
                // �D�n��ΰϰ�
                if (x >= 8 && x < 56 && y >= 8 && y < 56)
                {
                    pixelColor = Color.red;
                }
                
                // �K�[���
                if ((x >= 6 && x < 58 && y >= 6 && y < 58) && 
                    !(x >= 8 && x < 56 && y >= 8 && y < 56))
                {
                    pixelColor = Color.black;
                }
                
                pixels[y * 64 + x] = pixelColor;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    Texture2D CreateMonsterFaceTexture()
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];
        
        // �z���I��
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        
        // ���c������ (����)
        DrawEye(pixels, 64, 20, 45, 3, Color.yellow, Color.red);
        DrawEye(pixels, 64, 44, 45, 3, Color.yellow, Color.red);
        
        // ���c���L��
        DrawEvilMouth(pixels, 64);
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    Texture2D CreateHornTexture()
    {
        Texture2D texture = new Texture2D(64, 32);
        Color[] pixels = new Color[64 * 32];
        
        // �z���I��
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.clear;
        
        // ����
        DrawHorn(pixels, 64, 32, 16, 16, Color.black);
        // �k��
        DrawHorn(pixels, 64, 32, 48, 16, Color.black);
        
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
    
    void DrawEvilMouth(Color[] pixels, int texSize)
    {
        // ���c���L��
        for (int x = 25; x <= 39; x++)
        {
            int y = 30 - (int)(2 * Mathf.Sin((x - 25) * Mathf.PI / 14));
            if (y >= 0 && y < texSize)
            {
                pixels[y * texSize + x] = Color.black;
                if (y > 0) pixels[(y-1) * texSize + x] = Color.black;
            }
        }
        
        // ����
        for (int i = 0; i < 3; i++)
        {
            int x = 28 + i * 4;
            for (int y = 25; y <= 28; y++)
            {
                if (x >= 0 && x < texSize && y >= 0 && y < texSize)
                {
                    pixels[y * texSize + x] = Color.white;
                }
            }
        }
    }
    
    void DrawHorn(Color[] pixels, int texWidth, int texHeight, int baseX, int baseY, Color color)
    {
        // �e�T���Ϊ���
        for (int y = 0; y < 12; y++)
        {
            int width = 6 - y / 2;
            for (int x = -width; x <= width; x++)
            {
                int pixelX = baseX + x;
                int pixelY = baseY + y;
                if (pixelX >= 0 && pixelX < texWidth && pixelY >= 0 && pixelY < texHeight)
                {
                    pixels[pixelY * texWidth + pixelX] = color;
                }
            }
        }
    }
    
    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is null!");
            return;
        }
        
        UpdateAI();
        UpdateVisuals();
        
        // Debug����
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log($"�Ǫ����A: {currentState}, �Z�����a: {Vector2.Distance(transform.position, player.position):F2}");
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("���թǪ����ˡI");
        }
    }
    
    void UpdateAI()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        switch (currentState)
        {
            case MonsterState.Patrol:
                Patrol();
                if (distanceToPlayer <= detectionRange)
                {
                    ChangeState(MonsterState.Chase);
                }
                break;
                
            case MonsterState.Chase:
                ChasePlayer();
                if (distanceToPlayer <= attackRange)
                {
                    ChangeState(MonsterState.Attack);
                }
                else if (distanceToPlayer > detectionRange * 1.5f)
                {
                    ChangeState(MonsterState.Return);
                }
                break;
                
            case MonsterState.Attack:
                AttackPlayer();
                if (distanceToPlayer > attackRange)
                {
                    ChangeState(MonsterState.Chase);
                }
                break;
                
            case MonsterState.Return:
                ReturnToStart();
                if (Vector2.Distance(transform.position, startPosition) < 0.5f)
                {
                    ChangeState(MonsterState.Patrol);
                }
                else if (distanceToPlayer <= detectionRange)
                {
                    ChangeState(MonsterState.Chase);
                }
                break;
        }
    }
    
    void UpdateVisuals()
    {
        // �ھڪ��A�����C��
        Color bodyColor = Color.red;
        switch (currentState)
        {
            case MonsterState.Patrol:
                bodyColor = Color.red;
                break;
            case MonsterState.Chase:
                bodyColor = Color.magenta;
                break;
            case MonsterState.Attack:
                bodyColor = new Color(1f, 0.5f, 0f); // ���
                break;
            case MonsterState.Return:
                bodyColor = Color.yellow;
                break;
        }
        
        if (mainSprite != null)
        {
            mainSprite.color = bodyColor;
        }
    }
    
    void ChangeState(MonsterState newState)
    {
        currentState = newState;
        Debug.Log($"�Ǫ����A���ܬ�: {currentState}");
    }
    
    void Patrol()
    {
        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * 0.5f * Time.fixedDeltaTime);
        
        if (Vector2.Distance(transform.position, patrolTarget) < 0.5f)
        {
            SetNewPatrolTarget();
        }
    }
    
    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }
    
    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"�Ǫ��������a�y�� {attackDamage} �I�ˮ`�I");
            }
            lastAttackTime = Time.time;
        }
    }
    
    void ReturnToStart()
    {
        Vector2 direction = (startPosition - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * 0.7f * Time.fixedDeltaTime);
    }
    
    void SetNewPatrolTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle * patrolRadius;
        patrolTarget = startPosition + randomDirection;
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showDebugRanges) return;
        
        // �����d��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // �����d��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // ���޽d��
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPosition, patrolRadius);
        
        // ���ޥؼ�
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(patrolTarget, 0.3f);
    }
}