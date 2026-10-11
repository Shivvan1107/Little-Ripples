
using UnityEngine;
using TMPro;

public class BoatScoreUI : MonoBehaviour
{
    [Header("References")]
    public BoatTrashCollector collector;
    public TMP_Text coinsText;

    private void Start()
    {
        if (collector == null)
        {
            Debug.LogError(
                "BoatScoreUI: Collector is not assigned!"
            );
        }

        if (coinsText == null)
        {
            Debug.LogError(
                "BoatScoreUI: Coins Text is not assigned!"
            );
        }
    }

    private void Update()
    {
        if (collector == null || coinsText == null)
            return;

        coinsText.text = "Coins: " + collector.coins;
    }
}
