
using UnityEngine;

public class PondWaterController : MonoBehaviour
{
    [Header("Water")]
    public Renderer waterRenderer;

    [Header("Cleaning Progress")]
    [Min(1)]
    public int trashToFullyClean = 50;

    [SerializeField]
    private int collectedTrash = 0;

    [Header("Color Transition")]
    public float transitionSpeed = 0.5f;

    private static readonly int PollutionID =
        Shader.PropertyToID("_PollutionAmount");

    private Material waterMaterial;
    private float currentPollution = 1f;
    private float targetPollution = 1f;

    public float CleanProgress =>
        Mathf.Clamp01(
            (float)collectedTrash /
            Mathf.Max(1, trashToFullyClean)
        );

    private void Start()
    {
        // Restore saved cleaning progress.
        if (SaveManager.Instance != null)
        {
            collectedTrash =
                SaveManager.Instance.SavedTrashCount;
        }

        if (waterRenderer == null)
        {
            Debug.LogError(
                "PondWaterController: Renderer missing!"
            );

            enabled = false;
            return;
        }

        waterMaterial = waterRenderer.material;

        if (!waterMaterial.HasProperty(PollutionID))
        {
            Debug.LogError(
                "Water shader missing _PollutionAmount"
            );

            enabled = false;
            return;
        }

        UpdateTarget();

        // Immediately restore the saved water color.
        currentPollution = targetPollution;

        waterMaterial.SetFloat(
            PollutionID,
            currentPollution
        );
    }

    public void OnTrashCollected()
    {
        collectedTrash++;

        // Save the cleaning progress.
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveTrashCount(
                collectedTrash
            );
        }

        UpdateTarget();

        Debug.Log(
            "Pond Cleanliness: " +
            Mathf.RoundToInt(CleanProgress * 100f) +
            "%"
        );
    }

    private void UpdateTarget()
    {
        targetPollution = 1f - CleanProgress;
    }

    private void Update()
    {
        if (waterMaterial == null)
            return;

        currentPollution = Mathf.MoveTowards(
            currentPollution,
            targetPollution,
            Mathf.Max(0f, transitionSpeed) *
            Time.deltaTime
        );

        waterMaterial.SetFloat(
            PollutionID,
            currentPollution
        );
    }

    private void OnDestroy()
    {
        if (waterMaterial != null)
            Destroy(waterMaterial);
    }
}
