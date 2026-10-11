
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoatHealthUI : MonoBehaviour
{
    public BoatHealth boatHealth;
    public Slider healthSlider;
    public TMP_Text healthText;

    private void OnEnable()
    {
        if (boatHealth != null)
            boatHealth.OnHealthChanged += UpdateHealth;

        UpdateHealth();
    }

    private void OnDisable()
    {
        if (boatHealth != null)
            boatHealth.OnHealthChanged -= UpdateHealth;
    }

    private void UpdateHealth()
    {
        if (boatHealth == null)
            return;

        if (healthSlider != null)
        {
            healthSlider.maxValue = boatHealth.maxHealth;
            healthSlider.value = boatHealth.currentHealth;
        }

        if (healthText != null)
        {
            healthText.text =
                boatHealth.currentHealth +
                " / " + boatHealth.maxHealth + " HP";
        }
    }
}
