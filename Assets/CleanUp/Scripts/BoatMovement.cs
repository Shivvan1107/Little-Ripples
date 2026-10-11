
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxForwardSpeed = 7f;
    public float maxReverseSpeed = 3f;
    public float acceleration = 5f;
    public float deceleration = 3f;
    public float braking = 8f;

    [Header("Steering")]
    public float turnSpeed = 110f;
    public float minimumSteeringFactor = 0.2f;
    public bool reverseSteering = true;

    private Rigidbody rb;

    private float throttleInput;
    private float steeringInput;
    private float currentSpeed;

    public float CurrentSpeed => currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Dynamic Rigidbody for physical collisions.
        rb.isKinematic = false;
        rb.useGravity = false;

        rb.interpolation =
            RigidbodyInterpolation.Interpolate;

        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        throttleInput =
            Input.GetAxisRaw("Vertical");

        steeringInput =
            Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        UpdateSpeed();
        UpdateSteering();
        MoveBoat();
    }

    private void UpdateSpeed()
    {
        float targetSpeed = throttleInput >= 0f
            ? throttleInput * maxForwardSpeed
            : throttleInput * maxReverseSpeed;

        float rate;

        if (Mathf.Abs(throttleInput) < 0.01f)
        {
            rate = deceleration;
        }
        else if (
            Mathf.Abs(currentSpeed) > 0.01f &&
            Mathf.Sign(currentSpeed) !=
            Mathf.Sign(targetSpeed))
        {
            rate = braking;
        }
        else
        {
            rate = acceleration;
        }

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            rate * Time.fixedDeltaTime
        );
    }

    private void UpdateSteering()
    {
        if (Mathf.Abs(steeringInput) < 0.01f)
            return;

        float speedRatio = Mathf.Clamp01(
            Mathf.Abs(currentSpeed) /
            Mathf.Max(0.01f, maxForwardSpeed)
        );

        float steeringFactor = Mathf.Lerp(
            minimumSteeringFactor,
            1f,
            speedRatio
        );

        float direction = 1f;

        if (reverseSteering && currentSpeed < -0.01f)
            direction = -1f;

        float turnAmount =
            steeringInput *
            turnSpeed *
            steeringFactor *
            direction *
            Time.fixedDeltaTime;

        Quaternion newRotation =
            rb.rotation *
            Quaternion.Euler(0f, turnAmount, 0f);

        rb.MoveRotation(newRotation);
    }

    private void MoveBoat()
    {
        // Set horizontal velocity instead of
        // directly moving through colliders.
        Vector3 forward =
            rb.rotation * Vector3.forward;

        Vector3 targetVelocity =
            forward * currentSpeed;

        rb.velocity = new Vector3(
            targetVelocity.x,
            0f,
            targetVelocity.z
        );
    }
}
