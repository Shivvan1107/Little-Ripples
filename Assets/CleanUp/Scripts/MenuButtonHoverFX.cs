using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButtonHoverFX : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Image targetImage;
    public Outline targetOutline;
    public TMP_Text targetText;

    [Header("Normal State")]
    public Color normalImageColor = new Color(0.07f, 0.16f, 0.20f, 0.95f);
    public Color normalOutlineColor = new Color(0.44f, 0.96f, 1f, 0.25f);
    public Color normalTextColor = new Color(0.90f, 1f, 1f, 1f);

    [Header("Hover State")]
    public Color hoverImageColor = new Color(0.10f, 0.24f, 0.28f, 1f);
    public Color hoverOutlineColor = new Color(0.44f, 0.96f, 1f, 0.65f);
    public Color hoverTextColor = new Color(1f, 1f, 1f, 1f);

    [Header("Scale")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.03f, 1.03f, 1.03f);

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (targetOutline == null)
            targetOutline = GetComponent<Outline>();

        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>();

        ApplyNormalState();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetImage != null)
            targetImage.color = hoverImageColor;

        if (targetOutline != null)
            targetOutline.effectColor = hoverOutlineColor;

        if (targetText != null)
            targetText.color = hoverTextColor;

        if (rectTransform != null)
            rectTransform.localScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ApplyNormalState();
    }

    private void ApplyNormalState()
    {
        if (targetImage != null)
            targetImage.color = normalImageColor;

        if (targetOutline != null)
            targetOutline.effectColor = normalOutlineColor;

        if (targetText != null)
            targetText.color = normalTextColor;

        if (rectTransform != null)
            rectTransform.localScale = normalScale;
    }
}