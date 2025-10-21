using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private InputManager inputManager;
    private PlayerLocomotion playerLocomotion;
    private WallRun wallRun;

    public PlayerBaseState currentState;
    public PlayerState locomotionState;
    public bool invertAnimation = false;
    public Animator animator;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        wallRun = GetComponent<WallRun>();
        animator = GetComponent<Animator>();

        SetState(new IdleState());
        SetLocomotionState(PlayerState.Idle);
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        UpdateAnimationState();
        currentState.Update(this);
    }

    private void FixedUpdate()
    {
        // Always use HandleAllMovement - it handles all cases internally
        playerLocomotion.HandleAllMovement();
    }

    // NEW: Separate method to handle animation updates
    private void UpdateAnimationState()
    {
        // Update animator with current locomotion state
        animator.SetInteger("State", (int)locomotionState);
        animator.SetBool("Mirror", invertAnimation);
    }
    
    // Getters - using properties instead of methods (C# convention)
    public InputManager InputManager => inputManager;
    public PlayerLocomotion PlayerLocomotion => playerLocomotion;
    public WallRun WallRun => wallRun;

    public void SetLocomotionState(PlayerState newState)
    {
        this.SetLocomotionState(newState, false);
    }    

    public void SetLocomotionState(PlayerState newState, bool invertAnimation)
    {
        if (locomotionState == newState && !invertAnimation)
            return;

        PlayerState previousState = locomotionState;
        locomotionState = newState;
        this.invertAnimation = invertAnimation;

        // Trigger immediate animation update for critical transitions
        if (ShouldForceAnimationUpdate(previousState, newState))
        {
            UpdateAnimationState();
        }

        Debug.Log($"Locomotion: {previousState} → {newState}");
    }

    // NEW: Determines if state transition needs immediate animation update
    private bool ShouldForceAnimationUpdate(PlayerState from, PlayerState to)
    {
        // Force immediate update for jump/fall/land transitions
        return to == PlayerState.Jumping ||
               to == PlayerState.Falling ||
               to == PlayerState.Landing ||
               to == PlayerState.HardLanding ||
               (from.IsGrounded() && to.IsAirborne()) ||
               (from.IsAirborne() && to.IsGrounded());
    }

    public void SetState(PlayerBaseState newState)
    {
        if (currentState == newState)
            return;

        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);

        Debug.Log($"State Changed - Animator State: {animator.GetInteger("State")}");
    }

    public bool IsJumping()
    {
        return playerLocomotion.IsGrounded() && inputManager.isJumping;
    }
}

public enum PlayerState
{
    Idle = 0,
    Moving = 1,
    Running = 2,
    Jumping = 3,
    WallRunning = 4,
    WallRightRunning = -4,
    Falling = 5,
    Landing = 6,
    HardLanding = 7,
    WallJumping = 8,
    Combat = 9,
    Dead = -1
}

public static class PlayerStateExtensions
{
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
    public static bool IsGrounded(this PlayerState state)
    {
        return state == PlayerState.Idle ||
               state == PlayerState.Moving ||
               state == PlayerState.Running ||
               state == PlayerState.Landing ||
               state == PlayerState.HardLanding;
    }

    public static bool IsAirborne(this PlayerState state)
    {
        return state == PlayerState.Jumping ||
               state == PlayerState.Falling;
    }

    public static bool IsLocomotion(this PlayerState state)
    {
        return state == PlayerState.Moving ||
               state == PlayerState.Running;
    }

    public static bool IsRecovering(this PlayerState state)
    {
        return state == PlayerState.Landing ||
               state == PlayerState.HardLanding;
    }
}