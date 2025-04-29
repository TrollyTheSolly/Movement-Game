using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [SerializeField] private VelocityModifierSystem modifierSystem;
    [SerializeField] private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] private PlayerMovementConfig movementConfig;
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerToolManager toolManager;
    [SerializeField] private FeedbackManager feedbackManager;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private PlayerVisualInterpolator interpolator;
    [SerializeField] private PlayerCollisionHandler collisionHandler;

    private int _framesSinceBuffer = 0;
    private bool _jumpBuffered = false;

    private IPlayerState _currentState;
    private IPlayerState _lastState;

    private PlayerContext _context = new PlayerContext();
    private Transform _cameraTransform;

    private bool _jumpRequested = false;

    private Dictionary<State, IPlayerState> _stateMap;

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
            [State.Grappling] = new PlayerGrappling(),
            [State.Wallriding] = new PlayerWallriding(),
            [State.GroundSlam] = new PlayerGroundSlam()
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
    public void Tick()
    {
        UpdateContext();
        State newState = DecideState();

        if (newState == State.Jumping) _jumpRequested = true;

        IPlayerState newPlayerState = LookupState(newState);

        if (newPlayerState != _currentState)
        {
            SetState(newPlayerState);
            CallFeedbacks();
            Debug.Log(newPlayerState);
        }
    }

    public void FixedTick()
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
        
        //Check collisions before moving
        collisionHandler.CheckHeadCollision(ref currentVelocity);
        collisionHandler.CheckSideCollision(ref currentVelocity);
        
        controller.Move(currentVelocity);
        //Interpolation
        interpolator.RegisterFixedTick(transform.position);

        _jumpRequested = false;
    }

    void ApplyGravity()
    {
        if (_currentState == LookupState(State.Wallriding)) return;
        
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
            if (inputHandler.IsGroundSlamTriggered() || _currentState == LookupState(State.GroundSlam))
            {
                return State.GroundSlam;
            }
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