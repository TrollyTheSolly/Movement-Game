using UnityEngine;

public class PlayerAirborne : IPlayerState
{
    public void Enter(PlayerContext context)
    {

    }

    public void Exit(PlayerContext context)
    {

    }

    public Vector3 Update(PlayerContext context)
    {
        Vector3 inputDir = new Vector3(context.moveInput.x, 0f, context.moveInput.y);
        inputDir = context.cameraTransform.TransformDirection(inputDir);
        inputDir.y = 0f;
        inputDir.Normalize();

        Vector3 currentVelocity = context.currentVelocity;
        Vector3 velocityChange = Vector3.zero;

        float maxAirAccel = context.movementConfig.airborneAcceleration * Time.fixedDeltaTime;

        if (inputDir.sqrMagnitude > 0.001f)
        {
            // Apply acceleration in input direction without speed limits
            Vector3 targetDirection = inputDir * maxAirAccel;
            velocityChange = targetDirection;

            // Project current velocity onto input direction to preserve perpendicular momentum
            Vector3 velocityInMoveDir = Vector3.Project(currentVelocity, inputDir);
            Vector3 perpendicularVelocity = currentVelocity - velocityInMoveDir;

            // Only apply acceleration in the input direction
            velocityChange = velocityInMoveDir + velocityChange - velocityInMoveDir;
        }
        // No deceleration when no input (let gravity/physics handle it)

        return currentVelocity + velocityChange;
    }
}
