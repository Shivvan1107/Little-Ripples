using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("References")]
    [SerializeField] private PlayerHitFeedback hitFeedback;

    private float currentHealth;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;

        if (hitFeedback == null)
        {
            hitFeedback =
                GetComponent<PlayerHitFeedback>();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        damage = Mathf.Max(0f, damage);

        currentHealth -= damage;
        currentHealth = Mathf.Max(
            currentHealth,
            0f
        );

        Debug.Log(
            "Player took " + damage +
            " damage. HP: " +
            currentHealth + "/" +
            maxHealth
        );

        if (hitFeedback != null)
        {
            hitFeedback.PlayHitFeedback();
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("PLAYER DIED!");

        gameObject.SetActive(false);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }
}