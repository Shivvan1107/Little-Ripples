
using UnityEngine;
using TMPro;

public class FloatingScorePopup : MonoBehaviour
{
    [Header("References")]
    public TMP_Text scoreText;
    public Camera targetCamera;

    [Header("Animation")]
    public float floatSpeed = 1.5f;
    public float lifetime = 1.5f;
    public float popupScale = 1f;

    private Color originalColor;
    private Vector3 initialScale;
    private float timer;

    private void Awake()
    {
        if (scoreText == null)
            scoreText = GetComponentInChildren<TMP_Text>(true);

        if (targetCamera == null)
            targetCamera = Camera.main;

        initialScale = transform.localScale;
    }

    public void Initialize(int points)
    {
        if (scoreText == null)
        {
            Debug.LogError(
                "FloatingScorePopup: TMP Text missing!"
            );
            return;
        }

        scoreText.gameObject.SetActive(true);
        scoreText.text = "+" + points;

        originalColor = Color.yellow;
        originalColor.a = 1f;
        scoreText.color = originalColor;

        timer = 0f;
        transform.localScale =
            initialScale * popupScale;

        FaceCamera();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Move upward.
        transform.position +=
            Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out in the second half.
        if (scoreText != null)
        {
            float fadeStart = lifetime * 0.5f;

            float alpha = timer < fadeStart
                ? 1f
                : Mathf.InverseLerp(
                    lifetime,
                    fadeStart,
                    timer
                );

            Color color = originalColor;
            color.a = alpha;
            scoreText.color = color;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }

    private void LateUpdate()
    {
        // Rotate after the camera has moved.
        FaceCamera();
    }

    private void FaceCamera()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

        // Make the TextMeshPro object face
        // the camera at all times.
        transform.rotation =
            targetCamera.transform.rotation;
    }
}
