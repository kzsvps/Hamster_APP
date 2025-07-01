using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("生命值設定")]
    public int maxHealth = 100;
    
    private int currentHealth;
    private bool isDead = false;
    
    // 事件委託
    public System.Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
    public System.Action OnPlayerDead;
    
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"玩家生命值初始化: {currentHealth}/{maxHealth}");
    }
    
    // 公開方法來訪問私有字段
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
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        Debug.Log($"玩家受到 {damage} 點傷害！剩餘生命值: {currentHealth}/{maxHealth}");
        
        // 觸發事件
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        if (isDead) return;
        
        int oldHealth = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        int actualHeal = currentHealth - oldHealth;
        if (actualHeal > 0)
        {
            Debug.Log($"玩家治療了 {actualHeal} 點生命值！當前生命值: {currentHealth}/{maxHealth}");
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
    
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void FullHeal()
    {
        if (isDead) return;
        
        currentHealth = maxHealth;
        Debug.Log($"玩家完全治療！生命值: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void Revive(int reviveHealth = -1)
    {
        if (!isDead) return;
        
        isDead = false;
        currentHealth = reviveHealth < 0 ? maxHealth / 2 : Mathf.Clamp(reviveHealth, 1, maxHealth);
        
        Debug.Log($"玩家復活！生命值: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    private void Die()
    {
        isDead = true;
        Debug.Log("玩家死亡！");
        OnPlayerDead?.Invoke();
        
        // 可以在這裡添加死亡效果
        // 例如：播放死亡動畫、停止移動等
    }
    
    // Debug功能
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugSetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Debug: 玩家生命值設定為 {currentHealth}/{maxHealth}");
    }
    
    void OnGUI()
    {
        // 簡單的血量條UI
        if (!isDead)
        {
            float barWidth = 200f;
            float barHeight = 20f;
            float healthPercentage = GetHealthPercentage();
            
            // 背景
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(10, 10, barWidth + 4, barHeight + 4), Texture2D.whiteTexture);
            
            // 血量條背景
            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(12, 12, barWidth, barHeight), Texture2D.whiteTexture);
            
            // 當前血量
            GUI.color = Color.green;
            GUI.DrawTexture(new Rect(12, 12, barWidth * healthPercentage, barHeight), Texture2D.whiteTexture);
            
            // 血量文字
            GUI.color = Color.white;
            GUI.Label(new Rect(15, 12, barWidth, barHeight), $"HP: {currentHealth}/{maxHealth}");
        }
        else
        {
            // 死亡提示
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 10, 200, 30), "玩家已死亡！");
        }
        
        GUI.color = Color.white; // 重置顏色
    }
}