using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("�ͩR�ȳ]�w")]
    public int maxHealth = 100;
    
    private int currentHealth;
    private bool isDead = false;
    
    // �ƥ�e�U
    public System.Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
    public System.Action OnPlayerDead;
    
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"���a�ͩR�Ȫ�l��: {currentHealth}/{maxHealth}");
    }
    
    // ���}��k�ӳX�ݨp���r�q
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
        
        Debug.Log($"���a���� {damage} �I�ˮ`�I�Ѿl�ͩR��: {currentHealth}/{maxHealth}");
        
        // Ĳ�o�ƥ�
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
            Debug.Log($"���a�v���F {actualHeal} �I�ͩR�ȡI��e�ͩR��: {currentHealth}/{maxHealth}");
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
        Debug.Log($"���a�����v���I�ͩR��: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void Revive(int reviveHealth = -1)
    {
        if (!isDead) return;
        
        isDead = false;
        currentHealth = reviveHealth < 0 ? maxHealth / 2 : Mathf.Clamp(reviveHealth, 1, maxHealth);
        
        Debug.Log($"���a�_���I�ͩR��: {currentHealth}/{maxHealth}");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    private void Die()
    {
        isDead = true;
        Debug.Log("���a���`�I");
        OnPlayerDead?.Invoke();
        
        // �i�H�b�o�̲K�[���`�ĪG
        // �Ҧp�G���񦺤`�ʵe�B����ʵ�
    }
    
    // Debug�\��
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugSetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"Debug: ���a�ͩR�ȳ]�w�� {currentHealth}/{maxHealth}");
    }
    
    void OnGUI()
    {
        // ²�檺��q��UI
        if (!isDead)
        {
            float barWidth = 200f;
            float barHeight = 20f;
            float healthPercentage = GetHealthPercentage();
            
            // �I��
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(10, 10, barWidth + 4, barHeight + 4), Texture2D.whiteTexture);
            
            // ��q���I��
            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(12, 12, barWidth, barHeight), Texture2D.whiteTexture);
            
            // ��e��q
            GUI.color = Color.green;
            GUI.DrawTexture(new Rect(12, 12, barWidth * healthPercentage, barHeight), Texture2D.whiteTexture);
            
            // ��q��r
            GUI.color = Color.white;
            GUI.Label(new Rect(15, 12, barWidth, barHeight), $"HP: {currentHealth}/{maxHealth}");
        }
        else
        {
            // ���`����
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 10, 200, 30), "���a�w���`�I");
        }
        
        GUI.color = Color.white; // ���m�C��
    }
}