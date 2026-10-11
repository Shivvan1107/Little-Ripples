
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Explosion")]
    public int damage = 25;
    public GameObject explosionEffect;
    public AudioClip explosionSound;

    [Header("Visuals")]
    public float effectLifetime = 3f;

    private bool hasExploded;

    private void OnTriggerEnter(Collider other)
    {
        if (hasExploded)
            return;

        BoatHealth boat =
            other.GetComponentInParent<BoatHealth>();

        if (boat == null || boat.IsDestroyed)
            return;

        Explode(boat);
    }

    private void Explode(BoatHealth boat)
    {
        hasExploded = true;

        Vector3 explosionPosition = transform.position;

        // Damage the player.
        boat.TakeDamage(damage);

        // Spawn explosion effect.
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(
                explosionEffect,
                explosionPosition,
                Quaternion.identity
            );

            Destroy(effect, effectLifetime);
        }

        // Play sound.
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(
                explosionSound,
                explosionPosition
            );
        }

        Debug.Log("BOOM! Explosive barrel triggered!");

        Destroy(gameObject);
    }
}
