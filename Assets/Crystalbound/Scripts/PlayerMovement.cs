using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform cameraTransform;

    private Vector3 movementInput;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
        }
    }

    private void Update()
    {
        GetMovementInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movementInput = new Vector3(horizontal, 0f, vertical).normalized;
    }

    private void MovePlayer()
    {
        if (cameraTransform == null)
            return;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Remove vertical component.
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movementDirection =
            cameraForward * movementInput.z +
            cameraRight * movementInput.x;

        movementDirection.Normalize();

        Vector3 movement = movementDirection * moveSpeed;

        rb.velocity = new Vector3(
            movement.x,
            rb.velocity.y,
            movement.z
        );
    }
}