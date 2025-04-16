using UnityEngine;

public class PlayerContext
{
    public Vector3 currentVelocity = Vector3.zero;
    public Vector3 position = Vector3.zero;
    public Transform cameraTransform;
    public PlayerMovementConfig movementConfig;
    public ToolBase playerTool;

    public Vector2 moveInput;
    public bool jumpPressed;
    public bool jumpHeld;
}
