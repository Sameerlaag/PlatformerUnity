using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLocomotion : MonoBehaviour
{
    [Header("References")] private PlayerManager player;
    public Rigidbody Rigidbody { get; private set; }
    private InputManager input;
    private WallRun wallRun;
    [Header("Movement Settings")] public float moveSpeed = 2.2f;
    public float runSpeed = 6.8f;
    public float rotationSpeed = 15f;
    public float jumpForce = 5f;
    public LayerMask groundMask;
    private CameraBehavior cameraHandler;
    [Header("Jump Settings")] public float jumpCooldown = 0.5f;
    private bool canJump = true;
    private float jumpRecoveryTimer = 0f;
    private float groundCheckRadius = 0.5f;
    private bool wasGrounded;
    public float coyoteTime = 0.15f;
    private float coyoteTimer;
    
    [SerializeField] private CapsuleCollider capsule;


    private void Awake()
    {
        player = GetComponent<PlayerManager>();
        Rigidbody = GetComponent<Rigidbody>();
        input = GetComponent<InputManager>();
        cameraHandler = FindFirstObjectByType<CameraBehavior>();
        wallRun = FindFirstObjectByType<WallRun>();
    }

    private void FixedUpdate()
    {
        HandleGroundCheck();
        UpdateJumpCooldown();
    }

    public void HandleMovementForState(PlayerState state)
    {
        UpdateJumpCooldown();
        switch (state)
        {
            case PlayerState.Idle: HandleIdle(); break;
            case PlayerState.Moving: HandleMove(); break;
            case PlayerState.Jumping: HandleJump(); break;
            case PlayerState.Falling: HandleFalling(); break;
            case PlayerState.WallRunning: HandleWallRun(); break;
            case PlayerState.Landing:
            case PlayerState.HardLanding: break;
            // root motion handles these
        }
    }

    private void HandleIdle()
    {
        Vector3 vel = Rigidbody.linearVelocity;
        vel.x = vel.z = 0;
        Rigidbody.linearVelocity = vel;
    }

    private void HandleMove()
    {
        Vector3 moveDir = cameraHandler.transform.forward * input.verticalInput +
                          cameraHandler.transform.right * input.horizontalInput;
        moveDir.Normalize();
        moveDir.y = 0;
        float targetSpeed = input.isRunning ? runSpeed : moveSpeed;
        Vector3 velocity = moveDir * targetSpeed;
        velocity.y = Rigidbody.linearVelocity.y;
        Rigidbody.linearVelocity = velocity;
        HandleRotation(moveDir);
    }

    private void HandleRotation(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (!canJump || !IsGrounded()) return;
        canJump = false;
        jumpRecoveryTimer = jumpCooldown;
        Vector3 vel = Rigidbody.linearVelocity;
        vel.y = jumpForce;
        Rigidbody.linearVelocity = vel;
        player.SetState(PlayerState.Jumping);
    }

    private void HandleFalling()
    {
        // Check if player is in air and moving downward
        if (!IsGrounded() && Rigidbody.linearVelocity.y <= 0f)
        {
            // If already in JumpingState, switch to FallingState
            if (player.CurrentState is PlayerState.Jumping)
            {
                player.SetState(PlayerState.Falling);
            }
        }
    }

    private void HandleWallRun()
    {
        if (wallRun == null || !wallRun.IsRunning) return;
        wallRun.ApplyWallRunMovement(Rigidbody);
    }

    public bool IsGrounded()
    {
        Vector3 checkPos = transform.position + Vector3.down * 0.05f; // small offset
        return Physics.CheckSphere(checkPos, groundCheckRadius, groundMask);
    }

    private void HandleGroundCheck()
    {
        bool grounded = IsGrounded();
        if (grounded)
        {
            coyoteTimer = coyoteTime;
            if (jumpRecoveryTimer <= 0f) canJump = true;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        wasGrounded = grounded;
    }

    private void UpdateJumpCooldown()
    {
        if (jumpRecoveryTimer > 0f)
        {
            jumpRecoveryTimer -= Time.deltaTime;
        }
    }

    public float GetHorizontalVelocityMagnitude()
    {
        Vector3 flatVel = new Vector3(Rigidbody.linearVelocity.x, 0, Rigidbody.linearVelocity.z);
        return flatVel.magnitude;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position - Vector3.up * 0.1f, groundCheckRadius);
    }
}