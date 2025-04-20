using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // Events to broadcast input changes
    public delegate void JumpInputEvent(bool jumpTriggered);
    public delegate void MoveInputEvent(Vector2 moveInput);
    public delegate void RunInputEvent(bool isRunning);

    public event JumpInputEvent OnJumpInput;
    public event MoveInputEvent OnMoveInput;
    public event RunInputEvent OnRunInput;

    private PlayerInputActions playerControls;
    private bool jumpTriggered = false;
    private Vector2 moveInput = Vector2.zero;
    private bool isRunning = false;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        playerControls.Enable();

        // Setup input callbacks
        playerControls.Player.Jump.performed += ctx => jumpTriggered = true;
        playerControls.Player.Jump.canceled += ctx => jumpTriggered = false;
        playerControls.Player.Run.performed += ctx => isRunning = true;
        playerControls.Player.Run.canceled += ctx => isRunning = false;
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        // Read move input and broadcast it
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        OnMoveInput?.Invoke(moveInput);

        // Broadcast jump input
        if (jumpTriggered)
        {
            OnJumpInput?.Invoke(true);
            jumpTriggered = false;  // Reset after broadcasting
        }

        // Broadcast run input
        OnRunInput?.Invoke(playerControls.Player.Run.IsPressed());
    }

    // Accessor methods for direct polling if needed
    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public bool IsJumpTriggered()
    {
        return playerControls.Player.Jump.triggered;
    }

    public bool IsJumpBufferable()
    {
        return playerControls.Player.Jump.triggered;
    }

    public bool IsRunning()
    {
        return playerControls.Player.Run.IsPressed();
    }
}