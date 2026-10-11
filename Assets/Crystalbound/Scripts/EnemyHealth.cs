using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Hit Feedback")]
    [SerializeField] private Renderer enemyRenderer;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Knockback")]
    [SerializeField] private float knockbackDistance = 1f;
    [SerializeField] private float knockbackDuration = 0.25f;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathVFX;
    [SerializeField] private float deathDelay = 0.5f;

    private float currentHealth;
    private bool isDead;

    private Material enemyMaterial;
    private Color originalColor;

    private Coroutine flashCoroutine;
    private Coroutine knockbackCoroutine;

    private Transform player;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;

        if (enemyRenderer == null)
        {
            enemyRenderer =
                GetComponentInChildren<Renderer>();
        }

        if (enemyRenderer != null)
        {
            enemyMaterial =
                enemyRenderer.material;

            originalColor =
                enemyMaterial.color;
        }

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        if (deathVFX != null)
        {
            deathVFX.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        damage = Mathf.Max(0f, damage);

        currentHealth -= damage;

        currentHealth =
            Mathf.Max(currentHealth, 0f);

        Debug.Log(
            gameObject.name +
            " took " +
            damage +
            " damage. Remaining HP: " +
            currentHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        PlayHitFlash();
        PlayKnockback();
    }

    private void PlayHitFlash()
    {
        if (enemyMaterial == null)
            return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine =
            StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        enemyMaterial.color = Color.white;

        yield return new WaitForSeconds(
            flashDuration
        );

        if (enemyMaterial != null)
        {
            enemyMaterial.color =
                originalColor;
        }

        flashCoroutine = null;
    }

    private void PlayKnockback()
    {
        if (player == null)
        {
            return;
        }

        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
        }

        Vector3 direction =
            transform.position - player.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        direction.Normalize();

        knockbackCoroutine =
            StartCoroutine(
                KnockbackRoutine(direction)
            );
    }

    private IEnumerator KnockbackRoutine(
        Vector3 direction
    )
    {
        Vector3 startPosition =
            transform.position;

        Vector3 targetPosition =
            startPosition +
            direction * knockbackDistance;

        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / knockbackDuration
                );

            float smoothT =
                Mathf.SmoothStep(0f, 1f, t);

            transform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    smoothT
                );

            yield return null;
        }

        transform.position =
            targetPosition;

        knockbackCoroutine = null;
    }

    public bool IsBeingKnockedBack()
    {
        return knockbackCoroutine != null;
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(
            gameObject.name +
            " DIED!"
        );

        // Stop enemy movement immediately.
        EnemyAI enemyAI =
            GetComponent<EnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        // Disable targeting.
        EnemyTarget enemyTarget =
            GetComponent<EnemyTarget>();

        if (enemyTarget != null)
        {
            enemyTarget.SetSelected(false);
            enemyTarget.enabled = false;
        }

        // Play death VFX.
        if (deathVFX != null)
        {
            deathVFX.gameObject.SetActive(true);

            deathVFX.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            deathVFX.Play();
        }

        // Hide the enemy visuals.
        if (enemyRenderer != null)
        {
            enemyRenderer.enabled = false;
        }

        // Destroy after VFX.
        Destroy(
            gameObject,
            deathDelay
        );
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