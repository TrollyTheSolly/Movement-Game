using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] private VelocityModifierSystem modifierSystem;
    [SerializeField] private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] private PlayerMovementConfig movementConfig;
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerToolManager toolManager;
    [SerializeField] private float checkHeadDistance = 0.1f;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private FeedbackManager feedbackManager;
    [SerializeField] private PlayerInputHandler inputHandler;  // Reference to our new input handler

    private int _framesSinceBuffer = 0;
    private bool _jumpBuffered = false;

    private IPlayerState _currentState;
    private IPlayerState _lastState;

    private PlayerContext _context = new PlayerContext();
    private Transform _cameraTransform;

    private bool _jumpRequested = false;

    private Dictionary<State, IPlayerState> _stateMap;

    //Interpolation
    private Vector3 _lastFixedPosition;
    private Vector3 _currentFixedPosition;
    private float _fixedDeltaTimeTimer = 0f;


    private void Awake()
    {
        _cameraTransform = Camera.main?.transform;

        // If we don't have a reference, try to get from this GameObject
        if (inputHandler == null)
        {
            inputHandler = GetComponent<PlayerInputHandler>();
            if (inputHandler == null)
            {
                Debug.LogError("PlayerInputHandler component is missing!");
            }
        }

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
        // Subscribe to input events
        if (inputHandler != null)
        {
            inputHandler.OnJumpInput += HandleJumpInput;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        if (inputHandler)
        {
            inputHandler.OnJumpInput -= HandleJumpInput;
        }
    }

    private void HandleJumpInput(bool jumpTriggered)
    {
        if (jumpTriggered && !controller.isGrounded)
        {
            _jumpBuffered = true;
        }
    }

    private IPlayerState LookupState(State state)
    {
        _stateMap.TryGetValue(state, out var output);
        return output;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateContext();
        State newState = DecideState();

        if (newState == State.Jumping) _jumpRequested = true;

        IPlayerState newPlayerState = LookupState(newState);

        if (newPlayerState != _currentState)
        {
            SetState(newPlayerState);
            CallFeedbacks();
        }
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
        if (controller.isGrounded && _currentState != LookupState(State.Jumping) && _currentState != LookupState(State.Grappling))
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
        _context.MoveInput = inputHandler.GetMoveInput();  // Use the input handler
        _context.CurrentVelocity = currentVelocity;
        _context.Position = transform.position;
        _context.CameraTransform = _cameraTransform;
        _context.MovementConfig = movementConfig;
        _context.PlayerTool = toolManager.GetCurrentTool();
    }

    State DecideState()
    {
        _lastState = _currentState;
        if (toolManager.GetCurrentTool() is ToolGrapplingHook grapplingHook && grapplingHook.IsActive())
        {
            return State.Grappling;
        }

        if (controller.isGrounded)
        {
            if (inputHandler.IsJumpTriggered() || _jumpRequested || _jumpBuffered)
            {
                _jumpRequested = true;
                _jumpBuffered = false;
                _framesSinceBuffer = 0;
                return State.Jumping;
            }
            else
            {
                if (_lastState != _currentState && _lastState == LookupState(State.Airborne))
                {
                    feedbackManager.PlayLandingFeedback();
                }

                if (inputHandler.IsRunning())
                {
                    return State.Running;
                }
                else
                {
                    return State.Walking;
                }
            }
        }
        else
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

    private void CallFeedbacks()
    {
        if (_lastState != _currentState && _lastState == LookupState(State.Airborne) && _currentState != LookupState(State.Jumping) && _currentState != LookupState(State.Grappling))
        {
            feedbackManager.PlayLandingFeedback();
            return;
        }
        if (_currentState == LookupState(State.Jumping))
        {
            feedbackManager.PlayJumpFeedback();
            return;
        }
    }
}