
using UnityEngine;
using TMPro;

public class EnvironmentUpgrade : MonoBehaviour
{
    [Header("Save Identification")]
    [Tooltip("Must be unique for every structure.")]
    public string structureID = "bridge_01";

    [Header("Structure Details")]
    public string structureName = "Wooden Bridge";

    [Min(0)]
    public int upgradeCost = 100;

    [Header("Visual Versions")]
    public GameObject brokenVersion;
    public GameObject restoredVersion;

    [Header("Interaction")]
    public Transform interactionPoint;

    [Header("Floating Upgrade Text")]
    public TMP_Text floatingUpgradeText;

    [Header("Visibility Settings")]
    public Transform player;
    public float textVisibilityDistance = 25f;

    [Header("Upgrade Status")]
    [SerializeField]
    private bool isRestored = false;

    public bool IsRestored => isRestored;

    public Vector3 InteractionPosition =>
        interactionPoint != null
            ? interactionPoint.position
            : transform.position;

    private void Awake()
    {
        ApplyVisualState();
        UpdateFloatingText();
    }

    private void Start()
    {
        LoadUpgradeState();
    }

    private void Update()
    {
        UpdateTextVisibility();
    }

    private void LoadUpgradeState()
    {
        if (SaveManager.Instance != null)
        {
            if (SaveManager.Instance.IsStructureRestored(
                structureID))
            {
                isRestored = true;
            }
        }

        ApplyVisualState();
        UpdateFloatingText();
    }

    public bool TryUpgrade(BoatTrashCollector collector)
    {
        if (isRestored || collector == null)
            return false;

        if (string.IsNullOrWhiteSpace(structureID))
        {
            Debug.LogError(
                "Missing structure ID on " + name
            );

            return false;
        }

        // Deduct coins through the existing wallet.
        if (!collector.SpendCoins(upgradeCost))
        {
            Debug.Log(
                "Not enough coins for " + structureName
            );

            return false;
        }

        isRestored = true;

        ApplyVisualState();
        UpdateFloatingText();

        // Record restoration permanently.
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveRestoredStructure(
                structureID
            );
        }

        Debug.Log(
            structureName + " restored and saved!"
        );

        return true;
    }

    private void ApplyVisualState()
    {
        if (brokenVersion != null)
            brokenVersion.SetActive(!isRestored);

        if (restoredVersion != null)
            restoredVersion.SetActive(isRestored);
    }

    private void UpdateFloatingText()
    {
        if (floatingUpgradeText == null)
            return;

        floatingUpgradeText.text =
            structureName +
            "\n" +
            upgradeCost + " Coins" +
            "\nPress E to Upgrade";

        UpdateTextVisibility();
    }

    private void UpdateTextVisibility()
    {
        if (floatingUpgradeText == null)
            return;

        if (isRestored)
        {
            floatingUpgradeText.gameObject.SetActive(false);
            return;
        }

        if (player == null)
        {
            floatingUpgradeText.gameObject.SetActive(true);
            return;
        }

        Vector3 difference =
            player.position - InteractionPosition;

        difference.y = 0f;

        bool isNearby =
            difference.sqrMagnitude <=
            textVisibilityDistance * textVisibilityDistance;

        floatingUpgradeText.gameObject.SetActive(isNearby);
    }
}
