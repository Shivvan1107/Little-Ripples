using UnityEngine;
using UnityEngine.UI;

public class SpellSelectionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpellManager spellManager;

    [SerializeField] private Spell fireBolt;
    [SerializeField] private Spell iceShard;

    [Header("Buttons")]
    [SerializeField] private Button fireBoltButton;
    [SerializeField] private Button iceShardButton;

    [Header("Selected Button Color")]
    [SerializeField] private Color selectedColor = Color.yellow;

    private Color fireBoltOriginalColor;
    private Color iceShardOriginalColor;

    private void Awake()
    {
        if (spellManager == null)
        {
            spellManager =
                FindObjectOfType<SpellManager>();
        }

        if (fireBoltButton != null)
        {
            fireBoltOriginalColor =
                fireBoltButton.image.color;

            fireBoltButton.onClick.RemoveAllListeners();

            fireBoltButton.onClick.AddListener(
                SelectFireBolt
            );
        }

        if (iceShardButton != null)
        {
            iceShardOriginalColor =
                iceShardButton.image.color;

            iceShardButton.onClick.RemoveAllListeners();

            iceShardButton.onClick.AddListener(
                SelectIceShard
            );
        }
    }

    private void Start()
    {
        if (spellManager == null)
        {
            Debug.LogError(
                "SpellSelectionUI: SpellManager not found."
            );

            return;
        }

        if (fireBolt == null)
        {
            Debug.LogError(
                "SpellSelectionUI: Fire Bolt is not assigned."
            );
        }

        if (iceShard == null)
        {
            Debug.LogError(
                "SpellSelectionUI: Ice Shard is not assigned."
            );
        }

        if (fireBoltButton == null)
        {
            Debug.LogError(
                "SpellSelectionUI: Fire Bolt Button is not assigned."
            );
        }

        if (iceShardButton == null)
        {
            Debug.LogError(
                "SpellSelectionUI: Ice Shard Button is not assigned."
            );
        }

        UpdateButtons();

        Spell currentSpell =
            spellManager.GetCurrentSpell();

        if (currentSpell != null)
        {
            UpdateSelectedVisual(
                currentSpell
            );
        }
    }

    private void Update()
    {
        UpdateButtons();
    }

    private void SelectFireBolt()
    {
        Debug.Log("FIRE BOLT BUTTON PRESSED");

        if (spellManager == null)
            return;

        if (fireBolt == null)
        {
            Debug.LogError(
                "Fire Bolt spell is not assigned."
            );

            return;
        }

        if (!spellManager.IsSpellUnlocked(
                fireBolt))
        {
            Debug.Log(
                "Fire Bolt is currently locked."
            );

            return;
        }

        spellManager.SelectSpell(
            fireBolt
        );

        Debug.Log(
            "CURRENT SPELL: FIRE BOLT"
        );

        UpdateSelectedVisual(
            fireBolt
        );
    }

    private void SelectIceShard()
    {
        Debug.Log("ICE SHARD BUTTON PRESSED");

        if (spellManager == null)
            return;

        if (iceShard == null)
        {
            Debug.LogError(
                "Ice Shard spell is not assigned."
            );

            return;
        }

        if (!spellManager.IsSpellUnlocked(
                iceShard))
        {
            Debug.Log(
                "Ice Shard is currently locked."
            );

            return;
        }

        spellManager.SelectSpell(
            iceShard
        );

        Debug.Log(
            "CURRENT SPELL: ICE SHARD"
        );

        UpdateSelectedVisual(
            iceShard
        );
    }

    private void UpdateButtons()
    {
        if (spellManager == null)
            return;

        if (fireBoltButton != null)
        {
            fireBoltButton.interactable =
                fireBolt != null &&
                spellManager.IsSpellUnlocked(
                    fireBolt
                );
        }

        if (iceShardButton != null)
        {
            iceShardButton.interactable =
                iceShard != null &&
                spellManager.IsSpellUnlocked(
                    iceShard
                );
        }
    }

    private void UpdateSelectedVisual(
        Spell selectedSpell
    )
    {
        if (fireBoltButton != null)
        {
            fireBoltButton.image.color =
                selectedSpell == fireBolt
                    ? selectedColor
                    : fireBoltOriginalColor;
        }

        if (iceShardButton != null)
        {
            iceShardButton.image.color =
                selectedSpell == iceShard
                    ? selectedColor
                    : iceShardOriginalColor;
        }
    }
}