
using System.Collections.Generic;
using UnityEngine;

public class CameraObstructionFader : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public LayerMask obstructionLayers;

    [Header("Fade Settings")]
    [Range(0.05f, 1f)]
    public float fadedAlpha = 0.25f;
    public float fadeSpeed = 6f;

    [Header("Detection")]
    public float sphereRadius = 0.35f;

    private class FadeObject
    {
        public Renderer renderer;
        public Material[] originals;
        public Material[] fadeMaterials;
        public float alpha = 1f;
        public bool obstructing;
    }

    private readonly Dictionary<Renderer, FadeObject>
        tracked = new Dictionary<Renderer, FadeObject>();

    private readonly List<Renderer> removal =
        new List<Renderer>();

    private void LateUpdate()
    {
        if (player == null)
            return;

        foreach (FadeObject obj in tracked.Values)
            obj.obstructing = false;

        Vector3 direction =
            player.position - transform.position;

        float distance = direction.magnitude;

        if (distance > 0.01f)
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                sphereRadius,
                direction.normalized,
                distance,
                obstructionLayers,
                QueryTriggerInteraction.Ignore
            );

            foreach (RaycastHit hit in hits)
            {
                Renderer renderer =
                    hit.collider.GetComponent<Renderer>();

                if (renderer == null)
                    renderer =
                        hit.collider.GetComponentInChildren<Renderer>();

                if (renderer == null)
                    renderer =
                        hit.collider.GetComponentInParent<Renderer>();

                if (renderer == null)
                    continue;

                if (!tracked.ContainsKey(renderer))
                    Register(renderer);

                if (tracked.TryGetValue(renderer, out FadeObject obj))
                    obj.obstructing = true;
            }
        }

        removal.Clear();

        foreach (var pair in tracked)
        {
            FadeObject obj = pair.Value;

            if (obj.renderer == null)
            {
                removal.Add(pair.Key);
                continue;
            }

            float target = obj.obstructing
                ? fadedAlpha
                : 1f;

            obj.alpha = Mathf.MoveTowards(
                obj.alpha,
                target,
                fadeSpeed * Time.deltaTime
            );

            foreach (Material material in obj.fadeMaterials)
            {
                if (material == null)
                    continue;

                if (material.HasProperty("_BaseColor"))
                {
                    Color c =
                        material.GetColor("_BaseColor");
                    c.a = obj.alpha;
                    material.SetColor("_BaseColor", c);
                }
                else if (material.HasProperty("_Color"))
                {
                    Color c =
                        material.GetColor("_Color");
                    c.a = obj.alpha;
                    material.SetColor("_Color", c);
                }
            }

            if (!obj.obstructing &&
                obj.alpha >= 0.999f)
            {
                obj.renderer.sharedMaterials =
                    obj.originals;

                foreach (Material material in obj.fadeMaterials)
                {
                    if (material != null)
                        Destroy(material);
                }

                removal.Add(pair.Key);
            }
        }

        foreach (Renderer renderer in removal)
            tracked.Remove(renderer);
    }

    private void Register(Renderer renderer)
    {
        FadeObject obj = new FadeObject();

        obj.renderer = renderer;
        obj.originals = renderer.sharedMaterials;
        obj.fadeMaterials =
            new Material[obj.originals.Length];

        for (int i = 0; i < obj.originals.Length; i++)
        {
            Material source = obj.originals[i];

            if (source == null)
                continue;

            Material copy = new Material(source);

            // URP Lit material transparency.
            if (copy.HasProperty("_Surface"))
            {
                copy.SetFloat("_Surface", 1f);
                copy.SetFloat("_ZWrite", 0f);

                copy.SetOverrideTag(
                    "RenderType", "Transparent"
                );

                copy.renderQueue = 3000;

                copy.EnableKeyword(
                    "_SURFACE_TYPE_TRANSPARENT"
                );

                // Standard alpha blending.
                copy.SetInt("_SrcBlend", 5);
                copy.SetInt("_DstBlend", 10);
            }

            obj.fadeMaterials[i] = copy;
        }

        renderer.sharedMaterials = obj.fadeMaterials;

        tracked.Add(renderer, obj);
    }

    private void OnDisable()
    {
        foreach (FadeObject obj in tracked.Values)
        {
            if (obj.renderer != null)
                obj.renderer.sharedMaterials =
                    obj.originals;

            foreach (Material material in obj.fadeMaterials)
            {
                if (material != null)
                    Destroy(material);
            }
        }

        tracked.Clear();
    }
}
