
using UnityEngine;

public class BoatHealthUpgrade : MonoBehaviour
{
    [Header("References")]
    public BoatHealth boatHealth;
    public BoatTrashCollector collector;

    [Header("Upgrade Settings")]
    public int upgradeCost = 50;
    public int healthIncrease = 25;
    public float costMultiplier = 1.5f;

    public int upgradeLevel = 0;

    private void Update()
    {
        // Temporary shop testing key.
        if (Input.GetKeyDown(KeyCode.U))
            PurchaseHealthUpgrade();
    }

    public void PurchaseHealthUpgrade()
    {
        if (boatHealth == null || collector == null)
            return;

        if (boatHealth.IsDestroyed)
            return;

        if (collector.coins < upgradeCost)
        {
            Debug.Log(
                "Not enough coins! Need " +
                upgradeCost
            );
            return;
        }

        if (!collector.SpendCoins(upgradeCost))
            return;

        boatHealth.IncreaseMaxHealth(healthIncrease);
        upgradeLevel++;

        upgradeCost = Mathf.CeilToInt(
            upgradeCost * Mathf.Max(1f, costMultiplier)
        );

        Debug.Log(
            "Health upgraded! Level: " +
            upgradeLevel +
            " | Max HP: " + boatHealth.maxHealth
        );
    }
}
