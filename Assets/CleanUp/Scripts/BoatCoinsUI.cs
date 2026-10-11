
using UnityEngine;
using TMPro;

public class BoatCoinsUI : MonoBehaviour
{
    public BoatTrashCollector collector;
    public TMP_Text coinsText;

    private void Update()
    {
        if (collector == null || coinsText == null)
            return;

        coinsText.text = "Coins: " + collector.coins;
    }
}
