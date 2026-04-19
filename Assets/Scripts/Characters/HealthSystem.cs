using System;
using System.Collections;
using NUnit.Framework.Internal.Commands;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    //this may later refer to iframes or any backtracking objects
    private bool canTakeDamage = true; 

    [Header("Healing")]
    [SerializeField] private int maxHeals;
    [SerializeField] private int currentHeals;

    [SerializeField] private int healAmount;
    [SerializeField] private float healTime;

    private bool isHealing;

    public event Action OnDeath;
    public event Action<int, int> OnHealthChange;

    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentHeals = maxHeals;
    }

    public void TakeDamage(int damageAmount)
    {
        if (CanTakeDamage() == false) return;

        ApplyDamage(damageAmount);

        if (currentHealth == 0)
        {
            Death();
        }
    }

    private bool CanTakeDamage()
    {
        if (currentHealth == 0) return false;
        if (!canTakeDamage) return false;
        return true;
    }

    private void ApplyDamage(int damageAmount)
    {
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);
        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }

    private void Death()
    {
        OnDeath?.Invoke();
    }

    //healing
    public void TryHeal()
    {
        if (CanHeal() == false) return;

        StartCoroutine(StartHealing());
    }
    private bool CanHeal()
    {
        if (isHealing) return false;
        if (currentHeals <= 0) return false;
        if (currentHealth == maxHealth) return false;
        return true;
    }
    private void ApplyHeal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        OnHealthChange?.Invoke(currentHealth, maxHealth);
    }
    private IEnumerator StartHealing()
    {
        isHealing = true;
        //stop movement/play animation
        yield return new WaitForSeconds(healTime);
        ConsumeHeal();
        ApplyHeal(healAmount);

        isHealing = false;
    }

    private void ConsumeHeal()
    {
        currentHeals--;
    }
    public void RefillHeals()
    {
        currentHeals = maxHeals;
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

}
