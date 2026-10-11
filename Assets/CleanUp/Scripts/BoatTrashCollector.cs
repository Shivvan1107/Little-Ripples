
using UnityEngine;

public class BoatTrashCollector : MonoBehaviour
{
    [Header("Currency")]
    public int coins = 0;

    [Header("Collection Feedback")]
    public FloatingScorePopup scorePopupPrefab;
    public float popupHeight = 1.5f;

    [Header("Pond Cleaning")]
    public PondWaterController pondWater;

    private BoatHealth boatHealth;

    private void Awake()
    {
        boatHealth = GetComponentInParent<BoatHealth>();
    }

    private void Start()
    {
        // Restore the previously saved wallet.
        if (SaveManager.Instance != null)
        {
            coins = SaveManager.Instance.SavedCoins;

            Debug.Log(
                "Loaded coins: " + coins
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (boatHealth != null && boatHealth.IsDestroyed)
            return;

        TrashCollectible trash =
            other.GetComponentInParent<TrashCollectible>();

        if (trash == null)
            return;

        Vector3 collectionPosition =
            trash.transform.position;

        if (!trash.TryCollect(out int reward))
            return;

        // Add the currency.
        coins += reward;

        // Save coin balance immediately.
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveCoins(coins);
        }

        // Update pond water cleanliness.
        if (pondWater != null)
        {
            pondWater.OnTrashCollected();
        }

        // Spawn pickup feedback.
        if (scorePopupPrefab != null)
        {
            FloatingScorePopup popup = Instantiate(
                scorePopupPrefab,
                collectionPosition +
                Vector3.up * popupHeight,
                Quaternion.identity
            );

            popup.Initialize(reward);
        }

        Debug.Log(
            "Collected +" + reward +
            " Coins | Balance: " + coins
        );
    }

    public bool SpendCoins(int amount)
    {
        if (amount < 0 || coins < amount)
        {
            Debug.Log(
                "Not enough coins! Balance: " + coins
            );

            return false;
        }

        coins -= amount;

        // Save updated wallet after purchasing.
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveCoins(coins);
        }

        Debug.Log(
            "Spent " + amount +
            " Coins | Remaining: " + coins
        );

        return true;
    }
}
