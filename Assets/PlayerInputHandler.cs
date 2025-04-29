using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // Events to broadcast input changes
    public delegate void JumpInputEvent(bool jumpTriggered);
    public delegate void MoveInputEvent(Vector2 moveInput);
    public delegate void RunInputEvent(bool isRunning);
    public delegate void GroundSlamInputEvent(bool groundSlamTriggered);

    public event JumpInputEvent OnJumpInput;
    public event MoveInputEvent OnMoveInput;
    public event RunInputEvent OnRunInput;
    public event GroundSlamInputEvent OnGroundSlamInput;

    private PlayerInputActions _playerControls;
    private bool _jumpTriggered = false;
    private bool _groundSlamTriggered = false;
    private Vector2 _moveInput = Vector2.zero;
    private bool _isRunning = false;

    private void Awake()
    {
        _playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _playerControls.Enable();

        // Setup input callbacks
        _playerControls.Player.Jump.performed += ctx => _jumpTriggered = true;
        _playerControls.Player.Jump.canceled += ctx => _jumpTriggered = false;

        _playerControls.Player.Run.performed += ctx => _isRunning = true;
        _playerControls.Player.Run.canceled += ctx => _isRunning = false;

        _playerControls.Player.GroundSlam.performed += ctx => _groundSlamTriggered = true;
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    private void Update()
    {
        // Read move input and broadcast it
        _moveInput = _playerControls.Player.Move.ReadValue<Vector2>();
        OnMoveInput?.Invoke(_moveInput);

        // Broadcast jump input
        if (_jumpTriggered)
        {
            OnJumpInput?.Invoke(true);
            _jumpTriggered = false;  // Reset after broadcasting
        }

        // Broadcast ground slam input
        if (_groundSlamTriggered)
        {
            OnGroundSlamInput?.Invoke(true);
            _groundSlamTriggered = false;  // Reset after broadcasting
        }

        // Broadcast run input
        OnRunInput?.Invoke(_playerControls.Player.Run.IsPressed());
    }

    // Accessor methods for direct polling if needed
    public Vector2 GetMoveInput()
    {
        return _moveInput;
    }

    public bool IsJumpTriggered()
    {
        return _playerControls.Player.Jump.triggered;
    }

    public bool IsJumpBufferable()
    {
        return _playerControls.Player.Jump.triggered;
    }

    public bool IsRunning()
    {
        return _playerControls.Player.Run.IsPressed();
    }

    public bool IsGroundSlamTriggered()
    {
        return _groundSlamTriggered;
    }
}
