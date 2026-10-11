using UnityEngine;

public class SpellPickup : MonoBehaviour
{
    [Header("Spell")]
    [SerializeField] private Spell spellToUnlock;

    [Header("Pickup")]
    [SerializeField] private float interactionDistance = 2f;

    private Transform player;
    private bool collected;

    private void Start()
    {
        Debug.Log(
            "SPELL PICKUP STARTED: " +
            gameObject.name
        );

        // Find PlayerHealth anywhere in the scene.
        PlayerHealth playerHealth =
            FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
        {
            player = playerHealth.transform;

            Debug.Log(
                "SPELL PICKUP FOUND PLAYER: " +
                player.name
            );
        }
        else
        {
            Debug.LogError(
                "SPELL PICKUP ERROR: " +
                "Could not find PlayerHealth in the scene."
            );
        }
    }

    private void Update()
    {
        if (collected)
            return;

        if (player == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distance <= interactionDistance)
        {
            Debug.Log(
                "PLAYER IS IN PICKUP RANGE: " +
                distance
            );

            Collect();
        }
    }

    private void Collect()
    {
        if (collected)
            return;

        collected = true;

        Debug.Log(
            "COLLECTING SPELL: " +
            gameObject.name
        );

        if (spellToUnlock == null)
        {
            Debug.LogError(
                "SPELL PICKUP ERROR: " +
                "No Spell assigned."
            );

            collected = false;
            return;
        }

        SpellManager spellManager =
            player.GetComponent<SpellManager>();

        if (spellManager == null)
        {
            Debug.LogError(
                "SPELL PICKUP ERROR: " +
                "Player does not have SpellManager."
            );

            collected = false;
            return;
        }

        spellManager.UnlockSpell(
            spellToUnlock
        );

        Debug.Log(
            "SPELL UNLOCKED: " +
            spellToUnlock.spellName
        );

        Destroy(gameObject);
    }
}