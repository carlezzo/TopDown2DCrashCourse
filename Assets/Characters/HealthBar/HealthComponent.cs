using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public HealthBarController healthBar;
    
    [Header("Events")]
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent OnDeath;
    
    int currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
        
        OnHealthChanged.Invoke(currentHealth);
        
        if (currentHealth <= 0)
        {
            OnDeath.Invoke();
        }
    }
    
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    
    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
        OnHealthChanged.Invoke(currentHealth);
    }
    
    public void Heal(int amount)
    {
        SetHealth(currentHealth + amount);
    }
}