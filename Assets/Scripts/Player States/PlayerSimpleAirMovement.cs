using UnityEngine;

public abstract class PlayerSimpleAirMovement : IPlayerState
{
    public virtual void Enter(PlayerContext context) { }
    public virtual void Exit(PlayerContext context) { }

    protected Vector3 ApplyAirControl(PlayerContext context, Vector3 currentVelocity, float controlFactor)
    {
        Vector3 inputDir = new Vector3(context.MoveInput.x, 0f, context.MoveInput.y);
        inputDir = context.CameraTransform.TransformDirection(inputDir);
        inputDir.y = 0f;
        inputDir.Normalize();

        Vector3 velocityChange = Vector3.zero;
        float maxAirAccel = context.MovementConfig.airborneAcceleration * Time.fixedDeltaTime * controlFactor;

        if (inputDir.sqrMagnitude > 0.001f)
        {
            // Apply acceleration in input direction
            Vector3 targetDirection = inputDir * maxAirAccel;

            // Project current velocity onto input direction to preserve perpendicular momentum
            Vector3 velocityInMoveDir = Vector3.Project(currentVelocity, inputDir);

            // Only apply acceleration in the input direction
            velocityChange = targetDirection;
        }

        return currentVelocity + velocityChange;
    }

    public abstract Vector3 Update(PlayerContext context);
}