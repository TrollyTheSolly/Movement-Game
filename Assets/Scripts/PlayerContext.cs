using UnityEngine;

public class PlayerContext
{
    public Vector3 CurrentVelocity = Vector3.zero;
    public Vector3 Position = Vector3.zero;
    public Transform CameraTransform;
    public PlayerMovementConfig MovementConfig;
    public ToolBase PlayerTool;

    public Vector2 MoveInput;
    public bool JumpPressed;
    public bool JumpHeld;
}
