using UnityEngine;

public class PlayerSpellCaster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerTargeting playerTargeting;
    [SerializeField] private PlayerMana playerMana;
    [SerializeField] private SpellManager spellManager;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("VFX")]
    [SerializeField] private ParticleSystem castVFX;

    [Header("Facing")]
    [SerializeField] private float rotationSpeed = 12f;

    private float nextCastTime;

    private void Awake()
    {
        if (playerTargeting == null)
        {
            playerTargeting =
                GetComponent<PlayerTargeting>();
        }

        if (playerMana == null)
        {
            playerMana =
                GetComponent<PlayerMana>();
        }

        if (spellManager == null)
        {
            spellManager =
                GetComponent<SpellManager>();
        }
    }

    public void CastSpell()
    {
        if (spellManager == null)
        {
            Debug.LogError(
                "SpellManager is missing."
            );

            return;
        }

        Spell currentSpell =
            spellManager.GetCurrentSpell();

        if (currentSpell == null)
        {
            Debug.LogError(
                "No spell is currently selected."
            );

            return;
        }

        if (Time.time < nextCastTime)
        {
            return;
        }

        if (playerTargeting == null)
        {
            Debug.LogError(
                "PlayerTargeting is missing."
            );

            return;
        }

        if (playerMana == null)
        {
            Debug.LogError(
                "PlayerMana is missing."
            );

            return;
        }

        EnemyTarget target =
            playerTargeting.GetCurrentTarget();

        if (target == null)
        {
            Debug.Log(
                "No enemy selected."
            );

            return;
        }

        if (!playerMana.HasEnoughMana(
                currentSpell.manaCost))
        {
            Debug.Log(
                "Not enough mana!"
            );

            return;
        }

        if (currentSpell.projectilePrefab == null)
        {
            Debug.LogError(
                "Projectile Prefab is missing from " +
                currentSpell.spellName
            );

            return;
        }

        if (projectileSpawnPoint == null)
        {
            Debug.LogError(
                "Projectile Spawn Point is missing."
            );

            return;
        }

        FaceTarget(
            target.transform
        );

        PlayCastVFX();

        GameObject projectileObject =
            Instantiate(
                currentSpell.projectilePrefab,
                projectileSpawnPoint.position,
                Quaternion.identity
            );

        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError(
                "Projectile prefab does not contain Projectile component."
            );

            Destroy(projectileObject);
            return;
        }

        projectile.Initialize(
            target.transform,
            currentSpell.projectileSpeed,
            currentSpell.damage
        );

        playerMana.SpendMana(
            currentSpell.manaCost
        );

        nextCastTime =
            Time.time +
            currentSpell.cooldown;

        Debug.Log(
            "Cast " +
            currentSpell.spellName
        );
    }

    private void FaceTarget(
        Transform target
    )
    {
        if (target == null)
            return;

        Vector3 direction =
            target.position -
            transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed *
                Time.deltaTime
            );
    }

    private void PlayCastVFX()
    {
        if (castVFX == null)
            return;

        castVFX.gameObject.SetActive(true);

        castVFX.Stop(
            true,
            ParticleSystemStopBehavior
                .StopEmittingAndClear
        );

        castVFX.Play();
    }
}