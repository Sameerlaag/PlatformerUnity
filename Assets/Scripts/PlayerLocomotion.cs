using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLocomotion : MonoBehaviour
{
    [Header("References")]
    private InputManager inputManager;
    private CameraBehavior cameraHandler;
    private WallRun wallRunHandler;
    private PlayerManager playerManager;
    private Rigidbody playerRigidbody;

    [Header("Movement Settings")]
    public float movementSpeed = 1.4f;
    public float runningSpeedModifier = 3f;
    public float rotationSpeed = 15f;
    public float jumpForce = 5f;

    [Header("Ground & Fall Settings")]
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float hardLandingThreshold = 7f;

    private bool wasGrounded;
    private float fallStartHeight;
    private float fallDistance;
    private Vector3 jumpDirection; // Store movement direction when jumping

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraHandler = FindFirstObjectByType<CameraBehavior>();
        wallRunHandler = GetComponent<WallRun>();
        playerManager = GetComponent<PlayerManager>();

        wasGrounded = IsGrounded();
        jumpDirection = Vector3.zero;
    }

    public void HandleAllMovement()
    {
        UpdateLocomotionState();
        HandleMovement();
        HandleRotation();
        HandleCamera();
        HandleJumpAndGravity();
        HandleGroundCheck();
        HandleWallRun();
    }

    // ---------------- MOVEMENT ----------------
    private void HandleMovement()
    {
        Vector3 moveDirection;

        if (IsGrounded())
        {
            // Ground movement - respond to input normally
            moveDirection = cameraHandler.transform.forward * inputManager.verticalInput;
            moveDirection += cameraHandler.transform.right * inputManager.horizontalInput;
            moveDirection.Normalize();
            moveDirection.y = 0f;

            float currentSpeed = inputManager.isRunning
                ? movementSpeed * runningSpeedModifier
                : movementSpeed;

            Vector3 movementVelocity = moveDirection * currentSpeed;
            movementVelocity.y = playerRigidbody.linearVelocity.y;
            playerRigidbody.linearVelocity = movementVelocity;
        }
        else
        {
            // Airborne - maintain jump direction without input control
            Vector3 airborneVelocity = jumpDirection;
            airborneVelocity.y = playerRigidbody.linearVelocity.y;
            playerRigidbody.linearVelocity = airborneVelocity;
        }
    }

    // ---------------- ROTATION ----------------
    private void HandleRotation()
    {
        // Only allow rotation when grounded
        if (!IsGrounded())
            return;

        Vector3 targetDirection = cameraHandler.transform.forward * inputManager.verticalInput;
        targetDirection += cameraHandler.transform.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0f;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }

    // ---------------- CAMERA ----------------
    private void HandleCamera()
    {
        cameraHandler.HandleCameraMovement(
            inputManager.cameraHorizontal,
            inputManager.cameraVertical,
            inputManager.zoomInput
        );
    }

    // ---------------- JUMP & FALL ----------------
    private void HandleJumpAndGravity()
    {
        if (inputManager.isJumping && IsGrounded())
        {
            // Store current horizontal velocity before jumping
            jumpDirection = new Vector3(
                playerRigidbody.linearVelocity.x,
                0f,
                playerRigidbody.linearVelocity.z
            );

            // Apply jump force
            Vector3 velocity = playerRigidbody.linearVelocity;
            velocity.y = jumpForce;
            playerRigidbody.linearVelocity = velocity;
        }
    }

    private void HandleGroundCheck()
    {
        bool grounded = IsGrounded();

        // Detect landing
        if (!wasGrounded && grounded)
        {
            fallDistance = fallStartHeight - transform.position.y;
            jumpDirection = Vector3.zero; // Clear jump direction on landing
        }

        // Detect fall start
        if (wasGrounded && !grounded)
        {
            fallStartHeight = transform.position.y;

            // If falling without jumping, store current velocity
            if (jumpDirection == Vector3.zero)
            {
                jumpDirection = new Vector3(
                    playerRigidbody.linearVelocity.x,
                    0f,
                    playerRigidbody.linearVelocity.z
                );
            }
        }

        wasGrounded = grounded;
    }

    // ---------------- WALL RUN ----------------
    private void HandleWallRun()
    {
        // Placeholder for now — logic will go here later.
        // Eventually, this will detect when to start wall-running and call:
        // playerManager.RequestState(PlayerState.WallRunning);
    }

    // ---------------- GROUND CHECK ----------------
    public bool IsGrounded()
    {
        // Check sphere below the player's center, not at center
        Vector3 checkPosition = transform.position - Vector3.up * 0.1f;
        return Physics.CheckSphere(checkPosition, groundCheckRadius, groundMask);
    }

    private bool IsNearWall()
    {
        // Check if there's a wall in front/around the player
        return Physics.Raycast(transform.position, transform.forward, 1f) ||
               Physics.Raycast(transform.position, -transform.forward, 1f) ||
               Physics.Raycast(transform.position, transform.right, 1f) ||
               Physics.Raycast(transform.position, -transform.right, 1f);
    }

    private void UpdateLocomotionState()
    {
        if (!IsGrounded())
        {
            playerManager.SetLocomotionState(playerRigidbody.linearVelocity.y > 0
                ? PlayerState.Jumping
                : PlayerState.Falling);
        }
        else
        {
            if (!wasGrounded)
            {
                playerManager.SetLocomotionState(fallDistance > hardLandingThreshold
                    ? PlayerState.HardLanding
                    : PlayerState.Landing);
            }
            else
            {
                playerManager.SetLocomotionState(inputManager.HasMovementInput
                    ? (inputManager.isRunning ? PlayerState.Running : PlayerState.Moving)
                    : PlayerState.Idle);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check sphere in editor
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
}