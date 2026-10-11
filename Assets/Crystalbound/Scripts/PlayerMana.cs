using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [Header("Mana")]
    [SerializeField] private float maxMana = 100f;

    private float currentMana;

    private void Awake()
    {
        currentMana = maxMana;
    }

    public bool HasEnoughMana(float amount)
    {
        return currentMana >= amount;
    }

    public void SpendMana(float amount)
    {
        if (amount <= 0f)
            return;

        currentMana -= amount;
        currentMana = Mathf.Max(currentMana, 0f);

        Debug.Log(
            "Mana spent: " + amount +
            " | Mana: " +
            currentMana + "/" +
            maxMana
        );
    }

    public void RestoreMana(float amount)
    {
        if (amount <= 0f)
            return;

        currentMana += amount;
        currentMana =
            Mathf.Min(currentMana, maxMana);

        Debug.Log(
            "Mana restored: " + amount +
            " | Mana: " +
            currentMana + "/" +
            maxMana
        );
    }

    public float GetCurrentMana()
    {
        return currentMana;
    }

    public float GetMaxMana()
    {
        return maxMana;
    }
}