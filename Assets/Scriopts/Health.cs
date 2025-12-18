using TMPro;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth { get; private set; }
    public TextMeshProUGUI HealthText;
    
    public event Action<int, int> OnHealthChanged;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        HealthText.text = null;
        HealthText.text += CurrentHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth < 0) CurrentHealth = 0;
        HealthText.text = null;
        HealthText.text += CurrentHealth;
        
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void Heal(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
        HealthText.text = null;
        HealthText.text += CurrentHealth;
        
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}
