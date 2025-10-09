using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    Vector2 movementInput;
    Vector2 cameraInput;
    
    bool runningValue;
    bool jumpingValue;
    
    public float verticalInput;
    public float horizontalInput;

    public float cameraVertical;
    public float cameraHorizontal;

    public bool isRunning;    
    public bool isJumping;

    public float zoomInput;
    public float zoomValue;
    
    public bool HasMovementInput { get; private set; }

    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += OnMovementInput;
            playerControls.PlayerMovement.Movement.canceled += OnMovementInput;

            playerControls.PlayerMovement.Camera.performed += OnCameraInput;
            playerControls.PlayerMovement.Camera.canceled += OnCameraInput;

            playerControls.PlayerMovement.Zoom.performed += OnZoomInput;
            playerControls.PlayerMovement.Zoom.canceled += OnZoomInput;
            
            playerControls.PlayerMovement.Jump.performed += OnJumpInput;
            playerControls.PlayerMovement.Jump.canceled += OnJumpInput;


            playerControls.PlayerMovement.Running.performed += OnRunningInput;
            playerControls.PlayerMovement.Running.canceled += OnRunningInput;
        }

        playerControls.Enable();
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void OnCameraInput(InputAction.CallbackContext context)
    {
        cameraInput = context.ReadValue<Vector2>();
    }

    private void OnZoomInput(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<float>();
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        jumpingValue = context.ReadValue<float>() > 0.5f;
    }
    
    private void OnRunningInput(InputAction.CallbackContext context)
    {
        runningValue = context.ReadValue<float>() > 0.5f;
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        HasMovementInput = movementInput.sqrMagnitude > 0.01f;
    }
    private void HandleCameraInput()
    {
        cameraVertical = cameraInput.y;
        cameraHorizontal = cameraInput.x;
        zoomValue = zoomInput;
    }
    
    private void HandleRunningInput()
    {
        isRunning = runningValue;
    }

    private void HandleJumpInput()
    {
        isJumping = jumpingValue;
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleCameraInput();
        HandleRunningInput();
        HandleJumpInput();
        //HandleActionInput();
        
    }
}
