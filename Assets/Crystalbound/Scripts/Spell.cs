using UnityEngine;

[CreateAssetMenu(
    fileName = "New Spell",
    menuName = "Crystalbound/Spell"
)]
public class Spell : ScriptableObject
{
    [Header("Spell Info")]
    public string spellName;

    [TextArea(2, 4)]
    public string description;

    [Header("Combat")]
    public float damage = 25f;
    public float cooldown = 0.5f;
    public float manaCost = 10f;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
}