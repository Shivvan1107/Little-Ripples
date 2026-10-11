using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class MainMenuPanelPulse : MonoBehaviour
{
    [Header("Panel References")]
    public Image panelImage;
    public Outline panelOutline;
    public Shadow panelShadow;

    [Header("Pulse Scale")]
    public bool animateScale = true;
    public float pulseSpeed = 1.1f;
    public float pulseAmount = 0.025f;

    [Header("Glow Animation")]
    public bool animateGlow = true;
    public float glowSpeed = 1.4f;

    [Header("Panel Alpha")]
    [Range(0f, 1f)] public float minPanelAlpha = 0.82f;
    [Range(0f, 1f)] public float maxPanelAlpha = 0.95f;

    [Header("Glow Alpha")]
    [Range(0f, 1f)] public float minOutlineAlpha = 0.18f;
    [Range(0f, 1f)] public float maxOutlineAlpha = 0.45f;

    [Range(0f, 1f)] public float minShadowAlpha = 0.08f;
    [Range(0f, 1f)] public float maxShadowAlpha = 0.25f;

    [Header("Glow Colors")]
    public Color outlineGlowColor = new Color(0.44f, 0.96f, 1f, 1f);
    public Color shadowGlowColor = new Color(0.44f, 0.96f, 1f, 1f);

    private RectTransform rectTransform;
    private Vector3 baseScale;
    private Color basePanelColor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (panelImage == null)
            panelImage = GetComponent<Image>();

        if (panelOutline == null)
            panelOutline = GetComponent<Outline>();

        if (panelShadow == null)
            panelShadow = GetComponent<Shadow>();

        baseScale = rectTransform.localScale;

        if (panelImage != null)
            basePanelColor = panelImage.color;
    }

    private void OnEnable()
    {
        if (rectTransform != null)
            rectTransform.localScale = baseScale;
    }

    private void Update()
    {
        float pulse = (Mathf.Sin(Time.unscaledTime * pulseSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
        float glow = (Mathf.Sin(Time.unscaledTime * glowSpeed * Mathf.PI * 2f) + 1f) * 0.5f;

        if (animateScale && rectTransform != null)
        {
            float scaleMultiplier = 1f + Mathf.Lerp(-pulseAmount, pulseAmount, pulse);
            rectTransform.localScale = baseScale * scaleMultiplier;
        }

        if (panelImage != null)
        {
            Color panelColor = basePanelColor;
            panelColor.a = Mathf.Lerp(minPanelAlpha, maxPanelAlpha, glow);
            panelImage.color = panelColor;
        }

        if (animateGlow && panelOutline != null)
        {
            Color outlineColor = outlineGlowColor;
            outlineColor.a = Mathf.Lerp(minOutlineAlpha, maxOutlineAlpha, glow);
            panelOutline.effectColor = outlineColor;
        }

        if (animateGlow && panelShadow != null)
        {
            Color shadowColor = shadowGlowColor;
            shadowColor.a = Mathf.Lerp(minShadowAlpha, maxShadowAlpha, glow);
            panelShadow.effectColor = shadowColor;
        }
    }
}