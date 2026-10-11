using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 7f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 2.2f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Chase")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float chaseRange = 10f;

    private float nextAttackTime;

    private EnemyHealth enemyHealth;

    private bool isEngaged;

    private void Awake()
    {
        enemyHealth =
            GetComponent<EnemyHealth>();

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player =
                    playerObject.transform;
            }
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        if (enemyHealth != null)
        {
            if (enemyHealth.IsDead())
                return;

            if (enemyHealth.IsBeingKnockedBack())
                return;
        }

        PlayerHealth playerHealth =
            player.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        if (playerHealth.IsDead())
            return;

        float distanceToPlayer =
            Vector3.Distance(
                transform.position,
                player.position
            );

        // --------------------------------
        // PLAYER OUTSIDE DETECTION RANGE
        // --------------------------------

        if (!isEngaged)
        {
            if (distanceToPlayer <= detectionRange)
            {
                isEngaged = true;

                Debug.Log(
                    gameObject.name +
                    " detected the player!"
                );
            }
            else
            {
                return;
            }
        }

        // --------------------------------
        // PLAYER TOO FAR AWAY
        // --------------------------------

        if (distanceToPlayer > chaseRange)
        {
            isEngaged = false;

            Debug.Log(
                gameObject.name +
                " stopped chasing the player."
            );

            return;
        }

        // --------------------------------
        // ATTACK
        // --------------------------------

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
            return;
        }

        // --------------------------------
        // CHASE
        // --------------------------------

        ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (player == null)
            return;

        Vector3 direction =
            player.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();

        transform.position +=
            direction *
            moveSpeed *
            Time.deltaTime;

        transform.rotation =
            Quaternion.LookRotation(direction);
    }

    private void AttackPlayer()
    {
        if (Time.time < nextAttackTime)
            return;

        if (player == null)
            return;

        PlayerHealth playerHealth =
            player.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        Debug.Log(
            gameObject.name +
            " attacks the player!"
        );

        playerHealth.TakeDamage(
            attackDamage
        );

        nextAttackTime =
            Time.time +
            attackCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );

        // Attack range
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );

        // Chase range
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(
            transform.position,
            chaseRange
        );
    }
}