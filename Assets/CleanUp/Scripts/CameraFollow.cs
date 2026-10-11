
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Distance")]
    public float height = 11f;
    public float distance = 9f;

    [Header("Smoothing")]
    public float positionSmoothTime = 0.2f;
    public float rotationSmoothTime = 0.35f;

    [Header("Camera Look")]
    public float lookHeight = 0.5f;
    public float lookRotationSpeed = 10f;

    private Vector3 positionVelocity;
    private float currentYaw;
    private float yawVelocity;

    private void Start()
    {
        if (target == null)
            return;

        currentYaw = target.eulerAngles.y;

        // Start directly behind the boat.
        Quaternion yaw =
            Quaternion.Euler(0f, currentYaw, 0f);

        transform.position =
            target.position +
            yaw * new Vector3(
                0f, height, -distance
            );

        transform.LookAt(
            target.position +
            Vector3.up * lookHeight
        );
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Smoothly follow boat steering.
        currentYaw = Mathf.SmoothDampAngle(
            currentYaw,
            target.eulerAngles.y,
            ref yawVelocity,
            rotationSmoothTime
        );

        Quaternion yawRotation =
            Quaternion.Euler(
                0f, currentYaw, 0f
            );

        Vector3 offset =
            yawRotation *
            new Vector3(
                0f, height, -distance
            );

        Vector3 desiredPosition =
            target.position + offset;

        // Smooth camera translation.
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref positionVelocity,
            positionSmoothTime
        );

        // Camera continuously looks at the boat.
        Vector3 lookTarget =
            target.position +
            Vector3.up * lookHeight;

        Vector3 direction =
            lookTarget - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion desiredRotation =
                Quaternion.LookRotation(
                    direction.normalized,
                    Vector3.up
                );

            float smoothing =
                1f - Mathf.Exp(
                    -lookRotationSpeed *
                    Time.deltaTime
                );

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    desiredRotation,
                    smoothing
                );
        }
    }
}
