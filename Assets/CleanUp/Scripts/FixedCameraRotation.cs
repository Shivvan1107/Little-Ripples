
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(200)]
public class FixedCameraRotation : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Camera Offset")]
    public Vector3 cameraOffset =
        new Vector3(-15f, 18f, -20f);

    [Header("Camera Follow")]
    public float followSmoothTime = 0.15f;

    [Header("Mouse Rotation")]
    public float rotationSensitivity = 3f;
    public bool invertMouse = false;

    [Header("Camera Look")]
    public float lookHeight = 1f;

    [Header("Debug")]
    public KeyCode resetCameraKey = KeyCode.R;
    public bool showDebugLogs = true;

    private Vector3 followVelocity;
    private float currentYaw;
    private bool initialized;

    private IEnumerator Start()
    {
        if (player == null)
        {
            Debug.LogError(
                "Camera player reference is missing!"
            );

            enabled = false;
            yield break;
        }

        // Wait until scene startup has completed.
        // This allows SaveManager to restore the boat.
        yield return new WaitForEndOfFrame();

        currentYaw = 0f;
        SnapCameraToPlayer();
        initialized = true;

        if (showDebugLogs)
        {
            Debug.Log(
                "Camera initialized. Boat: " +
                player.position +
                " | Camera: " +
                transform.position
            );
        }
    }

    private void LateUpdate()
    {
        if (!initialized || player == null)
            return;

        // Hold left mouse button to orbit.
        if (Input.GetMouseButton(0))
        {
            float mouseX =
                Input.GetAxis("Mouse X");

            currentYaw +=
                mouseX *
                rotationSensitivity *
                (invertMouse ? -1f : 1f);
        }

        // Manual camera reset.
        if (Input.GetKeyDown(resetCameraKey))
        {
            currentYaw = 0f;
            SnapCameraToPlayer();
            return;
        }

        Quaternion orbit =
            Quaternion.Euler(0f, currentYaw, 0f);

        Vector3 desiredPosition =
            player.position +
            orbit * cameraOffset;

        transform.position =
            Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref followVelocity,
                Mathf.Max(0.01f, followSmoothTime)
            );

        LookAtPlayer();
    }

    public void SnapCameraToPlayer()
    {
        if (player == null)
            return;

        Quaternion orbit =
            Quaternion.Euler(0f, currentYaw, 0f);

        transform.position =
            player.position +
            orbit * cameraOffset;

        followVelocity = Vector3.zero;

        LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        Vector3 targetPosition =
            player.position +
            Vector3.up * lookHeight;

        Vector3 direction =
            targetPosition - transform.position;

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
