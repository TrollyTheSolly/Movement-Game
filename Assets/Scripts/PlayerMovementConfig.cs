using UnityEngine;

[CreateAssetMenu(menuName = "Player/MovementConfig")]
public class PlayerMovementConfig : ScriptableObject
{
    public float gravity = 9.8f;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 15f;
    public float acceleration = 20f;
    public float decelerationRate = 1f;
    public float overSpeedDeceleration = 1f;
    public float airborneAcceleration = 1f;
    public int jumpBufferFrames = 30;
    public bool interpolate = false;
}