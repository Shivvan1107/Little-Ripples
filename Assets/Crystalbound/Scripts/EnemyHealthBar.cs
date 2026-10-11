using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        if (enemyHealth == null)
        {
            enemyHealth =
                GetComponentInParent<EnemyHealth>();
        }

        if (healthSlider == null)
        {
            healthSlider =
                GetComponentInChildren<Slider>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Start()
    {
        if (enemyHealth != null && healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue =
                enemyHealth.GetMaxHealth();

            healthSlider.value =
                enemyHealth.GetCurrentHealth();
        }
    }

    private void Update()
    {
        UpdateHealthBar();
        FaceCamera();
    }

    private void UpdateHealthBar()
    {
        if (enemyHealth == null)
            return;

        if (healthSlider == null)
            return;

        healthSlider.value =
            enemyHealth.GetCurrentHealth();
    }

    private void FaceCamera()
    {
        if (mainCamera == null)
            return;

        transform.rotation =
            Quaternion.LookRotation(
                transform.position -
                mainCamera.transform.position
            );
    }
}