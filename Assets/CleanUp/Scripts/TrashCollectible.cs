
using UnityEngine;

public class TrashCollectible : MonoBehaviour
{
    [Header("Collection")]
    public int points = 10;

    [Header("Feedback")]
    public ParticleSystem collectionParticles;
    public AudioClip collectionSound;

    private bool isCollected;

    public bool TryCollect(out int collectedPoints)
    {
        collectedPoints = 0;

        if (isCollected)
            return false;

        isCollected = true;
        collectedPoints = points;

        // Spawn independent particles so they remain after
        // the trash object is destroyed.
        if (collectionParticles != null)
        {
            ParticleSystem particles = Instantiate(
                collectionParticles,
                transform.position,
                Quaternion.identity
            );

            particles.Play();

            Destroy(
                particles.gameObject,
                particles.main.duration +
                particles.main.startLifetime.constantMax
            );
        }

        if (collectionSound != null)
        {
            AudioSource.PlayClipAtPoint(
                collectionSound,
                transform.position
            );
        }

        Destroy(gameObject);

        return true;
    }
}
