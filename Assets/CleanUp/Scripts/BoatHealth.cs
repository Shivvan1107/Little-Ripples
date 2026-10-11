
using UnityEngine;
using System;

public class BoatHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Damage")]
    public float invulnerabilityTime = 1f;

    public event Action OnHealthChanged;
    public event Action OnBoatDestroyed;

    private float nextDamageTime;
    private bool isDestroyed;

    public bool IsDestroyed => isDestroyed;

    private void Start()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke();
    }

    public void TakeDamage(int amount)
    {
        if (isDestroyed || amount <= 0)
            return;

        if (Time.time < nextDamageTime)
            return;

        currentHealth = Mathf.Max(
            0, currentHealth - amount
        );

        nextDamageTime =
            Time.time + invulnerabilityTime;

        Debug.Log(
            "Boat damaged! HP: " +
            currentHealth + "/" + maxHealth
        );

        OnHealthChanged?.Invoke();

        if (currentHealth <= 0)
            Die();
    }

    public void IncreaseMaxHealth(int amount)
    {
        if (isDestroyed || amount <= 0)
            return;

        maxHealth += amount;

        // Upgrade also increases current health.
        currentHealth = Mathf.Min(
            maxHealth,
            currentHealth + amount
        );

        OnHealthChanged?.Invoke();
    }

    private void Die()
    {
        isDestroyed = true;

        Debug.Log("GAME OVER - Boat destroyed!");

        BoatMovement movement =
            GetComponent<BoatMovement>();

        if (movement != null)
            movement.enabled = false;

        OnBoatDestroyed?.Invoke();
    }
}
