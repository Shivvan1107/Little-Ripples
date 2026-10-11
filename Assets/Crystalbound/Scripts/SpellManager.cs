using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    [Header("Starting Spell")]
    [SerializeField] private Spell startingSpell;

    private List<Spell> unlockedSpells =
        new List<Spell>();

    private Spell currentSpell;

    private void Awake()
    {
        if (startingSpell != null)
        {
            UnlockSpell(startingSpell);
            currentSpell = startingSpell;
        }
    }

    public void UnlockSpell(Spell spell)
    {
        if (spell == null)
            return;

        if (unlockedSpells.Contains(spell))
            return;

        unlockedSpells.Add(spell);

        Debug.Log(
            "Unlocked Spell: " +
            spell.spellName
        );
    }

    public bool IsSpellUnlocked(Spell spell)
    {
        if (spell == null)
            return false;

        return unlockedSpells.Contains(spell);
    }

    public void SelectSpell(Spell spell)
    {
        if (spell == null)
            return;

        if (!IsSpellUnlocked(spell))
        {
            Debug.Log(
                "Spell is locked: " +
                spell.spellName
            );

            return;
        }

        currentSpell = spell;

        Debug.Log(
            "Selected Spell: " +
            spell.spellName
        );
    }

    public Spell GetCurrentSpell()
    {
        return currentSpell;
    }

    public List<Spell> GetUnlockedSpells()
    {
        return unlockedSpells;
    }
}