using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Windows;

[RequireComponent(typeof(PlayerLocomotion), typeof(InputManager), typeof(Animator))]
public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    public InputManager Input { get; private set; }
    public PlayerLocomotion Locomotion { get; private set; }
    public Animator Animator { get; private set; }
    public WallRun WallRun { get; private set; }

    private CameraBehavior cameraHandler;


    [Header("State Management")]
    public PlayerState CurrentState { get; private set; } = PlayerState.Idle;
    public bool InvertAnimation { get; private set; }


    private void Awake()
    {
        Input = GetComponent<InputManager>();
        Locomotion = GetComponent<PlayerLocomotion>();
        Animator = GetComponent<Animator>();
        cameraHandler = FindFirstObjectByType<CameraBehavior>();

        WallRun = GetComponent<WallRun>();
        Animator.applyRootMotion = false;
    }

    private void Update()
    {
        // 1️⃣ Input & intent
        Input.HandleAllInputs();

        // 2️⃣ Evaluate locomotion and jump
        EvaluateState();

    }

    private void FixedUpdate()
    {

        // 3️⃣ Update animations
        UpdateAnimator();
        HandleCamera();

        // 4️⃣ Apply movement
        Locomotion.HandleMovementForState(CurrentState);
    }

    // --- STATE DECISION ---
    private void EvaluateState()
    {
        if (WallRun != null && WallRun.IsRunning)
        {
            SetState(PlayerState.WallRunning);
            return;
        }

        if (Input.isJumping)
        {
            SetState(PlayerState.Jumping);
            return;
        }
        
        if (!Locomotion.IsGrounded())
        {
            SetState(PlayerState.Falling);
            return;
        }

        if (Input.HasMovementInput)
        {
            SetState(PlayerState.Moving);
            return;
        }

        SetState(PlayerState.Idle);
    }

    public void SetState(PlayerState newState)
    {
        if (CurrentState == newState) return;
        Debug.Log("state " + CurrentState + " > " + newState);
        CurrentState = newState;
        Animator.applyRootMotion = UsesRootMotion(newState);

    }

    private void UpdateAnimator()
    {
        // ✅ Use MoveSpeed for blend tree
        float moveSpeed = Locomotion.GetHorizontalVelocityMagnitude();
        Animator.SetFloat("MoveSpeed", moveSpeed);
        Animator.SetInteger("State", (int)CurrentState);
        Animator.SetBool("Mirror", InvertAnimation);
    }

    private bool UsesRootMotion(PlayerState state)
    {
        return state == PlayerState.WallRunning ||
               state == PlayerState.Combat ||
               state == PlayerState.Landing;
    }
    private void HandleCamera() { cameraHandler.HandleCameraMovement(Input.cameraHorizontal, Input.cameraVertical, Input.zoomInput); }

}
public enum PlayerState { Idle = 0, Moving = 1, Running = 2, Jumping = 3, WallRunning = 4, WallRightRunning = -4, Falling = 5, Landing = 6, HardLanding = 7, WallJumping = 8, Combat = 9, Dead = -1 }
public static class PlayerStateExtensions { 
    public static bool IsIdle(this PlayerState state) => state == PlayerState.Idle; 
    public static bool IsMoving(this PlayerState state) => state == PlayerState.Moving; 
    public static bool IsRunning(this PlayerState state) => state == PlayerState.Running; 
    public static bool IsWallRunning(this PlayerState state) => state == PlayerState.WallRunning; 
    public static bool IsJumping(this PlayerState state) => state == PlayerState.Jumping; 
    public static bool IsFalling(this PlayerState state) => state == PlayerState.Falling; 
    public static bool IsLanding(this PlayerState state) => state == PlayerState.Landing; 
    public static bool IsHardLanding(this PlayerState state) => state == PlayerState.HardLanding; 
    public static bool IsDead(this PlayerState state) => state == PlayerState.Dead; 
    // Grouped helpers for convenience
    public static bool IsGrounded(this PlayerState state) { return state == PlayerState.Idle || state == PlayerState.Moving || state == PlayerState.Running || state == PlayerState.Landing || state == PlayerState.HardLanding; } 
    public static bool IsAirborne(this PlayerState state) { return state == PlayerState.Jumping || state == PlayerState.Falling; } 
    public static bool IsLocomotion(this PlayerState state) { return state == PlayerState.Moving || state == PlayerState.Running; } 
    public static bool IsRecovering(this PlayerState state) { return state == PlayerState.Landing || state == PlayerState.HardLanding; } }