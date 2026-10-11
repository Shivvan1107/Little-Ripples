using System.Collections;
using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer playerRenderer;

    [Header("Feedback")]
    [SerializeField] private float flashDuration = 0.12f;

    private Material playerMaterial;
    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (playerRenderer == null)
        {
            playerRenderer =
                GetComponentInChildren<Renderer>();
        }

        if (playerRenderer != null)
        {
            playerMaterial =
                playerRenderer.material;

            originalColor =
                playerMaterial.color;
        }
    }

    public void PlayHitFeedback()
    {
        if (playerMaterial == null)
            return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine =
            StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        playerMaterial.color = Color.red;

        yield return new WaitForSeconds(
            flashDuration
        );

        playerMaterial.color = originalColor;

        flashCoroutine = null;
    }
}