using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLocomotion : MonoBehaviour
{
    [Header("References")]
    private InputManager inputManager;
    private CameraBehavior cameraHandler;
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
    [SerializeField] private WallRun wallRun;

    private bool wasGrounded;
    private float fallStartHeight;
    private float fallDistance;
    private Vector3 jumpDirection; // Store movement direction when jumping

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraHandler = FindFirstObjectByType<CameraBehavior>();
        playerManager = GetComponent<PlayerManager>();
        wallRun = FindFirstObjectByType<WallRun>();
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
    public void HandleParkourMovement()
    {
        UpdateLocomotionState();
        HandleCamera();
        HandleWallJumpAndGravity();
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
    private void HandleWallJumpAndGravity()
    {
        
            // === WALL JUMP ===
            if (wallRun.IsRunning && inputManager.isJumping)
            {
                // Jump direction = upward + away from the wall (approx 90°)
                Vector3 jumpDir = Vector3.up * wallRun.wallJumpUpForce + wallRun.WallNormal * wallRun.wallJumpSideForce;

                playerRigidbody.linearVelocity = Vector3.zero;
                playerRigidbody.AddForce(jumpDir, ForceMode.Impulse);

                wallRun.StopWallRun();
                return;
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
        if (!wallRun.IsRunning)
        {
            // Try to start wall run
            if (wallRun.CanStartWallRun(transform))
            {
                wallRun.StartWallRun();
                playerManager.SetLocomotionState(PlayerState.WallRunning);
            }
        }
        else
        {
            // Already wall running - maintain it
            // First check for wall jump
            if (inputManager.isJumping)
            {
                Debug.Log("Wall jump triggered");
                Vector3 jumpDir = Vector3.up * wallRun.wallJumpUpForce +
                                wallRun.WallNormal * wallRun.wallJumpSideForce;
                playerRigidbody.linearVelocity = jumpDir;
                wallRun.StopWallRun();
                playerManager.SetLocomotionState(PlayerState.WallJumping);
                return;
            }

            // Update timer - if expired, stop
            bool timerActive = wallRun.UpdateTimer();
            if (!timerActive)
            {
                Debug.Log("Timer expired");
                wallRun.StopWallRun();
                playerManager.SetLocomotionState(PlayerState.Falling);
                return;
            }

            // Check if still on wall
            bool onWall = wallRun.CheckForWall(transform);
            if (!onWall)
            {
                Debug.Log("Lost wall contact");
                wallRun.StopWallRun();
                playerManager.SetLocomotionState(PlayerState.Falling);
                return;
            }

            // Apply wall run velocity
            Vector3 wallVelocity = wallRun.RunDirection * wallRun.wallRunSpeed;
            wallVelocity.y = -wallRun.wallRunGravity;
            playerRigidbody.linearVelocity = wallVelocity;
        }
    }


    // ---------------- GROUND CHECK ----------------
    public bool IsGrounded()
    {
        // Check sphere below the player's center, not at center
        Vector3 checkPosition = transform.position - Vector3.up * 0.1f;
        return Physics.CheckSphere(checkPosition, groundCheckRadius, groundMask);
    }

    private void UpdateLocomotionState()
    {
        // CRITICAL: Check wall running FIRST before anything else
        if (wallRun.IsRunning)
        {
            playerManager.SetLocomotionState(PlayerState.WallRunning);
            return; // Don't process other states
        }

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



    public void HandleCombatMovement()
    {
        throw new NotImplementedException();
    }
}