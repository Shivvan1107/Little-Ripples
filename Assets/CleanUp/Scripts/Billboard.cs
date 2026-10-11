
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Camera")]
    public Camera targetCamera;

    [Header("Billboard Settings")]
    public bool fullBillboard = true;

    private void LateUpdate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

        if (fullBillboard)
        {
            // Matches the camera's rotation.
            // Best for floating UI and TextMeshPro.
            transform.rotation =
                targetCamera.transform.rotation;
        }
        else
        {
            // Y-axis only billboard.
            // Remains upright while facing the camera.
            Vector3 direction =
                transform.position -
                targetCamera.transform.position;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation =
                    Quaternion.LookRotation(
                        direction,
                        Vector3.up
                    );
            }
        }
    }
}
