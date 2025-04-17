using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] private VelocityModifierSystem modifierSystem;
    [SerializeField] private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] private PlayerMovementConfig movementConfig;
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerToolManager toolManager;
    [SerializeField] private float checkHeadDistance = 0.1f;
    private int _framesSinceBuffer = 0;
    private bool _jumpBuffered = false;

    private PlayerInputActions playerControls;

    private IPlayerState _currentState;

    private PlayerContext _context = new PlayerContext();
    private Transform _cameraTransform;
    [SerializeField] private Transform visualTransform;

    private bool _jumpRequested = false;

    private Dictionary<State, IPlayerState> _stateMap;

    //Interpolation
    private Vector3 _lastFixedPosition;
    private Vector3 _currentFixedPosition;
    private float _fixedDeltaTimeTimer = 0f;


    private void Awake()
    {
        playerControls = new PlayerInputActions();
        _cameraTransform = Camera.main.transform;

        //States created here
        _stateMap = new Dictionary<State, IPlayerState>
        {
            [State.Walking] = new PlayerWalking(),
            [State.Airborne] = new PlayerAirborne(),
            [State.Jumping] = new PlayerJumping(),
            [State.Running] = new PlayerRunning(),
            [State.Grappling] = new PlayerGrappling()
        };
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private IPlayerState LookupState(State state)
    {
        IPlayerState output;
        _stateMap.TryGetValue(state, out output);
        return output;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControls.Player.Jump.triggered && !controller.isGrounded)
        {
            _jumpBuffered = true;
        }

        UpdateContext();
        State newState = DecideState();
        if (newState == State.Jumping) _jumpRequested = true;
        IPlayerState newPlayerState = LookupState(newState);
        if (newPlayerState != _currentState) SetState(newPlayerState);

        InterpolatePosition();
    }

    void FixedUpdate()
    {
        if (_jumpBuffered)
        {
            _framesSinceBuffer++;
            if (_framesSinceBuffer > movementConfig.jumpBufferFrames)
            {
                _jumpBuffered = false;
                _framesSinceBuffer = 0;
            }
        }

        if (_currentState != null) currentVelocity = _currentState.Update(_context);
        ApplyGravity();
        currentVelocity += modifierSystem.GetModifiers();
        CheckHeadCollision();
        CheckSideCollision();
        controller.Move(currentVelocity);
        //Interpolation
        _lastFixedPosition = _currentFixedPosition;
        _currentFixedPosition = transform.position;
        _fixedDeltaTimeTimer = 0f;

        _jumpRequested = false;
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && _currentState != LookupState(State.Jumping))
        {
            currentVelocity.y = -0.1f;
        }
        else
        {
            currentVelocity.y -= movementConfig.gravity * Time.fixedDeltaTime;
        }
    }

    void UpdateContext()
    {
        _context.moveInput = playerControls.Player.Move.ReadValue<Vector2>();

        _context.currentVelocity = currentVelocity;
        _context.position = transform.position;
        _context.cameraTransform = _cameraTransform;
        _context.movementConfig = movementConfig;
        _context.playerTool = toolManager.GetCurrentTool();
    }

    State DecideState()
    {
        var grapplingHook = toolManager.GetCurrentTool() as ToolGrapplingHook;
        if (grapplingHook != null && grapplingHook.IsActive())
        {
            return State.Grappling;
        }

        if (controller.isGrounded)
        {
            if (playerControls.Player.Jump.triggered || _jumpRequested || _jumpBuffered)
            {
                _jumpRequested = true;
                _jumpBuffered = false;
                _framesSinceBuffer = 0;
                return State.Jumping;
            }
            else
            {
                if (playerControls.Player.Run.IsPressed())
                {
                    return State.Running;
                }
                else
                {
                    return State.Walking;
                }
            }
        } else
        {
            return State.Airborne;
        }
    }

    void SetState(IPlayerState newState)
    {
        _currentState?.Exit(_context);
        _currentState = newState;
        _currentState.Enter(_context);
    }

    public Vector3 GetVelocity()
    {
        return currentVelocity;
    }

    void CheckHeadCollision()
    {
        Vector3 top = transform.position + Vector3.up * (controller.height / 2f);
        if (Physics.Raycast(top, Vector3.up, checkHeadDistance))
        {
            if (currentVelocity.y > 0)
            {
                currentVelocity.y = 0;
            }
        }
    }

    void CheckSideCollision()
    {
        Vector3[] directions = {
        transform.right,       // Right
        -transform.right,      // Left
        transform.forward,     // Forward
        -transform.forward     // Backward
    };

        Vector3 center = transform.position + Vector3.up * (controller.height / 2f); // Approximate body center

        foreach (var dir in directions)
        {
            if (Physics.Raycast(center, dir, out RaycastHit hit, controller.radius + 0.1f))
            {
                Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
                if (Vector3.Dot(horizontalVelocity, dir) > 0)
                {
                    currentVelocity.x = 0;
                    currentVelocity.z = 0;
                    break;
                }
            }
        }
    }

    private void InterpolatePosition()
    {
        if (movementConfig.interpolate == false) return;

        _fixedDeltaTimeTimer += Time.deltaTime;

        float interpolationFactor = _fixedDeltaTimeTimer / Time.fixedDeltaTime;
        interpolationFactor = Mathf.Clamp01(interpolationFactor);

        Vector3 interpolatedPosition = Vector3.Lerp(_lastFixedPosition, _currentFixedPosition, interpolationFactor);
        visualTransform.position = interpolatedPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position + Vector3.up * (controller.height / 2f), Vector3.up * checkHeadDistance);
    }


}
