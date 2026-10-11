using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    [Header("Target Indicator")]
    [SerializeField] private GameObject targetIndicator;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 4f;
    [SerializeField] private float pulseAmount = 0.12f;

    private bool isSelected;
    private Vector3 originalScale;

    private void Awake()
    {
        if (targetIndicator != null)
        {
            targetIndicator.SetActive(false);
            originalScale = targetIndicator.transform.localScale;
        }
    }

    private void Update()
    {
        if (!isSelected)
            return;

        if (targetIndicator == null)
            return;

        PulseIndicator();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (targetIndicator == null)
            return;

        targetIndicator.SetActive(selected);

        if (selected)
        {
            originalScale = targetIndicator.transform.localScale;
        }
        else
        {
            targetIndicator.transform.localScale =
                originalScale;
        }
    }

    private void PulseIndicator()
    {
        float pulse =
            1f +
            Mathf.Sin(Time.time * pulseSpeed) *
            pulseAmount;

        targetIndicator.transform.localScale =
            originalScale * pulse;
    }

    public bool IsSelected()
    {
        return isSelected;
    }
}