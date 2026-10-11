
using UnityEngine;
using TMPro;

public class PondCleanlinessUI : MonoBehaviour
{
    public PondWaterController pondWater;
    public TMP_Text cleanlinessText;

    private void Update()
    {
        if (pondWater == null ||
            cleanlinessText == null)
            return;

        float percentage =
            pondWater.CleanProgress * 100f;

        cleanlinessText.text =
            "Pond Cleanled: " +
            Mathf.RoundToInt(percentage) + "%";
    }
}
