using UnityEngine;
using System.Collections.Generic;

public class PlayerToolManager : MonoBehaviour
{
    [SerializeField] public List<ToolBase> Toolbelt = new List<ToolBase>();
    [SerializeField] private PlayerStateManager stateManager;
    //[SerializeField] private int maxTools = 0;
    [SerializeField] private int currentTool = 0;
    private int _lastToolUsed = 0;

    private ToolContext _toolContext = new ToolContext();
    private PlayerInputActions _playerControls;
    private Transform _cameraTransform;
    [SerializeField] private VelocityModifierSystem modifierSystem;
    [SerializeField] private Transform visualTransform;

    private void Awake() => _playerControls = new PlayerInputActions();
    private void OnEnable() => _playerControls.Enable();
    private void OnDisable() => _playerControls.Disable();

    private void Start()
    {
        if (Camera.main) _cameraTransform = Camera.main.transform;

        Toolbelt.Add(new ToolGrapplingHook());
        Toolbelt.Add(new ToolMomentumFreezer());
        Toolbelt.Add(new ToolGhostDash());
        Toolbelt.Add(new ToolRocketLauncher());
        Toolbelt.Add(new ToolTeleportOrb());
    }

    private void Update()
    {
        UpdateContext();

        // Check for tool selection changes
        int previousTool = currentTool;
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentTool = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentTool = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentTool = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentTool = 3;
        if (Input.GetKeyDown(KeyCode.Alpha5)) currentTool = 4;

        // If tool changed, call Held() on the new tool
        if (previousTool != currentTool)
        {
            Debug.Log($"Switching tools: {previousTool} -> {currentTool}");
            Toolbelt[currentTool].Held(_toolContext);
            Toolbelt[previousTool].Unheld(_toolContext);
        }

        if (_playerControls.Player.Fire.triggered)
        {
            Debug.Log("Tool used");
            if (Toolbelt.Count > 0)
            {
                Toolbelt[currentTool].Activate(_toolContext);
                if (_lastToolUsed != currentTool) Toolbelt[_lastToolUsed].Clear(_toolContext);
                _lastToolUsed = currentTool;
            }
        }

        if (_playerControls.Player.Clear.triggered)
        {
            Toolbelt[currentTool].Clear(_toolContext);
        }
    }

    private void UpdateContext()
    {
        _toolContext.PlayerLocation = transform.position;
        _toolContext.PlayerVelocity = stateManager.GetVelocity();
        _toolContext.CameraTransform = _cameraTransform;
        _toolContext.Executor = this;
        _toolContext.ModifierSystem = modifierSystem;
        _toolContext.PlayerTransform = transform;
        _toolContext.PlayerVisualTransform = visualTransform;
    }

    public ToolBase GetCurrentTool()
    {
        return Toolbelt[currentTool];
    }

    private void OnDrawGizmos()
    {
        if (Toolbelt != null && Toolbelt.Count > 0 && Toolbelt[0] != null)
        {
            ToolGrapplingHook grapplingHook = Toolbelt[0] as ToolGrapplingHook;
            if (grapplingHook != null && grapplingHook.IsActive())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(grapplingHook.GrapplePoint, 1f);
                Gizmos.DrawLine(transform.position, grapplingHook.GrapplePoint);
            }
        }

    }
}