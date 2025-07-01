using UnityEngine;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    [Header("戰鬥設定")]
    public float attackRange = 1.8f;
    public int attackDamage = 25;
    public float attackCooldown = 0.8f;
    public LayerMask enemyLayer = -1; // 敵人圖層，-1表示所有圖層
    
    [Header("攻擊視覺效果")]
    public bool showAttackRange = true;
    public Color attackRangeColor = Color.red;
    public float attackEffectDuration = 0.3f;
    
    [Header("音效設定")]
    public bool playAttackSounds = true;
    
    private float lastAttackTime;
    private bool isAttacking = false;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    
    // 視覺效果
    private GameObject attackEffectObject;
    private SpriteRenderer attackEffectSprite;
    private float attackEffectTimer;
    
    // 引用其他組件
    private TopDown2D_TestPlayer playerController;
    private SpriteRenderer playerSprite;
    private Color originalPlayerColor;
    
    void Start()
    {
        playerController = GetComponent<TopDown2D_TestPlayer>();
        playerSprite = GetComponent<SpriteRenderer>();
        if (playerSprite != null)
        {
            originalPlayerColor = playerSprite.color;
        }
        
        CreateAttackEffect();
        
        Debug.Log("玩家戰鬥系統初始化完成！");
    }
    
    void CreateAttackEffect()
    {
        // 創建攻擊特效物件
        attackEffectObject = new GameObject("AttackEffect");
        attackEffectObject.transform.SetParent(transform);
        attackEffectObject.transform.localPosition = Vector3.zero;
        
        attackEffectSprite = attackEffectObject.AddComponent<SpriteRenderer>();
        
        // 創建攻擊特效材質（圓形光環）
        Texture2D effectTexture = CreateAttackEffectTexture();
        Sprite effectSprite = Sprite.Create(effectTexture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), 64);
        attackEffectSprite.sprite = effectSprite;
        attackEffectSprite.sortingOrder = 5;
        attackEffectSprite.color = new Color(1f, 1f, 0f, 0.7f); // 黃色半透明
        
        attackEffectObject.SetActive(false);
    }
    
    Texture2D CreateAttackEffectTexture()
    {
        Texture2D texture = new Texture2D(128, 128);
        Color[] pixels = new Color[128 * 128];
        
        float center = 64f;
        
        for (int x = 0; x < 128; x++)
        {
            for (int y = 0; y < 128; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                
                // 創建光環效果
                if (distance >= 45 && distance <= 55)
                {
                    float alpha = 1f - Mathf.Abs(distance - 50f) / 5f;
                    pixels[y * 128 + x] = new Color(1f, 1f, 0f, alpha * 0.8f);
                }
                else if (distance >= 40 && distance <= 60)
                {
                    float alpha = 1f - Mathf.Abs(distance - 50f) / 10f;
                    pixels[y * 128 + x] = new Color(1f, 0.8f, 0f, alpha * 0.4f);
                }
                else
                {
                    pixels[y * 128 + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    void Update()
    {
        DetectEnemiesInRange();
        HandleAutoAttack();
        UpdateAttackEffect();
        
        // Debug按鍵
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"範圍內敵人數量: {enemiesInRange.Count}");
            ManualAttack();
        }
    }
    
    void DetectEnemiesInRange()
    {
        enemiesInRange.Clear();
        
        // 使用OverlapCircle檢測範圍內的敵人
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        
        foreach (Collider2D collider in colliders)
        {
            // 確保不是自己，並且有MonsterHealth組件
            if (collider.gameObject != gameObject)
            {
                MonsterHealth monsterHealth = collider.GetComponent<MonsterHealth>();
                if (monsterHealth != null && !monsterHealth.IsDead())
                {
                    enemiesInRange.Add(collider.gameObject);
                }
            }
        }
    }
    
    void HandleAutoAttack()
    {
        // 如果範圍內有敵人且攻擊冷卻完成，自動攻擊
        if (enemiesInRange.Count > 0 && Time.time - lastAttackTime >= attackCooldown && !isAttacking)
        {
            PerformAttack();
        }
    }
    
    void ManualAttack()
    {
        // 手動攻擊（可以忽略冷卻時間用於測試）
        if (enemiesInRange.Count > 0)
        {
            PerformAttack();
        }
        else
        {
            Debug.Log("攻擊範圍內沒有敵人！");
        }
    }
    
    void PerformAttack()
    {
        if (enemiesInRange.Count == 0) return;
        
        lastAttackTime = Time.time;
        isAttacking = true;
        
        // 播放攻擊特效
        PlayAttackEffect();
        
        // 攻擊範圍內的所有敵人
        int enemiesHit = 0;
        foreach (GameObject enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                MonsterHealth monsterHealth = enemy.GetComponent<MonsterHealth>();
                if (monsterHealth != null && !monsterHealth.IsDead())
                {
                    monsterHealth.TakeDamage(attackDamage);
                    enemiesHit++;
                    
                    // 可以在這裡添加擊退效果
                    ApplyKnockback(enemy);
                }
            }
        }
        
        Debug.Log($"玩家攻擊命中 {enemiesHit} 個敵人，造成 {attackDamage} 點傷害！");
        
        // 玩家攻擊時的視覺反饋
        if (playerSprite != null)
        {
            StartCoroutine(PlayerAttackFlash());
        }
        
        // 攻擊動作完成
        isAttacking = false;
    }
    
    void ApplyKnockback(GameObject enemy)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
            float knockbackForce = 3f;
            enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }
    
    System.Collections.IEnumerator PlayerAttackFlash()
    {
        if (playerSprite != null)
        {
            playerSprite.color = Color.yellow;
            yield return new WaitForSeconds(0.1f);
            playerSprite.color = originalPlayerColor;
        }
    }
    
    void PlayAttackEffect()
    {
        if (attackEffectObject != null)
        {
            attackEffectObject.SetActive(true);
            attackEffectTimer = attackEffectDuration;
            
            // 隨機旋轉攻擊特效
            attackEffectObject.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }
    }
    
    void UpdateAttackEffect()
    {
        if (attackEffectObject != null && attackEffectObject.activeInHierarchy)
        {
            attackEffectTimer -= Time.deltaTime;
            
            // 特效淡出
            if (attackEffectSprite != null)
            {
                float alpha = attackEffectTimer / attackEffectDuration;
                Color color = attackEffectSprite.color;
                color.a = alpha * 0.7f;
                attackEffectSprite.color = color;
                
                // 特效縮放
                float scale = 1f + (1f - alpha) * 0.5f;
                attackEffectObject.transform.localScale = Vector3.one * scale;
            }
            
            if (attackEffectTimer <= 0)
            {
                attackEffectObject.SetActive(false);
            }
        }
    }
    
    // 公共方法
    public bool IsAttacking()
    {
        return isAttacking;
    }
    
    public int GetEnemiesInRangeCount()
    {
        return enemiesInRange.Count;
    }
    
    public bool CanAttack()
    {
        return Time.time - lastAttackTime >= attackCooldown;
    }
    
    public void SetAttackDamage(int newDamage)
    {
        attackDamage = newDamage;
        Debug.Log($"玩家攻擊力設定為: {attackDamage}");
    }
    
    public void SetAttackRange(float newRange)
    {
        attackRange = newRange;
        Debug.Log($"玩家攻擊範圍設定為: {attackRange}");
    }
    
    // Gizmos顯示攻擊範圍
    void OnDrawGizmosSelected()
    {
        if (!showAttackRange) return;
        
        Gizmos.color = attackRangeColor;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // 顯示範圍內的敵人
        if (Application.isPlaying && enemiesInRange != null)
        {
            Gizmos.color = Color.yellow;
            foreach (GameObject enemy in enemiesInRange)
            {
                if (enemy != null)
                {
                    Gizmos.DrawLine(transform.position, enemy.transform.position);
                }
            }
        }
    }
}