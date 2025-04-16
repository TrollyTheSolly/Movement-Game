using UnityEngine;

public abstract class PlayerSimpleMovement : IPlayerState
{
    protected abstract float GetTargetSpeed(PlayerContext context);

    public virtual void Enter(PlayerContext context) { }
    public virtual void Exit(PlayerContext context) { }

    public Vector3 Update(PlayerContext context)
    {
        Vector3 inputDir = new Vector3(context.moveInput.x, 0f, context.moveInput.y);
        inputDir = context.cameraTransform.TransformDirection(inputDir);
        inputDir.y = 0f;
        inputDir.Normalize();

        float targetSpeed = GetTargetSpeed(context);
        Vector3 targetVelocity = inputDir * targetSpeed;
        Vector3 currentVelocity = context.currentVelocity;
        Vector3 velocityChange = Vector3.zero;

        float maxAccel = context.movementConfig.acceleration * Time.fixedDeltaTime;
        float maxDecel = context.movementConfig.decelerationRate * Time.fixedDeltaTime;
        float maxOverSpeedDecel = context.movementConfig.overSpeedDeceleration * Time.fixedDeltaTime;

        // First check if we're overspeeding (regardless of input)
        if (currentVelocity.magnitude > targetSpeed * 1.01)
        {
            // Calculate velocity change to bring us back to target speed
            Vector3 overspeedDir = currentVelocity.normalized;
            Vector3 desiredVelocity = overspeedDir * targetSpeed;
            velocityChange = desiredVelocity - currentVelocity;
            velocityChange = Vector3.ClampMagnitude(velocityChange, maxOverSpeedDecel);
        }
        else if (inputDir.sqrMagnitude > 0.001f)
        {
            // Normal acceleration when we have input and aren't overspeeding
            velocityChange = targetVelocity - currentVelocity;
            velocityChange = Vector3.ClampMagnitude(velocityChange, maxAccel);
        }
        else
        {
            // Normal deceleration when no input
            velocityChange = -currentVelocity;
            velocityChange = Vector3.ClampMagnitude(velocityChange, maxDecel);
        }

        return currentVelocity + velocityChange;
    }
}