using UnityEngine;

public class PlayerGrappling : PlayerSimpleAirMovement
{
    public override void Enter(PlayerContext context)
    {
        // Initialize grappling specific state if needed
    }

    public override void Exit(PlayerContext context)
    {
        // Clean up grappling specific state if needed
    }

    public override Vector3 Update(PlayerContext context)
    {
        ToolGrapplingHook grappleTool = context.PlayerTool as ToolGrapplingHook;
        Vector3 toBody = context.Position - grappleTool.GrapplePoint;
        float currentDistance = toBody.magnitude;

        if (currentDistance == 0) return Vector3.zero;

        Vector3 forceDirection = toBody.normalized;
        float stretch = currentDistance - grappleTool.Config.ropeLength;

        // If rope isn't stretched, apply standard movement with air control
        if (stretch <= 0)
        {
            return ApplyAirControl(context, context.CurrentVelocity, grappleTool.Config.grappleControlFactor * 1.5f); // More control when rope is slack
        }

        // Calculate rope physics forces
        Vector3 tensionForce = -forceDirection * stretch * grappleTool.Config.ropeStiffness;
        Vector3 velocity = context.CurrentVelocity;
        Vector3 parallelVelocity = Vector3.Project(velocity, forceDirection);
        Vector3 tangentialVelocity = velocity - parallelVelocity;
        Vector3 correctedVelocity = tangentialVelocity + parallelVelocity * 0.5f;
        Vector3 dampingForce = -velocity * grappleTool.Config.dampingFactor * Time.fixedDeltaTime;

        // Apply rope physics first
        Vector3 ropePhysicsVelocity = correctedVelocity + tensionForce + dampingForce;

        // Now apply player control on top of rope physics, with reduced control factor
        return ApplyAirControl(context, ropePhysicsVelocity, grappleTool.Config.grappleControlFactor);
    }

    // Helper method to determine if player is swinging actively (could be used for animations)
    public bool IsActivelySwinging(PlayerContext context)
    {
        ToolGrapplingHook grappleTool = context.PlayerTool as ToolGrapplingHook;
        Vector3 toBody = context.Position - grappleTool.GrapplePoint;
        float currentDistance = toBody.magnitude;
        float stretch = currentDistance - grappleTool.Config.ropeLength;

        return stretch > 0 && context.CurrentVelocity.magnitude > 2.0f;
    }
}