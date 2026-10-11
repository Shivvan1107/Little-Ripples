
using UnityEngine;
using TMPro;

public class EnvironmentUpgradeInteractor : MonoBehaviour
{
    [Header("References")]
    public BoatTrashCollector collector;
    public BoatHealth boatHealth;

    [Header("UI")]
    public GameObject promptPanel;
    public TMP_Text promptText;

    [Header("Interaction Settings")]
    public float interactionRadius = 12f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private EnvironmentUpgrade[] upgrades;
    private EnvironmentUpgrade nearestUpgrade;

    private void Start()
    {
        if (collector == null)
            collector = GetComponentInChildren<BoatTrashCollector>();

        if (boatHealth == null)
            boatHealth = GetComponentInParent<BoatHealth>();

        // Include inactive structure objects in the search.
        upgrades = FindObjectsOfType<EnvironmentUpgrade>(true);

        if (promptPanel != null)
            promptPanel.SetActive(false);

        if (showDebugLogs)
        {
            Debug.Log(
                "Environment Interactor initialized. " +
                "Structures found: " + upgrades.Length
            );
        }
    }

    private void Update()
    {
        if (boatHealth != null && boatHealth.IsDestroyed)
        {
            HidePrompt();
            return;
        }

        FindNearestUpgrade();

        if (nearestUpgrade == null)
        {
            HidePrompt();

            if (Input.GetKeyDown(interactKey) &&
                showDebugLogs)
            {
                Debug.LogWarning(
                    "No available structure within " +
                    interactionRadius + " units."
                );
            }

            return;
        }

        UpdatePrompt();

        if (Input.GetKeyDown(interactKey))
            AttemptUpgrade();
    }

    private void FindNearestUpgrade()
    {
        nearestUpgrade = null;

        float closestDistance =
            interactionRadius * interactionRadius;

        foreach (EnvironmentUpgrade upgrade in upgrades)
        {
            if (upgrade == null ||
                !upgrade.isActiveAndEnabled ||
                upgrade.IsRestored)
                continue;

            Vector3 difference =
                upgrade.InteractionPosition -
                transform.position;

            difference.y = 0f;

            float sqrDistance = difference.sqrMagnitude;

            if (sqrDistance <= closestDistance)
            {
                closestDistance = sqrDistance;
                nearestUpgrade = upgrade;
            }
        }
    }

    private void AttemptUpgrade()
    {
        if (nearestUpgrade == null)
            return;

        if (collector == null)
        {
            Debug.LogError(
                "EnvironmentUpgradeInteractor: " +
                "BoatTrashCollector missing!"
            );
            return;
        }

        if (showDebugLogs)
        {
            Debug.Log(
                "Attempting upgrade: " +
                nearestUpgrade.structureName +
                " | Cost: " + nearestUpgrade.upgradeCost +
                " | Coins: " + collector.coins
            );
        }

        bool success = nearestUpgrade.TryUpgrade(collector);

        if (success)
        {
            if (showDebugLogs)
                Debug.Log("Structure upgraded successfully!");

            nearestUpgrade = null;
            HidePrompt();
        }
        else
        {
            Debug.LogWarning(
                "Upgrade failed. Check coins, " +
                "structure ID, and saved state."
            );
        }
    }

    private void UpdatePrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(true);

        if (promptText == null)
            return;

        int balance = collector != null ? collector.coins : 0;
        int cost = nearestUpgrade.upgradeCost;

        if (balance >= cost)
        {
            promptText.text =
                nearestUpgrade.structureName +
                "\nCost: " + cost + " Coins" +
                "\nPress E to Upgrade";
        }
        else
        {
            promptText.text =
                nearestUpgrade.structureName +
                "\nCost: " + cost + " Coins" +
                "\nNeed " + (cost - balance) +
                " More Coins";
        }
    }

    private void HidePrompt()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}
