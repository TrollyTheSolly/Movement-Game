using UnityEngine;
using System.Collections.Generic;

public class PlayerToolManager : MonoBehaviour
{
    [SerializeField] public List<ToolBase> toolbelt = new List<ToolBase>();
    [SerializeField] private PlayerStateManager stateManager;
    [SerializeField] private int maxTools = 0;
    [SerializeField] private int currentTool = 0;
    private int lastToolUsed = 0;

    private ToolContext toolContext = new ToolContext();
    private PlayerInputActions _playerControls;
    private Transform cameraTransform;
    [SerializeField] private VelocityModifierSystem modifierSystem;

    [SerializeField] private GameObject jumpPadPrefab;
    [SerializeField] private Transform visualTransform;

    private void Awake() => _playerControls = new PlayerInputActions();
    private void OnEnable() => _playerControls.Enable();
    private void OnDisable() => _playerControls.Disable();

    private void Start()
    {
        cameraTransform = Camera.main.transform;

        toolbelt.Add(new ToolGrapplingHook());
        toolbelt.Add(new ToolMomentumFreezer());
        toolbelt.Add(new ToolGhostDash());
        toolbelt.Add(new ToolRocketLauncher());
        toolbelt.Add(new ToolTeleportOrb());
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
            toolbelt[currentTool].Held(toolContext);
            toolbelt[previousTool].Unheld(toolContext);
        }

        if (_playerControls.Player.Fire.triggered)
        {
            Debug.Log("Tool used");
            if (toolbelt.Count > 0)
            {
                toolbelt[currentTool].Activate(toolContext);
                if (lastToolUsed != currentTool) toolbelt[lastToolUsed].Clear(toolContext);
                lastToolUsed = currentTool;
            }
        }

        if (_playerControls.Player.Clear.triggered)
        {
            toolbelt[currentTool].Clear(toolContext);
        }
    }

    void UpdateContext()
    {
        toolContext.PlayerLocation = transform.position;
        toolContext.PlayerVelocity = stateManager.GetVelocity();
        toolContext.CameraTransform = cameraTransform;
        toolContext.Executor = this;
        toolContext.ModifierSystem = modifierSystem;
        toolContext.PlayerTransform = transform;
        toolContext.PlayerVisualTransform = visualTransform;
    }

    public ToolBase GetCurrentTool()
    {
        return toolbelt[currentTool];
    }

    private void OnDrawGizmos()
    {
        if (toolbelt != null && toolbelt.Count > 0 && toolbelt[0] != null)
        {
            ToolGrapplingHook grapplingHook = toolbelt[0] as ToolGrapplingHook;
            if (grapplingHook != null && grapplingHook.IsActive())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(grapplingHook.grapplePoint, 1f);
                Gizmos.DrawLine(transform.position, grapplingHook.grapplePoint);
            }
        }

    }
}