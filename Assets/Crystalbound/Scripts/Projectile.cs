using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Impact")]
    [SerializeField] private GameObject impactEffect;

    private Transform target;
    private float speed;
    private float damage;
    private bool hasHit;

    public void Initialize(
        Transform newTarget,
        float newSpeed,
        float newDamage
    )
    {
        target = newTarget;
        speed = newSpeed;
        damage = newDamage;
    }

    private void Update()
    {
        if (hasHit)
            return;

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        Vector3 direction =
            target.position - transform.position;

        if (direction != Vector3.zero)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }

        float distance = Vector3.Distance(
            transform.position,
            target.position
        );

        if (distance <= 0.2f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (hasHit)
            return;

        hasHit = true;

        Vector3 impactPosition = target.position;

        EnemyHealth enemyHealth =
            target.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            enemyHealth =
                target.GetComponentInParent<EnemyHealth>();
        }

        if (enemyHealth != null)
        {
            Debug.Log(
                "Fire Bolt hit " +
                enemyHealth.gameObject.name +
                " for " +
                damage +
                " damage."
            );

            enemyHealth.TakeDamage(damage);
        }
        else
        {
            Debug.LogError(
                "Projectile hit " +
                target.name +
                " but EnemyHealth was not found."
            );
        }

        if (impactEffect != null)
        {
            Instantiate(
                impactEffect,
                impactPosition,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}